using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.Reports
{
    public class ExcelFastNPOI : IDisposable
    {
        private bool _disposed = false;
        private ICellStyle cellStyle = null;
        private ICellStyle cellStyleHead = null;
        private ICellStyle cellStyle_string = null;
        private ICellStyle cellStyleBackground = null;
        private ICellStyle cellStyleBackgroundDefault = null;
        private ICellStyle cellStyleDefault = null;
        private List<int> ListRows = null;
        public IWorkbook oWorkbook_ = null;
        private ISheet oWorksheet_ = null;
        private IRow oRows_ = null;

        public ExcelFastNPOI()
        {
            ListRows = new List<int>();
        }
        public ICellStyle GetCellStyleBorder()
        {
            return cellStyle;
        }

        public ICellStyle GetCellStyleCustom()
        {
            return cellStyle_string;
        }

        public ICellStyle GetCellStyleBackgroundColor()
        {
            return cellStyleBackground;
        }

        public ICellStyle GetCellStyleBackgroundDefaultColor()
        {
            return cellStyleBackgroundDefault;
        }

        public IWorkbook GetWorkBook(string Path)
        {
            oWorkbook_ = new XSSFWorkbook(Path);
            oWorksheet_ = oWorkbook_.GetSheetAt(0);
            return oWorkbook_;
        }


        public object GetCellValue(int col, int row)
        {
            object val = "";
            IRow rows = oWorksheet_.GetRow(row);
            if (rows != null)
            {

                ICell c_val = rows.GetCell(col);
                ICellStyle style = c_val.CellStyle;
                IDataFormat dataFormatCustom = oWorkbook_.CreateDataFormat();
                c_val.CellStyle.DataFormat = dataFormatCustom.GetFormat("dd.MM.yyyy");
                CellType tp = c_val.CellType;
                switch (tp)
                {
                    case CellType.Numeric:
                        val = c_val.NumericCellValue;
                        break;
                    case CellType.String:
                        val = c_val.StringCellValue;
                        break;
                    case CellType.Boolean:
                        val = c_val.BooleanCellValue;
                        break;
                }
                if (col == 5)
                {
                    val = c_val.DateCellValue;
                }
            }
            return val;
        }

        public object GetCellValueTxt(int col, int row)
        {
            object val = "";
            IRow rows = oWorksheet_.GetRow(row);
            if (rows != null)
            {
                ICell c_val = rows.GetCell(col);
                ICellStyle style = c_val.CellStyle;
                IDataFormat dataFormatCustom = oWorkbook_.CreateDataFormat();
                c_val.CellStyle.DataFormat = dataFormatCustom.GetFormat("dd.MM.yyyy");
                CellType tp = c_val.CellType;
                switch (tp)
                {
                    case CellType.Numeric:
                        val = c_val.NumericCellValue;
                        try
                        {
                            if (c_val.DateCellValue != null)
                            {
                                val = c_val.DateCellValue;
                            }
                        }
                        catch (Exception) { }
                        break;
                    case CellType.String:
                        val = c_val.StringCellValue;
                        break;
                    case CellType.Boolean:
                        val = c_val.BooleanCellValue;
                        break;
                }
            }
            return val;
        }

        public ICellStyle GetCellStyleDefault()
        {
            return cellStyleDefault;
        }

        public ICellStyle GetCellStyleHead()
        {
            return cellStyleHead;
        }


        // Освободить "книгу" 
        private void ReleaseWorkbook()
        {
            if (oWorkbook_ != null)
            {
                oWorkbook_ = null;
                ListRows = null;
            }
        }

        // Освободить "лист" - закладку на "книге"
        private void ReleaseWorksheet()
        {
            if (oWorksheet_ != null)
            {
                oWorksheet_ = null;
                ListRows = null;
            }
        }

        /// <summary>
        /// Установить цвет фона для ячейки
        /// </summary>
        /// <param name="col">Номер колонки, начинается с 1</param>
        /// <param name="row">Номер строки, начинается с 1</param>
        /// <param name="color">цвет фона</param>
        public void SetCellBackgroundColor(int col, int row, Color color)
        {

        }

        /// <summary>
        /// Перед использованием нужно добавить книгу.
        /// У каждой книги по умолчанию присуствует один лист
        /// </summary>
        public void AddWorkbook(string NameSheet)
        {
            ReleaseWorkbook();
            ReleaseWorksheet();
            GC.GetTotalMemory(true); // Вызываем сборщик мусора для немедленной очистки памяти
            oWorkbook_ = new XSSFWorkbook();
            oWorksheet_ = oWorkbook_.CreateSheet(NameSheet);
            SetBorderStyle(false);
            SetBorderStyleHead(false);
            SetBackgroundColor(false);
            SetDefaultBackgroundColor(false);
            SetBorderStyle(0, 0, 0, 0, 0);
        }


        public IWorkbook GetWorkbookEx()
        {
            return oWorkbook_;
        }

        /// <summary>
        /// Вставка WorkSheet
        /// </summary>
        /// <param name="NameSheet"></param>
        public void AddWorkbookEx(IWorkbook wk_, string NameSheet)
        {
            ReleaseWorksheet();
            ListRows = new List<int>();
            oWorksheet_ = wk_.CreateSheet(NameSheet);
            SetBorderStyle(false);
            SetBackgroundColor(false);
            SetBorderStyle(0, 0, 0, 0, 0);
        }

        /// <summary>
        /// Установка ширины столбцов
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Width"></param>

        public void SetColumnWidth(int col, int length_row)
        {
            oWorksheet_.SetColumnWidth(col, (int)(1.25 * length_row * 256));
        }

        /// <summary>
        /// Установка направления текста
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Orientation"></param>
        public void SetTextOrientation(int col, int row, int Orientation)
        {
            //
        }


        /// <summary>
        /// Выравнивание текста в ячейке по вертикали
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Alignment"></param>
        public void SetVerticalAlignment(int col, int row, int Alignment)
        {
            //
        }

        /// <summary>
        /// Выравнивание текста в ячейке по горизонтали
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Alignment"></param>
        public void SetHorisontalAlignment(int col, int row, int Alignment)
        {
            //
        }

        /// <summary>
        /// Установка высоты строки
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Height"></param>
        public void SetRowHeight(int col, int row, double Height)
        {
            //
        }

        /// <summary>
        /// Объединение ячеек
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        public void UnionCell(int col1, int row1, int col2, int row2, ICellStyle st, string val)
        {
            IRow row = null;
            ICell cell = null;

            oWorksheet_.AddMergedRegion(new CellRangeAddress(row1, row2, col1, col2));
            oWorksheet_.AutoSizeColumn(col1);

            if (!ListRows.Contains(row1))
            {
                row = oWorksheet_.CreateRow(row1);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row1);
            }
            else
            {
                row = oWorksheet_.GetRow(row1);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);

            }

            for (int i = col1; i <= col2; i++)
            {
                cell = row.CreateCell(i);
                cell.CellStyle = st;
            }
            cell = row.CreateCell(col1);
            cell.CellStyle = st;
            cell.SetCellValue(val);

            if (!ListRows.Contains(row2))
            {
                row = oWorksheet_.CreateRow(row2);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row2);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);

            }
            else
            {
                row = oWorksheet_.GetRow(row2);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);
            }
        }


        /// <summary>
        /// Объединение ячеек
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        public void UnionCellByHeight(int col1, int row1, int col2, int row2, ICellStyle st, string val, float heightCell)
        {
            IRow row = null;
            ICell cell = null;

            oWorksheet_.AddMergedRegion(new CellRangeAddress(row1, row2, col1, col2));
            oWorksheet_.AutoSizeColumn(col1);

            if (!ListRows.Contains(row1))
            {
                row = oWorksheet_.CreateRow(row1);
                row.HeightInPoints = ((heightCell) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row1);
            }
            else
            {
                row = oWorksheet_.GetRow(row1);
                row.HeightInPoints = ((heightCell) * oWorksheet_.DefaultRowHeightInPoints);

            }

            for (int i = col1; i <= col2; i++)
            {
                cell = row.CreateCell(i);
                cell.CellStyle = st;
            }
            cell = row.CreateCell(col1);
            cell.CellStyle = st;
            cell.SetCellValue(val);

            if (!ListRows.Contains(row2))
            {
                row = oWorksheet_.CreateRow(row2);
                row.HeightInPoints = ((heightCell) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row2);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);

            }
            else
            {
                row = oWorksheet_.GetRow(row2);
                row.HeightInPoints = ((heightCell) * oWorksheet_.DefaultRowHeightInPoints);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);

            }
        }

        public void UnionCell(int col1, int row1, int col2, int row2, ICellStyle st, IFont font, string val)
        {
            IRow row = null;
            ICell cell = null;

            oWorksheet_.AddMergedRegion(new CellRangeAddress(row1, row2, col1, col2));
            oWorksheet_.AutoSizeColumn(col1);

            if (!ListRows.Contains(row1))
            {
                row = oWorksheet_.CreateRow(row1);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row1);
            }
            else
            {
                row = oWorksheet_.GetRow(row1);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
            }

            for (int i = col1; i <= col2; i++)
            {
                cell = row.CreateCell(i);
                cell.CellStyle = st;
            }
            cell = row.CreateCell(col1);
            cell.CellStyle = st;
            cell.SetCellValue(val);

            if (!ListRows.Contains(row2))
            {
                row = oWorksheet_.CreateRow(row2);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row2);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);
            }
            else
            {
                row = oWorksheet_.GetRow(row2);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);

            }
            cell.CellStyle.SetFont(font);
        }

        public void UnionCell(int col1, int row1, int col2, int row2, ICellStyle st, string val, bool isCheck)
        {
            IRow row = null;
            ICell cell = null;

            oWorksheet_.AddMergedRegion(new CellRangeAddress(row1, row2, col1, col2));
            if (!ListRows.Contains(row1))
            {
                row = oWorksheet_.CreateRow(row1);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row1);
            }
            else
            {
                row = oWorksheet_.GetRow(row1);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
            }

            for (int i = col1; i <= col2; i++)
            {
                cell = row.CreateCell(i);
                cell.CellStyle = st;
            }
            cell = row.CreateCell(col1);
            cell.CellStyle = st;
            cell.SetCellValue(val);

            if (!ListRows.Contains(row2))
            {
                row = oWorksheet_.CreateRow(row2);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);
                ListRows.Add(row2);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);
            }
            else
            {
                row = oWorksheet_.GetRow(row2);
                row.HeightInPoints = (int)((3) * oWorksheet_.DefaultRowHeightInPoints);

                cell = row.CreateCell(col2);
                cell.CellStyle = st;
                cell.SetCellValue(val);
            }

        }

        private void mergeAndCenter(ICell startCell, CellRangeAddress range)
        {
            oWorksheet_.AddMergedRegion(range);
        }

        /// <summary>
        /// Установить размер шрифта
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Size"></param>
        public void SetCellFontSize(int col, int row, string Name, int Size)
        {
            //
        }

        /// <summary>
        /// Установить цвет ячейки
        /// </summary>

        public void SetBackgroundColor(bool isItalic)
        {
            cellStyleBackground = null;

            //fill background
            IFont font = null;

            cellStyleBackground = oWorkbook_.CreateCellStyle();
            cellStyleBackground.FillForegroundColor = IndexedColors.SkyBlue.Index;
            cellStyleBackground.FillPattern = FillPattern.Diamonds;
            cellStyleBackground.FillBackgroundColor = IndexedColors.SkyBlue.Index;
            cellStyleBackground.Alignment = HorizontalAlignment.Center;
            cellStyleBackground.VerticalAlignment = VerticalAlignment.Center;
            cellStyleBackground.WrapText = true;

            font = oWorkbook_.CreateFont();
            font.FontHeightInPoints = 14;
            font.FontName = "Times New Roman";
            font.IsItalic = isItalic;
            cellStyleBackground.SetFont(font);
        }

        /// <summary>
        /// Установить цвет ячейки
        /// </summary>

        public void SetDefaultBackgroundColor(bool isItalic)
        {
            cellStyleBackgroundDefault = null;

            //fill background
            IFont font = null;

            cellStyleBackgroundDefault = oWorkbook_.CreateCellStyle();
            cellStyleBackgroundDefault.Alignment = HorizontalAlignment.Center;
            cellStyleBackgroundDefault.VerticalAlignment = VerticalAlignment.Center;
            cellStyleBackgroundDefault.WrapText = true;

            font = oWorkbook_.CreateFont();
            font.FontHeightInPoints = 14;
            font.FontName = "Times New Roman";
            font.IsItalic = isItalic;
            cellStyleBackgroundDefault.SetFont(font);
        }

        /// <summary>
        /// Установить вид границ
        /// </summary>
        public void SetBorderStyle(bool isItalic)
        {
            cellStyle = null;

            IFont font = null;
            cellStyle = oWorkbook_.CreateCellStyle();
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BottomBorderColor = IndexedColors.Black.Index;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.LeftBorderColor = IndexedColors.Black.Index;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.RightBorderColor = IndexedColors.Black.Index;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.TopBorderColor = IndexedColors.Black.Index;
            cellStyle.WrapText = true;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            font = oWorkbook_.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Times New Roman";
            font.IsItalic = isItalic;
            cellStyle.SetFont(font);
        }

        /// <summary>
        /// Установить вид границ
        /// </summary>
        public void SetBorderStyleHead(bool isItalic)
        {
            cellStyleHead = null;

            IFont font = null;
            cellStyleHead = oWorkbook_.CreateCellStyle();

            cellStyleHead.FillForegroundColor = IndexedColors.Yellow.Index;
            cellStyleHead.FillPattern = FillPattern.Diamonds;
            cellStyleHead.FillBackgroundColor = IndexedColors.Yellow.Index;

            cellStyleHead.Alignment = HorizontalAlignment.Center;
            cellStyleHead.VerticalAlignment = VerticalAlignment.Center;
            cellStyleHead.BorderBottom = BorderStyle.Thin;
            cellStyleHead.BottomBorderColor = IndexedColors.Black.Index;
            cellStyleHead.BorderLeft = BorderStyle.Thin;
            cellStyleHead.LeftBorderColor = IndexedColors.Black.Index;
            cellStyleHead.BorderRight = BorderStyle.Thin;
            cellStyleHead.RightBorderColor = IndexedColors.Black.Index;
            cellStyleHead.BorderTop = BorderStyle.Thin;
            cellStyleHead.TopBorderColor = IndexedColors.Black.Index;
            cellStyleHead.WrapText = true;

            font = oWorkbook_.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Times New Roman";
            font.IsItalic = isItalic;
            cellStyleHead.SetFont(font);
        }

        public void SetColumnWidth(int col, int row, double Width)
        {
            //
        }

        /// <summary>
        /// Установить стиль по умолчанию
        /// </summary>
        public void SetBorderStyleDefault(bool isItalic)
        {
            cellStyleDefault = null;

            IFont font = null;
            cellStyleDefault = oWorkbook_.CreateCellStyle();
            cellStyleDefault.BorderBottom = BorderStyle.None;
            cellStyleDefault.BorderLeft = BorderStyle.None;
            cellStyleDefault.BorderRight = BorderStyle.None;
            cellStyleDefault.BorderTop = BorderStyle.None;
            cellStyleDefault.Alignment = HorizontalAlignment.Right;
            cellStyleDefault.WrapText = true;

            font = oWorkbook_.CreateFont();
            font.FontHeightInPoints = 12;
            font.FontName = "Times New Roman";
            font.IsItalic = isItalic;
            cellStyleDefault.SetFont(font);
        }

        public void InsertTextFinded(string SourceFile, string OutNumber, string Department, string Address, string Worker, string Telephone, string Devices)
        {
            using (FileStream fstream = new FileStream(SourceFile, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                string s = textFromFile.Replace(@":OUT_NUMBER", OutNumber).Replace(@":DEPARTMENT", Department).Replace(@":ADDRESS", Address).Replace(@":WORKER", Worker).Replace(@":TELEPHONE", Telephone).Replace(@":EQUIP", Devices);
                byte[] array_paste = System.Text.Encoding.Default.GetBytes(s);
                fstream.Position = 0;
                fstream.Write(array_paste, 0, array_paste.Length);
                fstream.Close();
            }
        }

        public void InsertTextNotFinded(string SourceFile, string OutNumber, string Department, string Address, string Worker, string Telephone, string Users)
        {

            using (FileStream fstream = new FileStream(SourceFile, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                string s = textFromFile.Replace(@":OUT_NUMBER", OutNumber).Replace(@":DEPARTMENT", Department).Replace(@":ADDRESS", Address).Replace(@":WORKER", Worker).Replace(@":TELEPHONE", Telephone).Replace(@":USERS", Users);
                byte[] array_paste = System.Text.Encoding.Default.GetBytes(s);
                fstream.Position = 0;
                fstream.Write(array_paste, 0, array_paste.Length);
                fstream.Close();
            }
        }

        public void InsertText(string SourceFile, string Devices)
        {

            using (FileStream fstream = new FileStream(SourceFile, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                string s = textFromFile.Replace(@":EQUIP", Devices);
                byte[] array_paste = System.Text.Encoding.Default.GetBytes(s);
                fstream.Position = 0;
                fstream.Write(array_paste, 0, array_paste.Length);
                fstream.Close();
            }
        }

        /// <summary>
        ///  Создать новую строку
        /// </summary>
        /// <param name="row">Номер строки, начинается с 1</param>

        public void CreateNewRow(int row)
        {
            oRows_ = oWorksheet_.CreateRow(row);
        }

        public void SetCellStyle(int col, int row, ICellStyle st)
        {
            ICell rtx = null;
            if (oRows_ == null)
            {
                oRows_ = oWorksheet_.GetRow(row);
                rtx = oRows_.GetCell(col);
                rtx.CellStyle = st;
            }
            else
            {
                rtx = oRows_.GetCell(col);
                rtx.CellStyle = st;
            }
        }

        /// <summary>
        ///  Создать новую строку
        /// </summary>
        /// <param name="row">Номер строки, начинается с 1</param>

        public void SetCellValue(int col, int row, string val, ICellStyle st)
        {
            ICell rtx = null;
            rtx = oRows_.CreateCell(col);
            rtx.CellStyle = st;
            rtx.SetCellValue(val);
        }

        /// <summary>
        ///  Создать новую строку (без стиля)
        /// </summary>
        /// <param name="row">Номер строки, начинается с 1</param>

        public void SetCellValue(int col, int row, string val)
        {
            ICell rtx = null;
            if (oRows_ == null)
            {
                oRows_ = oWorksheet_.CreateRow(row);
                rtx = oRows_.CreateCell(col);
                rtx.SetCellValue(val);
            }
            else
            {
                rtx = oRows_.CreateCell(col);
                rtx.SetCellValue(val);
            }
        }

        public void SetCellValue(int col, int row, double val, ICellStyle st)
        {
            ICell rtx = null;
            if (oRows_ == null)
                oRows_ = oWorksheet_.CreateRow(row);

            rtx = oRows_.CreateCell(col);
            rtx.CellStyle = st;
            rtx.SetCellValue(val);
        }
        public void SetCellValue(int col, int row, double val)
        {
            ICell rtx = null;
            if (oRows_ == null)
                oRows_ = oWorksheet_.CreateRow(row);

            rtx = oRows_.CreateCell(col);
            rtx.SetCellValue(val);
        }

        public void SetBorderStyle(int col1, int row1, int col2, int row2, int Style)
        {
            //
            cellStyle_string = null;

            IDataFormat frm = null;
            frm = oWorkbook_.CreateDataFormat();
            IFont font = null;
            cellStyle_string = oWorkbook_.CreateCellStyle();
            cellStyle_string.BorderBottom = BorderStyle.Thin;
            cellStyle_string.BottomBorderColor = IndexedColors.Black.Index;
            cellStyle_string.BorderLeft = BorderStyle.Thin;
            cellStyle_string.LeftBorderColor = IndexedColors.Black.Index;
            cellStyle_string.BorderRight = BorderStyle.Thin;
            cellStyle_string.RightBorderColor = IndexedColors.Black.Index;
            cellStyle_string.BorderTop = BorderStyle.Thin;
            cellStyle_string.TopBorderColor = IndexedColors.Black.Index;
            cellStyle_string.WrapText = true;
            cellStyle_string.DataFormat = frm.GetFormat("@");


            font = oWorkbook_.CreateFont();
            font.FontHeightInPoints = 8;
            font.FontName = "Times New Roman";
            cellStyle_string.SetFont(font);
        }


        /// <summary>
        /// Установить вид границ
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Size"></param>
        public void SetBorderStyle(int col, int row, int Style)
        {
            //
        }

        /// <summary>
        ///  Создать новую строку
        /// </summary>
        /// <param name="row">Номер строки, начинается с 1</param>

        public void ReleaseRow()
        {
            if (oRows_ != null)
            {
                oRows_ = null;
            }
        }

        /// <summary>
        /// Закрыть Excel
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            ReleaseWorksheet();
            ReleaseWorkbook();
            GC.GetTotalMemory(true); // Вызываем сборщик мусора для немедленной очистки памяти
            return true;
        }

        /// <summary>
        /// Сохранить Excel файл
        /// </summary>
        /// <param name="fileName">имя файла</param>
        public void Save(string fileName)
        {
            FileStream sw = File.Create(fileName);
            oWorkbook_.Write(sw);
            sw.Close();
        }

        #region Implementation of IDisposable

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Close();
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
