using BomComparer.Attributes;
using BomComparer.Enums;

namespace BomComparer.Models;

public class BomComparisonResultEntry
{
    [ColumnOrder(1)]
    [ColumnName("Status")]
    public ComparisonResult Status { get; set; }

    [ColumnOrder(2)]
    [ColumnName("Quantity")]
    public ComparedValues<int> Quantity { get; set; } = null!;

    [ColumnOrder(3)]
    [ColumnName("Part Number")]
    public string PartNumber { get; set; } = null!;

    [ColumnOrder(4)]
    [ColumnName("Designators")]
    public List<DesignatorComparisonResultEntry> Designators { get; set; } = new();

    [ColumnName("Value")]
    public ComparedValues<string> Value { get; set; } = null!;

    [ColumnName("SMD")]
    public ComparedValues<string> Smd { get; set; } = null!;

    [ColumnName("Description")]
    public ComparedValues<string>? Description { get; set; }

    [ColumnName("Manufacturer")]
    public ComparedValues<string> Manufacturer { get; set; } = null!;

    [ColumnName("Manufacturer Part Number")]
    public ComparedValues<string> ManufacturerPartNumber { get; set; } = null!;

    [ColumnName("Distributor")]
    public ComparedValues<string>? Distributor { get; set; }

    [ColumnName("Distributor Part Number")]
    public ComparedValues<string>? DistributorPartNumber { get; set; }
}
