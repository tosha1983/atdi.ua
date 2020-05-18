using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Atdi.WpfControls.EntityOrm.Charts
{
    public class ChartColumnsStyle
    {
        public Brush FillColor { get; set; }

        public Brush StrokeColor { get; set; }

        public double Thickness { get; set; }

        public LinePatternEnum Pattern { get; set; }
    }
}
