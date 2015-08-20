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
    /// Tags the class as an Analysis Services cube.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CubeAttribute : global::System.Attribute
    {
        public string Tag { get; set; }
    }
}
