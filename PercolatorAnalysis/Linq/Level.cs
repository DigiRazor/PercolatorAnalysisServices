/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Text;

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
        public Set AllMembers => $"{assembleSet()}.AllMembers";
        /// <summary>
        /// Returns the hierarchy that contains a specified member or level.
        /// </summary>
        public Set Hierarchy => $"{assembleSet()}.Hierarchy";
        /// <summary>
        /// C# Indexer representing the member brackets in an MDX query. 
        /// </summary>
        /// <param name="hierarchyMemberNames">The members of the level. Chain the members together to create the entire hierarchy level member.</param>
        /// <returns></returns>
        public Member this[params string[] hierarchyMemberNames] => memberFrom(hierarchyMemberNames); 

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
        public static implicit operator string(Level level) => level.assembleSet();

        public static implicit operator Level(string str) => new Level(str);
        
        /// <summary>
        /// Overridden to string returns this level's tag.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => assembleSet();

        string assembleExtension(string str) => $"{assembleSet()}.{str}";

        Member memberFrom(string[] memberNames)
        {
            string att = assembleSet();
            var members = new List<string>(memberNames.Length);
            var sb = new StringBuilder(att);
            foreach (var value in memberNames)
            {
                var val = value.Replace("[", "").Replace("]", "");
                if (val.StartsWith("&"))
                    val = $".&[{val.Substring(1)}]";
                else
                    val = $".[{val}]";
                sb.Append(val);
            }
            return new Member(sb.ToString());
        }
    }
}
