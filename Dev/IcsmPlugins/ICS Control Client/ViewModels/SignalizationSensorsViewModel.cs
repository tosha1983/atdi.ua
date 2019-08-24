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
    public class CustomDataGridSignSensors : DataGrid
    {
        public CustomDataGridSignSensors()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridSignSensors), new PropertyMetadata(null));

        #endregion
    }
    public class SignalizationSensorsViewModel : WpfViewModelBase
    {
        private IList _currentShortSensor;
        private MP.MapDrawingData _currentMapData;
        private ShortSensorDataAdatper _shortSensors;
        public WpfCommand StartCommand { get; set; }

        public SignalizationSensorsViewModel()
        {
            this._shortSensors = new ShortSensorDataAdatper();
            this.StartCommand = new WpfCommand(this.OnStartCommand);
            this.ReloadShortSensors();
        }
        public ShortSensorDataAdatper ShortSensors => this._shortSensors;
        public IList CurrentShortSensors
        {
            get => this._currentShortSensor;
            set
            {
                this._currentShortSensor = value;
                RedrawMap();
            }
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        private void ReloadShortSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
            this._shortSensors.Source = sdrSensors;
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

            if (this._currentShortSensor != null)
            {
                foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                {
                    var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id);
                    if (svcSensor != null)
                    {
                        var modelSensor = Mappers.Map(svcSensor);
                        if (modelSensor.Locations != null && modelSensor.Locations.Length > 0)
                        {
                            var sensorPoints = modelSensor.Locations
                                .Where(l => ("A".Equals(l.Status, StringComparison.OrdinalIgnoreCase)
                                        || "Z".Equals(l.Status, StringComparison.OrdinalIgnoreCase))
                                        && l.Lon.HasValue
                                        && l.Lat.HasValue)
                                .Select(l => this.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
                                .ToArray();

                            points.AddRange(sensorPoints);
                        }
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        private void OnStartCommand(object parameter)
        {
            try
            {

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
