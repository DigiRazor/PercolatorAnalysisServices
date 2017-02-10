/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Attributes
{
    using System;

    public sealed class MapToAttribute : Attribute
    {
        public MapToAttribute()
        {
        }

        public MapToAttribute(string mdxColumn)
        {
            MdxColumn = mdxColumn;
        }

        public string MdxColumn { get; set; }
    }
}
