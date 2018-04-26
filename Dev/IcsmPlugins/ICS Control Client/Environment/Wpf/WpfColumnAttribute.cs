using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XICSM.ICSControlClient.Environment.Wpf
{
    public enum ColumnWidthRule
    {
        Value = 0,
        AutoSize,
        SizeToCells,
        SizeToHeader
    }
    public class WpfColumnAttribute : Attribute
    {
        private string _displayName;
        private double _width;
        private string _cellStyle;
        private string _headerStyle;
        private ColumnWidthRule _widthRule;

        public WpfColumnAttribute(string displayName)
        {
            this._displayName = displayName;
        }

        public string DisplayName
        {
            get
            {
                return this._displayName;
            }
            set
            {
                this._displayName = value;
            }
        }

        public ColumnWidthRule WidthRule
        {
            get
            {
                return this._widthRule;
            }
            set
            {
                this._widthRule = value;
            }
        }

        public double Width
        {
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }

        public string CellStyle
        {
            get
            {
                return this._cellStyle;
            }
            set
            {
                this._cellStyle = value;
            }
        }

        public string HeaderStyle
        {
            get
            {
                return this._headerStyle;
            }
            set
            {
                this._headerStyle = value;
            }
        }
    }
}
