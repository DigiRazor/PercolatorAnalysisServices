using System;
using System.Linq.Expressions;
using Percolator.AnalysisServices.Attributes;
using System.Reflection;

namespace Percolator.AnalysisServices.Linq
{
    public class MdxQuery
    {
        [MdxQueryable(AxisNumber = 1)]
        public ICubeObject OnRows { get; set; }
        [MdxQueryable(AxisNumber = 0)]
        public ICubeObject OnColumns { get; set; }
        [MdxQueryable(AxisNumber = 2)]
        public ICubeObject OnPages { get; set; }
        [MdxQueryable(AxisNumber = 3)]
        public ICubeObject OnChapters { get; set; }
        [MdxQueryable(AxisNumber = 4)]
        public ICubeObject OnSections { get; set; }

        internal Type CubeType { get; set; }
    }

    public static class MdxQueryable
    {
        /// <summary>
        /// Used for the LINQ Free Form syntax queries. Uses the convention of selecting a new anon object and assigning 
        /// the properties of the anon object the axis on which you want to select (i.e. .Select(x => new { OnColumns = x.TransactionCount });).
        /// </summary>
        /// <typeparam name="T">The infered type of the cube used for the query.</typeparam>
        /// <param name="source">The source cube.</param>
        /// <param name="axesSelections">The anonymous object used to specify the axis selections.</param>
        /// <returns></returns>
        public static IMdxQueryable<T> Select<T>(this IMdxQueryable<T> source, Expression<Func<T, MdxQuery>> axesSelections)
        {
            var bod = axesSelections.Body;
            
            var memberInit = bod as MemberInitExpression;
            var expBlocks = memberInit.Reduce() as BlockExpression;
            foreach(var block in expBlocks.Expressions)
            {
                if (block is BinaryExpression)
                {
                    var bin = block as BinaryExpression;
                    if (bin.Left is MemberExpression)
                    {
                        var left = bin.Left as MemberExpression;
                        var axisNumber = left.Member.GetCustomAttribute<MdxQueryableAttribute>().AxisNumber;
                        var lambda = Expression.Lambda<Func<T, ICubeObject>>(bin.Right, axesSelections.Parameters[0]);
                        source.OnAxis(axisNumber, lambda);
                    }
                }
            }
            
            return source;
        }

        /// <summary>
        /// Where method used for the LINQ Free Form queries. Used to specify the Where slicer in an MDX query.
        /// </summary>
        /// <typeparam name="T">The infered type of the cube that will be queried.</typeparam>
        /// <param name="source">The cube.</param>
        /// <param name="slicer">The expression used to build the Where slicer.</param>
        /// <returns></returns>
        public static IMdxQueryable<T> Where<T>(this IMdxQueryable<T> source, Expression<Func<T, bool>> slicer)
        {
            var exp = slicer.Body.StripQuotes();
            var param = slicer.Parameters[0];
            var lambda = Expression.Lambda<Func<T, ICubeObject>>(exp, new[] { param });
            source.Slice(lambda);
            return source;
        }
    }
}
