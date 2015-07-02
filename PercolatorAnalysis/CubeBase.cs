/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Percolator.AnalysisServices.Linq;
using System.Linq.Expressions;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;

namespace Percolator.AnalysisServices
{
    /// <summary>
    /// Base class for the main database class generated from the T4 template.
    /// </summary>
    public class CubeBase : IDisposable
    {
        string _connectionString;
        /// <summary>
        /// The cube's IQueryProvider implementation.
        /// </summary>
        protected Providerlator _provider;

        public Providerlator Provider { get { return this._provider; } }
        public string ConnectionString 
        { 
            get
            {
                return this._connectionString;
            }
            set
            {
                if (this._connectionString != value)
                {
                    this._connectionString = value;
                    this._provider = new Providerlator(value);
                }

                else if (this._provider == null)
                    this._provider = new Providerlator(value);
            }
        }

        /// <summary>
        /// Instantiates new CubeBase as well as the provider and static connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public CubeBase(string connectionString)
        {
            this._provider = new Providerlator(connectionString);
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Executes the query string and returns the result in a DataTable object.
        /// </summary>
        /// <param name="mdxQuery">The string mdx query.</param>
        /// <returns></returns>
        public DataTable Execute(string mdxQuery)
        {
            return this._provider.GetDataTable(mdxQuery);
        }

        /// <summary>
        /// Executes the quer string and returns the result in a Microsoft.AnalysisServices.AdomdClient CellSet.
        /// </summary>
        /// <param name="mdxQuery">The string mdx query.</param>
        /// <returns></returns>
        public CellSet ExecuteCellSet(string mdxQuery)
        {
            return this._provider.GetCellSet(mdxQuery);
        }

        /// <summary>
        /// The magic word. Executes this cube's objects in an MDX query against the analysis services, and maps the results to the type applied to the generic parameter. 
        /// </summary>
        /// <typeparam name="T_MapTo">The type to map the results of the query to.</typeparam>
        /// <param name="mdx">The mdx query string.</param>
        /// <returns>A collection of objects containing the mapped results of the query.</returns>
        public IEnumerable<T_MapTo> Percolate<T_MapTo>(string mdx) where T_MapTo : new()
        {
            return this._provider.GetCellSet(mdx).FlattenAndReturn<T_MapTo>();
        }

        #region IDisposable Members
        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose()
        {
            this._provider.Dispose();
        }
        #endregion
    }
}
