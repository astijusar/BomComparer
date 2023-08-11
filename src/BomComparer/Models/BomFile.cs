using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomComparer.Models
{
    public class BomFile
    {
        public string Name { get; set; }
        public List<BomDataRow> Data { get; set; } = new();
    }
}
