using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows.Forms;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasTaskShortViewModel
    {
        public long MeasTaskId { get; set; }
        public string TaskType { get; set; }
        public string TaskName { get; set; }
        public double? FqMin { get; set; }
        public double? FqMax { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateStop { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public string SensorIds { get; set; }
    }
}
