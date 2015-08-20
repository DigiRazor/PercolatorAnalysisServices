namespace Percolator.AnalysisServices.Attributes
{
    public sealed class MapToAttribute : global::System.Attribute
    {
        public string MdxColumn { get; set; }
        public MapToAttribute()
        {

        }

        public MapToAttribute(string mdxColumn)
        {
            MdxColumn = mdxColumn;
        }
    }
}
