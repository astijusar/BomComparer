using BomComparer.Attributes;
using BomComparer.Enums;

namespace BomComparer.Models;

public class BomComparisonResultEntry
{
    public string? PartNumber { get; set; }
    public List<DesignatorComparisonResultEntry>? Designators { get; set; } = new();
    public ComparedValues<int>? Quantity { get; set; }
    public ComparedValues<string>? Value { get; set; }
    public ComparedValues<string>? Smd { get; set; }
    public ComparedValues<string>? Description { get; set; }
    public ComparedValues<string>? Manufacturer { get; set; }
    public ComparedValues<string>? ManufacturerPartNumber { get; set; }
    public ComparedValues<string>? Distributor { get; set; }
    public ComparedValues<string>? DistributorPartNumber { get; set; }

    // Status of the entire row
    public ComparisonResult Status { get; set; }
}
