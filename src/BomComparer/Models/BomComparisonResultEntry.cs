using System.Security.Cryptography;
using BomComparer.Enums;

namespace BomComparer.Models;

public class BomComparisonResultEntry
{
    public ComparisonResultStatus Status { get; set; }
    public List<DesignatorComparisonResultEntry>? Designators { get; set; }
    public string? PartNumber { get; set; }
    public ComparedValues<int>? Quantity { get; set; }
    public ComparedValues<string>? Value { get; set; }
    public ComparedValues<string>? SMD { get; set; }
    public ComparedValues<string>? Description { get; set; }
    public ComparedValues<string>? Manufacturer { get; set; }
    public ComparedValues<string>? ManufacturerPartNumber { get; set; }
    public ComparedValues<string>? Distributor { get; set; }
    public ComparedValues<string>? DistributorPartNumber { get; set; }
}
