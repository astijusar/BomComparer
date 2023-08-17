using BomComparer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomComparer.ExcelReader
{
    public interface IExcelReader
    {
        public BomFile ReadData(string filePath);
    }
}
