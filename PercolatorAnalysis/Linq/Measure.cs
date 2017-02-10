/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Linq
{
    using System;

    /// <summary>
    /// Representation af a MDX 'Measure'
    /// </summary>
    public class Measure : Member
    {
        string _tag;

        /// <summary>
        /// The MDX syntax representation of this measure.
        /// </summary>
        public string Tag => $"Measures.[{_tag}]";

        /// <summary>
        /// The name of the measure.
        /// </summary>
        public string MeasureName => _tag;

        /// <summary>
        /// Creates a representation of a MDX measure.
        /// </summary>
        /// <param name="tag">The name of the measure.</param>
        public Measure(string tag)
        {
            _tag = tag;
        }
        
        /// <summary>
        /// sbyte conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator sbyte(Measure m) => 0;

        /// <summary>
        /// byte conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator byte(Measure m) => 0;

        /// <summary>
        /// ushort conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator ushort(Measure m) => 0;

        /// <summary>
        /// short conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator short(Measure m) => 0;

        /// <summary>
        /// ulong conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator ulong(Measure m) => 0;

        /// <summary>
        /// long conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator long(Measure m) => 0;

        /// <summary>
        /// conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator int(Measure m) => 0;

        /// <summary>
        /// float conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator float(Measure m) => 0;

        /// <summary>
        /// double conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator double(Measure m) => 0;

        /// <summary>
        /// decimal conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator decimal(Measure m) => 0;

        /// <summary>
        /// Guid conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator Guid(Measure m) => default(Guid);

        /// <summary>
        /// string conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static implicit operator string(Measure m) => m.ToString();

        /// <summary>
        /// Member conversion for a Measure.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        //public static implicit operator Measure(Member m) { return new Measure(m); }

        /// <summary>
        /// Overridden toString that returns this measure's tag.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Tag;

        public static Member operator &(Measure measure1, Measure measure2) => $"{measure1}, {measure2}";
    }
}
