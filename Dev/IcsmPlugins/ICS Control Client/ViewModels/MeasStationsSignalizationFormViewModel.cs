using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using INP = System.Windows.Input;
using System.Collections;

namespace XICSM.ICSControlClient.ViewModels
{
    public class CustomDataGridStations : DataGrid
    {
        public CustomDataGridStations()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }
        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridStations), new PropertyMetadata(null));
    }

    public class MeasStationsSignalizationFormViewModel : WpfViewModelBase
    {
        public FM.MeasStationsSignalizationForm _form;
        private Visibility _buttonAssociatedVisibility;
        private SDR.LocationSensorMeasurement _currentSensorLocation;
        private SDR.MeasurementResults _measResult;
        private MeasStationsSignalization[] _stationData;
        private int? _emittingId;

        private MP.MapDrawingData _currentMapData;
        private IList _currentStations;
        private MeasStationsSignalizationViewModel _currentStation;
        private MeasStationsSignalizationDataAdapter _stations;

        public MeasStationsSignalizationDataAdapter Stations => this._stations;

        public WpfCommand AssociatedCommand { get; set; }

        public MeasStationsSignalizationFormViewModel(MeasStationsSignalization[] stationData, SDR.MeasurementResults measResult, bool buttonAssociatedVisible, int? emittingId)
        {
            this._stationData = stationData;
            this._measResult = measResult;
            this._emittingId = emittingId;
            if (buttonAssociatedVisible)
                this._buttonAssociatedVisibility = Visibility.Visible;
            else
                this._buttonAssociatedVisibility = Visibility.Hidden;
            this._stations = new MeasStationsSignalizationDataAdapter();
            this._currentSensorLocation = new SDR.LocationSensorMeasurement();
            this.AssociatedCommand = new WpfCommand(this.OnAssociatedCommand);
            this.ReloadData();
            this.RedrawMap();
        }
        public Visibility ButtonAssociatedVisibility
        {
            get => this._buttonAssociatedVisibility;
            set => this.Set(ref this._buttonAssociatedVisibility, value);
        }
        public IList CurrentStations
        {
            get => this._currentStations;
            set
            {
                this._currentStations = value;
                RedrawMap();
            }
        }
        public MeasStationsSignalizationViewModel CurrentStation
        {
            get => this._currentStation;
            set => this.Set(ref this._currentStation, value);
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        private void ReloadData()
        {
            this._stations.Source = this._stationData;
        }
        private MP.MapDrawingDataPoint MakeDrawingPointForStation(double lon, double lat, string name)
        {
            return new MP.MapDrawingDataPoint
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
        private MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat, string name)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Location = new Models.Location(lon, lat),
                Opacity = 0.85,
                Width = 10,
                Height = 10,
                Name = name
            };
        }
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this._measResult != null && this._measResult.LocationSensorMeasurement != null && this._measResult.LocationSensorMeasurement.Count() > 0)
            {
                this._currentSensorLocation = this._measResult.LocationSensorMeasurement[this._measResult.LocationSensorMeasurement.Count() - 1];

                if (this._currentSensorLocation.Lon.HasValue && this._currentSensorLocation.Lat.HasValue)
                    points.Add(this.MakeDrawingPointForSensor("A", this._currentSensorLocation.Lon.Value, this._currentSensorLocation.Lat.Value, this._measResult.SensorName));
            }

            if (this._stationData != null && this._stationData.Length > 0)
            {
                foreach (var station in this._stationData)
                {
                    if (station.Lat != IM.NullD && station.Lon != IM.NullD)
                        points.Add(this.MakeDrawingPointForStation(station.Lon, station.Lat, station.StationName));
                }
            }

            var routes = new List<MP.MapDrawingDataRoute>();
            if (this._currentStations != null && this._currentStations.Count > 0)
            {
                foreach (MeasStationsSignalizationViewModel station in this._currentStations)
                {
                    if (station.Lat != IM.NullD && station.Lon != IM.NullD)
                    {
                        var routePoints = new List<Location>();
                        routePoints.Add(new Location(station.Lon, station.Lat));
                        routePoints.Add(new Location(this._currentSensorLocation.Lon.Value, this._currentSensorLocation.Lat.Value));
                        routes.Add(new MP.MapDrawingDataRoute() { Points = routePoints.ToArray(), Color = System.Windows.Media.Colors.Green, Fill = System.Windows.Media.Colors.Green });
                    }
                }
            }
            data.Routes = routes.ToArray();
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        private void OnAssociatedCommand(object parameter)
        {
            if (this._emittingId.HasValue && this._currentStation != null)
            {
                SVC.SdrnsControllerWcfClient.AddAssociationStationByEmitting(new int[] { this._emittingId.Value }, this._currentStation.IcsmId, this._currentStation.IcsmTable);
                _form.Close();
            }
        }
    }
}
