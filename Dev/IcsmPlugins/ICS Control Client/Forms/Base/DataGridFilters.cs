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
}
