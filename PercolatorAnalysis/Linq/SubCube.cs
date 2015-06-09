/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices.Linq
{
    using Percolator.AnalysisServices;
    /// <summary>
    /// SubeCube class to implement querying from a sub cube in a LINQ to MDX query.
    /// </summary>
    /// <typeparam name="T">The name of the cube to query.</typeparam>
    public class SubCube<T> : IQueryable<T>, ICubeObject
    {
        Expression _expression;
        MdxProvider _provider;
        /// <summary>
        /// The translated query string from the query expression.
        /// </summary>
        public string SubQuery { get; private set; }
        /// <summary>
        /// Creates a new SubCube object.
        /// </summary>
        /// <param name="subQuery">The linq query that represents the sub cube selection query.</param>
        public SubCube(IQueryable subQuery)
        {
            this._expression = Expression.Constant(this);
            this._provider = new Providerlator(CubeBase.ConnectionString);
            this.SubQuery = new Translator(subQuery.Expression, false, false, true).MdxCommand;
        }
        /// <summary>
        /// Empty sub cube creation.
        /// </summary>
        public SubCube() { }
        /// <summary>
        /// string conversion for this sub cube.
        /// </summary>
        /// <param name="subCube"></param>
        /// <returns></returns>
        public static implicit operator string(SubCube<T> subCube) { return subCube.SubQuery; }
        /// <summary>
        /// bool conversion for this sub cube.
        /// </summary>
        /// <param name="subCube"></param>
        /// <returns></returns>
        public static implicit operator bool(SubCube<T> subCube) { return true; }
        /// <summary>
        /// Overridden ToString that returns this sub cube's query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return this.SubQuery; }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public Expression Expression
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotImplementedException(); }
        }
    }
}
