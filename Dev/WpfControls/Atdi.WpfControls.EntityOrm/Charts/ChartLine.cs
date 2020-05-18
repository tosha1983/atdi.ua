using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Atdi.WpfControls.EntityOrm.Charts
{
    public class ChartLine
    {
        public string Name { get; set; }
        public double LabelLeft { get; set; }
        public double LabelTop { get; set; }
        public Point Point { get; set; }
        public Brush LineColor { get; set; }
        public bool IsHorizontal { get; set; }
        public bool IsVertical { get; set; }
    }
}
