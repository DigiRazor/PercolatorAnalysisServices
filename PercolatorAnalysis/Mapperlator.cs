/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using Microsoft.AnalysisServices.AdomdClient;
using Percolator.AnalysisServices.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Percolator.AnalysisServices
{
    internal class Mapperlator<T> : IEnumerable<T>
    {
        IEnumerator<T> _rator;

        internal Mapperlator(AdomdDataReader reader)
        {
            _rator = new Enumerlator(reader);
        }

        public IEnumerator<T> GetEnumerator() => _rator;

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        class Enumerlator : IEnumerator<T>
        {
            delegate T Creatorlator(TypeConverter[] converters, string[] values);

            AdomdDataReader _reader;
            Creatorlator _creator;
            TypeConverter[] _converters;
            int[] _ornials;

            public T Current { get; private set; }
            object System.Collections.IEnumerator.Current => Current; 

            public Enumerlator(AdomdDataReader reader)
            {
                _reader = reader;
                init();
            }

            public bool MoveNext()
            {
                if (!_reader.IsClosed && _reader.Read())
                {
                    Current = get();
                    return true;
                }

                else
                    return false;
            }

            T get()
            {
                var rawValues = new string[_reader.FieldCount];

                _ornials
                    .For((v, i) => rawValues[i] = _reader[v] == null ? null : _reader[v].ToString());

                return _creator(_converters, rawValues);
            }

            void init()
            {
                var type = typeof(T);
                var schema = _reader.GetSchemaTable();
                var columnOrds = schema.Rows
                    .Cast<DataRow>()
                    .Select(x => new
                    {
                        Name = x[0].ToString().Replace("[", "").Replace("]", "").Split('.')[1],
                        Ordianl = Convert.ToInt32(x[1])
                    });

                var props = type.GetProperties()
                    .Where(x => System.Attribute.IsDefined(x, typeof(MapToAttribute)))
                    .Select(x => new
                    {
                        Attribute = x.GetCustomAttribute<MapToAttribute>(),
                        PropertyInfo = x
                    })
                    .Join(columnOrds, p => p.Attribute.MdxColumn, co => co.Name, (p, co) => new
                    {
                        Ordinal = co.Ordianl,
                        Property = p
                    })
                    .OrderBy(x => x.Property.Attribute.MdxColumn);

                _ornials = props.Select(x => x.Ordinal).ToArray();

                var bindingList = new Dictionary<ParameterExpression, MemberAssignment>();
                _converters = new TypeConverter[props.Count()];
                var stringArrayParam = Expression.Parameter(typeof(string[]), "values");
                var converterArrayParam = Expression.Parameter(typeof(TypeConverter[]), "converters");
                var converter = typeof(TypeConverter).GetMethod("ConvertFromString", new[] { typeof(string) });

                props.For((v, i) =>
                {
                    var prop = v.Property.PropertyInfo;
                    var paramExp = Expression.Parameter(prop.PropertyType, prop.Name);
                    var arrayAssignment = Expression.ArrayIndex(stringArrayParam, Expression.Constant(i));
                    var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                    var converterArrayAssignment = Expression.ArrayIndex(converterArrayParam, Expression.Constant(i));
                    var defaultValue = prop.PropertyType.GetDefault();
                    var defaultConstant = Expression.Constant(defaultValue == null ? null : defaultValue.ToString(), typeof(string));
                    var methodExp = Expression.Call(converterArrayAssignment, converter, Expression.Coalesce(arrayAssignment, defaultConstant));

                    Expression.Bind(prop,
                        Expression.Convert(methodExp, prop.PropertyType))
                        .Finally(bind => bindingList.Add(paramExp, bind));

                    _converters[i] = typeConverter;
                });

                var newExp = Expression.New(typeof(T));
                var memberInit = Expression.MemberInit(newExp, bindingList.Values.ToArray());
                var lambda = Expression.Lambda<Creatorlator>(memberInit, new[] { converterArrayParam, stringArrayParam });
                _creator = lambda.Compile();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose() =>_reader.Dispose();
        }
    }
}
