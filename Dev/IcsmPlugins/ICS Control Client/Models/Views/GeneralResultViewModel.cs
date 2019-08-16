using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class GeneralResultViewModel
    {
        public double? CentralFrequency { get; set; }
        public double? CentralFrequencyMeas { get; set; }
        public double? OffsetFrequency { get; set; }
        public decimal? SpecrumStartFreq { get; set; }
        public decimal? SpecrumSteps { get; set; }
        public float[] LevelsSpecrum { get; set; }
        public MaskElements[] MaskBW { get; set; }
        public double? DurationMeas { get; set; }
        public DateTime? TimeStartMeas { get; set; }
        public DateTime? TimeFinishMeas { get; set; }
        public double? T1 { get; set; }
        public double? T2 { get; set; }
        public double? MarkerIndex { get; set; }
        public int? NumberPointsOfSpectrum { get; set; }
    }
}
