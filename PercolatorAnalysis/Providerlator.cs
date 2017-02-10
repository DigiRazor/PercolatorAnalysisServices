/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices
{
    using System.Data;

    using Microsoft.AnalysisServices.AdomdClient;

    /// <summary>
    /// The Percolator Analysis Services IQueryProvider implementation.
    /// </summary>
    public class Providerlator : IMdxProvider
    {
        private AdomdConnection _connection;

        public Providerlator(string connectionString)
        {
            _connection = new AdomdConnection(connectionString);
        }

        public CellSet GetCellSet(string mdx)
        {
            using (var command = prepareCommand(mdx))
                return command.ExecuteCellSet();
        }

        public AdomdDataReader GetReader(string mdx)
        {
            using (var command = prepareCommand(mdx))
                return command.ExecuteReader();
        }

        public DataTable GetDataTable(string mdx)
        {
            using (var command = prepareCommand(mdx))
            {
                var table = new DataTable("CubeResults");
                using (var daptor = new AdomdDataAdapter(command))
                {
                    daptor.Fill(table);
                }
                return table;
            }
        }

        public void Dispose()
        {
            this._connection.Dispose();
        }

        /// <summary>
        /// Opens a connection if the current connection is closed or broken.
        /// </summary>
        private void openConnection()
        {
            if (this._connection.State == ConnectionState.Closed)
            {
                this._connection.Open();
            }

            else if (this._connection.State == ConnectionState.Broken)
            {
                this._connection.Close();
                this._connection.Open();
            }

            else return;
        }

        private AdomdCommand prepareCommand(string mdx)
        {
            openConnection();
            var command = new AdomdCommand(mdx, _connection);
            return command;
        }
    }
}