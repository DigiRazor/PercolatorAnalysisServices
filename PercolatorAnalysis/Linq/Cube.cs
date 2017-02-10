/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;

    using Microsoft.AnalysisServices.AdomdClient;

    using Percolator.AnalysisServices;

    /// <summary>
    /// Where all the magic happens - The main IMdxQueryable object to run against LINQ queries.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cube<T> : IMdxQueryable<T>
    {
        IMdxProvider _provider;

        Expression _expression;

        List<Axis<T>> _axisGroups;

        List<MdxComponent> _components;

        byte _createdDepth;

        public IMdxProvider Provider { get { return _provider; } }

        /// <summary>
        /// Creates a new Cube to query against.
        /// </summary>
        /// <param name="provider">The MdxProvider associated with this cube.</param>
        public Cube(IMdxProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            _provider = provider;
            _createdDepth = 0;
            _expression = Expression.Constant(this);
            _axisGroups = new List<Axis<T>>();
            _components = new List<MdxComponent>();
        }

        #region IMdxQueryable<T> Members
        /// <summary>
        /// The current collection of axes waiting to be queried against.
        /// </summary>
        public List<Axis<T>> AxisCollection { get { return _axisGroups; } }

        /// <summary>
        /// The current collection of Mdx components (Slicers, Subcubes, etc) that are waiting to be queried against.
        /// </summary>
        public List<MdxComponent> Components { get { return _components; } }

        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried.</param>
        /// <returns>This IMdxQueryable object.</returns>
        public IMdxQueryable<T> OnAxis(byte axisNumber, Expression<Func<T, ICubeObject>> axisObjects)
        {
            if (axisNumber > 127)
                throw new PercolatorException("Axis max is 128");
            return OnAxis((byte)axisNumber, false, axisObjects);
        }

        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried.</param>
        /// <returns>This IMdxQueryable object.</returns>
        public IMdxQueryable<T> OnAxis(byte axisNumber, Expression<Func<T, IEnumerable<ICubeObject>>> axisObjects)
        {
            if (axisNumber > 127)
                throw new PercolatorException("Axis max is 128");
            return OnAxis(axisNumber, false, axisObjects);
        }

        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="isNonEmpty">Specifies whether the axis should be queried as "NON EMPTY".</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried</param>
        /// <returns>This IMdxQueryable object.</returns>
        public IMdxQueryable<T> OnAxis(byte axisNumber, bool isNonEmpty, Expression<Func<T, ICubeObject>> axisObjects)
        {
            if (axisNumber > 127)
                throw new PercolatorException("Axis max is 128");
            var axis = new Axis<T>(axisNumber, isNonEmpty, axisObjects);
            _axisGroups.Add(axis);
            return this;
        }

        /// <summary>
        /// Applies mdx objects to an axis and stores the axis in this object to be queried.
        /// </summary>
        /// <param name="axisNumber">The axis number, or axis specification (ie Axis.COLUMNS).</param>
        /// <param name="isNonEmpty">Specifies whether the axis should be queried as "NON EMPTY".</param>
        /// <param name="axisObjects">An expression to build the objects that will be present when the axis is queried</param>
        /// <returns>This IMdxQueryable object.</returns>
        public IMdxQueryable<T> OnAxis(byte axisNumber, bool isNonEmpty, Expression<Func<T, IEnumerable<ICubeObject>>> axisObjects)
        {
            if (axisNumber > 127)
                throw new PercolatorException("Axis max is 128");
            var axis = new Axis<T>(axisNumber, isNonEmpty, axisObjects);
            _axisGroups.Add(axis);
            return this;
        }

        /// <summary>
        /// Applies the "WHERE" slicer to the Mdx query.
        /// </summary>
        /// <param name="slicers">The expression to build the slicer statement.</param>
        /// <returns></returns>
        public IMdxQueryable<T> Slice(Expression<Func<T, ICubeObject>> slicers)
        {
            _components.Add(new MdxComponent(Component.Where, null, slicers));
            return this;
        }

        /// <summary>
        /// Applies the "WHERE" slicer to the Mdx query.
        /// </summary>
        /// <param name="slicers">The expression to build the slicer statement.</param>
        /// <returns></returns>
        public IMdxQueryable<T> Slice(Expression<Func<T, IEnumerable<ICubeObject>>> slicers)
        {
            _components.Add(new MdxComponent(Component.Where, null, slicers));
            return this;
        }

        /// <summary>
        /// Introduces a new query scoped calculated member and stores it to be queried.
        /// </summary>
        /// <param name="name">The name of the calculated member.</param>
        /// <param name="axisNumber">The axis number this query should be queried in. If the value is null, it will not be placed on any axis.</param>
        /// <param name="memberCreator">The expression to create this calculated member.</param>
        /// <returns></returns>
        public IMdxQueryable<T> WithMember(string name, byte? axisNumber, Expression<Func<T, Member>> memberCreator)
        {
            var comp = new MdxComponent(Component.CreatedMember, name, memberCreator);
            comp.Axis = axisNumber;
            comp.DeclarationOrder = _createdDepth++;
            _components.Add(comp);
            return this;
        }

        /// <summary>
        /// Introduces a new query scoped calculated set and stores it to be queried.
        /// </summary>
        /// <param name="name">The name of the calculated set.</param>
        /// <param name="axisNumber">The axis number this query should be queried in. If the value is null, it will not be placed on any axis.</param>
        /// <param name="setCreator">The expression to create this calculated set.</param>
        /// <returns></returns>
        public IMdxQueryable<T> WithSet(string name, byte? axisNumber, Expression<Func<T, Set>> setCreator)
        {
            var comp = new MdxComponent(Component.CreatedSet, name, setCreator);
            comp.Axis = axisNumber;
            comp.DeclarationOrder = _createdDepth++;
            _components.Add(comp);
            return this;
        }

        /// <summary>
        /// Introduces a sub cube that will be used in the query.
        /// </summary>
        /// <param name="subCube">The expression to create the sub cube.</param>
        /// <returns></returns>
        public IMdxQueryable<T> FromSubCube(Expression<Func<T, ICubeObject>> subCube)
        {
            var comp = new MdxComponent(Component.SubCube);
            comp.Creator = subCube;
            _components.Add(comp);
            return this;
        }

        /// <summary>
        /// Introduces a sub cube that will be used in the query.
        /// </summary>
        /// <param name="subCube">The expression to create the sub cube.</param>
        /// <returns></returns>
        public IMdxQueryable<T> FromSubCube(Expression<Func<T, IEnumerable<ICubeObject>>> subCube)
        {
            var comp = new MdxComponent(Component.SubCube);
            comp.Creator = subCube;
            _components.Add(comp);
            return this;
        }

        /// <summary>
        /// The magic word. Executes this cube's objects in an MDX query against the analysis services, and maps the results to the type applied to the generic parameter. 
        /// </summary>
        /// <typeparam name="T_MapTo">The type to map the results of the query to.</typeparam>
        /// <param name="clearQueryContents">Optional boolean to indicate whether to clear this object's query objects after the query is executed.</param>
        /// <returns>An IEnumerable of the type specified.</returns>
        public IEnumerable<T_MapTo> Percolate<T_MapTo>(bool clearQueryContents = true) where T_MapTo : new()
        {
            var lator = new Percolator<T>(_axisGroups, _components);
            var command = lator.MdxCommand;
            //var cellSet = this._provider.GetCellSet(command);
            var reader = _provider.GetReader(command);
            if (clearQueryContents)
                Clear();

            var mapper = new Mapperlator<T_MapTo>(reader);

            return mapper;
        }

        public Member CreateMember(string name, Func<T, Member> memberCreator)
        {
            var mem = Member.Create<T>(memberCreator);
            mem.Tag = name;
            return mem;
        }

        public Set CreateSet(string name, Func<T, Set> setCreator)
        {
            var set = Set.Create<T>(setCreator);
            set.Tag = name;
            return set;
        }

        public CellSet ExecuteCellSet(bool clearQueryContents = true)
        {
            var lator = new Percolator<T>(_axisGroups, _components);
            var command = lator.MdxCommand;
            if (clearQueryContents)
                Clear();
            return _provider.GetCellSet(command);
        }

        public DataTable ExecuteDataTable(bool clearQueryContents = true)
        {
            var lator = new Percolator<T>(_axisGroups, _components);
            var command = lator.MdxCommand;
            if (clearQueryContents)
                Clear();
            return _provider.GetDataTable(command);
        }

        /// <summary>
        /// Returns the string of the translated MDX query.
        /// </summary>
        /// <returns></returns>
        public string TranslateToMdx()
        {
            return new Percolator<T>(_axisGroups, _components).MdxCommand;
        }

        /// <summary>
        /// Clears this object's stored query axes and components.
        /// </summary>
        public void Clear()
        {
            _axisGroups.Clear();
            _components.Clear();
        }
        #endregion

        /// <summary>
        /// Overridden ToString returns a translated query.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TranslateToMdx();
        }
    }
}