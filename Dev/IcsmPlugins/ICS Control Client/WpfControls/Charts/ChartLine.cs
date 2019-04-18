﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace XICSM.ICSControlClient.WpfControls.Charts
{
    public class ChartLine
    {
        public Point Point { get; set; }
        public Brush LineColor { get; set; }
        public bool IsHorizontal { get; set; }
        public bool IsVertical { get; set; }
    }
}
