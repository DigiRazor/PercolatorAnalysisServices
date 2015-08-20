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
using System.Reflection;
using Percolator.AnalysisServices.Attributes;

namespace Percolator.AnalysisServices.Linq
{
    /// <summary>
    /// Representation of a MDX 'Set'.
    /// </summary>
    public class Set : ICubeObject
    {
        protected List<object> _values;
        /// <summary>
        /// This property stores the value from a returned anonymous object from a query execution.
        /// </summary>
        public object ObjectValue { get; protected set; }
        /// <summary>
        /// The type of the Value property.
        /// </summary>
        public Type ValueType { get; protected set; }
        /// <summary>
        /// Returns the number of cells in a set.
        /// </summary>
        public Member Count { get { return new Member(assembleExtension("Count")); } }
        /// <summary>
        /// Returns the set of children of a specified member.
        /// </summary>
        public Set Children { get { return new Set(assembleExtension("Children")); } }
        /// <summary>
        /// Returns the current tuple from a set during iteration.
        /// </summary>
        public Member Current { get { return new Member(assembleExtension("Current")); } }
        /// <summary>
        /// Returns the current member along a specified hierarchy during iteration.
        /// </summary>
        public Member CurrentMember { get { return new Member(assembleExtension("CurrentMember")); } }
        /// <summary>
        /// Returns the current iteration number within a set during iteration.
        /// </summary>
        public Member CurrentOrdinal { get { return new Member(assembleExtension("CurrentOrdinal")); } }
        /// <summary>
        /// Returns the set of members in a dimension, level, or hierarchy.
        /// </summary>
        public Set Members { get { return new Set(assembleExtension("Members")); } }
        /// <summary>
        /// Returns the hierarchy that contains a specified member, level, or hierarchy.
        /// </summary>
        public Member Dimension { get { return new Member(assembleExtension("Dimension")); } }
        /// <summary>
        /// The named of the Set.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Representation of a MDX 'Set'.
        /// </summary>
        /// <param name="objs">The cube objects to assemble the set.</param>
        public Set(params ICubeObject[] objs)
        {
            _values = new List<object>();
            foreach (object val in objs)
                _values.Add(val);
        }
        internal Set(object value, Type type, string tag = "")
        {
            ObjectValue = value;
            ValueType = type;
        }
        /// <summary>
        /// Representation of a MDX 'Set'.
        /// </summary>
        /// <param name="obj">String representation of a set.</param>
        public Set(string obj)
        {
            _values = new List<object>();
            _values.Add(obj);
        }

        public Member Item(int itemNumber) { return new Member(string.Format("{0}.Item({1})", assembleSet(), itemNumber)); }

        /// <summary>
        /// Returns the MDX syntax for this set.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return assembleSet(); }
        public static implicit operator string(Set set) { return set.ToString(); }
        public static implicit operator bool(Set set) { return true; } 
        public static implicit operator Set(string str) { return new Set(str); }

        public static Set operator *(Set set1, Set set2)
        {
            return new Set(string.Format("{0} * {1}", set1, set2));
        }

        public static Set operator *(Set set, Member member)
        {
            return new Set(string.Format("{0} * {1}", set, member));
        }

        public static Set operator *(Member member, Set set)
        {
            return new Set(string.Format("{0} * {1}", set, member));
        }
       
        public static Set operator &(Measure measure, Set set)
        {
            return new Set(string.Format("({0}, {1})", measure, set));
        }

        public static Set operator &(Set set, Measure measure)
        {
            return new Set(string.Format("({0}, {1})", set, measure));
        }

        public static Set operator &(Set set1, Set set2)
        {
            return new Set(string.Format("({0}, {1})", set1, set2));
        }

        public static Set operator &(Member member, Set set)
        {
            return new Set(string.Format("({0}, {1})", member, set));
        }

        public static Set operator &(Set set, Member member)
        {
            return new Set(string.Format("({0}, {1})", set, member));
        }

        /// <summary>
        /// Factory method to create a new set based on the expression passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setCreator"></param>
        /// <returns></returns>
        public static Set Create<T>(Func<T, Set> setCreator) 
        {
            return setCreator(typeof(T).GetCubeInstance<T>());
        }

        protected string assembleSet()
        {
            if (_values == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();

            if (_values.Count > 1)
                sb.Append("{");

            _values
                .Aggregate((a, b) => String.Format("{0} * {1}"))
                .To(sb.Append);
            
            if (_values.Count > 1)
                sb.Append("}");
            
            return sb.ToString();
        }

        string getAttributeValue(Attribute att)
        {
            return att.GetType().GetCustomAttribute<TagAttribute>().Tag;
        }

        string getAttributeValue(Level level)
        {
            return level.GetType().GetCustomAttribute<TagAttribute>().Tag;
        }

        string assembleExtension(string str)
        {
            return string.Format("{0}.{1}", assembleSet(), str);
        }
    }
}
