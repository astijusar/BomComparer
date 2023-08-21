using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;

namespace BomWriter.Styles
{
    public class FontStyleProvider
    {
        private readonly IWorkbook _workbook;
        private IFont? _addedFontStyle;
        private IFont? _removedFontStyle;

        public FontStyleProvider(IWorkbook workbook)
        {
            _workbook = workbook;
        }

        public IFont GetAddedFontStyle() =>
            _addedFontStyle ??= CreateFontStyle(IndexedColors.Green.Index, false, true);

        public IFont GetRemovedFontStyle() =>
            _removedFontStyle ??= CreateFontStyle(IndexedColors.Red.Index, true, true);

        private IFont CreateFontStyle(short fontColor, bool isStrikeout, bool isBold)
        {
            var font = _workbook.CreateFont();

            font.Color = fontColor;
            font.IsStrikeout = isStrikeout;
            font.IsBold = isBold;

            return font;
        }
    }
}
