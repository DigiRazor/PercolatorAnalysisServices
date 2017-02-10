/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Attributes
{
    public sealed class MapToAttribute : global::System.Attribute
    {
        public string MdxColumn { get; set; }

        public MapToAttribute()
        {
        }

        public MapToAttribute(string mdxColumn)
        {
            MdxColumn = mdxColumn;
        }
    }
}
