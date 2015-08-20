using System;

namespace Percolator.AnalysisServices.Attributes
{
    internal sealed class MdxQueryableAttribute : Attribute
    {
        public byte AxisNumber { get; set; }
    }
}