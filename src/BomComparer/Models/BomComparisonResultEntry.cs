using BomComparer.Attributes;
using BomComparer.Enums;

namespace BomComparer.Models;

public class BomComparisonResultEntry
{
    // Status of the entire row
    [ColumnOrder(1)]
    public ComparisonResult Status { get; set; }

    [ColumnOrder(2)]
    public ComparedValues<int>? Quantity { get; set; }

    [ColumnOrder(3)]
    public string? PartNumber { get; set; }

    [ColumnOrder(4)]
    public List<DesignatorComparisonResultEntry>? Designators { get; set; } = new();
    public ComparedValues<string>? Value { get; set; }
    public ComparedValues<string>? Smd { get; set; }
    public ComparedValues<string>? Description { get; set; }
    public ComparedValues<string>? Manufacturer { get; set; }
    public ComparedValues<string>? ManufacturerPartNumber { get; set; }
    public ComparedValues<string>? Distributor { get; set; }
    public ComparedValues<string>? DistributorPartNumber { get; set; }
}
