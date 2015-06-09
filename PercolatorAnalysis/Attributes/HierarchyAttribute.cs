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
    /// Tags a property as an Analysis Services user hierarchy
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HierarchyAttribute : global::System.Attribute
    {
        
    }
}
