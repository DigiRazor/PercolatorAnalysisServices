/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using Percolator.AnalysisServices;
    using Percolator.AnalysisServices.Attributes;

    /// <summary>
    /// The order type for the MDX 'Order' function.
    /// </summary>
    public enum OrderType 
    { 
        /// <summary>
        /// Orders in ascending order.
        /// </summary>
        ASC,

        /// <summary>
        /// Orders in descending order.
        /// </summary>
        DESC,

        /// <summary>
        /// Orders in ascending order, breaking the hierarchy.
        /// </summary>
        BASC,

        /// <summary>
        /// Orders in descending order, breaking the hierarchy.
        /// </summary>
        BDESC 
    }

    /// <summary>
    /// The count type for the MDX 'Count' function.
    /// </summary>
    public enum CountType 
    { 
        /// <summary>
        /// Excludes empty cells.
        /// </summary>
        ExcludeEmpty,

        /// <summary>
        /// Includes empty cells. This is the default selection.
        /// </summary>
        IncludeEmpty 
    }

    /// <summary>
    /// The HINT type for the MDX 'Filter' function.
    /// </summary>
    public enum HINT 
    { 
        /// <summary>
        /// Eager
        /// </summary>
        Eager,

        /// <summary>
        /// Strict
        /// </summary>
        Strict,

        /// <summary>
        /// Lazy
        /// </summary>
        Lazy,

        /// <summary>
        /// No HINT. Does not apply a HINT to the returned filter expression.
        /// </summary>
        None 
    }

    /// <summary>
    /// Static class that holds the MDX functions to be used in MDX queries and object builders.
    /// </summary>
    public static class Mdx
    {
        static StringBuilder sb = new StringBuilder();

        /// <summary>
        /// A default method for assembling a MDX function to be used in a MDX query.
        /// </summary>
        /// <typeparam name="T">The type to be returned.  Must be either a Set or Member</typeparam>
        /// <param name="funcName"></param>
        /// <param name="paramz"></param>
        /// <returns></returns>
        public static T MdxFunction<T>(string funcName, params object[] paramz) where T : ICubeObject
        {
            var funcBuilder = new StringBuilder($"{funcName}(");

            paramz
                .Select(x => x.ToString())
                .Aggregate((a, b) => $"{a}, {b}")
                .To(funcBuilder.Append);

            funcBuilder.AppendLine(")");

            var type = typeof(T);
            if(type == typeof(Set) || type == typeof(Member))
                return (T)Activator.CreateInstance(typeof(T), funcBuilder.ToString());
            else
                throw new PercolatorException("The generic type must be a Set or a Member");
        }

        #region Ordering and Pickers
        /// <summary>
        /// MDX 'NonEmpty' function. Returns the set of tuples that are not empty from a specified set, based on the cross product of the specified set with a second set.
        /// </summary>
        /// <param name="set1">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="set2">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <returns></returns>
        public static Set NonEmpty(ICubeObject set1, ICubeObject set2)
        {
            if (set2 == null)
                return new Set($"NonEmpty({set1})");
            return new Set($"NonEmpty({set1}, {set2})");
        }

        /// <summary>
        /// MDX 'Head' function. Returns the first specified number of elements in a set, while retaining duplicates.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="count">A valid numeric expression that specifies the number of tuples to be returned.</param>
        /// <returns></returns>
        public static Set Head(Set set, int count) => new Set($"Head({set}, {count})");

        /// <summary>
        /// MDX 'Tail' function. Returns a subset from the end of a set
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="count">A valid numeric expression that specifies the number of tuples to be returned.</param>
        /// <returns></returns>
        public static Set Tail(Set set, int count) => new Set($"Tail({set}, {count})");

        /// <summary>
        /// MDX 'TopCount' function. Sorts a set in descending order and returns the specified number of elements with the highest values.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="count">A valid numeric expression that specifies the number of tuples to be returned.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Set TopCount(Set set, int count, object numericExpression)
        {
            if (numericExpression == null)
                return new Set($"TopCount({set}, {count})");

            return new Set($"TopCount({set}, {count}, {numericExpression})");
        }

        /// <summary>
        /// MDX 'BottomCount' function. Sorts a set in ascending order, and returns the specified number of tuples in the specified set with the lowest values.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="count">A valid numeric expression that specifies the number of tuples to be returned.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Set BottomCount(Set set, int count, object numericExpression)
        {
            if (numericExpression == null)
                return new Set($"BottomCount({set}, {count})");

            return new Set($"BottomCount({set}, {count}, {numericExpression})");
        }

        /// <summary>
        /// MDX 'Order' function. Arranges members of a specified set, optionally preserving or breaking the hierarchy.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <param name="orderType">A valid string expression that is typically a valid Multidimensional Expressions (MDX) expression of cell coordinates that return a number expressed as a string.</param>
        /// <returns></returns>
        public static Set Order(Set set, object numericExpression, OrderType orderType) =>
            new Set($"Order({set}, {numericExpression}, {orderType})");

        /// <summary>
        /// MDX 'Filter' function. Returns the set that results from filtering a specified set based on a search condition.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="logicalExpression">A valid Multidimensional Expressions (MDX) logical expression that evaluates to true or false.</param>
        /// <returns></returns>
        public static Set Filter(Set set, Expression<Func<bool>> logicalExpression)
        {
            visit(logicalExpression.Body);
            string str = sb.ToString();
            sb.Clear();
            return new Set($"Filter({set}, {str})");
        }

        /// <summary>
        /// MDX 'Except' function. Evaluates two sets and removes those tuples in the first set that also exist in the second set, optionally retaining duplicates.
        /// </summary>
        /// <param name="set1">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="set2">A valid Multidimensional Expressions (MDX) expression that returns a set.></param>
        /// <param name="retainAll">A boolean indicating whether to retain ALL or not.</param>
        /// <returns></returns>
        public static Set Except(Set set1, Set set2, bool retainAll = false)
        {
            if (retainAll)
                return new Set($"Except({set1}, {set2}, ALL)");

            return new Set($"Except({set1}, {set2})");
        }

        /// <summary>
        /// MDX 'Exists' function. Returns the set of tuples of the first set specified that exist with one or more tuples of the second set specified.
        /// </summary>
        /// <param name="set1">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="set2">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="measureGroupName">A valid string expression specifying a measure group name.</param>
        /// <returns>Set</returns>
        public static Set Exists(Set set1, Set set2, string measureGroupName = null)
        {
            if (string.IsNullOrEmpty(measureGroupName))
                return new Set($"Exists({set1}, {set2})");

            return new Set($"Exists({set1}, {set2}, \"{measureGroupName}\")");
        }

        /// <summary>
        /// Returns a set of tuples from extracted hierarchy elements.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="hierarchies">An array of valid Multidimensional Expressions (MDX) expression that returns a hierarchy.</param>
        /// <returns>Set</returns>
        public static Set Extract(Set set, params Set[] hierarchies)
        {
            var sb = new StringBuilder($"Extract({set}");
            
            foreach (var hier in hierarchies)
                sb.Append($", {hier}");
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the intersection of two input sets, optionally retaining duplicates. 
        /// </summary>
        /// <param name="set1">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="set2">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="retainAll">Boolean indicating whether to retain duplicates.</param>
        /// <returns>Set</returns>
        public static Set Intersect(Set set1, Set set2, bool retainAll = false)
        {
            var retainer = retainAll ? ", ALL" : "";
            return new Set($"Intersect({set1}, {set2}{retainer})");
        }

        #endregion

        #region Dates
        /// <summary>
        /// MDX 'PeriodsToDate' function. Returns a set of sibling members from the same level as a given member, 
        /// starting with the first sibling and ending with the given member, as constrained by a specified level in the Time dimension.
        /// </summary>
        /// <param name="level">A valid Multidimensional Expressions (MDX) expression that returns a level.</param>
        /// <param name="memberExpression">A valid Multidimensional Expressions (MDX) expression that returns a member.</param>
        /// <returns></returns>
        public static Set PeriodsToDate(Level level, Member memberExpression) =>
            new Set($"PeriodsToDate({level}, {memberExpression} )");

        /// <summary>
        /// MDX 'ParallelPeriod' function. Returns a member from a prior period in the same relative position as a specified member.
        /// </summary>
        /// <param name="level">A valid Multidimensional Expressions (MDX) expression that returns a level.</param>
        /// <param name="index">A valid numeric expression that specifies the number of parallel periods to lag.</param>
        /// <param name="memberExpression">A valid Multidimensional Expressions (MDX) expression that returns a member.</param>
        /// <returns></returns>
        public static Member ParallelPeriod(Level level, int? index, Member memberExpression) =>
            new Member($"ParallelPeriod({level}, {index}, {memberExpression})");
        #endregion

        #region Aggregations
        /// <summary>
        /// MDX 'Aggregate' function. Returns a number that is calculated by aggregating over the cells returned by the set expression. 
        /// If a numeric expression is not provided, this function aggregates each measure within the current query context by using the 
        /// default aggregation operator that is specified for each measure. If a numeric expression is provided, this function first evaluates, 
        /// and then sums, the numeric expression for each cell in the specified set.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Member Aggregate(Set set, Member numericExpression)
        {
            if (numericExpression == null)
                return new Member($"Aggregate({set})");

            return new Member($"Aggregate({set}, {numericExpression})");
        }

        /// <summary>
        /// MDX 'Avg' function. Evaluates a set and returns the average of the non empty values of the cells in the set, averaged over the measures in the set or over a specified measure.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Member Average(Set set, Member numericExpression)
        {
            if (numericExpression == null)
                return new Member($"Avg({set})");

            return new Member($"Avg({set}, {numericExpression})");
        }

        /// <summary>
        /// MDX 'Sum' function. Returns the sum of a numeric expression evaluated over a specified set.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) set expression.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Member Sum(Set set, Member numericExpression)
        {
            if (numericExpression == null)
                return new Member(string.Format("Sum({0})", "{" + set + "}"));
            return new Member($"Sum({set}, {numericExpression})");
        }

        public static Member Sum(Member member) => new Member(string.Format("Sum({0})", "{" + member + "}"));

        /// <summary>
        /// MDX 'Count' function. Returns the number of cells in a set.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="countType">Indicator to include or exclude empty cells.</param>
        /// <returns></returns>
        public static Member Count(Set set, CountType countType) => 
            new Member($"Count({set}, {countType.ToString().ToUpper()})");

        /// <summary>
        /// MDX 'Max' function. Returns the maximum value of a numeric expression that is evaluated over a set.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Member Max(Set set, Member numericExpression)
        {
            if (numericExpression == null)
                return new Member($"Max({set})");

            return new Member($"Max({set}, {numericExpression})");
        }

        /// <summary>
        /// MDX 'Min' function. Returns the minimum value of a numeric expression that is evaluated over a set.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <returns></returns>
        public static Member Min(Set set, Member numericExpression)
        {
            if (numericExpression == null)
                return new Member($"Min({set})");

            return new Member($"Min({set}, {numericExpression})");
        }

        /// <summary>
        /// MDX 'DistinctCount' function. Returns the number of distinct, nonempty tuples in a set.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <returns></returns>
        public static Member DistinctCount(Set set) => new Member($"DistinctCount({set})");
        #endregion 

        #region Program Flow
        /// <summary>
        /// MDX 'IIf' function. Evaluates different branch expressions depending on whether a Boolean condition is true or false.
        /// </summary>
        /// <param name="logicalExpression">A condition that evaluates to true (1) or false (0). It must be a valid Multidimensional Expressions (MDX) logical expression.</param>
        /// <param name="expression1">Used when the logical expression evaluates to true. Expression1 must be a valid Multidimensional Expressions (MDX) expression.</param>
        /// <param name="expression2">Used when the logical expression evaluates to false. Expression2 must be valid Multidimensional Expressions (MDX) expression.</param>
        /// <param name="hint1">The HINT type associated with the first expression.</param>
        /// <param name="hint2">The HINT type associated with the second expression.</param>
        /// <returns></returns>
        public static Member IIf(Expression<Func<bool>> logicalExpression, object expression1, object expression2, HINT hint1, HINT hint2)
        {
            visit(logicalExpression.Body);
            var logical = sb.ToString();
            sb.Clear();
            var tru = hint1 != HINT.None ? $"{expression1} HINT {hint1}" : expression1.ToString();
            var fls = hint2 != HINT.None ? $"{expression2} HINT {hint2}" : expression2.ToString();   
            if (expression1 is string && expression2 is string)
                return new Member($"IIf( {logical}, \"{tru}\", \"{fls}\" )");
            else if (expression1 is string)
                return new Member($"IIf( {logical}, \"{tru}\", {fls} )");
            else if (expression2 is string)
                return new Member($"IIf( {logical}, {tru}, \"{fls}\" )");
            else
                return new Member($"IIf( {logical}, {tru}, {fls} )");
        }
        #endregion

        #region Joins and Non Empty Stuff
        /// <summary>
        /// MDX 'CrossJoin' function. Returns the cross product of one or more sets.
        /// </summary>
        /// <param name="joinedSets">Any other valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <returns></returns>
        public static Set CrossJoin(params ICubeObject[] joinedSets)
        {
            var sb = new StringBuilder().AppendLine($"CrossJoin({joinedSets.First()}");
            foreach(var s in joinedSets.Skip(1))
                sb.AppendLine($"\t, {s}");
            sb.Append(")");
            return new Set(sb.ToString());
        }

        /// <summary>
        /// MDX 'NonEmptyCrossJoin' function. Returns a set that contains the cross product of one or more sets, excluding empty tuples and tuples without associated fact table data.
        /// </summary>
        /// <param name="set">A valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <param name="joinedSets">Any other valid Multidimensional Expressions (MDX) expression that returns a set.</param>
        /// <returns></returns>
        public static Set NonEmptyCrossJoin(Set set, params ICubeObject[] joinedSets)
        {
            var sb = new StringBuilder().AppendLine($"NonEmptyCrossJoin({set}");
            foreach (Set s in joinedSets)
                sb.AppendLine($"\t, {s}");
            sb.Append(")");
            return new Set(sb.ToString());
        }

        /// <summary>
        /// MDX 'IsEmpty' function. Returns whether the evaluated expression is the empty cell value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEmpty(ICubeObject obj) => new Member($"IsEmpty({obj})");

        /// <summary>
        /// MDX 'Generate' function. Applies a set to each member of another set, and then joins the resulting sets by union. Alternatively, this function returns 
        /// a concatenated string created by evaluating a string expression over a set.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static Set Generate(Set set1, Set set2, bool all = false)
        {
            if (all)
                return new Set($"Generate({set1}, {set2})");
            else
                return new Set($"Generate({set1}, {set2}, ALL)");

        }

        /// <summary>
        /// MDX 'CoalesceEmpty' function. Converts an empty cell value to a specified nonempty cell value, which can be either a number or string.
        /// </summary>
        /// <param name="numericExpression">A valid numeric expression that is typically a Multidimensional Expressions (MDX) expression of cell coordinates that return a number.</param>
        /// <param name="replacement">The value to return if the numeric expression is null.</param>
        /// <returns></returns>
        public static Member CoalesceEmpty(Member numericExpression, object replacement) => 
            new Member($"CoalesceEmpty({numericExpression}, {replacement})");
        #endregion

        #region Hierarchy Functions
        public static Member LinkMember(Member memberExpression, Hierarchy hierarchyExpression) =>
            new Member($"LinkMember({memberExpression}, {hierarchyExpression})");
        #endregion

        #region Privates
        static Expression visit(Expression node)
        {
            switch(node.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    visitBinary((BinaryExpression)node);
                    break;

                case ExpressionType.Convert:
                    visit(((UnaryExpression)node).Operand);
                    break;

                case ExpressionType.MemberAccess:
                    visitMember((MemberExpression)node);
                    break;

                case ExpressionType.Constant:
                    visitConstant((ConstantExpression)node);
                    break;
            }
            return node;
        }

        static Expression visitConstant(ConstantExpression node)
        {
            sb.Append(node.Value);
            return node;
        }

        static Expression visitMember(MemberExpression node)
        {
            sb.Append(getObjectValue(node));
            return node;
        }

        static Expression visitBinary(BinaryExpression node)
        {
            visit(node.Left);           
            switch(node.NodeType)
            {
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;

                case ExpressionType.NotEqual:
                    sb.Append(" <> ");
                    break;

                default:
                    throw new NotImplementedInPAS_Exception($"The '{node.NodeType}' is not supported.");
            }
            visit(node.Right);
            return node;
        }

        static string getObjectValue(MemberExpression member)
        {
            if (member.Expression.NodeType == ExpressionType.Constant)
            {
                var objectMember = Expression.Convert(member, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                return getter.Invoke().ToString();
            }

            else if (System.Attribute.IsDefined(member.Member, typeof(TagAttribute)))
                return member.Member.GetCustomAttribute<TagAttribute>().Tag;

            else if (System.Attribute.IsDefined(member.Member, typeof(MeasureAttribute)))
                return member.Member.GetCustomAttribute<MeasureAttribute>().Tag;
            else
                return Expression.Lambda<Func<object>>(member, new ParameterExpression[0]).Compile()().ToString();
        }
        #endregion
    }
}
