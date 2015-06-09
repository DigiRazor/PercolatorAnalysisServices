/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient;
using System.Runtime.Serialization;
using System.Data;
using Percolator.AnalysisServices.Attributes;

namespace Percolator.AnalysisServices
{
    internal class DataMapper<T> : IEnumerable<T>, IDisposable where T : class, new()
    {
        Enumerator _rator;

        internal DataMapper(AdomdDataReader reader, Dictionary<string, int> ordinalMapper)
        {
            this._rator = new Enumerator(reader, ordinalMapper);
        }

        internal DataMapper(AdomdDataReader reader)
        {
            this._rator = new Enumerator(reader);
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
            AdomdDataReader _reader;
            Dictionary<string, int> _ordMapper;
            T _current;

            internal Enumerator(AdomdDataReader reader)
            {

            }

            internal Enumerator(AdomdDataReader reader, Dictionary<string, int> ordinalMapper)
            {
                this._reader = reader;
                this._ordMapper = ordinalMapper;
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
                    if (this._ordMapper != null)
                    {
                        T obj = new T();
                        foreach (KeyValuePair<string, int> mapping in this._ordMapper.OrderBy(x => x.Value))
                        {
                            PropertyInfo prop = obj.GetType().GetProperty(mapping.Key);
                            var val = this._reader.GetValue(mapping.Value);
                            prop.SetValue(obj, val);
                        }

                        this._current = (T)obj;
                    }
                    else
                    {
                        T obj = new T();
                        foreach(var prop in typeof(T).GetProperties())
                        {
                            prop.SetValue(obj, this._reader[prop.Name]);
                        }
                        this._current = (T)obj;
                    }

                    return true;
                }
                else
                {
                    //this._reader.Close();
                    return false;
                }
            }

            public void Reset()
            {
                //nO oNe UsEs ThIs, MiCrOsOfT1!1!!
            }
            #endregion

            #region IDisposable Members
            public void Dispose()
            {
                if (!this._reader.IsClosed)
                    this._reader.Dispose();
            }
            #endregion
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._rator.Dispose();
        }

        #endregion
    }
}
