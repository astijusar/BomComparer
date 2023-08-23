using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BomWriter.Styles
{
    public class CellStyleProvider
    {
        private readonly IWorkbook _workbook;
        private ICellStyle? _addedCellStyle;
        private ICellStyle? _removedCellStyle;
        private ICellStyle? _defaultCellStyle;
        private ICellStyle? _headerCellStyle;
        private ICellStyle? _modifiedRemovedCellStyle;
        private ICellStyle? _modifiedAddedCellStyle;

        public CellStyleProvider(IWorkbook workbook)
        {
            _workbook = workbook;
        }

        public ICellStyle GetAddedCellStyle() =>
            _addedCellStyle ??= CreateCellStyle(IndexedColors.Green.Index, false, false);

        public ICellStyle GetRemovedCellStyle() => 
            _removedCellStyle ??= CreateCellStyle(IndexedColors.Red.Index, true, false);

        public ICellStyle GetDefaultCellStyle() => 
            _defaultCellStyle ??= CreateCellStyle(IndexedColors.Black.Index, false, false);

        public ICellStyle GetModifiedRemovedCellStyle() =>
            _modifiedRemovedCellStyle ??= CreateCellStyle(IndexedColors.Red.Index, true, true);

        public ICellStyle GetModifiedAddedCellStyle() =>
            _modifiedAddedCellStyle ??= CreateCellStyle(IndexedColors.Green.Index, false, true);

        public ICellStyle GetHeaderCellStyle() => 
            _headerCellStyle ??= CreateHeaderCellStyle();

        private ICellStyle CreateCellStyle(short fontColor, bool isStrikeout, bool isBold)
        {
            var cellStyle = _workbook.CreateCellStyle();
            var font = _workbook.CreateFont();

            font.Color = fontColor;
            font.IsStrikeout = isStrikeout;
            font.IsBold = isBold;
            cellStyle.SetFont(font);

            return cellStyle;
        }

        private ICellStyle CreateHeaderCellStyle()
        {
            var cellStyle = _workbook.CreateCellStyle();
            var font = _workbook.CreateFont();

            font.IsBold = true;
            cellStyle.SetFont(font);
            cellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            return cellStyle;
        }
    }
}
