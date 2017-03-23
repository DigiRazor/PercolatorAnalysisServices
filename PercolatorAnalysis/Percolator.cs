/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using Percolator.AnalysisServices.Attributes;
    using Percolator.AnalysisServices.Linq;

    internal class Translation
    {
        public Translation(byte type, string value)
            : this(type, value, false)
        {
        }

        public Translation(byte type, string value, bool isNonEmpty)
        {
            this.Type = type;
            this.Value = value;
            this.IsNonEmpty = isNonEmpty;
        }

        public byte Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public byte? DeclarationOrder { get; set; }

        public bool IsNonEmpty { get; set; }
    }

    internal class Percolator<T>
    {
        private const byte _FROM = 191;
        private const byte _WHERE = 192;
        private const byte _WMEMBER = 193;
        private const byte _WSET = 194;
        private const byte _SUBCUBE = 195;
        private byte _setDepth;
        private byte _memberDepth;
        private T _cube;
        private StringBuilder _sb;
        private List<Axis<T>> _axis;
        private Axis<T> _currentAxisObject;
        private List<MdxComponent> _components;
        private List<Translation> _translations;
        private byte _currentAxis;
        private Component? _currentComponent;

        internal Percolator(List<Axis<T>> axis, List<MdxComponent> components)
        {
            this._cube = typeof(T).GetCubeInstance<T>();

            this._setDepth = 0;
            this._memberDepth = 0;
            this._sb = new StringBuilder();
            this._translations = new List<Translation>();
            this._components = components;
            this._axis = axis;
            this._currentAxis = axis.Count == 0 ? (byte)0 : axis.Min(x => x.AxisNumber);
            this._currentComponent = null;
            this.MdxCommand = this.translate();
        }

        public string MdxCommand { get; private set; }

        private string translate()
        {
            try
            {
                var from = typeof(T).GetCustomAttribute<CubeAttribute>().Tag;
                this._translations.Add(new Translation(_FROM, string.Format("FROM [{0}]", from)));
            }
            catch (NullReferenceException e)
            {
                throw new PercolatorException(string.Format("The cube type of '{0}' is not queryable", typeof(T).Name));
            }

            this._axis.ForEach(axis =>
            {
                this._currentAxis = axis.AxisNumber;
                this._currentAxisObject = axis;
                this.Evaluate(axis.Creator);
            });
            this._currentAxis = 190;
            this._components.ForEach(component =>
            {
                this._currentComponent = component.ComponentType;
                this._currentAxis = this.getComponentValue(component.ComponentType);
                this.Evaluate(component.Creator);
            });

            return this.assembleTranslations();
        }

        private void Evaluate(Expression node)
        {
            switch (this._currentComponent)
            {
                case Component.Where:
                    this.prepareWhere(node);
                    break;

                case Component.CreatedMember:
                    this.prepareCalculatedMember(node);
                    break;

                case Component.CreatedSet:
                    this.prepareCalculatedSet(node);
                    break;

                case Component.SubCube:
                    this.prepareSubCube(node);
                    break;

                default:
                    this.prepareAxis(node);
                    break;
            }
        }

        private void prepareWhere(Expression node)
        {
            var obj = node.GetValue<T>();
            this.setTranslation(obj);
        }

        private void prepareCalculatedMember(Expression node)
        {
            var memberExp = ((LambdaExpression)node).Body as MemberExpression;
            var obj = node.GetValue<T>();
            var comp = this._components.First(x => x.Creator == node && x.ComponentType == this._currentComponent);
            var name = string.Empty;
            if (!this.tryGetTagName(memberExp, out name))
            {
                if (string.IsNullOrEmpty(comp.Name))
                {
                    name = $"_member{this._memberDepth++}";
                }
                else
                {
                    name = comp.Name;
                }
            }
            if (comp.Axis.HasValue)
            {
                var axis = this._axis.FirstOrDefault(x => x.AxisNumber == comp.Axis);
                if (axis != null)
                {
                    if (!axis.WithMembers.Contains(name))
                    {
                        axis.WithMembers.Add(name);
                    }
                }
                else
                {
                    var a = new Axis<T>(comp.Axis.Value);
                    a.WithMembers.Add(name);
                    this._axis.Add(a);
                }
            }

            this._translations.Add(new Translation(this._currentAxis, obj.ToString()) { Name = name });            
        }

        private void prepareCalculatedSet(Expression node)
        {
            var memberExp = ((LambdaExpression)node).Body as MemberExpression;
            var obj = node.GetValue<T>();
            var comp = this._components.First(x => x.Creator == node && x.ComponentType == this._currentComponent);
            var name = string.Empty;
            if (!this.tryGetTagName(memberExp, out name))
            {
                if (string.IsNullOrEmpty(comp.Name))
                {
                    name = $"_set{this._setDepth++}";
                }
                else
                {
                    name = comp.Name;
                }
            }

            if (comp.Axis.HasValue)
            {
                var axis = this._axis.FirstOrDefault(x => x.AxisNumber == comp.Axis);
                if (axis != null)
                {
                    if (!axis.WithSets.Contains(name))
                    {
                        axis.WithSets.Add(name);
                    }
                }
                else
                {
                    var a = new Axis<T>(comp.Axis.Value);
                    a.WithSets.Add(name);
                    this._axis.Add(a);
                }
            }

            this._translations.Add(new Translation(this._currentAxis, obj.ToString()) { Name = name });    
        }

        private void prepareSubCube(Expression node)
        {
            var val = node.GetValue<T>();
            var currentSubcube = this._translations.FirstOrDefault(x => x.Type == _SUBCUBE);

            if (val is IEnumerable<ICubeObject>)
            {
                var concact = ((IEnumerable<ICubeObject>)val)
                    .Select(x => x.ToString())
                    .Aggregate((a, b) => $"{a}, {b}");

                if (currentSubcube != null)
                {
                    currentSubcube.Value = concact;
                }
                else
                {
                    new Translation(this.getComponentValue(Component.SubCube), concact)
                        .Finally(this._translations.Add);
                }
            }
            else
            {
                if (currentSubcube != null)
                {
                    currentSubcube.Value = val.ToString();
                }
                else
                {
                    new Translation(this.getComponentValue(Component.SubCube), val.ToString())
                        .Finally(this._translations.Add);
                }
            }
        }

        private void prepareAxis(Expression node)
        {
            var obj = node.GetValue<T>();
            var axis = this._axis.FirstOrDefault(x => x.AxisNumber == this._currentAxis);
            this.setTranslation(obj, this._currentAxisObject.IsNonEmpty);
        }

        private void setTranslation(object obj)
        {
            if (obj != null)
            {
                if (obj is IEnumerable<ICubeObject>)
                {
                    foreach (var o in (IEnumerable<ICubeObject>)obj)
                    {
                        this._translations.Add(new Translation(this._currentAxis, o.ToString()));
                    }
                }
                else
                {
                    this._translations.Add(new Translation(this._currentAxis, obj.ToString()));
                }
            }
        }

        private void setTranslation(object obj, bool isNonEmpty)
        {
            if (obj != null)
            {
                if (obj is IEnumerable<ICubeObject>)
                {
                    foreach (var o in (IEnumerable<ICubeObject>)obj)
                    {
                        this._translations.Add(new Translation(this._currentAxis, o.ToString(), isNonEmpty));
                    }
                }
                else
                {
                    this._translations.Add(new Translation(this._currentAxis, obj.ToString(), isNonEmpty));
                }
            }
        }

        private byte getComponentValue(Component component)
        {
            switch (component)
            {
                case Component.From:
                    return _FROM;

                case Component.Where:
                    return _WHERE;

                case Component.CreatedMember:
                    return _WMEMBER;

                case Component.CreatedSet:
                    return _WSET;

                case Component.SubCube:
                    return _SUBCUBE;

                default:
                    throw new PercolatorException(
                        $"The component type '{component}' durring the PAS tanslation is not valid");
            }
        }

        private bool tryGetTagName(MemberExpression member, out string name)
        {
            if (member == null)
            {
                name = null;
                return false;
            }

            var tagAtt = member.Member.GetCustomAttribute<TagAttribute>();
            if (tagAtt != null)
            {
                name = tagAtt.Tag;
                return true;
            }

            name = null;
            return false;
        }

        private string assembleTranslations()
        {
            var sb = new StringBuilder(Comment.PAS_HEADER).AppendLine();
            sb.AppendLine();
            var members = this._translations.Where(x => x.Type == _WMEMBER);
            var sets = this._translations.Where(x => x.Type == _WSET);
            var combined = members.Union(sets).OrderBy(x => x.DeclarationOrder);
            
            if (combined.Count() > 0)
            {
                sb.AppendLine(Comment.FOR_CREATED_REGION);
                sb.AppendLine("WITH");
                foreach (var com in combined.OrderBy(x => x.DeclarationOrder))
                {
                    var type = com.Type == _WMEMBER ? "MEMBER" : "SET";
                    if (com.Name.Contains("_set"))
                    {
                        sb.AppendLine(Comment.FOR_NO_SET_NAME);
                    }

                    if (com.Name.Contains("_member"))
                    {
                        sb.AppendLine(Comment.FOR_NO_MEMBER_NAME);
                    }
                    sb.AppendLine($"{type} {com.Name} AS");
                    sb.AppendLine(com.Value);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(Comment.FOR_SELECT_REGION);
            sb.AppendLine("SELECT");

            this._axis
                .OrderBy(x => x.AxisNumber)
                .Select(x => x.ToString())
                .Aggregate((a, b) => $"{a},\r\n{b}")
                .To(sb.AppendLine);

            sb.AppendLine(Comment.FOR_FROM_REGION);
            var subCube = this._translations.FirstOrDefault(x => x.Type == _SUBCUBE);
            if (subCube != null)
            {
                sb.AppendLine("FROM")
                    .AppendLine("(")
                    .AppendLine("\tSELECT")
                    .AppendLine("\t{0}", subCube.Value)
                    .AppendLine($"\tON 0 {this._translations.First(x => x.Type == _FROM).Value}")
                    .AppendLine(")");
            }
            else
            {
                sb.AppendLine(this._translations.First(x => x.Type == _FROM).Value);
            }

            var slicers = this._translations.Where(x => x.Type == _WHERE);
            var slicerCount = slicers.Count();
            if (slicerCount > 0)
            {
                sb.AppendLine(Comment.FOR_SLICER_REGION);
                sb.AppendLine("WHERE\r\n(");

                slicers
                    .Select(x => x.Value)
                    .Aggregate((a, b) => $"\t{a},\r\n\t{b}")
                    .To(sb.AppendLine)
                    .To(s => s.AppendLine(")"));
            }

            return sb.ToString();
        }
    }
}
