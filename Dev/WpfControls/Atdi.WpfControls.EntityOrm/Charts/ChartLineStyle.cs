﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Atdi.WpfControls.EntityOrm.Charts
{
    public class ChartLineStyle
    {
        public Brush Color { get; set; }

        public double Thickness { get; set; }

        public LinePatternEnum Pattern { get; set; }
    }
}
