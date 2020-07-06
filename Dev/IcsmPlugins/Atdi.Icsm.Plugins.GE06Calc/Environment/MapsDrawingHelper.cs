using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.WpfControls.EntityOrm.Controls;

namespace Atdi.Icsm.Plugins.GE06Calc.Environment
{
    public static class MapsDrawingHelper
    {
        public static MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat, string name = "") => new MapDrawingDataPoint
        {
            Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
            Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
            Location = new Location() { Lat = lat, Lon = lon },
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
        public static MapDrawingDataPoint MakeDrawingPointForStation(double lon, double lat, string name = "") => new MapDrawingDataPoint
        {
            Color = System.Windows.Media.Brushes.Green,
            Fill = System.Windows.Media.Brushes.ForestGreen,
            Location = new Location() { Lat = lat, Lon = lon},
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
    }
}
