using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;

namespace BomWriter.Styles
{
    public static class FontStyleProvider
    {
        public static IFont AddedFontStyle(IWorkbook workbook)
        {
            var font = workbook.CreateFont();

            font.Color = IndexedColors.Green.Index;
            font.IsBold = true;

            return font;
        }

        public static IFont RemovedFontStyle(IWorkbook workbook)
        {
            var font = workbook.CreateFont();

            font.Color = IndexedColors.Red.Index;
            font.IsBold = true;
            font.IsStrikeout = true;

            return font;
        }
    }
}
