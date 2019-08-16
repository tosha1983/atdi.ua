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
    public class CustomDataGridMeasResult : DataGrid
    {
        public CustomDataGridMeasResult()
        {
            this.MouseDoubleClick += DoubleClick;
        }
        private void DoubleClick(object sender, INP.MouseButtonEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
            foreach (MeasurementResultsViewModel item in this.SelectedItemsList)
            {
                if (item.TypeMeasurements == SDR.MeasurementType.Signaling)
                { 
                    var dlgForm = new FM.MeasResultSignalizationForm(item.MeasSdrResultsId);
                    dlgForm.ShowDialog();
                    dlgForm.Dispose();
                }
            }
        }
        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridMeasResult), new PropertyMetadata(null));
    }
    public class MeasTaskSignalizationViewModel : WpfViewModelBase
    {
        private long _taskId;

        #region Current Objects
        private MeasTaskViewModel _currentMeasTask;
        private MP.MapDrawingData _currentMapData;
        #endregion
        private ShortSensorDataAdatper _shortSensors;
        private MeasurementResultsDataAdatper _measResults;
        #region Sources (Adapters)
        public MeasurementResultsDataAdatper MeasResults => this._measResults;
        public ShortSensorDataAdatper ShortSensors => this._shortSensors;
        #endregion
        #region Commands
        public WpfCommand RunMeasTaskCommand { get; set; }
        public WpfCommand StopMeasTaskCommand { get; set; }
        public WpfCommand RefreshShortTasksCommand { get; set; }
        #endregion

        public MeasTaskSignalizationViewModel(long taskId)
        {
            this.RunMeasTaskCommand = new WpfCommand(this.OnRunMeasTaskCommand);
            this.StopMeasTaskCommand = new WpfCommand(this.OnStopMeasTaskCommand);
            this.RefreshShortTasksCommand = new WpfCommand(this.OnRefreshShortTasksCommand);

            this._taskId = taskId;
            this._measResults = new MeasurementResultsDataAdatper();
            this._shortSensors = new ShortSensorDataAdatper();
            this.ReloadMeasTasksAndResult();
            this.ReloadShorSensors();
        }
        public MeasTaskViewModel CurrentMeasTask
        {
            get => this._currentMeasTask;
            set => this.Set(ref this._currentMeasTask, value);
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        private void ReloadMeasTasksAndResult()
        {
            var sdrResult = SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(_taskId);
            this._measResults.Source = sdrResult.OrderByDescending(c => c.Id).ToArray();
            var sdrTasks = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(_taskId);
            var taskViewModel = Mappers.Map(sdrTasks);
            this._currentMeasTask = taskViewModel;
        }
        private void ReloadShorSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();

            var measTask = this.CurrentMeasTask;
            if (measTask != null)
            {
                if (measTask.Sensors != null && measTask.Sensors.Length > 0)
                {
                    sdrSensors = sdrSensors
                        .Where(sdrSensor => measTask.Sensors
                                .FirstOrDefault(s => s.SensorId.Value == sdrSensor.Id.Value) != null
                            )
                        .ToArray();
                }
                else
                {
                    sdrSensors = new SDR.ShortSensor[] { };
                }
            }
            this._shortSensors.Source = sdrSensors.OrderByDescending(c => c.Id.Value).ToArray();
            RedrawMap();
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

            if (this._shortSensors != null)
            {
                foreach (var shortSensor in this._shortSensors.Source)
                {
                    var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id.Value);
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
        private void OnRunMeasTaskCommand(object parameter)
        {
            if (this._currentMeasTask == null)
            {
                return;
            }
            var taskId = this._currentMeasTask.Id;
            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Run the meas task with ID #{taskId}", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            SVC.SdrnsControllerWcfClient.RunMeasTask(taskId);
        }
        private void OnStopMeasTaskCommand(object parameter)
        {
            if (this._currentMeasTask == null)
            {
                return;
            }

            var taskId = this._currentMeasTask.Id;
            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Stop the meas task with ID #{taskId}", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }
            SVC.SdrnsControllerWcfClient.StopMeasTask(taskId);
        }

        private void OnRefreshShortTasksCommand(object parameter)
        {
            this.ReloadMeasTasksAndResult();
        }
    }
}
