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
        private SDR.MeasurementResults _measResult;
        private MeasStationsSignalization[] _stationData;

        private MP.MapDrawingData _currentMapData;
        private IList _currentStations;
        private MeasStationsSignalizationDataAdapter _stations;

        public MeasStationsSignalizationDataAdapter Stations => this._stations;

        public MeasStationsSignalizationFormViewModel(MeasStationsSignalization[] stationData, SDR.MeasurementResults measResult)
        {
            this._stationData = stationData;
            this._measResult = measResult;
            this._stations = new MeasStationsSignalizationDataAdapter();
            this.ReloadData();
            this.RedrawMap();
        }
        public IList CurrentStations
        {
            get => this._currentStations;
            set => this.Set(ref this._currentStations, value, () => { RedrawMap(); });
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
        private MP.MapDrawingDataPoint MakeDrawingPointForStation(double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = System.Windows.Media.Brushes.Green,
                Fill = System.Windows.Media.Brushes.ForestGreen,
                Location = new Models.Location(lon, lat),
                Opacity = 0.85,
                Width = 10,
                Height = 10
            };
        }
        private MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Location = new Models.Location(lon, lat),
                Opacity = 0.85,
                Width = 10,
                Height = 10
            };
        }
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            var sensorLocation = new SDR.LocationSensorMeasurement();

            if (this._measResult != null && this._measResult.LocationSensorMeasurement != null && this._measResult.LocationSensorMeasurement.Count() > 0)
            {
                sensorLocation = this._measResult.LocationSensorMeasurement[this._measResult.LocationSensorMeasurement.Count() - 1];

                if (sensorLocation.Lon.HasValue && sensorLocation.Lat.HasValue)
                    points.Add(this.MakeDrawingPointForSensor("A", sensorLocation.Lon.Value, sensorLocation.Lat.Value));
            }

            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
    }
}
