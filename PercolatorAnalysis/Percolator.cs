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
using System.Reflection;
using Percolator.AnalysisServices.Attributes;
using Percolator.AnalysisServices.Linq;

namespace Percolator.AnalysisServices
{
    internal class Translation
    {
        public byte Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public byte? DeclarationOrder { get; set; }
        public bool IsNonEmpty { get; set; }

        public Translation(byte type, string value)
            : this(type, value, false) { }

        public Translation(byte type, string value, bool isNonEmpty)
        {
            Type = type;
            Value = value;
            IsNonEmpty = isNonEmpty;
        }
    }

    internal class Percolator<T>
    {
        const byte _FROM = 191;
        const byte _WHERE = 192;
        const byte _WMEMBER = 193;
        const byte _WSET = 194;
        const byte _SUBCUBE = 195;
        byte _setDepth;
        byte _memberDepth;
        T _cube;
        StringBuilder _sb;
        List<Axis<T>> _axis;
        Axis<T> _currentAxisObject;
        List<MdxComponent> _components;
        List<Translation> _translations;
        byte _currentAxis;
        Component? _currentComponent;

        public string MdxCommand { get; private set; }

        internal Percolator(List<Axis<T>> axis, List<MdxComponent> components)
        {
            _cube = typeof(T).GetCubeInstance<T>();

            _setDepth = 0;
            _memberDepth = 0;
            _sb = new StringBuilder();
            _translations = new List<Translation>();
            _components = components;
            _axis = axis;
            _currentAxis = axis.Count == 0 ? (byte)0 : axis.Min(x => x.AxisNumber);
            _currentComponent = null;
            MdxCommand = translate();
        }

        string translate()
        {
            try
            {
                string from = typeof(T).GetCustomAttribute<CubeAttribute>().Tag;
                _translations.Add(new Translation(_FROM, string.Format("FROM [{0}]", from)));
            }
            catch(NullReferenceException e)
            {
                throw new PercolatorException(string.Format("The cube type of '{0}' is not queryable", typeof(T).Name));
            }

            _axis.ForEach(axis =>
            {
                _currentAxis = axis.AxisNumber;
                _currentAxisObject = axis;
                Evaluate(axis.Creator);
            });
            _currentAxis = 190;
            _components.ForEach(component =>
            {
                _currentComponent = component.ComponentType;
                _currentAxis = getComponentValue(component.ComponentType);
                Evaluate(component.Creator);
            });

            return assembleTranslations();
        }

        void Evaluate(Expression node)
        {
            switch (_currentComponent)
            {
                case Component.Where:
                    prepareWhere(node);
                    break;

                case Component.CreatedMember:
                    prepareCalculatedMember(node);
                    break;

                case Component.CreatedSet:
                    prepareCalculatedSet(node);
                    break;

                case Component.SubCube:
                    prepareSubCube(node);
                    break;

                default:
                    prepareAxis(node);
                    break;
            }
        }

        void prepareWhere(Expression node)
        {
            var obj = node.GetValue<T>();
            setTranslation(obj);
        }

        void prepareCalculatedMember(Expression node)
        {
            var memberExp = ((LambdaExpression)node).Body as MemberExpression;
            var obj = node.GetValue<T>();
            var comp = _components.First(x => x.Creator == node && x.ComponentType == _currentComponent);
            string name = string.Empty;
            if (!tryGetTagName(memberExp, out name))
            {
                if (string.IsNullOrEmpty(comp.Name))
                    name = $"_member{_memberDepth++}";
                else
                    name = comp.Name;
            }
            if (comp.Axis.HasValue)
            {
                var axis = _axis.FirstOrDefault(x => x.AxisNumber == comp.Axis);
                if (axis != null)
                {
                    if(!axis.WithMembers.Contains(name))
                        axis.WithMembers.Add(name);
                }
                else
                {
                    var a = new Axis<T>(comp.Axis.Value);
                    a.WithMembers.Add(name);
                    _axis.Add(a);
                }
            }
            _translations.Add(new Translation(_currentAxis, obj.ToString()) { Name = name });            
        }

