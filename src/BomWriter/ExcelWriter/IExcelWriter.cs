using BomComparer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomWriter.ExcelWriter
{
    public interface IExcelWriter
    {
        public void Write(string path, BomComparisonResult data);
    }
}
