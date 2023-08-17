using BomComparer.Attributes;

namespace BomComparer.Models
{
    public class BomDataRow
    {
        [ColumnName("Quantity")]
        public int Quantity { get; set; }

        [PrimaryKey]
        [ColumnName("Part Number")]
        public string? PartNumber { get; set; }

        [ColumnName("Designator")]
        public List<string>? Designators { get; set; }

        [ColumnName("Value")]
        public string? Value { get; set; }

        [ColumnName("Smd")]
        public string? Smd { get; set; }

        [ColumnName("Description")]
        public string? Description { get; set; }

        [ColumnName("Manufacturer")]
        public string? Manufacturer { get; set; }

        [ColumnName("Manufacturer Part Number")]
        public string? ManufacturerPartNumber { get; set; }

        [ColumnName("Distributor")]
        public string? Distributor { get; set; }

        [ColumnName("Distributor Part Number")]
        public string? DistributorPartNumber { get; set; }

        public override bool Equals(object? obj)
        {
            var item = obj as BomDataRow;

            return item != null && PartNumber == item.PartNumber;
        }

        public override int GetHashCode()
        {
            return PartNumber.GetHashCode();
        }
    }
}
