/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
            AxisNumber = axisNumber;
            IsNonEmpty = isNonEmpty;
            Creator = axisCreator;
            WithMembers = new List<string>();
            WithSets = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var obj = Creator.GetValue<T>();
            string str = string.Empty;
            if (obj is IEnumerable<ICubeObject>)
                str = ((IEnumerable<ICubeObject>)obj)
                    .Select(c => c.ToString())
                    .Aggregate((a, b) => $"{a},\r\n\t{b}");   //.JoinWith(",\t", true);
            else
                str = obj?.ToString();
            if (IsNonEmpty)
                sb.AppendLine("NON EMPTY");
            sb.AppendLine("{");
            if(str != null)
                sb.AppendLine("\t{0}", str);
            if (WithMembers.Count > 0)
            {
                if(str != null)
                    sb.AppendLine(",\t{0}", WithMembers.Aggregate((a, b) => $"{a},\r\n\t{b}"));
                else
                    sb.AppendLine("\t{0}", WithMembers.Aggregate((a, b) => $"{a},\r\n\t{b}"));
            }
            if (WithSets.Count > 0)
            {
                if(str != null || WithMembers.Count > 0)
                    sb.AppendLine("*\t{0}", WithSets.Aggregate((a, b) => $"{a} *\r\n\t{b}"));
                else
                    sb.AppendLine("\t{0}", WithSets.Aggregate((a, b) => $"{a} *\r\n\t{b}"));
            }
            sb.Append("}")
                .Append($" ON {AxisNumber}");
            return sb.ToString();
        }

        public static implicit operator string(Axis<T> axis)
        {
            return axis.ToString();
        }

        public Axis<T> AssembleAxis(Expression<Func<T, ICubeObject>> axisCreator)
        {
            Creator = axisCreator;
            return this;
        }
    }
}
