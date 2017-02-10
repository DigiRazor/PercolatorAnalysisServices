/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Attributes
{
    using System;

    /// <summary>
    /// Tags the class as an Analysis Services dimension
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DimensionAttribute : Attribute
    {
    }
}
