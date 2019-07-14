using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class SignSysInfoViewModel
    {
        public decimal Freq_Hz { get; set; }
        public string Standart { get; set; }
        public double? BandWidth_Hz { get; set; }
        public double? Level_dBm { get; set; }
        public int? CID { get; set; }
        public int? MCC { get; set; }
        public int? MNC { get; set; }
        public WorkTime[] WorkTimes { get; set; }
        public int? BSIC { get; set; }
        public int? ChannelNumber { get; set; }
        public int? LAC { get; set; }
        public int? RNC { get; set; }
        public double? CtoI { get; set; }
        public double? Power { get; set; }
    }
}
