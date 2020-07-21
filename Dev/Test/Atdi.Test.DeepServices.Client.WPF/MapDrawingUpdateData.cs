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
    public static class MapDrawingUpdateData
    {
        public static MapDrawingData UpdateData(TypeObject typeObjectInput, Location[] locationsInput,
                                                TypeObject typeObjectOutput, Location[] locationsOutput)
        {

            var data = new MapDrawingData();
            var points = new List<MapDrawingDataPoint>();
            var polygons = new List<MapDrawingDataPolygon>();

            if (typeObjectInput == TypeObject.Points)
            {
                if ((locationsInput != null) && (locationsInput.Length > 0))
                {
                    for (int i = 0; i < locationsInput.Length; i++)
                    {
                        points.Add(new MapDrawingDataPoint()
                        {
                            Location = locationsInput[i],
                            Color = System.Windows.Media.Brushes.Blue,
                            Fill = System.Windows.Media.Brushes.BlueViolet,
                            Opacity = 0.85,
                            Width = 10,
                            Height = 10
                        });
                    }
                }
            }

            if (typeObjectOutput == TypeObject.Points)
            {
                if ((locationsOutput != null) && (locationsOutput.Length > 0))
                {
                    for (int i = 0; i < locationsOutput.Length; i++)
                    {
                        points.Add(new MapDrawingDataPoint()
                        {
                            Location = locationsOutput[i],
                            Color = System.Windows.Media.Brushes.Orange,
                            Fill = System.Windows.Media.Brushes.OrangeRed,
                            Opacity = 0.85,
                            Width = 5,
                            Height = 5
                        });
                    }
                }
            }

            if (typeObjectInput == TypeObject.Polygon)
            {
                polygons.Add(new MapDrawingDataPolygon()
                {
                    Points = locationsInput,
                    Color = System.Windows.Media.Colors.Blue,
                    Fill = System.Windows.Media.Colors.BlueViolet,
                });
            }

            if (typeObjectOutput == TypeObject.Polygon)
            {
                polygons.Add(new MapDrawingDataPolygon()
                {
                    Points = locationsOutput,
                    Color = System.Windows.Media.Colors.Orange,
                    Fill = System.Windows.Media.Colors.OrangeRed,
                });
            }

            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();

            return data;
        }
    }
}
