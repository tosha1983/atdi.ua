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

namespace XICSM.ICSControlClient.WpfControls.Maps
{
    public class Map : GMapControl
    {
        private MapDrawingData _drawingData;

        public static DependencyProperty DrawingDataProperty = DependencyProperty.Register("DrawingData", typeof(MapDrawingData), typeof(Map),
           new FrameworkPropertyMetadata(default(MapDrawingData), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));

        public MapDrawingData DrawingData
        {
            get { return this._drawingData; } // (ChartOption)GetValue(OptionProperty); }
            set
            {
                //SetValue(OptionProperty, value);
                this._drawingData = value;
                this.UpdateComponents();
            }
        }

        private void UpdateComponents()
        {
            this.Markers.Clear();

            if (this._drawingData != null)
            {
                var points = this._drawingData.Points;
                if (points != null && points.Length > 0)
                {
                    points.ToList().ForEach(point =>
                    {
                        var mapPoint = this.FromLatLngToLocal(new PointLatLng(point.Location.Lat, point.Location.Lon));
                       
                        var marker = new GMapMarker(new PointLatLng(point.Location.Lat, point.Location.Lon));
                        marker.Shape = new Ellipse() { Stroke = point.Color, Fill = point.Fill, Opacity = 0.85, Width = 10, Height = 10 };
                        marker.ZIndex = int.MaxValue;
                        this.Markers.Add(marker);
                    });
                    
                }
            }
            if (this.Markers.Count > 0)
            {
                this.ZoomAndCenterMarkers(null);
            }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var map = sender as Map;

            if (e.Property == DrawingDataProperty)
                map.DrawingData = (MapDrawingData)e.NewValue;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMapProvider.WebProxy = System.Net.WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //gMapControl1.set .SetPositionByKeywords("Paris, France");
            this.ShowCenter = false;
            this.Bearing = 0;
            this.CanDragMap = true;
            this.DragButton = System.Windows.Input.MouseButton.Left;
            this.MaxZoom = 18;
            this.MinZoom = 2;
            this.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            this.ShowTileGridLines = false;
            this.MapProvider = GMapProviders.GoogleMap;
            this.Zoom = 2;
        }
    }
}
