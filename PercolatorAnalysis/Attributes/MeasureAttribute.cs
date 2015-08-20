/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices.Attributes
{
    using System;
    /// <summary>
    /// Tags a property as an Analysis Services measure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MeasureAttribute : global::System.Attribute
    {
        /// <summary>
        /// The name of the measure.
        /// </summary>
        public string Tag { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public MeasureAttribute() { }
        /// <summary>
        /// Creates a new PAS 'MeasureAttribute'.
        /// </summary>
        /// <param name="tag">The name of the measure.</param>
        public MeasureAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
