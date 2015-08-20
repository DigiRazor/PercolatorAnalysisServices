/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Percolator.AnalysisServices.Linq
{
    /// <summary>
    /// Implement to create a PAS IMdxQueryable object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMdxQueryable<T> 
    {
        IMdxProvider Provider { get; }
        /// <summary>
        /// The current collection of axes waiting to be queried against.
        /// </summary>
        List<Axis<T>> AxisCollection { get; }
        /// <summary>
        /// The current collection of Mdx components (Slicers, Subcubes, etc) that are waiting to be queried against.
        /// </summary>
        List<MdxComponent> Components { get; }
        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried.</param>
        /// <returns>This IMdxQueryable object.</returns>
        IMdxQueryable<T> OnAxis(byte axisNumber, Expression<Func<T, ICubeObject>> axisObjects);
        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried.</param>
        /// <returns>This IMdxQueryable object.</returns>
        IMdxQueryable<T> OnAxis(byte axisNumber, Expression<Func<T, IEnumerable<ICubeObject>>> axisObjects);
        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="isNonEmpty">Specifies whether the axis should be queried as "NON EMPTY".</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried</param>
        /// <returns>This IMdxQueryable object.</returns>
        IMdxQueryable<T> OnAxis(byte axisNumber, bool isNonEmpty, Expression<Func<T, ICubeObject>> axisObjects);
        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="isNonEmpty">Specifies whether the axis should be queried as "NON EMPTY".</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried</param>
        /// <returns>This IMdxQueryable object.</returns>
        IMdxQueryable<T> OnAxis(byte axisNumber, bool isNonEmpty, Expression<Func<T, IEnumerable<ICubeObject>>> axisObjects);
        /// <summary>
        /// Applies the "WHERE" slicer to the Mdx query.
        /// </summary>
        /// <param name="slicers">The expression to build the slicer statement.</param>
        /// <returns></returns>
        IMdxQueryable<T> Slice(Expression<Func<T, ICubeObject>> slicers);
        /// <summary>
        /// Applies the "WHERE" slicer to the Mdx query.
        /// </summary>
        /// <param name="slicers">The expression to build the slicer statement.</param>
        /// <returns></returns>
        IMdxQueryable<T> Slice(Expression<Func<T, IEnumerable<ICubeObject>>> slicers);
        /// <summary>
        /// Introduces a new query scoped calculated member and stores it to be queried.
        /// </summary>
        /// <param name="name">The name of the calculated member.</param>
        /// <param name="axisNumber">The axis number this query should be queried in. If the value is null, it will not be placed on any axis.</param>
        /// <param name="memberCreator">The expression to create this calculated member.</param>
        /// <returns></returns>
        IMdxQueryable<T> WithMember(string name, byte? axisNumber, Expression<Func<T, Member>> memberCreator);
        /// <summary>
        /// Introduces a new query scoped calculated set and stores it to be queried.
        /// </summary>
        /// <param name="name">The name of the calculated set.</param>
        /// <param name="axisNumber">The axis number this query should be queried in. If the value is null, it will not be placed on any axis.</param>
        /// <param name="setCreator">The expression to create this calculated set.</param>
        /// <returns></returns>
        IMdxQueryable<T> WithSet(string name, byte? axisNumber, Expression<Func<T, Set>> setCreator);
        /// <summary>
        /// Introduces a sub cube that will be used in the query.
        /// </summary>
        /// <param name="subCube">The expression to create the sub cube.</param>
        /// <returns></returns>
        IMdxQueryable<T> FromSubCube(Expression<Func<T, ICubeObject>> subCube);
        /// <summary>
        /// Introduces a sub cube that will be used in the query.
        /// </summary>
        /// <param name="subCube">The expression to create the sub cube.</param>
        /// <returns></returns>
        IMdxQueryable<T> FromSubCube(Expression<Func<T, IEnumerable<ICubeObject>>> subCube);
        /// <summary>
        /// The magic word. Executes this cube's objects in an MDX query against the analysis services, and maps the results to the type applied to the generic parameter. 
        /// </summary>
        /// <typeparam name="T_MapTo">The type to map the results of the query to.</typeparam>
        /// <param name="clearQueryContents">Optional boolean to indicate whether to clear this object's query objects after the query is executed.</param>
        /// <returns>An IEnumerable of the type specified.</returns>
        IEnumerable<T_MapTo> Percolate<T_MapTo>(bool clearQueryContents = true) where T_MapTo : new();
        CellSet ExecuteCellSet(bool clearQueryContents = true);
        DataTable ExecuteDataTable(bool clearQueryContents = true);
        /// <summary>
        /// Clears this object's stored query axes and components.
        /// </summary>
        void Clear();
        /// <summary>
        /// Returns the string of the translated MDX query.
        /// </summary>
        /// <returns></returns>
        string TranslateToMdx();
    }
}
