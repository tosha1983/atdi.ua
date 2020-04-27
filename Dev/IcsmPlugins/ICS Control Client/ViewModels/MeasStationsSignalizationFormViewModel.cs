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
        private EmittingViewModel _emitting;
        private int _startType;
        private SDR.Emitting[] _inputEmittings;

        private MP.MapDrawingData _currentMapData;
        private IList _currentStations;
        private MeasStationsSignalizationViewModel _currentStation;
        private MeasStationsSignalizationDataAdapter _stations;

        public MeasStationsSignalizationDataAdapter Stations => this._stations;

        public WpfCommand AssociatedCommand { get; set; }

        public MeasStationsSignalizationFormViewModel(MeasStationsSignalization[] stationData, SDR.MeasurementResults measResult, bool buttonAssociatedVisible, EmittingViewModel emitting, int startType, SDR.Emitting[] inputEmittings)
        {
            this._stationData = stationData;
            this._measResult = measResult;
            this._emitting = emitting;
            this._startType = startType;
            this._inputEmittings = inputEmittings;

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
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this._measResult != null && this._measResult.LocationSensorMeasurement != null && this._measResult.LocationSensorMeasurement.Count() > 0)
            {
                this._currentSensorLocation = this._measResult.LocationSensorMeasurement[this._measResult.LocationSensorMeasurement.Count() - 1];

                if (this._currentSensorLocation.Lon.HasValue && this._currentSensorLocation.Lat.HasValue)
                    points.Add(MapsDrawingHelper.MakeDrawingPointForSensor("A", this._currentSensorLocation.Lon.Value, this._currentSensorLocation.Lat.Value, this._measResult.SensorName));
            }

            if (this._stationData != null && this._stationData.Length > 0)
            {
                foreach (var station in this._stationData)
                {
                    if (station.Lat != IM.NullD && station.Lon != IM.NullD)
                        points.Add(MapsDrawingHelper.MakeDrawingPointForStation(station.Lon, station.Lat, station.StationName));
                }
            }

            var routes = new List<MP.MapDrawingDataRoute>();
            if (this._currentStations != null && this._currentStations.Count > 0)
            {
                foreach (MeasStationsSignalizationViewModel station in this._currentStations)
                {
                    if (station.Lat != IM.NullD && station.Lon != IM.NullD)
                    {
                        var routePoints = new List<Location>()
                        {
                            new Location(station.Lon, station.Lat),
                            new Location(this._currentSensorLocation.Lon.Value, this._currentSensorLocation.Lat.Value)
                        };
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
            if (this._emitting.Id.HasValue && this._currentStation != null)
            {
                SVC.SdrnsControllerWcfClient.AddAssociationStationByEmitting(new long[] { this._emitting.Id.Value }, this._currentStation.IcsmId, this._currentStation.IcsmTable);

                if (_startType == 0)
                {
                    foreach (var em in this._measResult.Emittings.Where(e => e.Id == this._emitting.Id.Value).ToArray())
                    {
                        em.AssociatedStationID = this._currentStation.IcsmId;
                        em.AssociatedStationTableName = this._currentStation.IcsmTable;
                    }
                }
                else
                {
                    foreach (var em in this._inputEmittings.Where(e => e.Id == this._emitting.Id.Value).ToArray())
                    {
                        em.AssociatedStationID = this._currentStation.IcsmId;
                        em.AssociatedStationTableName = this._currentStation.IcsmTable;
                    }
                }
                _form.Close();
            }
        }
    }
}
