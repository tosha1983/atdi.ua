using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WpfControls.Charts
{
    static class CalcBWForChart
    {
        public struct ResultBWForChart
        {
            public double T1_MHz { get; set; }
            public double T2_MHz { get; set; }
            public double Marker_MHz { get; set; }
            public double BW_kHz { get; set; }
            public bool CorrectnessEstimations { get; set; }
        }
        public static ResultBWForChart getBW(float[] levels_dBm, double[] freq_MHz, BandWidthEstimation.BandwidthEstimationType bandwidthEstimationType, double X_Beta, int MaximumIgnorPoint)
        {
            if ((freq_MHz == null)|| (levels_dBm == null) ||(levels_dBm.Length != freq_MHz.Length))
            {
                ResultBWForChart resultBWForChart = new ResultBWForChart
                {
                    CorrectnessEstimations = false
                };
                return resultBWForChart;
            }
            BandWidthEstimation.BandwidthResult bandwidthResult = new BandWidthEstimation.BandwidthResult();
            bandwidthResult = BandWidthEstimation.GetBandwidthPoint(levels_dBm, bandwidthEstimationType, X_Beta, MaximumIgnorPoint);
            if (bandwidthResult.СorrectnessEstimations)
            {
                ResultBWForChart resultBWForChart = new ResultBWForChart
                {
                    T1_MHz = freq_MHz[bandwidthResult.T1],
                    T2_MHz = freq_MHz[bandwidthResult.T2],
                    Marker_MHz = freq_MHz[bandwidthResult.MarkerIndex],
                    CorrectnessEstimations = true,
                    BW_kHz = freq_MHz[bandwidthResult.T2] - freq_MHz[bandwidthResult.T1]
                };
                return resultBWForChart;
            }
            else
            {
                ResultBWForChart resultBWForChart = new ResultBWForChart
                {
                    CorrectnessEstimations = false
                };
                return resultBWForChart;
            }
        }
    }
}
