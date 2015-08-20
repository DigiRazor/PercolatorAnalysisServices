using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MdxAttribute : Attribute
    {
        public string Tag { get; set; }
        public Axis Axis { get; set; }
        public MdxOptions Options { get; set; }

        public MdxAttribute() { }

        public MdxAttribute(string tag, Axis axis)
        {
            Tag = tag;
            Axis = axis;
            Options = MdxOptions.None;
        }

        public MdxAttribute(string tag, Axis axis, MdxOptions options)
        {
            Tag = tag;
            Axis = axis;
            Options = options;
        }
    }
}
