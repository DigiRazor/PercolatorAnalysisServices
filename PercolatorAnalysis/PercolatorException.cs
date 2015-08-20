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

namespace Percolator.AnalysisServices
{
    internal class NotImplementedInPAS_Exception : NotImplementedException
    {
        public string ReasonWhy { get; set; }
        public NotImplementedInPAS_Exception() { }

        public NotImplementedInPAS_Exception(string message)
            : base(message) { }

        public NotImplementedInPAS_Exception(string message, Exception innerException)
            : base(message, innerException) { }
    }

    internal class PercolatorException : Exception
    {
        public PercolatorException() { }

        public PercolatorException(string message)
            : base(message) { }

        public PercolatorException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    internal sealed class PercolatorQueryExeption : PercolatorException
    {
        public string MdxQuery { get; private set; }
        public PercolatorQueryExeption() { }
        
        public PercolatorQueryExeption(string message)
            : base(message) { }

        public PercolatorQueryExeption(string message, string query)
            : base (message)
        {
            MdxQuery = query;
        }
    }
}
