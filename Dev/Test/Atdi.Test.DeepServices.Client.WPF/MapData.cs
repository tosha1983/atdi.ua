using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using GMap.NET;
using GMap.NET.WindowsPresentation;
using GMap.NET.MapProviders;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Atdi.Test.DeepServices.Client.WPF
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
        public Color Color;
        public Color Fill;
        public Location[] Points;
    }
    public class MapDrawingDataPolygon
    {
        public Color Color;
        public Color Fill;
        public Location[] Points;
    }
    public class MapDrawingData
    {
        public MapDrawingDataPoint[] Points { get; set; }
        public MapDrawingDataRoute[] Routes { get; set; }
        public MapDrawingDataPolygon[] Polygons { get; set; }
    }
}
