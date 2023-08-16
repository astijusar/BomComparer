using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Enums;

namespace BomComparer.Models
{
    public class DesignatorComparisonResultEntry
    {
        public ComparisonResultStatus Status { get; set; }
        public string? Value { get; set; }
    }
}
