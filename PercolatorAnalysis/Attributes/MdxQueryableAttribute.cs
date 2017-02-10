/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Attributes
{
    using System;

    internal sealed class MdxQueryableAttribute : Attribute
    {
        public byte AxisNumber { get; set; }
    }
}