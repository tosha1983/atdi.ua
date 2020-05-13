using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP = XICSM.ICSControlClient.WpfControls.Maps;

namespace XICSM.ICSControlClient
{
    public static class MapsDrawingHelper
    {
        public static MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat, string name = "") => new MP.MapDrawingDataPoint
        {
            Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
            Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
            Location = new Models.Location(lon, lat),
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
        public static MP.MapDrawingDataPoint MakeDrawingPointForStation(double lon, double lat, string name = "") => new MP.MapDrawingDataPoint
        {
            Color = System.Windows.Media.Brushes.Green,
            Fill = System.Windows.Media.Brushes.ForestGreen,
            Location = new Models.Location(lon, lat),
            Opacity = 0.85,
            Width = 10,
            Height = 10,
            Name = name
        };
    }
}
