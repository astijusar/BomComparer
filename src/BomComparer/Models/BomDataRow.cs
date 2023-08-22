using BomComparer.Attributes;

namespace BomComparer.Models
{
    public class BomDataRow
    {
        [ColumnName("Quantity")]
        public int Quantity { get; set; }

        [PrimaryKey]
        [ColumnName("Part Number")]
        public string PartNumber { get; set; } = null!;

        [ColumnName("Designator")]
        public List<string> Designators { get; set; } = null!;

        [ColumnName("Value")]
        public string Value { get; set; } = null!;

        [ColumnName("Smd")]
        public string Smd { get; set; } = null!;

        [ColumnName("Description")] 
        public string? Description { get; set; }

        [ColumnName("Manufacturer")]
        public string Manufacturer { get; set; } = null!;

        [ColumnName("Manufacturer Part Number")]
        public string ManufacturerPartNumber { get; set; } = null!;

        [ColumnName("Distributor")]
        public string? Distributor { get; set; }

        [ColumnName("Distributor Part Number")]
        public string? DistributorPartNumber { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is BomDataRow dataRow && PartNumber == dataRow.PartNumber;
        }

        public override int GetHashCode()
        {
            return PartNumber.GetHashCode();
        }
    }
}
