using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace XICSM.ICSControlClient.ViewModels.Chart
{
    public class ChartLine
    {
        public double Freq_Mhz { get; set; }
        public double freqMiddle_MHz { get; set; }
        public double level_dBm { get; set; }
        public string Num { get; set; }
        public string Name { get; set; }
        public double LabelLeft { get; set; }
        public double LabelTop { get; set; }
        public PointF Point { get; set; }
        public Brush LineColor { get; set; }
        public bool IsHorizontal { get; set; }
        public bool IsVertical { get; set; }
    }
}
