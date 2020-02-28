using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Drawing;

namespace Atdi.Modules.Sdrn.Chart
{
    public enum LinePatternEnum
    {
        Solid = 1,
        Dash = 2,
        Dot = 3,
        DashDot = 4,
        None = 5
    }

    public enum GridlinePatternEnum
    {
        Solid = 1,
        Dash = 2,
        Dot = 3,
        DashDot = 4
    }

    public class ChartColumnsStyle
    {
        public Brush FillColor { get; set; }

        public Brush StrokeColor { get; set; }

        public double Thickness { get; set; }

        public LinePatternEnum Pattern { get; set; }
    }
}
