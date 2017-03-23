/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices
{
    using System;
    using System.Data;

    using Microsoft.AnalysisServices.AdomdClient;

    /// <summary>
    /// The IMdxProvider to implement a PAS LINQ to MDX provider.
    /// </summary>
    public interface IMdxProvider : IDisposable
    {
        CellSet GetCellSet(string mdx);

        DataTable GetDataTable(string mdx);

        AdomdDataReader GetReader(string mdx);
    }
}
