using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace XICSM.ICSControlClient.Models
{
    public class GroupeEmissionProtocolDataFilter : IDataErrorInfo
    {
        public string Owner { get; set; }
        public short? DateMeasDay { get; set; }
        public short? DateMeasMonth { get; set; }
        public short? DateMeasYear { get; set; }
        public string Standard { get; set; }
        public double? Freq_MHz { get; set; }
        public double? Probability { get; set; }
        public string Province { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string PermissionNumber { get; set; }
        public DateTime? PermissionStart { get; set; }
        public DateTime? PermissionStop { get; set; }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                //switch (columnName)
                //{
                //    case "DateMeasDay":
                //        if (DateMeasDay.HasValue && (DateMeasDay.Value < 1 || DateMeasDay.Value > 31))
                //        {
                //            error = "The value must be in the range from 1 to 31";
                //        }
                //        break;
                //    case "DateMeasMonth":
                //        if (DateMeasMonth.HasValue && (DateMeasMonth.Value < 1 || DateMeasMonth.Value > 12))
                //        {
                //            error = "The value must be in the range from 1 to 12";
                //        }
                //        break;
                //}
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
    }
}
