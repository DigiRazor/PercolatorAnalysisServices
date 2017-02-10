/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using Microsoft.AnalysisServices.AdomdClient;

    using Percolator.AnalysisServices.Attributes;

    internal static class Helpers
    {
        internal static Expression StripQuotes(this Expression source)
        {
            var e = source;
            while (e is UnaryExpression)
                e = ((UnaryExpression)e).Operand;
            return e;
        }

        internal static ParameterExpression GetParameter(this Expression node)
        {
            var e = node;
            if (e is BinaryExpression)
            {
                var bin = (BinaryExpression)e;
                var leftParam = bin.Left.GetParameter();
                if (leftParam != null)
                    return leftParam;
                else
                    return bin.Right.GetParameter();                    
            }
            while (e is MemberExpression || e is MethodCallExpression)
            {
                if (e is MemberExpression)
                    e = ((MemberExpression)e).Expression;
                else if (e is MethodCallExpression)
                    e = ((MethodCallExpression)e).Object;
            }
            return e as ParameterExpression;
        }

        internal static byte DetermineAxis(this MemberInfo mem)
        {
            switch(mem.Name.ToLower())
            {
                case "oncolumns":
                    return 0;

                case "onrows":
                    return 1;

                case "onpages":
                    return 2;

                case "onchapters":
                    return 3;

                case "onsections":
                    return 4;

                default:
                    throw new ArgumentException($"The {mem.Name} axis is not recognized.");
            }
        }

        internal static StringBuilder AppendLine(this StringBuilder source, string str, params object[] objs) =>
            source.AppendLine(string.Format(str, objs));

        internal static object GetValue<T>(this Expression expression)
        {
            if (expression == null)
                return null;

            try
            {
                var instanceProp = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Instance");
                if(instanceProp == null)
                    throw new PercolatorException($"Cannot find the sigleton instance of '{typeof(T).Name}'.");
                var t = (T)instanceProp.GetValue(null);
                var lambdaExp = (LambdaExpression)expression;
                var lambda = Expression.Lambda<Func<T, object>>(lambdaExp.Body, lambdaExp.Parameters.FirstOrDefault());
                var method = lambda.Compile();
                var obj = method(t);
                return obj;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        internal static T GetCubeInstance<T>(this Type source)
        {
            var instanceProp = source.GetProperty("Instance", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public);
            if(instanceProp == null)
                throw new PercolatorException($"Cannot find the sigleton instance of '{typeof(T).Name}'.");
            return (T)instanceProp.GetValue(null);
        }

        internal static IEnumerable<T> FlattenAndReturn<T>(this CellSet source) where T : new()
        {
            if (source.Axes.Count > 2)
            {
                var ex = new NotImplementedInPAS_Exception("Apologies, but maping to an object using more than two axes is not yet supported.");
                ex.ReasonWhy = "The differences in the Cell Values indexes have not been mapped yet.";
                throw ex;
            }

            var type = typeof(T);
            var flattend = Flat.Flatten(source);

            var names = flattend
                .SelectMany(x => x.MeasureValues.Select(y => y.Key).Distinct())
                .Concat(flattend.SelectMany(x => x.PositionValues.Select(y => y.Key).Distinct()))
                .Distinct()
                .OrderBy(x => x);

            var arrayExp = Expression.Parameter(typeof(string[]), "values");
            var converterArray = Expression.Parameter(typeof(TypeConverter[]), "converters");
            var bindingList = new Dictionary<ParameterExpression, MemberAssignment>();

            var ps = type.GetProperties()
                .Where(x => System.Attribute.IsDefined(x, typeof(MapToAttribute)))
                .Select(x => new
                {
                    Attribute = x.GetCustomAttribute<MapToAttribute>(),
                    PropertyInfo = x
                })
                .Where(x => names.Contains(x.Attribute.MdxColumn))
                .OrderBy(x => x.Attribute.MdxColumn);

            var converter = typeof(TypeConverter).GetMethod("ConvertFromString", new[] { typeof(string) });
            var converterList = new TypeConverter[ps.Count()];

                ps.For((v, i) =>
                    {
                        var prop = v.PropertyInfo;
                        var paramExp = Expression.Parameter(prop.PropertyType, prop.Name);
                        var arrayAssignment = Expression.ArrayIndex(arrayExp, Expression.Constant(i));
                        var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                        var converterArrayAssignment = Expression.ArrayIndex(converterArray, Expression.Constant(i));
                        var defaultValue = prop.PropertyType.GetDefault();
                        var defaultConstant = Expression.Constant(defaultValue == null ? null : defaultValue.ToString(), typeof(string));
                        var methodExp = Expression.Call(converterArrayAssignment, converter, Expression.Coalesce(arrayAssignment, defaultConstant));
                        
                        Expression.Bind(prop,
                            Expression.Convert(methodExp, prop.PropertyType))
                            .Finally(bind => bindingList.Add(paramExp, bind));

                        converterList[i] = typeConverter;
                    });

            var newExp = Expression.New(type);
            var memberInit = Expression.MemberInit(newExp, bindingList.Values.ToArray());
            var lambda = Expression.Lambda<Func<TypeConverter[], string[], T>>(memberInit, new[] { converterArray, arrayExp });
            var creator = lambda.Compile();

            foreach (var flat in flattend)
            {
                var results = flat.MeasureValues
                    .Select(x => new
                        {
                            Name = x.Key,
                            Value = x.Value == null ? null : x.Value.ToString()
                        })
                    .Concat(flat.PositionValues
                        .Select(x => new
                            {
                                Name = x.Key,
                                Value = x.Value
                            }))
                    .OrderBy(x => x.Name)
                    .Select(x => x.Value)
                    .ToArray();

                yield return creator(converterList, results);
            }
        }

        public static IEnumerable<T> ReturnFromReader<T>(this AdomdDataReader source) where T : new()
        {
            var type = typeof(T);
            var schemaTable = source.GetSchemaTable();
            var columnNames = schemaTable.Rows
                .Cast<DataRow>()
                .Select(x => x[0].ToString())
                .ToArray();

            var props = type.GetProperties()
                .Where(x => System.Attribute.IsDefined(x, typeof(MapToAttribute)))
                .Select(x => new
                {
                    Attribute = x.GetCustomAttribute<MapToAttribute>(),
                    PropertyInfo = x
                })
                .Where(x => columnNames.Contains(x.Attribute.MdxColumn))
                .OrderBy(x => x.Attribute.MdxColumn);

            var bindingList = new Dictionary<ParameterExpression, MemberAssignment>();
            var converterList = new TypeConverter[props.Count()];
            var stringArrayParam = Expression.Parameter(typeof(string[]), "values");
            var converterArrayParam = Expression.Parameter(typeof(TypeConverter[]), "converters");
            var converter = typeof(TypeConverter).GetMethod("ConvertFromString", new [] { typeof(string) });

            props.For((v, i) =>
                {
                    var prop = v.PropertyInfo;
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

                    converterList[i] = typeConverter;
                });

            var newExp = Expression.New(typeof(T));
            var memberInit = Expression.MemberInit(newExp, bindingList.Values.ToArray());
            var lambda = Expression.Lambda<Func<TypeConverter[], string[], T>>(memberInit, new[] { converterArrayParam, stringArrayParam });
            var creatorMethod = lambda.Compile();

            while(source.Read())
            {
                var rawResults = new string[columnNames.Length];

                columnNames
                    .OrderBy(x => x)
                    .For((v, i) => rawResults[i] = source[v].ToString());

                yield return creatorMethod(converterList, rawResults);
            }
        }

        public static object GetDefault(this Type source)
        {
            if(source == null)
                return null;

            if (source.IsValueType)
                return Activator.CreateInstance(source);

            else if (source == typeof(string))
                return String.Empty;

            else
                return null;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            using (var rator = source.GetEnumerator())
                while (rator.MoveNext())
                    action(rator.Current);
        }

        public static void For<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;
            using (var rator = source.GetEnumerator())
                while (rator.MoveNext())
                    action(rator.Current, index++);
        }

        /// <summary>
        /// Removes the occurance of the passed in string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="removal"></param>
        /// <returns></returns>
        public static string Remove(this string source, params string[] removal)
        {
            var result = source;
            foreach (var remove in removal)
                result = result.Replace(remove, "");
            return result;
        }

        public static Tout To<Tsource, Tout>(this Tsource source, Func<Tsource, Tout> function) => function(source);

        public static Tout To<Tsource, T1, Tout>(this Tsource source, T1 in1, Func<Tsource, T1, Tout> function) => function(source, in1);

        public static Tout To<Tsource, T1, T2, Tout>(this Tsource source, T1 in1, T2 in2, Func<Tsource, T1, T2, Tout> function) => function(source, in1, in2);

        public static Tout To<Tsource, T1, T2, T3, Tout>(this Tsource source, T1 in1, T2 in2, T3 in3, Func<Tsource, T1, T2, T3, Tout> function) => function(source, in1, in2, in3);

        public static Tout To<Tsource, T1, T2, T3, T4, Tout>(this Tsource source, T1 in1, T2 in2, T3 in3, T4 in4, Func<Tsource, T1, T2, T3, T4, Tout> function) => function(source, in1, in2, in3, in4);

        public static Tout To<Tsource, T1, T2, T3, T4, T5, Tout>(this Tsource source, T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, Func<Tsource, T1, T2, T3, T4, T5, Tout> function) => function(source, in1, in2, in3, in4, in5);

        public static Tout To<Tsource, T1, T2, T3, T4, T5, T6, Tout>(this Tsource source, T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, Func<Tsource, T1, T2, T3, T4, T5, T6, Tout> function) => function(source, in1, in2, in3, in4, in5, in6);

        public static void Finally<Tsource>(this Tsource source, Action<Tsource> action) => action(source);

        public static void Finally<Tsource, T1>(this Tsource source, T1 in1, Action<Tsource, T1> action) => action(source, in1);

        public static void Finally<Tsource, T1, T2>(this Tsource source, T1 in1, T2 in2, Action<Tsource, T1, T2> action) =>action(source, in1, in2);

        public static void Finally<Tsource, T1, T2, T3>(this Tsource source, T1 in1, T2 in2, T3 in3, Action<Tsource, T1, T2, T3> action) => action(source, in1, in2, in3);

        public static void Finally<Tsource, T1, T2, T3, T4>(this Tsource source, T1 in1, T2 in2, T3 in3, T4 in4, Action<Tsource, T1, T2, T3, T4> action) =>action(source, in1, in2, in3, in4);

        public static void Finally<Tsource, T1, T2, T3, T4, T5, T6>(this Tsource source, T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, Action<Tsource, T1, T2, T3, T4, T5, T6> action) => action(source, in1, in2, in3, in4, in5, in6);
    }
}
