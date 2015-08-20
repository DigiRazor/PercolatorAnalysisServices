/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Data;

namespace Percolator.AnalysisServices
{
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
