/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices.Linq
{
    using Percolator.AnalysisServices.Attributes;

    /// <summary>
    /// Represents an Analysis Services 'Attribute' within a dimension.
    /// </summary>
    public class Attribute : Set
    {
        public Member All { get { return new Member(string.Format("{0}.{1}", Tag, "[All]")); } }
        /// <summary>
        /// Retrieves a member by its name from the attribute.
        /// </summary>
        /// <param name="memberName">The name of the member. Start the name with an ampersand to retrieve the member by its address.
        /// Automatically places in the square brackets.</param>
        /// <returns></returns>
        public Member this[string memberName] { get { return memberFrom(memberName); } }
        /// <summary>
        /// Retrieves a member by its name from the attribute.
        /// </summary>
        /// <param name="memberName">The name of the member. Start the name with an ampersand to retrieve the member by its address.
        /// Automatically places in the square brackets.</param>
        /// <returns></returns>
        public Member this[int memberName] { get { return memberFrom(memberName.ToString()); } }
        /// <summary>
        /// The MDX syntax representation for this attribute.
        /// </summary>
        public string Tag { get; private set; }

        public Set This { get { return getThis(); } }
         
        /// <summary>
        /// Used by the T4 template to create an attribute for a dimension.
        /// </summary>
        /// <param name="tag"></param>
        public Attribute(string tag)
        {
            Tag = tag;
            _values.Add(tag);
        }

        internal Attribute(object value, Type type, string tag = "")
        {
            ObjectValue = value;
            ValueType = type;
            Tag = tag;
        }

        /// <summary>
        /// Appends a function to the end of the attribute in the query.
        /// </summary>
        /// <param name="function">The name/syntax for the extension function.</param>
        /// <returns></returns>
        public Member Function(string function)
        {
            return new Member(string.Format("{0}.{1}", Tag, function));
        }

        /// <summary>
        /// Implicit string conversion for the attribute.
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        public static implicit operator string(Attribute att)
        {
            return att.ToString();
        }

        public static implicit operator Attribute(string str)
        {
            return new Attribute(str);
        }

        /// <summary>
        /// Overridden ToString returns the Tag for this attribute.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Tag;
        }

        string getThis()
        {
            var tag = ToString();
            var split = tag.Split('.');

            if (split.Length == 2)
                return string.Format("{0}.{1}", tag, split[1]);
            else
                return tag;
        }

        Member memberFrom(string memberName)
        {
            string att = Tag;
            string val = memberName;
            val = val.Replace("[", "").Replace("]", "");
            if (val.StartsWith("&"))
                val = string.Format("&[{0}]", val.Substring(1));
            else
                val = string.Format("[{0}]", val);
            att = string.Format("{0}.{1}", att, val);
            return new Member(att);
        }
    }
}
