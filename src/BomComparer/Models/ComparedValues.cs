using BomComparer.Enums;

namespace BomComparer.Models
{
    public class ComparedValues<T>
    {
        public T SourceValue { get; set; }
        public T TargetValue { get; set; }
        public ComparisonResult Status { get; set; }

        public ComparedValues(T source, T targetValue, ComparisonResult status)
        {
            SourceValue = source;
            TargetValue = targetValue;
            Status = status;
        }
    }
}
