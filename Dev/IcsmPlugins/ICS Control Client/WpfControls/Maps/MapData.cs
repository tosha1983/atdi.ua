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
    }
    public class MapDrawingData
    {
        public MapDrawingDataPoint[] Points { get; set; }
    }
}
