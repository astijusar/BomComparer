using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Attributes;

namespace BomComparer.Models
{
    public class BomDataRow
    {
        public int Quantity { get; set; }
        [ColumnName("Part Number")]
        public string? PartNumber { get; set; }
        [ColumnName("Designator")]
        public List<string>? Designators { get; set; }
        public string? Value { get; set; }
        public string? SMD { get; set; }
        public string? Description { get; set; }
        public string? Manufacturer { get; set; }
        [ColumnName("Manufacturer Part Number")]
        public string? ManufacturerPartNumber { get; set; }
        public string? Distributor { get; set; }
        [ColumnName("Distributor Part Number")]
        public string? DistributorPartNumber { get; set; }
    }
}
