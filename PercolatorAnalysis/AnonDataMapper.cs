/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices
{
    using Percolator.AnalysisServices.Linq;
   
    internal class AnonDataMapper<T> : IEnumerable<T>, IDisposable where T : class
    {
        Enumerator _rator;

        public AnonDataMapper(AdomdDataReader reader, Dictionary<int, int> ordinalMapping)
        {
            this._rator = new AnonDataMapper<T>.Enumerator(reader, ordinalMapping);
        }

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            Enumerator e = this._rator;
            if (e == null)
                throw new InvalidOperationException("Cannot enumerate more than once!");
            this._rator = null;
            return e;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator<T>)this._rator;
        }
        #endregion

        class Enumerator : IEnumerator<T>, IDisposable
        {
            delegate T ObjActivator<T>(object[] args); 
            AdomdDataReader _reader;
            Dictionary<int, int> _ordinal;
            T _current;

            public Enumerator(AdomdDataReader reader, Dictionary<int, int> ordinalMapping)
            {
                this._reader = reader;
                this._ordinal = ordinalMapping;
            }

            #region IEnumerator<T> Members
            public T Current { get { return this._current; } }
            #endregion

            #region IEnumerator Members
            object System.Collections.IEnumerator.Current { get { return (T)this._current; } }

            public bool MoveNext()
            {
                if (this._reader.Read())
                {
                    //map data
                    var paramz = this.mapObject();
                    var expectingRows = FormatterServices.GetUninitializedObject(typeof(T)).GetType().GetProperties().Count();
                    if (paramz.Count() != expectingRows)
                        throw new Exception("The selected columns are not equal to the row count coming back from the cube.");

                    var ctor = typeof(T).GetConstructors()[0];
                    var prmz = ctor.GetParameters();
                    int count = prmz.Count();
                    Queue<object> pz = new Queue<object>();
                    for(int i = 0; i < count; i++) 
                    {
                        switch(prmz[i].ParameterType.Name)
                        {
                            case "Set":
                            case "Attribute":
                                pz.Enqueue(new Attribute(value: paramz[i], type: prmz[i].ParameterType, tag: ""));
                                break;

                            case "Level":
                                pz.Enqueue(new Level(value: paramz[i], type: prmz[i].ParameterType, tag: ""));
                                break;

                            case "Dimension":
                            case "Hierarchy":
                                throw new PercolatorException("Cannot query by a Dimension or Hierarchy");

                            case "Measure":
                                pz.Enqueue(new Measure(value: paramz[i], type: prmz[i].ParameterType, tag: ""));
                                break;

                            case "Member":
                                pz.Enqueue(new Member(value: paramz[i], type: prmz[i].ParameterType));
                                break;

                            case "Object":
                                pz.Enqueue(paramz[i]);
                                break;

                            default:

                                break;
                        }
                    }
                    var prm = Expression.Parameter(typeof(object[]), "args");
                    var argsExp = new Expression[prmz.Length];

                    for (int i = 0; i < prmz.Length; i++)
                    {
                        var index = Expression.Constant(i);
                        var paramType = prmz[i].ParameterType;
                        var paramAccess = Expression.ArrayIndex(prm, index);
                        var paramCast = Expression.Convert(paramAccess, paramType);
                        argsExp[i] = paramCast;
                    }

                    NewExpression newObj = Expression.New(ctor, argsExp);
                    LambdaExpression lamb = Expression.Lambda(typeof(ObjActivator<T>), newObj, prm);
                    var activator = (ObjActivator<T>)lamb.Compile();
                    var obj = activator.Invoke(pz.ToArray());
                    
                    this._current = (T)obj;
                    return true;
                }

                else return false;
            }

            public void Reset()
            {
                //nO oNe UsEs ThIs, MiCrOsOfT1!1!!
            }
            #endregion

            #region IDisposable Members
            public void Dispose()
            {
                this._reader.Dispose();
            }
            #endregion
            object[] mapObject()
            {
                Dictionary<int, object> values = new Dictionary<int, object>(this._reader.FieldCount);
                foreach(KeyValuePair<int, int> kvp in this._ordinal.OrderBy(x => x.Value))
                    values.Add(kvp.Key, this._reader.GetValue(kvp.Value));

                ArrayList al = new ArrayList();
                foreach (KeyValuePair<int, object> kvp in values.OrderBy(x => x.Key))
                    al.Add(kvp.Value);
                var retArray = al.ToArray();
                return retArray;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._rator.Dispose();
        }

        #endregion
    }
}
