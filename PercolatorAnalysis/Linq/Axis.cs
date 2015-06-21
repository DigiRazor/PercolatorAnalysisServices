using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Adomd = Microsoft.AnalysisServices.AdomdClient;

namespace Percolator.AnalysisServices.Linq
{
    public class Axis<T> : ICubeObject
    {
        public byte AxisNumber { get; set; }
        public bool IsNonEmpty { get; set; }
        internal Expression Creator { get; private set; }
        internal List<string> WithMembers { get; set; }
        internal List<string> WithSets { get; set; }

        public Axis(byte axisNumber)
            : this(axisNumber, false) { }

        public Axis(byte axisNumber, bool isNonEmpty)
            : this(axisNumber, isNonEmpty, null) { }

        internal Axis(byte axisNumber, bool isNonEmpty, Expression axisCreator)
        {
            this.AxisNumber = axisNumber;
            this.IsNonEmpty = isNonEmpty;
            this.Creator = axisCreator;
            this.WithMembers = new List<string>();
            this.WithSets = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var obj = this.Creator.GetValue<T>();
            string str = string.Empty;
            if (obj is IEnumerable<ICubeObject>)
                str = ((IEnumerable<ICubeObject>)obj)
                    .Select(c => c.ToString())
                    .Aggregate((a, b) => String.Format("{0},\r\n\t", a, b));   //.JoinWith(",\t", true);
            else
                str = obj == null ? null : obj.ToString();
            if (this.IsNonEmpty)
                sb.AppendLine("NON EMPTY");
            sb.AppendLine("{");
            if(str != null)
                sb.AppendLine("\t{0}", str);
            if (this.WithMembers.Count > 0)
            {
                if(str != null)
                    sb.AppendLine(",\t{0}", this.WithMembers.Aggregate((a, b) => String.Format("{0},\r\n\t{1}", a, b)));
                else
                    sb.AppendLine("\t{0}", this.WithMembers.Aggregate((a, b) => String.Format("{0},\r\n\t{1}", a, b)));
            }
            if (this.WithSets.Count > 0)
            {
                if(str != null || this.WithMembers.Count > 0)
                    sb.AppendLine("*\t{0}", this.WithSets.Aggregate((a, b) => String.Format("{0} *\r\n\t{1}", a, b)));
                else
                    sb.AppendLine("\t{0}", this.WithSets.Aggregate((a, b) => String.Format("{0} *\r\n\t{1}", a, b)));
            }
            sb.Append("}")
                .AppendFormat(" ON {0}", this.AxisNumber);
            return sb.ToString();
        }

        public static implicit operator string(Axis<T> axis)
        {
            return axis.ToString();
        }

        public Axis<T> AssembleAxis(Expression<Func<T, ICubeObject>> axisCreator)
        {
            this.Creator = axisCreator;
            return this;
        }
    }
}
