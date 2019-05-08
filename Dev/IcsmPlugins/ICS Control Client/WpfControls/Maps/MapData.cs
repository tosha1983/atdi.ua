using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XICSM.ICSControlClient.Models;

namespace XICSM.ICSControlClient.WpfControls.Maps
{
    public class MapDrawingDataPoint
    {
        public Brush Color;
        public Brush Fill;
        public Location Location;
        public double Opacity;
        public double Width;
        public double Height;
        public string Name;
    }
    public class MapDrawingDataRoute
    {
        public System.Windows.Media.Color Color;
        public System.Windows.Media.Color Fill;
        public Location[] Points;
    }
    public class MapDrawingDataPolygon
    {
        public System.Windows.Media.Color Color;
        public System.Windows.Media.Color Fill;
        public Location[] Points;
    }
    public class MapDrawingData
    {
        public MapDrawingDataPoint[] Points { get; set; }
        public MapDrawingDataRoute[] Routes { get; set; }
        public MapDrawingDataPolygon[] Polygons { get; set; }
    }
}
