using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.MdxColumn = mdxColumn;
        }
    }
}
