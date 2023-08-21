using NPOI.SS.UserModel;

namespace BomWriter.Styles
{
    public static class CellStyleProvider
    {
        public static ICellStyle AddedCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();

            font.Color = IndexedColors.Green.Index;
            cellStyle.SetFont(font);

            return cellStyle;
        }

        public static ICellStyle RemovedCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();

            font.Color = IndexedColors.Red.Index;
            font.IsStrikeout = true;
            cellStyle.SetFont(font);

            return cellStyle;
        }

        public static ICellStyle DefaultCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();

            font.Color = IndexedColors.Black.Index;
            font.IsStrikeout = false;
            font.IsBold = false;
            cellStyle.SetFont(font);

            return cellStyle;
        }

        public static ICellStyle HeaderCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            var font = workbook.CreateFont();

            font.IsBold = true;
            cellStyle.SetFont(font);
            cellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            return cellStyle;
        }
    }
}
