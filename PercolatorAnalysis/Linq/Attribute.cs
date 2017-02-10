/*  
 * Percolator Analysis Services
 *  Copyright (c) 2014 CoopDIGITy
 *  Author: Matthew Hallmark
 *  A Copy of the Liscence is included in the "AssemblyInfo.cs" file.
 */

namespace Percolator.AnalysisServices.Linq
{
    /// <summary>
    /// Represents an Analysis Services 'Attribute' within a dimension.
    /// </summary>
    public class Attribute : Set
    {
        /// <summary>
        /// Represents the "All" member.
        /// </summary>
        public Member All => $"{Tag}.[All]";

        /// <summary>
        /// Retrieves a member by its name from the attribute.
        /// </summary>
        /// <param name="memberName">The name of the member. Start the name with an ampersand to retrieve the member by its address.
        /// Automatically places in the square brackets.</param>
        /// <returns></returns>
        public Member this[string memberName] => memberFrom(memberName);

        /// <summary>
        /// Retrieves a member by its name from the attribute.
        /// </summary>
        /// <param name="memberName">The name of the member. Start the name with an ampersand to retrieve the member by its address.
        /// Automatically places in the square brackets.</param>
        /// <returns></returns>
        public Member this[int memberName] => memberFrom(memberName.ToString());

        /// <summary>
        /// The MDX syntax representation for this attribute.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Represents this attribute's member. i.e [Stores].[Store ID].[Store ID] 
        /// </summary>
        public Set This => getThis(); 
         
        /// <summary>
        /// Used by the T4 template to create an attribute for a dimension.
        /// </summary>
        /// <param name="tag"></param>
        public Attribute(string tag) : base(new ICubeObject[0])
        {
            Tag = tag;
            _values.Add(tag);
        }

        /// <summary>
        /// Appends a function to the end of the attribute in the query.
        /// </summary>
        /// <param name="function">The name/syntax for the extension function.</param>
        /// <returns></returns>
        public Member Function(string function) => $"{Tag}.{function}";

        /// <summary>
        /// Implicit string conversion for the attribute.
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        public static implicit operator string(Attribute att) => att.ToString();

        public static implicit operator Attribute(string str) => new Attribute(str);

        /// <summary>
        /// Overridden ToString returns the Tag for this attribute.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Tag;

        string getThis()
        {
            var tag = ToString();
            var split = tag.Split('.');

            if (split.Length == 2)
                return $"{tag}.{split[1]}";
            else
                return tag;
        }

        Member memberFrom(string memberName)
        {
            string att = Tag;
            string val = memberName;
            val = val.Replace("[", "").Replace("]", "");
            if (val.StartsWith("&"))
                val = $"&[{val.Substring(1)}]";
            else
                val = $"[{val}]";
            att = $"{att}.{val}";
            return new Member(att);
        }
    }
}
