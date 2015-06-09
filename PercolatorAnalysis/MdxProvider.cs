/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient;
using Percolator.AnalysisServices.Linq;

namespace Percolator.AnalysisServices
{
    /// <summary>
    /// Abstract implementation for the IQueryProvider.
    /// </summary>
    public abstract class MdxProvider : IMdxProvider
    {
        protected bool _nonEmptyColumns;
        protected bool _nonEmptyRows;
        /// <summary>
        /// The AdomdConnection used to execute queries.
        /// </summary>
        protected AdomdConnection _connection;
        /// <summary>
        /// Implement to safely open a connection.
        /// </summary>
        protected abstract void openConnection();

        /// <summary>
        /// The translated query string from the provider's expression.
        /// </summary>
        public string MdxCommand { get; protected set; }

        public CellSet GetCellSet(string mdx)
        {
            using(AdomdCommand command = new AdomdCommand(mdx, this._connection))
            {
                this.openConnection();
                try
                {
                    return command.ExecuteCellSet();
                }
                catch (AdomdErrorResponseException e)
                {
                    throw new PercolatorQueryExeption(e.InnerException != null ? e.InnerException.Message : e.Message, mdx);
                }
            }
        }

        public void SetNonEmptyAxis(bool columns, bool rows)
        {
            this._nonEmptyColumns = columns;
            this._nonEmptyRows = rows;
        }

        /// <summary>
        /// Abstract method to execute a query.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract object Execute(Expression expression);


        #region IDisposable Members
        /// <summary>
        /// Closes this connection.
        /// </summary>
        public void Dispose()
        {
            switch (this._connection.State)
            {
                case ConnectionState.Open:
                case ConnectionState.Broken:
                    this._connection.Dispose();
                    break;
            }
        }

        #endregion

        public DataTable GetDataTable(Expression expression)
        {
            throw new NotImplementedException();
        }

        public AdomdDataReader GetDataReader(Expression expression)
        {
            throw new NotImplementedException();
        }

        public CellSet GetCellSet(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
