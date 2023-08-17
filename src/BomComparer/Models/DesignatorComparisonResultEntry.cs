using BomComparer.Enums;

namespace BomComparer.Models
{
    public class DesignatorComparisonResultEntry
    {
        public DesignatorComparisonResult Status { get; set; }
        public string? Value { get; set; }

        public DesignatorComparisonResultEntry(DesignatorComparisonResult status, string? value)
        {
            Status = status;
            Value = value;
        }
    }
}
