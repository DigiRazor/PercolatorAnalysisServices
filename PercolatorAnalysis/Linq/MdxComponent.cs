/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Linq
{
    using System;
    using System.Linq.Expressions;

    public enum Component
    {
        Where,
        From,
        SubCube,
        CreatedSet,
        CreatedMember
    }

    public class MdxComponent
    {
        public MdxComponent(Component componentType)
            : this(componentType, null, null)
        {
        }

        public MdxComponent(Component componentType, string name)
            : this(componentType, name, null)
        {
        }

        public MdxComponent(Component componentType, string name, Expression componentAssembler)
        {
            this.ComponentType = componentType;
            this.Creator = componentAssembler;
            this.Name = name;
        }

        public Component ComponentType { get; set; }

        public byte DeclarationOrder { get; set; }

        public string Name { get; set; }

        public byte? Axis { get; set; }

        internal Expression Creator { get; set; }

        public MdxComponent AssembleComponent<T>(Expression<Func<T, object>> componentAssembler)
        {
            Creator = componentAssembler;
            return this;
        }
    }
}
