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
        public static MapDrawingDataPoint MakeDrawingPointForSensor(double lon, double lat, string name = "") => new MapDrawingDataPoint
        {
            Color = System.Windows.Media.Brushes.Blue,
            Fill = System.Windows.Media.Brushes.Blue,
            Location = new Location() { Lat = lat, Lon = lon },
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
        public static MapDrawingDataPoint MakeDrawingPointForCountour(double lon, double lat, string name = "") => new MapDrawingDataPoint
        {
            Color = System.Windows.Media.Brushes.Green,
            Fill = System.Windows.Media.Brushes.ForestGreen,
            Location = new Location() { Lat = lat, Lon = lon},
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
        public static MapDrawingDataPoint MakeDrawingPointForCountourAffected(double lon, double lat, string name = "") => new MapDrawingDataPoint
        {
            Color = System.Windows.Media.Brushes.Red,
            Fill = System.Windows.Media.Brushes.DarkRed,
            Location = new Location() { Lat = lat, Lon = lon },
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
    }
}
