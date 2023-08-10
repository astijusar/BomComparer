using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BomComparer.Factories
{
    public class WorkbookFactory
    {
        public IWorkbook CreateWorkbook(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileExtension = Path.GetExtension(filePath);

            if (fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
                return new HSSFWorkbook(fileStream);

            if (fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return new XSSFWorkbook(fileStream);

            throw new NotSupportedException("Unsupported file format.");
        }
    }
}