        void prepareCalculatedSet(Expression node)
        {
            var memberExp = ((LambdaExpression)node).Body as MemberExpression;
            var obj = node.GetValue<T>();
            var comp = _components.First(x => x.Creator == node && x.ComponentType == _currentComponent);
            string name = string.Empty;
            if(!tryGetTagName(memberExp, out name))
            {
                if (string.IsNullOrEmpty(comp.Name))
                    name = $"_set{_setDepth++}";
                else
                    name = comp.Name;
            }
            if(comp.Axis.HasValue)
            {
                var axis = _axis.FirstOrDefault(x => x.AxisNumber == comp.Axis);
                if (axis != null)
                {
                    if(!axis.WithSets.Contains(name))
                        axis.WithSets.Add(name);
                }
                else
                {
                    var a = new Axis<T>(comp.Axis.Value);
                    a.WithSets.Add(name);
                    _axis.Add(a);
                }
            }
            _translations.Add(new Translation(_currentAxis, obj.ToString()) { Name = name });    
        }

        void prepareSubCube(Expression node)
        {
            var val = node.GetValue<T>();
            var currentSubcube = _translations.FirstOrDefault(x => x.Type == _SUBCUBE);

            if (val is IEnumerable<ICubeObject>)
            {
                var concact = ((IEnumerable<ICubeObject>)val)
                    .Select(x => x.ToString())
                    .Aggregate((a, b) => $"{a}, {b}");

                if (currentSubcube != null)
                    currentSubcube.Value = concact;
                else
                    new Translation(getComponentValue(Component.SubCube), concact)
                        .Finally(_translations.Add);
            }

            else
            {
                if (currentSubcube != null)
                    currentSubcube.Value = val.ToString();
                else
                    new Translation(getComponentValue(Component.SubCube), val.ToString())
                        .Finally(_translations.Add);
            }
        }

        void prepareAxis(Expression node)
        {
            var obj = node.GetValue<T>();
            var axis = _axis.FirstOrDefault(x => x.AxisNumber == _currentAxis);
            setTranslation(obj, _currentAxisObject.IsNonEmpty);
        }

        void setTranslation(object obj)
        {
            if (obj != null)
            {
                if (obj is IEnumerable<ICubeObject>)
                    foreach (var o in (IEnumerable<ICubeObject>)obj)
                        _translations.Add(new Translation(_currentAxis, o.ToString()));
                else
                    _translations.Add(new Translation(_currentAxis, obj.ToString()));
            }
        }

        void setTranslation(object obj, bool isNonEmpty)
        {
            if (obj != null)
            {
                if (obj is IEnumerable<ICubeObject>)
                    foreach (var o in (IEnumerable<ICubeObject>)obj)
                        _translations.Add(new Translation(_currentAxis, o.ToString(), isNonEmpty));
                else
                    _translations.Add(new Translation(_currentAxis, obj.ToString(), isNonEmpty));
            }
        }

        byte getComponentValue(Component component)
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

        bool tryGetTagName(MemberExpression member, out string name)
        {
            if(member == null)
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

        string assembleTranslations()
        {
            var sb = new StringBuilder(Comment.PAS_HEADER).AppendLine();
            sb.AppendLine();
            var members = _translations.Where(x => x.Type == _WMEMBER);
            var sets = _translations.Where(x => x.Type == _WSET);
            var combined = members.Union(sets).OrderBy(x => x.DeclarationOrder);
            
            if(combined.Count() > 0)
            {
                sb.AppendLine(Comment.FOR_CREATED_REGION);
                sb.AppendLine("WITH");
                foreach(var com in combined.OrderBy(x => x.DeclarationOrder))
                {
                    string type = com.Type == _WMEMBER ? "MEMBER" : "SET";
                    if (com.Name.Contains("_set"))
                        sb.AppendLine(Comment.FOR_NO_SET_NAME);
                    if (com.Name.Contains("_member"))
                        sb.AppendLine(Comment.FOR_NO_MEMBER_NAME);
                    sb.AppendLine($"{type} {com.Name} AS");
                    sb.AppendLine(com.Value);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(Comment.FOR_SELECT_REGION);
            sb.AppendLine("SELECT");

            _axis
                .OrderBy(x => x.AxisNumber)
                .Select(x => x.ToString())
                .Aggregate((a, b) => $"{a},\r\n{b}")
                .To(sb.AppendLine);

            sb.AppendLine(Comment.FOR_FROM_REGION);
            var subCube = _translations.FirstOrDefault(x => x.Type == _SUBCUBE);
            if (subCube != null)
            {
                sb.AppendLine("FROM")
                    .AppendLine("(")
                    .AppendLine("\tSELECT")
                    .AppendLine("\t{0}", subCube.Value)
                    .AppendLine($"\tON 0 {_translations.First(x => x.Type == _FROM).Value}")
                    .AppendLine(")");
            }
            else
                sb.AppendLine(_translations.First(x => x.Type == _FROM).Value);

            var slicers = _translations.Where(x => x.Type == _WHERE);
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
