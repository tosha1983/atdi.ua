using System;
using System.IO;
using System.Linq;
using System.Drawing;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;


namespace XICSM.ICSControlClient.ViewModels.Reports
{
    class ReportExcelFast 
    {
        private ExcelFastNPOI _ew = null;
        private int _indexRow;
        private string _fileName;
        #region Implementation of IDisposable

        public void Dispose()
        {
            Close();
        }

        #endregion

        #region Implementation of IReport

        

        /// <summary>
        /// Установить текущий номер строки
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Size"></param>
        public void SetIndexRow(int Value)
        {
            _indexRow = Value;
        }

        /// <summary>
        /// Получить  номер текущей строки
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Size"></param>
        public int GetIndexRow()
        {
           return _indexRow;
        }
    
        /// <summary>
        /// Инициализация генератора
        /// </summary>
        /// <param name="fileName">Полный путь к файлу. Если разширение файла отсутствует, то оно добавиться автоматически</param>
        public void Init(string fileName,string NameSheet, string separator)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                fileName += ".xlsx";
            _fileName = fileName;
            _indexRow = 0;
            _ew = new ExcelFastNPOI();
            _ew.AddWorkbook(NameSheet);
        }

        /// <summary>
        /// Добавить новую палитру
        /// </summary>
        /// <param name="NameSheet"></param>
        public void AddSheet(string NameSheet)
        {
            _indexRow = 0;
            _ew.AddWorkbookEx(_ew.oWorkbook_, NameSheet);
        }

        public void SetCellStyle(int col, int row, ICellStyle st)
        {
            _ew.SetCellStyle(col, row, st);
        }

        /// <summary>
        /// Записть строку в файл
        /// </summary>
        /// <param name="columns">список значенией колонок</param>
        public void WriteLine(params string[] columns)
        {
            _ew.CreateNewRow(GetIndexRow());
            for (int j = 0; j < columns.Count(); j++)
            {
                _ew.SetCellValue(j, GetIndexRow(), columns[j], _ew.GetCellStyleBorder());
            }
            _ew.ReleaseRow();
            _indexRow++;
        }
    

        public void SetBackgroundColor(bool isItalic)
        {
            _ew.SetBackgroundColor(isItalic);
        }

        public void SetBorderStyle(bool isItalic)
        {
            _ew.SetBorderStyle(isItalic);
        }

        public void SetBorderStyleDefault(bool isItalic)
        {
            _ew.SetBorderStyleDefault(isItalic);
        }


        /// <summary>
        ///  Установить содержимое ячейки
        /// </summary>
        /// <param name="col">Номер колонки, начинается с 1</param>
        /// <param name="row">Номер строки, начинается с 1</param>
        /// <param name="val">Содержимое ячейки</param>
        public void SetCellValue(int col, int row, string val,ICellStyle st)
        {
            _ew.SetCellValue(col, row, val, st);
        }


     

        /// <summary>
        /// Сделать объединение ячеек
        /// </summary>
        /// <param name="columns">список значенией колонок</param>
        public void UnionCell(int col1, int row1, int col2, int row2, ICellStyle st, string val)
        {
            _ew.UnionCell(col1, row1, col2, row2,st, val);
        }

        /// <summary>
        /// Сделать объединение ячеек
        /// </summary>
        /// <param name="columns">список значенией колонок</param>
        public void UnionCellByHeight(int col1, int row1, int col2, int row2, ICellStyle st, string val, float height)
        {
            _ew.UnionCellByHeight(col1, row1, col2, row2, st, val, height);
        }

        public void UnionCell(int col1, int row1, int col2, int row2, ICellStyle st, string val, bool isCheck)
        {
            _ew.UnionCell(col1, row1, col2, row2,st, val, isCheck);
        }

        public void UnionCell(int col1, int row1, int col2, int row2, ICellStyle st, IFont font, string val)
        {
            _ew.UnionCell(col1, row1, col2, row2, st, font, val);
        }


         /// <summary>
        /// Установка ширины столбцов
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="Width"></param>

        public void SetColumnWidth(int col, int length_row)
        {
            _ew.SetColumnWidth(col, length_row);
        }



           /// <summary>
        /// Установить цвет фона для ячейки
        /// </summary>
        /// <param name="col">Номер колонки, начинается с 1</param>
        /// <param name="row">Номер строки, начинается с 1</param>
        /// <param name="color">цвет фона</param>
        public void SetCellBackgroundColor(int col, int row, Color color)
        {
            _ew.SetCellBackgroundColor(col, row, color);
        }
               
        /// <summary>
        /// Сохранить файл
        /// </summary>
        public void Save()
        {
            _ew.Save(_fileName);
        }


       
        /// <summary>
        /// Освободить все ресурсы
        /// </summary>
        public void Close()
        {
            if (_ew != null)
            {
                _ew.Close();
                _ew = null;
            }
        }

        public ICellStyle GetCellStyleBorder()
        {
            return _ew.GetCellStyleBorder();
        }

        public ICellStyle GetCellStyleHead()
        {
            return _ew.GetCellStyleHead();
        }

        public ICellStyle GetCellStyleBackgroundColor()
        {
            return _ew.GetCellStyleBackgroundColor();
        }

        public ICellStyle GetCellStyleBackgroundDefaultColor()
        {
            return _ew.GetCellStyleBackgroundDefaultColor();
        }

        public ICellStyle GetCellStyleDefault()
        {
            return _ew.GetCellStyleDefault();
        }

        public IWorkbook GetWorkBook()
        {
            return _ew.GetWorkbookEx();
        }


        #endregion
    }
}
