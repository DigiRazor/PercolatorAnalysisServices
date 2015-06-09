using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices.Attributes
{
    internal sealed class MdxQueryableAttribute : Attribute
    {
        public byte AxisNumber { get; set; }
    }
}
