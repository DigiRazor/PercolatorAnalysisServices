/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
