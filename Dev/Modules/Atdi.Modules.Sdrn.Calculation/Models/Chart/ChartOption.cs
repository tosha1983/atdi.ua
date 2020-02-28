using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace Atdi.Modules.Sdrn.Chart
{
    public struct ChartOption
    {
        public string Title { get; set; }

        public string LeftTitle { get; set; }

        public string RightTitle { get; set; }

        public ChartMenuItem[] MenuItems { get; set; }

        public PointF[] Points { get; set; }

        public ChartPoints[] PointsArray { get; set; }

        public ChartLine[] LinesArray { get; set; }

        public ChartType ChartType { get; set; }

        public float XMin { get; set; }

        public float XMax { get; set; }

        public float XTick { get; set; }

        public float XInnerTickCount { get; set; }

        public string XLabel { get; set; }

        public float YMin { get; set; }

        public float YMax { get; set; }

        public float YTick { get; set; }

        public float YInnerTickCount { get; set; }

        public string YLabel { get; set; }

        public bool UseZoom { get; set; }
        public bool IsEnableSaveToFile { get; set; }
}
}
