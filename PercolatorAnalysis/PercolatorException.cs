/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices
{
    using System;

    internal class NotImplementedInPAS_Exception : NotImplementedException
    {
        public NotImplementedInPAS_Exception()
        {
        }

        public NotImplementedInPAS_Exception(string message)
            : base(message)
        {
        }

        public NotImplementedInPAS_Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public string ReasonWhy { get; set; }
    }

    internal class PercolatorException : Exception
    {
        public PercolatorException()
        {
        }

        public PercolatorException(string message)
            : base(message)
        {
        }

        public PercolatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    internal sealed class PercolatorQueryExeption : PercolatorException
    {
        public PercolatorQueryExeption()
        {
        }

        public PercolatorQueryExeption(string message)
            : base(message)
        {
        }

        public PercolatorQueryExeption(string message, string query)
            : base(message)
        {
            MdxQuery = query;
        }

        public string MdxQuery { get; private set; }
    }
}
