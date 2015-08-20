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
    /// Represents an Analysis Services Hierarchy
    /// </summary>
    public abstract class Hierarchy : Set, ICubeObject
    {
        /// <summary>
        /// Returns the default member of a hierarchy.
        /// </summary>
        public Member DefaultMember { get { return new Member(string.Format("{0}.DefaultMember", assembleSet())); } }
        /// <summary>
        /// Returns the unique name of a specified dimension, hierarchy, level, or member.
        /// </summary>
        public Member UniqueName { get { return new Member(string.Format("{0}.UniqueName", assembleSet())); } }
 
        /// <summary>
        /// Creates a new Hierarchy.
        /// </summary>
        /// <param name="tag"></param>
        public Hierarchy(string tag)
            : base(tag) { }
    }
}
