using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Atdi.WpfControls.EntityOrm.Charts
{
    public class ChartGridStyle
    {
        public double LeftOffset { get; set; }
        public double BottomOffset { get; set; }
        public double RightOffset { get; set; }

        public GridlinePatternEnum LinePattern { get; set; }

        public GridlinePatternEnum InnerLinePattern { get; set; }

        public Brush LineColor { get; set; }

        public Brush InnerLineColor { get; set; }

        public double LineThickness { get; set; }
        public double InnerLineThickness { get; set; }

        public bool IsXGrid { get; set; }
        public bool IsYGrid { get; set; }

        public bool IsXInnerGrid { get; set; }
        public bool IsYInnerGrid { get; set; }
    }
}
