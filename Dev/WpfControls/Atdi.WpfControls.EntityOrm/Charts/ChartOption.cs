using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Atdi.WpfControls.EntityOrm.Charts
{
    public struct ChartOption
    {
        public string Title { get; set; }

        public string LeftTitle { get; set; }

        public string RightTitle { get; set; }

        public ChartMenuItem[] MenuItems { get; set; }

        public Point[] Points { get; set; }

        public ChartPoints[] PointsArray { get; set; }

        public ChartLine[] LinesArray { get; set; }

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

        public bool UseZoom { get; set; }
        public bool IsEnableSaveToFile { get; set; }
    }
}
