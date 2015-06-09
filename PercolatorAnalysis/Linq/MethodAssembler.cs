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
using System.Linq.Expressions;
using System.Reflection;

namespace Percolator.AnalysisServices.Linq
{
    using Percolator.AnalysisServices.Attributes;
    using Percolator.AnalysisServices;
    internal static class MethodAssembler
    {
        public static string Head(MethodCallExpression exp)
        {
            string index = MethodAssembler.visit(exp.Arguments[1]);
            string str = MethodAssembler.visit(exp.Arguments[0]);
            return string.Format("Head( {0}, {1} )", str, index);
        }

        public static string Tail(MethodCallExpression exp)
        {
            string index = MethodAssembler.visit(exp.Arguments[1]);
            string str = MethodAssembler.visit(exp.Arguments[0]);
            return string.Format("Tail( {0}, {1} )", str, index);
        }

        public static string TopCount(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string count = MethodAssembler.visit(exp.Arguments[1]);
            string num = MethodAssembler.visit(exp.Arguments[2]);
            return string.Format("TopCount( {0}, {1}, {2} )", set, count, num);
        }

        public static string BottomCount(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string count = MethodAssembler.visit(exp.Arguments[1]);
            string num = MethodAssembler.visit(exp.Arguments[2]);
            return string.Format("BottomCount( {0}, {1}, {2} )", set, count, num);
        }

        public static string Order(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(exp.Arguments[1]);
            string orderType = MethodAssembler.visit(exp.Arguments[2]);
            return string.Format("Order( {0}, {1}, {2} )", set, member, orderType);
        }

        public static string Filter(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            LambdaExpression logicalExp = (LambdaExpression)MethodAssembler.stripQuotes(exp.Arguments[1]);
            string logical = MethodAssembler.visit(logicalExp.Body);
            return string.Format("Filter( {0}, {1} )", set, logical);
        }

        public static string Except(MethodCallExpression exp)
        {
            string set1 = MethodAssembler.visit(exp.Arguments[0]);
            string set2 = MethodAssembler.visit(exp.Arguments[1]);
            string retain = MethodAssembler.visit(exp.Arguments[2]);
            bool retainAll = bool.Parse(retain);
            if (retainAll)
                return string.Format("Except( {0}, {1}, {2} )", set1, set2, "ALL");
            
            return string.Format("Except( {0}, {1} )", set1, set2);
        }

        public static string Exists(MethodCallExpression exp)
        {
            string set1 = MethodAssembler.visit(exp.Arguments[0]);
            string set2 = MethodAssembler.visit(exp.Arguments[1]);
            string measure = MethodAssembler.visit(exp.Arguments[2]);
            if (string.IsNullOrEmpty(measure))
                return string.Format("Exists( {0}, {1} )", set1, set2);

            return string.Format("Exists( {0}, {1}, \"{2}\" )", set1, set2, measure);
        }

        public static string PeriodsToDate(MethodCallExpression exp)
        {
            string level = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(MethodAssembler.stripQuotes(exp.Arguments[1]));
            return string.Format("PeriodsToDate( {0}, {1} )", level, member);
        }

        public static string ParallelPeriod(MethodCallExpression exp)
        {
            string level = MethodAssembler.visit(exp.Arguments[0]);
            string index = MethodAssembler.visit(exp.Arguments[1]);
            string member = MethodAssembler.visit(exp.Arguments[2]);
            bool levelNull = level == null;
            bool indexNull = index == null;
            bool memberNull = member == null;
            return string.Format("ParallelPeriod( {0}{1} {2}{3} {4} )", 
                level, (levelNull || (indexNull && memberNull)) ? "" : ",", 
                index, (indexNull || memberNull) ? "" : ",",
                member);
        }

        public static string Aggregate(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(exp.Arguments[1]);
            if (string.IsNullOrEmpty(member))
                return string.Format("Aggregate( {0} )", set);
            return string.Format("Aggregate( {0}, {1} )", set, member);
        }

