using Microsoft.AnalysisServices.AdomdClient;
using Percolator.AnalysisServices.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices
{
    using Percolator.AnalysisServices.Linq;

    internal static class Helpers
    {
        internal static Expression StripQuotes(this Expression source)
        {
            var e = source;
            while (e is UnaryExpression)
                e = ((UnaryExpression)e).Operand;
            return e;
        }

        internal static ParameterExpression GetParameter(this Expression node)
        {
            var e = node;
            if (e is BinaryExpression)
            {
                var bin = (BinaryExpression)e;
                var leftParam = bin.Left.GetParameter();
                if (leftParam != null)
                    return leftParam;
                else
                    return bin.Right.GetParameter();                    
            }
            while (e is MemberExpression || e is MethodCallExpression)
            {
                if (e is MemberExpression)
                    e = ((MemberExpression)e).Expression;
                else if (e is MethodCallExpression)
                    e = ((MethodCallExpression)e).Object;
            }
            return e as ParameterExpression;
        }

        internal static byte DetermineAxis(this MemberInfo mem)
        {
            switch(mem.Name.ToLower())
            {
                case "oncolumns":
                    return 0;

                case "onrows":
                    return 1;

                case "onpages":
                    return 2;

                case "onchapters":
                    return 3;

                case "onsections":
                    return 4;

                default:
                    throw new ArgumentException(string.Format("The {0} axis is not recognized.", mem.Name));
            }
        }

        internal static StringBuilder AppendLine(this StringBuilder source, string str, params object[] objs)
        {
            return source.AppendLine(string.Format(str, objs));
        }

        internal static string JoinWith<T>(this IEnumerable<T> source, string joinString, bool joinWithNewLine = false)
        {
            using (var rator = source.GetEnumerator())
            {
                var firstRun = true;
                var sb = new StringBuilder();
                while (rator.MoveNext())
                {
                    if (firstRun)
                    {
                        if (joinWithNewLine)
                            sb.AppendLine(rator.Current.ToString());
                        else
                            sb.Append(rator.Current.ToString());
                        firstRun = false;
                    }

                    else
                    {
                        if (joinWithNewLine)
                            sb.AppendLine("{0}{1}", joinString, rator.Current.ToString());
                        else
                            sb.AppendFormat("{0}{1}", joinString, rator.Current.ToString());
                    }
                }
                return sb.ToString();
            }
        }

        internal static object GetValue<T>(this Expression expression)
        {
            if (expression == null)
                return null;

            try
            {
                var instanceProp = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Instance");
                if(instanceProp == null)
                    throw new PercolatorException(string.Format("Cannot find the sigleton instance of '{0}'.", typeof(T).Name));
                var t = (T)instanceProp.GetValue(null);
                var lambdaExp = (LambdaExpression)expression;
                var lambda = Expression.Lambda<Func<T, object>>(lambdaExp.Body, lambdaExp.Parameters.FirstOrDefault());
                var method = lambda.Compile();
                var obj = method(t);
                return obj;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        internal static IEnumerable<T> Iterate<T>(this IEnumerable<T> source, Action<T> action)
        {
            using (var rator = source.GetEnumerator())
                while (rator.MoveNext())
                    action(rator.Current);
            return source;
        }

        internal static T GetCubeInstance<T>(this Type source)
        {
            var instanceProp = source.GetProperty("Instance", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public);
            if(instanceProp == null)
                throw new PercolatorException(string.Format("Cannot find the sigleton instance of '{0}'.", typeof(T).Name));
            return (T)instanceProp.GetValue(null);
        }

        internal static IEnumerable<T> FlattenAndReturn<T>(this CellSet source) where T : new()
        {
            if (source.Axes.Count > 2)
            {
                var ex = new NotImplementedInPAS_Exception("Apologies, but maping to an object using more than two axes is not yet supported.");
                ex.ReasonWhy = "The differences in the Cell Values indexes have not been mapped yet.";
                throw ex;
            }

            var objProps = typeof(T).GetProperties().Where(x => System.Attribute.IsDefined(x, typeof(MapToAttribute)));
            var flattend = Flat.Flatten(source);

            TypeConverter objConverter = null;
            TypeConverter memConverter = null;
            foreach (var flat in flattend)
            {
                var obj = new T();
                foreach (var val in flat.MeasureValues)
                {
                    var prop = objProps.FirstOrDefault(x => x.GetCustomAttribute<MapToAttribute>().MdxColumn == val.Key);
                    if (prop != null)
                    {
                        objConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                        if (val.Value != null)
                            prop.SetValue(obj, objConverter.ConvertFromString(val.Value.ToString()));
                    }
                }

                foreach (var mem in flat.PositionValues)
                {
                    var prop = objProps.FirstOrDefault(x => x.GetCustomAttribute<MapToAttribute>().MdxColumn == mem.Key);
                    if (prop != null)
                    {
                        memConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                        prop.SetValue(obj, memConverter.ConvertFromString(mem.Value.ToString()));
                    }
                }
                yield return obj;
            }
        }
    }
}
