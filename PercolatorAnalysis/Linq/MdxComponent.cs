using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            this.ComponentType = componentType;
            this.Creator = componentAssembler;
            this.Name = name;
        }

        public MdxComponent AssembleComponent<T>(Expression<Func<T, object>> componentAssembler)
        {
            this.Creator = componentAssembler;
            return this;
        }
    }
}