        public static string Average(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(exp.Arguments[1]);
            if (string.IsNullOrEmpty(member))
                return string.Format("Avg( {0} )", set);
            return string.Format("Avg( {0}, {1} )", set, member);
        }

        public static string Sum(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(exp.Arguments[1]);
            if (string.IsNullOrEmpty(member))
                return string.Format("Sum( {0} )", set);
            return string.Format("( {0}, {1} )", set, member);
        }

        public static string Count(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string count = MethodAssembler.visit(exp.Arguments[1]);
            return string.Format("Count( {0}, {1} )", set, count.ToUpper());
        }

        public static string Max(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(exp.Arguments[1]);
            if (string.IsNullOrEmpty(member))
                return string.Format("Max( {0} )", set);
            return string.Format("Max( {0}, {1} )", set, member);
        }

        public static string Min(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            string member = MethodAssembler.visit(exp.Arguments[1]);
            if (string.IsNullOrEmpty(member))
                return string.Format("Min( {0} )", set);
            return string.Format("Min( {0}, {1} )", set, member);
        }

        public static string DistinctCount(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            return string.Format("DistinctCount( {0} )", set);
        }

        public static string Case(MethodCallExpression exp)
        {
            throw new NotImplementedException("The case method is not quite finished.  Please come back later.");
        }

        public static string IIf(MethodCallExpression exp)
        {
            string logical = MethodAssembler.visit(exp.Arguments[0]);
            string obj1 = MethodAssembler.visit(exp.Arguments[1]);
            string obj2 = MethodAssembler.visit(exp.Arguments[2]);
            string h1 = MethodAssembler.visit(exp.Arguments[3]);
            string h2 = MethodAssembler.visit(exp.Arguments[4]);
            obj1 = obj1 ?? "NULL";
            obj2 = obj2 ?? "NULL";
            h1 = h1 == "None" ? "" : "HINT " + h1.ToUpper();
            h2 = h2 == "None" ? "" : "HINT " + h2.ToUpper();

            return string.Format("IIf( {0}, {1}{2}, {3}{4} )", logical, obj1, h1, obj2, h2);
        }

        public static string CrossJoin(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            NewArrayExpression array = (NewArrayExpression)exp.Arguments[1];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Expressions.Count; i++)
                sb.Append(string.Format(", {0}", MethodAssembler.visit(array.Expressions[i])));

            return string.Format("CrossJoin( {0} {1} )", set, sb.ToString());
        }

        public static string NonEmptyCrossJoin(MethodCallExpression exp)
        {
            string set = MethodAssembler.visit(exp.Arguments[0]);
            NewArrayExpression array = (NewArrayExpression)exp.Arguments[1];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Expressions.Count; i++)
                sb.Append(string.Format(", {0}", MethodAssembler.visit(array.Expressions[i])));

