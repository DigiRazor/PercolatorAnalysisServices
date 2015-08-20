/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Attributes
{
    using System;
    /// <summary>
    /// Stores the information for an attribute or level.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class TagAttribute : global::System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TagAttribute() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public TagAttribute(string tag)
        {
            Tag = tag;
            Ordinal = 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ordinal"></param>
        public TagAttribute(string tag, int ordinal)
        {
            Tag = tag;
            Ordinal = ordinal;
        }
    }
}
