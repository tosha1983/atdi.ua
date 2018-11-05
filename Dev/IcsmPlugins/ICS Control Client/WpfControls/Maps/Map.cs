﻿using System;
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
                // draw points
                var points = this._drawingData.Points;
                if (points != null && points.Length > 0)
                {
                    points.ToList().ForEach(point =>
                    {
                        var mapPoint = this.FromLatLngToLocal(new PointLatLng(point.Location.Lat, point.Location.Lon));

                        var marker = new GMapMarker(new PointLatLng(point.Location.Lat, point.Location.Lon));
                        marker.Shape = new Ellipse() { Stroke = point.Color, Fill = point.Fill, Opacity = point.Opacity, Width = point.Width, Height = point.Width };
                        marker.ZIndex = int.MaxValue;
                        this.Markers.Add(marker);
                    });
                }

                // draw routes
                var routes = this._drawingData.Routes;
                if (routes != null && routes.Length > 0)
                {
                    routes.ToList().ForEach(route =>
                    {
                        if (route.Points != null && route.Points.Length > 0)
                        {
                            IList<PointLatLng> routePoints = new List<PointLatLng>();
                            route.Points.ToList().ForEach(point =>
                            {
                                routePoints.Add(new PointLatLng(point.Lat, point.Lon));
                            });

                            GMapRoute mapRoute = new GMapRoute(routePoints);
                            mapRoute.RegenerateShape(this);
                            ((System.Windows.Shapes.Path)mapRoute.Shape).Stroke = new System.Windows.Media.SolidColorBrush(route.Color);
                            ((System.Windows.Shapes.Path)mapRoute.Shape).StrokeThickness = 1;

                            mapRoute.ZIndex = int.MaxValue;
                            this.Markers.Add(mapRoute);
                        }
                    });
                }

                // draw polygons
                var polygons = this._drawingData.Polygons;
                if (polygons != null && polygons.Length > 0)
                {
                    polygons.ToList().ForEach(polygon =>
                    {
                        if (polygon.Points != null && polygon.Points.Length > 0)
                        {
                            IList<PointLatLng> polygonPoints = new List<PointLatLng>();
                            polygon.Points.ToList().ForEach(point =>
                            {
                                polygonPoints.Add(new PointLatLng(point.Lat, point.Lon));
                            });

                            GMapPolygon mapPolygon = new GMapPolygon(polygonPoints);
                            mapPolygon.RegenerateShape(this);
                            ((System.Windows.Shapes.Path)mapPolygon.Shape).Stroke = new System.Windows.Media.SolidColorBrush(polygon.Color);
                            ((System.Windows.Shapes.Path)mapPolygon.Shape).StrokeThickness = 1;

                            mapPolygon.ZIndex = int.MaxValue;
                            this.Markers.Add(mapPolygon);
                        }
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