            return string.Format("NonEmptyCrossJoin( {0} {1} )", set, sb.ToString());
        }

        public static string Lag(MethodCallExpression exp)
        {
            string param = MethodAssembler.visit(exp.Arguments[0]);
            string obj = MethodAssembler.visit(exp.Object);
            return string.Format("{0}.Lag({1})", obj, param);
        }

        public static string Lead(MethodCallExpression exp)
        {
            string param = MethodAssembler.visit(exp.Arguments[0]);
            string obj = MethodAssembler.visit(exp.Object);
            return string.Format("{0}.Lag({1})", obj, param);
        }

        public static string Item(MethodCallExpression exp)
        {
            string param = MethodAssembler.visit(exp.Arguments[0]);
            string obj = MethodAssembler.visit(exp.Object);
            return string.Format("{0}.Lag({1})", obj, param);
        }

        #region Privates
        static string visit(Expression node)
        {
            string str = string.Empty;
            switch(node.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.Quote:
                    str = MethodAssembler.visit(((UnaryExpression)node).Operand);
                    break;

                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    str = visitBinary((BinaryExpression)node);
                    break;

                case ExpressionType.Constant:
                    str = ((ConstantExpression)node).Value == null ? null : ((ConstantExpression)node).Value.ToString();
                    break;

                case ExpressionType.Call:
                    str = MethodAssembler.visitMethod((MethodCallExpression)node);
                    break;

                case ExpressionType.MemberAccess:
                    str = MethodAssembler.visitMember((MemberExpression)node);
                    break;
            }
            return str;
        }

        static string visitMethod(MethodCallExpression node)
        {
            switch(node.Method.Name)
            {
                case "Head":
                    return MethodAssembler.Head(node);

                case "Tail":
                    return MethodAssembler.Tail(node);

                case "TopCount":
                    return MethodAssembler.TopCount(node);

                case "BottomCount":
                    return MethodAssembler.BottomCount(node);

                case "Order":
                    return MethodAssembler.Order(node);

                case "Filter":
                    return MethodAssembler.Filter(node);

                case "Except":
                    return MethodAssembler.Except(node);

                case "Exists":
                    return MethodAssembler.Exists(node);

                case "PeriodsToDate":
                    return MethodAssembler.PeriodsToDate(node);

                case "ParallelPeriod":
                    return MethodAssembler.ParallelPeriod(node);

                case "Aggregate":
                    return MethodAssembler.Aggregate(node);

                case "Average":
                    return MethodAssembler.Average(node);

                case "Sum":
                    return MethodAssembler.Sum(node);

                case "Count":
                    return MethodAssembler.Count(node);

                case "Max":
                    return MethodAssembler.Max(node);

                case "Min":
                    return MethodAssembler.Min(node);

                case "DistinctCount":
                    return MethodAssembler.DistinctCount(node);

                case "Case":
                    return MethodAssembler.Case(node);

                case "IIf":
                    return MethodAssembler.IIf(node);

                case "CrossJoin":
                    return MethodAssembler.CrossJoin(node);

                case "NonEmptyCrossJoin":
                    return MethodAssembler.NonEmptyCrossJoin(node);

                case "get_Item":
                    MemberExpression mem = (MemberExpression)node.Object;
                    Expression arg = node.Arguments[0];
                    string att = MethodAssembler.visit(mem);
                    if (arg is ConstantExpression)
                    {
                        ConstantExpression carg = (ConstantExpression)arg;
                        string val = carg.Value.ToString().Replace("[", "").Replace("]", "");
                        if (val.StartsWith("&"))
                            val = string.Format("&[{0}]", val.Substring(1));
                        else
                            val = string.Format("[{0}]", val);
                        att = string.Format("{0}.{1}", att, val);
                    }

                    if (arg is MemberExpression)
                    {
                        MemberExpression marg = (MemberExpression)arg;
                        string val = MethodAssembler.getValue(marg).ToString();
                        val = val.Replace("[", "").Replace("]", "");
                        if (val.StartsWith("&"))
                            val = string.Format("&[{0}]", val.Substring(1));
                        else
                            val = string.Format("[{0}]", val);
                        att = string.Format("{0}.{1}", att, val);
                    }
                    return att;

                case "Function":
                    MemberExpression memf = (MemberExpression)node.Object;
                    Expression argf = node.Arguments[0];
                    string attf = MethodAssembler.visit(memf);
                    if (argf is ConstantExpression)
                    {
                        ConstantExpression carg = (ConstantExpression)argf;
                        string val = carg.Value.ToString();
                        attf = string.Format("{0}.{1}", attf, val);
                    }

                    if (argf is MemberExpression)
                    {
                        MemberExpression marg = (MemberExpression)argf;
                        string val = MethodAssembler.visit(marg);
                        attf = string.Format("{0}.{1}", attf, val);
                    }
                    return attf;

                default:
                    throw new NotImplementedException(string.Format("The '{0}' method is not supported", node.Method.Name));
            }
        }

        static string visitMember(MemberExpression node)
        {
            string str = string.Empty;
            string funcName = MethodAssembler.switchExtensions(node);

            if (node.Expression.NodeType == ExpressionType.Constant)
                str = MethodAssembler.getValue((MemberExpression)node).ToString();

            else if (System.Attribute.IsDefined(node.Member, typeof(TagAttribute)))
                str = node.Member.GetCustomAttribute<TagAttribute>().Tag;

            else if (System.Attribute.IsDefined(node.Member, typeof(MeasureAttribute)))
                str = node.Member.GetCustomAttribute<MeasureAttribute>().Tag;

            else if (node.Expression.NodeType == ExpressionType.MemberAccess)
                str = MethodAssembler.visit(node.Expression);

            if (!string.IsNullOrEmpty(funcName))
                str = string.Format("{0}.{1}", str, funcName);

            return str;
        }

        static string visitBinary(BinaryExpression node)
        {
            string str = string.Empty;
            str = MethodAssembler.visit(MethodAssembler.stripQuotes(node.Left));
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                    str = string.Format("{0} AND ", str);
                    break;

                case ExpressionType.OrElse:
                    str = string.Format("{0} OR ", str);
                    break;

                case ExpressionType.GreaterThan:
                    str = string.Format("{0} > ", str);
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    str = string.Format("{0} >= ", str);
                    break;

                case ExpressionType.LessThan:
                    str = string.Format("{0} < ", str);
                    break;

                case ExpressionType.LessThanOrEqual:
                    str = string.Format("{0} <= ", str);
                    break;

                case ExpressionType.Equal:
                    str = string.Format("{0} = ", str);
                    break;

                case ExpressionType.NotEqual:
                    str = string.Format("{0} <> ", str);
                    break;

                default:
                    throw new NotImplementedInPAS_Exception(string.Format("The '{0}' is not supported."));
            }
            str = string.Format("{0} {1}", str, MethodAssembler.visit(MethodAssembler.stripQuotes(node.Right)));
            return str;
        }

        static object getValue(MemberExpression member)
        {
            UnaryExpression objectMember = Expression.Convert(member, typeof(object));
            Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);
            Func<object> getter = getterLambda.Compile();
            return getter.Invoke();
        }

        static Expression stripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote || e.NodeType == ExpressionType.Convert)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        internal static string switchExtensions(MemberExpression mem)
        {
            switch (mem.Member.Name)
            {
                case "All":
                    return "[ALL]";

                case "Children":
                    return "Children";

                case "AllMembers":
                    return "AllMembers";

                case "CurrentMember":
                    return "CurrentMember";

                case "UniqueName":
                    return "UniqueName";

                case "Member_Caption":
                    return "Member_Caption";

                case "Count":
                    return "Count";

                case "Current":
                    return "Current";

                case "CurrentOrdinal":
                    return "CurrentOrdinal";

                case "DataMember":
                    return "DataMember";

                case "DefaultMember":
                    return "DefaultMember";

                case "Dimension":
                    return "Dimension";

                case "FirstChild":
                    return "FirstChild";

                case "FirstSibling":
                    return "FirstSibling";

                case "Hierarchy":
                    return "Hierarchy";

                case "LastChild":
                    return "LastChild";

                case "LastSibling":
                    return "LastSibling";

                case "Level":
                    return "Level";

                case "Members":
                    return "Members";

                case "Name":
                    return "Name";

                case "NextMember":
                    return "NextMember";

                case "Ordinal":
                    return "Ordinal";

                case "Parent":
                    return "Parent";

                case "PrevMember":
                    return "PrevMember";

                case "UnknownMember":
                    return "UnkownMember";

                case "Value":
                    return "Value";

                default:
                    return null;
            }
        }
        #endregion
    }
}
