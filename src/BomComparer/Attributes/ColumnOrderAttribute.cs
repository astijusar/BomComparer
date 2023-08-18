namespace BomComparer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnOrderAttribute : Attribute
    {
        public int Order { get; }

        public ColumnOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
