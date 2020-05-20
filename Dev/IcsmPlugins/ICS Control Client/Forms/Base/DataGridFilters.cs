using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Forms
{
    public class DataGridFilters
    {
        public Dictionary<string, DataGridFilterBool> FiltersBool;
        public Dictionary<string, DataGridFilterNumeric> FiltersNumeric;
        public Dictionary<string, DataGridFilterString> FiltersString;
        public Dictionary<string, DataGridFilterDate> FiltersDate;
    }
    public class DataGridFilter
    {
    }
    public class DataGridFilterNumeric : DataGridFilter
    {
        public double? FromValue;
        public double? ToValue;
    }
    public class DataGridFilterBool : DataGridFilter
    {
        public bool? Value;
    }
    public class DataGridFilterString : DataGridFilter
    {
        public string Value;
    }
    public class DataGridFilterDate : DataGridFilter
    {
        public DateTime? FromValue;
        public DateTime? ToValue;
    }
}
