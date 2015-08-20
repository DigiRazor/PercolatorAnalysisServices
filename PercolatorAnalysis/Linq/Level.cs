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
    /// <summary>
    /// Represents an Analysis Services hierarchy level.
    /// </summary>
    public class Level : Hierarchy
    {
        /// <summary>
        /// The level's ordinal position in this hierachy.
        /// </summary>
        public int OrdinalLevel { get; private set; }
        /// <summary>
        /// Evaluates either a hierarchy or a level expression and returns a set that contains all members of the specified hierarchy or level, 
        /// which includes all calculated members in the hierarchy or level.
        /// </summary>
        public Set AllMembers { get { return new Set(string.Format("{0}.AllMembers", assembleSet())); } }
        /// <summary>
        /// Returns the hierarchy that contains a specified member or level.
        /// </summary>
        public Set Hierarchy { get { return new Set(string.Format("{0}.Hierarchy", assembleSet())); } }
        /// <summary>
        /// C# Indexer representing the member brackets in an MDX query. 
        /// </summary>
        /// <param name="hierarchyMemberNames">The members of the level. Chain the members together to create the entire hierarchy level member.</param>
        /// <returns></returns>
        public Member this[params string[] hierarchyMemberNames] { get { return memberFrom(hierarchyMemberNames); } }

        /// <summary>
        /// Creates a new level.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ordinalLevel"></param>
        public Level(string tag, int ordinalLevel)
            : base(tag)
        {
            OrdinalLevel = ordinalLevel;
        }

        internal Level(object value, Type type, string tag)
            :base(tag)
        {
            ObjectValue = value;
            ValueType = type;
        }

        /// <summary>
        /// Creates a new level.
        /// </summary>
        /// <param name="str"></param>
        public Level(string str)
            : base (str)
        {
            OrdinalLevel = default(int);
        }

        /// <summary>
        /// Implict string conversion for the level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static implicit operator string(Level level)
        {
            return level.assembleSet();
        }

        public static implicit operator Level(string str)
        {
            return new Level(str);
        }
        
        /// <summary>
        /// Overridden to string returns this level's tag.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return assembleSet();
        }

        string assembleExtension(string str)
        {
            return string.Format("{0}.{1}", assembleSet(), str);
        }

        Member memberFrom(string[] memberNames)
        {
            string att = assembleSet();
            var members = new List<string>(memberNames.Length);
            var sb = new StringBuilder(att);
            foreach (var value in memberNames)
            {
                var val = value.Replace("[", "").Replace("]", "");
                if (val.StartsWith("&"))
                    val = string.Format(".&[{0}]", val.Substring(1));
                else
                    val = string.Format(".[{0}]", val);
                sb.Append(val);
            }
            return new Member(sb.ToString());
        }
    }
}
