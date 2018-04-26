using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XICSM.ICSControlClient.WpfControls.Charts
{
    public struct ChartOption
    {
        public string Title { get; set; }

        public Point[] Points { get; set; }

        public ChartType ChartType { get; set; }

        public double XMin { get; set; }

        public double XMax { get; set; }

        public double XTick { get; set; }

        public double XInnerTickCount { get; set; }

        public string XLabel { get; set; }

        public double YMin { get; set; }

        public double YMax { get; set; }

        public double YTick { get; set; }

        public double YInnerTickCount { get; set; }

        public string YLabel { get; set; }
    }
}
