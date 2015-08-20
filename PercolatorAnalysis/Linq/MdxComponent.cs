/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

using System;
using System.Linq.Expressions;

namespace Percolator.AnalysisServices.Linq
{
    public enum Component { Where, From, SubCube, CreatedSet, CreatedMember }
    public class MdxComponent
    {
        public Component ComponentType { get; set; }
        internal Expression Creator { get; set; }
        public byte DeclarationOrder { get; set; }
        public string Name { get; set; }
        public byte? Axis { get; set; }

        public MdxComponent(Component componentType)
            : this(componentType, null, null) { }

        public MdxComponent(Component componentType, string name)
            : this(componentType, name, null) { }

        public MdxComponent(Component componentType, string name, Expression componentAssembler)
        {
            ComponentType = componentType;
            Creator = componentAssembler;
            Name = name;
        }

        public MdxComponent AssembleComponent<T>(Expression<Func<T, object>> componentAssembler)
        {
            Creator = componentAssembler;
            return this;
        }
    }
}
