using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Attributes;

namespace BomComparer.Models
{
    public class BomComparisonResult
    {
        public List<BomComparisonResultEntry> ResultEntries { get; set; } = new List<BomComparisonResultEntry>();
    }
}
