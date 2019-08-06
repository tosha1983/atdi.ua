using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
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
using INP = System.Windows.Input;
using System.Windows.Controls;
using System.Collections;
using XICSM.ICSControlClient.Models;
using System.Timers;
using XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.ViewModels
{
    public class CustomDataGridMeasTasks : DataGrid
    {
        public CustomDataGridMeasTasks()
        {
            this.MouseDoubleClick += DoubleClick;
        }
        private void DoubleClick(object sender, INP.MouseButtonEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
            foreach (ShortMeasTaskViewModel item in this.SelectedItemsList)
            {
                if (item.TypeMeasurements == SDR.MeasurementType.Signaling)
                {
                    var dlgForm = new FM.MeasTaskSignalizationForm(item.Id);
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

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridMeasTasks), new PropertyMetadata(null));
    }
    public class ControlClientViewModel : WpfViewModelBase
    {
        public enum ModelType
        {
            None,
            Tasks,
            TaskStations,
            TaskResults,
            TaskResultStations,
            Sensors
        }

        #region Current Objects
        
        // Tasks
        private MeasTaskViewModel _currentMeasTask;
        private ShortMeasTaskViewModel _currentShortMeasTask;

        // Task -> TaskStation
        private MeasTaskDetailStationViewModel _currentMeasTaskStation;

        // Task -> Sensors
        private ShortSensorViewModel _currentShortSensor;

        // Task -> Results
        private MeasurementResultsViewModel _currentMeasurementResults;

        // Task -> Results - Stations
        private ResultsMeasurementsStationViewModel _currentResultsMeasurementsStation;
        private ResultsMeasurementsStationViewModel _currentResultsMeasurementsStationData;

        // Task -> Results -> chart
        private CS.ChartOption _currentChartOption;

        // Task - Map (Task, TaskStation, TaskResult, TaskResultStation, TaskSensor)
        private MP.MapDrawingData _currentMapData;

        // The current model
        private ModelType _currentModel;

        #endregion
        private ShortMeasTaskDataAdatper _shortMeasTasks;
        private MeasurementResultsDataAdatper _measResults;
        private ShortSensorDataAdatper _shortSensors;
        private MeasTaskDetailStationDataAdapter _measTaskDetailStations;
        private ResultsMeasurementsStationDataAdapter _resultsMeasurementsStations;
        private LevelMeasurementsCarDataAdapter _levelMeasurements;

        private Visibility _measTaskDetailVisibility = Visibility.Hidden;
        private Visibility _measResultsDetailVisibility = Visibility.Hidden;
        private Visibility _taskStationsVisibility = Visibility.Hidden;
        private Visibility _resultStationsVisibility = Visibility.Hidden;
        private Visibility _resFreq1Visibility = Visibility.Hidden;
        private Visibility _resFreq2Visibility = Visibility.Hidden;
        private Visibility _resIdStationVisibility = Visibility.Hidden;
        private Visibility _resSpecVisibility = Visibility.Hidden;
        private Visibility _resLevelMes1Visibility = Visibility.Hidden;

        private double? _LowFreq;
        private double? _UpFreq;

        private bool _statusBarIsIndeterminate = false;
        private string _statusBarTitle = "";
        private string _statusBarDecription = "";


        #region Commands
        public WpfCommand GetCSVCommand { get; set; }
        public WpfCommand SearchStationCommand { get; set; }
        public WpfCommand CreateMeasTaskCommand { get; set; }
        public WpfCommand DeleteMeasTaskCommand { get; set; }
        public WpfCommand RunMeasTaskCommand { get; set; }
        public WpfCommand StopMeasTaskCommand { get; set; }

        public WpfCommand RefreshShortTasksCommand { get; set; }

        public WpfCommand RefreshShortSensorsCommand { get; set; }

        public WpfCommand ShowHideMeasTaskDetailCommand { get; set; }

        public WpfCommand ShowHideMeasResultsDetailCommand { get; set; }
        public WpfCommand MeasResultCommand { get; set; }

        #endregion

        //private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender != null)
        //    {
        //        DataGridRow row = sender as DataGridRow;
        //        if (row != null)
        //        {
        //            var data = row.Item as ShortMeasTaskViewModel;
        //            System.Windows.MessageBox.Show(data.Id.ToString());
        //        }
        //    }
        //}
        private CS.ChartOption GetDefaultChartOption()
        {
            return new CS.ChartOption
            {
                ChartType = CS.ChartType.Line,
                XLabel = "Freq (Mhz)",
                XMin = 900,
                XMax = 960,
                XTick = 5,
                XInnerTickCount = 5,
                YLabel = "Level (dBm)",
                YMin = -120,
                YMax = -10,
                YTick = 10,
                YInnerTickCount = 5,
                Title = "Measurements"
            };
        }

        public ControlClientViewModel(DataStore dataStore)
        {
            this._dataStore = dataStore;
            this._currentModel = ModelType.None;

            this._currentChartOption = this.GetDefaultChartOption();

            this.GetCSVCommand = new WpfCommand(this.OnGetCSVCommand);
            this.SearchStationCommand = new WpfCommand(this.OnSearchStationCommand);
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);
            this.DeleteMeasTaskCommand = new WpfCommand(this.OnDeleteMeasTaskCommand);
            this.RunMeasTaskCommand = new WpfCommand(this.OnRunMeasTaskCommand);
            this.StopMeasTaskCommand = new WpfCommand(this.OnStopMeasTaskCommand);

            this.RefreshShortTasksCommand = new WpfCommand(this.OnRefreshShortTasksCommand);
            this.RefreshShortSensorsCommand = new WpfCommand(this.OnRefreshShortSensorsCommand);
            this.ShowHideMeasTaskDetailCommand = new WpfCommand(this.OnShowHideMeasTaskDetailCommand);
            this.ShowHideMeasResultsDetailCommand = new WpfCommand(this.OnShowHideMeasResultsDetailCommand);
            this.MeasResultCommand = new WpfCommand(this.OnMeasResultCommand);

            this._shortMeasTasks = new ShortMeasTaskDataAdatper();
            this._measResults = new MeasurementResultsDataAdatper();
            this._shortSensors = new ShortSensorDataAdatper();
            this._measTaskDetailStations = new MeasTaskDetailStationDataAdapter();
            this._resultsMeasurementsStations = new ResultsMeasurementsStationDataAdapter();
            this._levelMeasurements = new LevelMeasurementsCarDataAdapter();

            this._dataStore.OnBeginInvoke += _dataStore_OnBeginInvoke;
            this._dataStore.OnEndInvoke += _dataStore_OnEndInvoke;

            this.ReloadShortMeasTasks();
            this.ReloadShortSensors(this.CurrentMeasTask);
            
        }
        private Timer _timer = null;
        private WaitForm _waitForm = null;

        private void _dataStore_OnEndInvoke(string description)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (_timer != null)
                {
                    _timer.Enabled = false;
                    _timer = null;
                }

                this.StatusBarIsIndeterminate = false;
                this.StatusBarTitle = $"Loaded data of {description}";
                
                
                if (_waitForm != null)
                {
                    _waitForm.Close();
                    _waitForm.Dispose();
                    _waitForm = null;
                }
            }));
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _timer.Enabled = false;
                _waitForm = new FM.WaitForm();
                _waitForm.SetMessage("Please wait...");
                _waitForm.TopMost = true;
                _waitForm.ShowDialog();
                //_waitForm.FormBorderStyle = FRM.FormBorderStyle.FixedSingle;
                //_waitForm.Refresh();
            }));
        }

        private void _dataStore_OnBeginInvoke(string description)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.StatusBarIsIndeterminate = true;
                this.StatusBarTitle = $"Loading data of {description} ...";

                this._timer = new Timer(300);
                this._timer.Elapsed += _timer_Elapsed;
                this._timer.Enabled = true;
            }));
        }

        public ModelType CurrentModel
        {
            get => this._currentModel;
            set => this.Set(ref this._currentModel, value);
        }

        public CS.ChartOption CurrentChartOption
        {
            get => this._currentChartOption;
            set => this.Set(ref this._currentChartOption, value);
        }

        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }

        public MeasTaskViewModel CurrentMeasTask
        {
            get => this._currentMeasTask;
            set => this.Set(ref this._currentMeasTask, value, () => {  });
        }

        public ShortMeasTaskViewModel CurrentShortMeasTask
        {
            get => this._currentShortMeasTask;
            set => this.Set(ref this._currentShortMeasTask, value, () => { this.OnChangedCurrentShortMeasTask(value); });
        }

        private Task _userActionTask = null;
        private readonly DataStore _dataStore;

        private void OnChangedCurrentShortMeasTask(ShortMeasTaskViewModel shortMeasTask)
        {
            if (_userActionTask == null)
            {
                var task = new Task(() => this.OnChangedCurrentShortMeasTaskAction(shortMeasTask));
                _userActionTask = task;
                task.Start();
            }
            else
            {
                _userActionTask = _userActionTask.ContinueWith(t => this.OnChangedCurrentShortMeasTaskAction(shortMeasTask));
            }
        }

        private void OnChangedCurrentShortMeasTaskAction(ShortMeasTaskViewModel shortMeasTask)
        {
            if (shortMeasTask != null)
            {
                Debug.Print($"OnChangedCurrentShortMeasTaskAction: ID = '{shortMeasTask.Id}'");
            }
            else
            {
                Debug.Print($"OnChangedCurrentShortMeasTaskAction: ID = 'is null'");
            }

            ReloadShortMeasResults(shortMeasTask);
            var currentMeasTask = ReloadCurrentMeasTask(shortMeasTask);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ReloadMeasTaskDetail(currentMeasTask);
            }));

            ReloadShortSensors(currentMeasTask);

            RedrawMap();

            //ReloadMeasResaltDetail();
        }
        private MeasTaskViewModel ReloadCurrentMeasTask(ShortMeasTaskViewModel shortMeasTask)
        {
            long taskId = 0;
            if (shortMeasTask != null)
            {
                taskId = shortMeasTask.Id;
            }

            var currentMeasTask = _dataStore.GetMeasTaskHeaderById(taskId);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.CurrentMeasTask = currentMeasTask;
            }));

            return currentMeasTask;
        }

        public ShortSensorViewModel CurrentShortSensor
        {
            get => this._currentShortSensor;
            set => this.Set(ref this._currentShortSensor, value, RedrawMap);
        }

        public MeasTaskDetailStationViewModel CurrentMeasTaskStation
        {
            get => this._currentMeasTaskStation;
            set => this.Set(ref this._currentMeasTaskStation, value, RedrawMap);
        }

        public MeasurementResultsViewModel CurrentMeasurementResults
        {
            get => this._currentMeasurementResults;
            set => this.Set(ref this._currentMeasurementResults, value,() => { OnChangedCurrentMeasurementResults(value); });
        }

        private void OnChangedCurrentMeasurementResults(MeasurementResultsViewModel measurementResults)
        {
            if (_userActionTask == null)
            {
                var task = new Task(() => this.OnChangedCurrentMeasurementResultsAction(measurementResults));
                _userActionTask = task;
                task.Start();
            }
            else
            {
                _userActionTask = _userActionTask.ContinueWith(t => this.OnChangedCurrentMeasurementResultsAction(measurementResults));
            }
        }

        private void OnChangedCurrentMeasurementResultsAction(MeasurementResultsViewModel measurementResults)
        {
            if (measurementResults != null)
            {
                Debug.Print($"OnChangedCurrentMeasurementResultsAction: ID = '{measurementResults.MeasSdrResultsId}'");
            }
            else
            {
                Debug.Print($"OnChangedCurrentMeasurementResultsAction: ID = 'is null'");
            }

            this.ReloadMeasResaltDetail(measurementResults);
            this.UpdateCurrentChartOption(measurementResults);
            this.RedrawMap();
        }

        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStation
        {
            get => this._currentResultsMeasurementsStation;
            set => this.Set(ref this._currentResultsMeasurementsStation, value, () => { ReloadMeasResultStationDetail(); ReloadLevelMeasurements(); UpdateCurrentChartOption(this.CurrentMeasurementResults); RedrawMap(); });
        }
        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStationData
        {
            get => this._currentResultsMeasurementsStationData;
            set => this.Set(ref this._currentResultsMeasurementsStationData, value);
        }
        #region Sources (Adapters)

        public LevelMeasurementsCarDataAdapter LevelMeasurements => this._levelMeasurements;

        public ShortMeasTaskDataAdatper ShortMeasTasks => this._shortMeasTasks;

        public MeasurementResultsDataAdatper MeasResults => this._measResults;

        public ShortSensorDataAdatper ShortSensors => this._shortSensors;

        public MeasTaskDetailStationDataAdapter MeasTaskDetailStations => this._measTaskDetailStations;

        public ResultsMeasurementsStationDataAdapter ResultsMeasurementsStations => this._resultsMeasurementsStations;

        #endregion

        public Visibility MeasTaskDetailVisibility
        {
            get => this._measTaskDetailVisibility;
            set => this.Set(ref this._measTaskDetailVisibility, value, () => { ReloadMeasTaskDetail(this.CurrentMeasTask); });
        }
        public Visibility MeasResultsDetailVisibility
        {
            get => this._measResultsDetailVisibility;
            set => this.Set(ref this._measResultsDetailVisibility, value, OnChangedMeasResultsDetailVisibility);
        }

        private void OnChangedMeasResultsDetailVisibility()
        {
            if (_userActionTask == null)
            {
                var task = new Task(() => this.OnChangedMeasResultsDetailVisibilityAction());
                _userActionTask = task;
                task.Start();
            }
            else
            {
                _userActionTask = _userActionTask.ContinueWith(t => this.OnChangedMeasResultsDetailVisibilityAction());
            }
        }
        private void OnChangedMeasResultsDetailVisibilityAction()
        {
            ReloadMeasResaltDetail(this.CurrentMeasurementResults);
            UpdateCurrentChartOption(this.CurrentMeasurementResults);
        }

        public Visibility TaskStationsVisibility
        {
            get => this._taskStationsVisibility;
            set => this.Set(ref this._taskStationsVisibility, value);
        }
        public Visibility ResultStationsVisibility
        {
            get => this._resultStationsVisibility;
            set => this.Set(ref this._resultStationsVisibility, value);
        }
        public Visibility ResFreq1Visibility
        {
            get => this._resFreq1Visibility;
            set => this.Set(ref this._resFreq1Visibility, value);
        }
        public Visibility ResFreq2Visibility
        {
            get => this._resFreq2Visibility;
            set => this.Set(ref this._resFreq2Visibility, value);
        }
        public Visibility ResIdStationVisibility
        {
            get => this._resIdStationVisibility;
            set => this.Set(ref this._resIdStationVisibility, value);
        }
        public Visibility ResSpecVisibility
        {
            get => this._resSpecVisibility;
            set => this.Set(ref this._resSpecVisibility, value);
        }
        public Visibility ResLevelMes1Visibility
        {
            get => this._resLevelMes1Visibility;
            set => this.Set(ref this._resLevelMes1Visibility, value);
        }

        public double? LowFreq
        {
            get => this._LowFreq;
            set => this.Set(ref this._LowFreq, value);
        }
        public double? UpFreq
        {
            get => this._UpFreq;
            set => this.Set(ref this._UpFreq, value);
        }

        private void ReloadShortMeasTasks()
        {
            var sdrTasks = SVC.SdrnsControllerWcfClient.GetShortMeasTasks();
            this._shortMeasTasks.Source = sdrTasks.OrderByDescending(c => c.Id.Value).ToArray();
        }

        private void ReloadShortMeasResults(ShortMeasTaskViewModel shortMeasTask)
        {
            long taskId = 0;
            if (shortMeasTask != null)
            {
                taskId = shortMeasTask.Id;
            }

            var sdrMeasResults = _dataStore.GetMeasResultsHeaderByTaskId(taskId); // SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(taskId);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this._measResults.Source = sdrMeasResults.OrderByDescending(c => c.Id).ToArray();
            }));
        }

        private void ReloadMeasTaskDetail(MeasTaskViewModel measTask)
        {
            //int taskId = 0;
            //if (this._currentShortMeasTask != null)
            //{
            //    taskId = this._currentShortMeasTask.Id;
            //}
            //else
            //{
            //    return;
            //}
            //var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(taskId);
            //var taskViewModel = Mappers.Map(task);
            //this.CurrentMeasTask = taskViewModel;

            if (this.MeasTaskDetailVisibility == Visibility.Visible && this._measTaskDetailStations != null)
            {

                if (measTask != null)
                {
                    //this._measTaskDetailStations.Source = task.StationsForMeasurements;
                    this._measTaskDetailStations.Source = _dataStore.GetStationDataForMeasurementsByTaskId(measTask.Id); // SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(measTask.Id);
                }
                else
                {
                    this._measTaskDetailStations.Source = null;
                }
            }

            if (measTask == null)
            {
                this.TaskStationsVisibility = Visibility.Hidden;
                return;
            }

            if (measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations && this.MeasTaskDetailVisibility == Visibility.Visible)
            {
                this.TaskStationsVisibility = Visibility.Visible;
            }
            else
            {
                this.TaskStationsVisibility = Visibility.Hidden;
            }

            ReloadDetailVisible(measTask);
        }
        private void ReloadDetailVisible(MeasTaskViewModel measTask)
        {
            if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations && this.MeasResultsDetailVisibility == Visibility.Visible)
            {
                this.ResFreq1Visibility = Visibility.Collapsed;
                this.ResFreq2Visibility = Visibility.Visible;
                this.ResIdStationVisibility = Visibility.Visible;
                this.ResSpecVisibility = Visibility.Visible;
                this.ResLevelMes1Visibility = Visibility.Visible;
            }
            else if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.SpectrumOccupation && this.MeasResultsDetailVisibility == Visibility.Visible)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.Level && this.MeasResultsDetailVisibility == Visibility.Visible)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Visible;
                this.ResIdStationVisibility = Visibility.Visible;
                this.ResSpecVisibility = Visibility.Visible;
                this.ResLevelMes1Visibility = Visibility.Visible;
            }
        }
        private void ReloadMeasResaltDetail(MeasurementResultsViewModel measurementResults)
        {
            if (measurementResults != null)
            {
                var sdrMeasResults = _dataStore.GetResMeasStationHeaderByResId(measurementResults.MeasSdrResultsId); // SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(this._currentMeasurementResults.MeasSdrResultsId);
                
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this._resultsMeasurementsStations.Source = sdrMeasResults.OrderByDescending(c => c.Id).ToArray();
                }));

                if (this.MeasResultsDetailVisibility == Visibility.Visible)
                {
                    var sdrMeasResultsDetail = _dataStore.GetMeasurementResultByResId(measurementResults.MeasSdrResultsId); // SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(this._currentMeasurementResults.MeasSdrResultsId);
                    var vLowFreq = sdrMeasResultsDetail.FrequenciesMeasurements == null ? (double?)null : (sdrMeasResultsDetail.FrequenciesMeasurements.Length == 0 ? 0 : sdrMeasResultsDetail.FrequenciesMeasurements.Min(f => f.Freq));
                    var vUpFreq = sdrMeasResultsDetail.FrequenciesMeasurements == null ? (double?)null : (sdrMeasResultsDetail.FrequenciesMeasurements.Length == 0 ? 0 : sdrMeasResultsDetail.FrequenciesMeasurements.Max(f => f.Freq));
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        LowFreq = vLowFreq;
                        UpFreq = vUpFreq;
                    }));
                }
            }
            else
            {
                //this.CurrentMeasurementResults = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this._resultsMeasurementsStations.Source = null;
                    LowFreq = null;
                    UpFreq = null;
                }));
                
            }
            if (this.CurrentMeasTask == null)
                return;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (this.CurrentMeasTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations && this.MeasResultsDetailVisibility == Visibility.Visible)
                {
                    this.ResultStationsVisibility = Visibility.Visible;
                }
                else
                {
                    this.ResultStationsVisibility = Visibility.Hidden;
                }
                ReloadDetailVisible(this.CurrentMeasTask);
            }));
        }

        private void ReloadMeasResultStationDetail()
        {
            if (this._currentResultsMeasurementsStation != null)
            {
                var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationById(this._currentResultsMeasurementsStation.Id);
                this.CurrentResultsMeasurementsStationData = Mappers.Map(sdrMeasResults);
            }
        }

        private void ReloadShortSensors(MeasTaskViewModel measTask)
        {
            //var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();

            //var measTask = this.CurrentMeasTask;
            //if (measTask != null )
            //{
            //    if (measTask.Stations != null && measTask.Stations.Length > 0)
            //    {
            //        sdrSensors = sdrSensors
            //            .Where(sdrSensor => measTask.Stations
            //                    .FirstOrDefault(s => s.StationId.Value == sdrSensor.Id.Value) != null
            //                )
            //            .ToArray();
            //    }
            //    else
            //    {
            //        sdrSensors = new SDR.ShortSensor[] { };
            //    }
            //}
            
            var sensors = _dataStore.GetShortSensorsByTask(measTask);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this._shortSensors.Source = sensors.OrderByDescending(c => c.Id.Value).ToArray();
            }));
        }

        private void ReloadLevelMeasurements()
        {
            if (this.CurrentResultsMeasurementsStation == null)
            {
                this._levelMeasurements.Source = null;
                return;
            }
            this._levelMeasurements.Source = this._currentResultsMeasurementsStationData.LevelMeasurements;
        }
        private void OnCreateMeasTaskCommand(object parameter)
        {
            try
            {
                var measTaskForm = new FM.MeasTaskForm();
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
                this.ReloadShortMeasTasks();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnGetCSVCommand(object parameter)
        {
            try
            {
                string filename = "";
                if (this._currentMeasurementResults == null || this._currentResultsMeasurementsStationData == null)
                    return;

                int recCount = LevelMeasurements.Source.Count();
                if (recCount == 0)
                {
                    MessageBox.Show("No data for export.");
                    return;
                }

                long taskId = this._currentMeasurementResults.MeasTaskId;
                string stationId = this._currentResultsMeasurementsStationData.StationId;

                FRM.SaveFileDialog sfd = new FRM.SaveFileDialog() { Filter = "CSV (*.csv)|*.csv", FileName = "FS_Meas_Res_" + taskId.ToString() + "_" + stationId + ".csv" };
                if (sfd.ShowDialog() == FRM.DialogResult.OK)
                {
                    //MessageBox.Show("Data will be exported and you will be notified when it is ready.");
                    if (File.Exists(filename))
                    {
                        try
                        {
                            File.Delete(filename);
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }

                    string[] output = new string[recCount + 1];
                    output[0] += "Lon;Lat;Level";

                    for (int i = 0; i < recCount; i++)
                    {
                        var ms = LevelMeasurements.Source[i] as SDR.LevelMeasurementsCar;
                        double leveldBmkVm = 0;
                        if ((!ms.LeveldBmkVm.HasValue || ms.LeveldBmkVm == 0 || ms.LeveldBmkVm == -1 || ms.LeveldBmkVm <= -30 || ms.LeveldBmkVm >= 200))
                        {
                            double freq = 0;
                            if (this._currentResultsMeasurementsStationData.GeneralResultCentralFrequencyMeas.HasValue && this._currentResultsMeasurementsStationData.GeneralResultCentralFrequencyMeas > 0.01)
                            {
                                freq = this._currentResultsMeasurementsStationData.GeneralResultCentralFrequencyMeas.Value;
                            }
                            if (this._currentResultsMeasurementsStationData.GeneralResultCentralFrequency.HasValue && this._currentResultsMeasurementsStationData.GeneralResultCentralFrequency > 0.01)
                            {
                                freq = this._currentResultsMeasurementsStationData.GeneralResultCentralFrequency.Value;
                            }
                            if (freq > 0 && ms.LeveldBm.HasValue && ms.LeveldBm > -300 && ms.LeveldBm < -10)
                            {
                                leveldBmkVm = (float)(77.2 + 20 * Math.Log10(freq) + ms.LeveldBm);
                            }
                        }
                        else
                        {
                            if (ms.LeveldBmkVm.HasValue)
                                leveldBmkVm = ms.LeveldBmkVm.Value;
                        }
                        if (leveldBmkVm > 0)
                            output[i + 1] += ms.Lon.ToString() + ";" + ms.Lat.ToString() + ";" + leveldBmkVm.ToString() + ";";
                    }
                    System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                    MessageBox.Show("Your file was generated and its ready for use.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnSearchStationCommand(object parameter)
        {
            try
            {
                if (this._currentResultsMeasurementsStationData == null || string.IsNullOrEmpty(this._currentResultsMeasurementsStationData.MeasGlobalSID) || this._currentResultsMeasurementsStationData.MeasGlobalSID.Length < 5)
                    return;

                //string stationName = this._currentResultsMeasurementsStationData.MeasGlobalSID;
                string stationName = this._currentResultsMeasurementsStationData.StationId.PadLeft(4, '0');

                double? lonMin = null;
                double? lonMax = null;
                double? latMin = null;
                double? latMax = null;
                decimal? freq = null;

                //stationName = stationName.Substring(stationName.Length - 5, 4).TrimStart('0');

                if (LevelMeasurements.Source == null)
                {
                    MessageBox.Show("No Level Measurements data.");
                    return;
                }
                int recCount = LevelMeasurements.Source.Count();

                if (recCount == 0)
                {
                    MessageBox.Show("No Level Measurements data.");
                    return;
                }

                for (int i = 0; i < recCount; i++)
                {
                    var ms = LevelMeasurements.Source[i] as SDR.LevelMeasurementsCar;

                    if (ms.Lon.HasValue)
                    {
                        if (!lonMin.HasValue || lonMin > ms.Lon)
                            lonMin = ms.Lon;
                        if (!lonMax.HasValue || lonMax < ms.Lon)
                            lonMax = ms.Lon;
                    }

                    if (ms.Lat.HasValue)
                    {
                        if (!latMin.HasValue || latMin > ms.Lat)
                            latMin = ms.Lat;
                        if (!latMax.HasValue || latMax < ms.Lat)
                            latMax = ms.Lat;
                    }

                    if (!freq.HasValue)
                        freq = ms.CentralFrequency;
                }

                var source = new IMRecordset("MOB_STATION", IMRecordset.Mode.ReadOnly);
                source.Select("ID,NAME,Position.LONGITUDE,Position.LATITUDE");
                source.SetWhere("NAME", IMRecordset.Operation.Eq, stationName);
                if (lonMin.HasValue)
                    source.SetWhere("Position.LONGITUDE", IMRecordset.Operation.Gt, lonMin.Value - 0.7);
                if (lonMax.HasValue)
                    source.SetWhere("Position.LONGITUDE", IMRecordset.Operation.Lt, lonMax.Value + 0.7);
                if (latMin.HasValue)
                    source.SetWhere("Position.LATITUDE", IMRecordset.Operation.Gt, latMin.Value - 0.7);
                if (latMax.HasValue)
                    source.SetWhere("Position.LATITUDE", IMRecordset.Operation.Lt, latMax.Value + 0.7);

                Dictionary<int, int> stations = new Dictionary<int, int>();

                for (source.Open(); !source.IsEOF(); source.MoveNext())
                {
                    var rs = new IMRecordset("MOBSTA_FREQS", IMRecordset.Mode.ReadOnly);
                    rs.Select("ID,STA_ID,TX_FREQ");
                    rs.SetWhere("STA_ID", IMRecordset.Operation.Eq, source.GetI("ID"));

                    double? minFreq = null;
                    double? maxFreq = null;

                    for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                    {
                        var frq = rs.GetD("TX_FREQ");
                        if (frq != IM.NullD)
                        {
                            if (!minFreq.HasValue || minFreq > frq)
                                minFreq = frq;
                            if (!maxFreq.HasValue || maxFreq < frq)
                                maxFreq = frq;
                        }
                    }
                    if (rs.IsOpen())
                        rs.Close();
                    rs.Destroy();

                    if (minFreq.HasValue && maxFreq.HasValue && minFreq - 0.2 < (double)freq && (double)freq < maxFreq + 0.2)
                        stations.Add(source.GetI("ID"), source.GetI("ID"));

                    //MessageBox.Show(source.GetS("ID") + " - " + source.GetS("NAME") + "(" + source.GetD("Position.LONGITUDE").ToString() + ":" + source.GetD("Position.LATITUDE").ToString() + ")");
                }
                if (source.IsOpen())
                    source.Close();
                source.Destroy();

                if (stations.Count() > 0)
                {
                    var dlgForm = new FM.StationListForm() { stationIDs = string.Join(",", stations.Keys.ToArray()) };
                    dlgForm.ShowDialog();
                    dlgForm.Dispose();
                }
                else
                {
                    MessageBox.Show("Stations not found");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnMeasResultCommand(object parameter)
        {
            try
            {
                var measTaskForm = new FM.MeasResultForm();
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void OnDeleteMeasTaskCommand(object parameter)
        {
            if (this.CurrentShortMeasTask == null)
            {
                return;
            }
            var taskId = this.CurrentShortMeasTask.Id;
            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Delete the meas task with ID #{taskId}", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            SVC.SdrnsControllerWcfClient.DeleteMeasTaskById(taskId);
        }
        private void OnRunMeasTaskCommand(object parameter)
        {
            if (this.CurrentShortMeasTask == null)
            {
                return;
            }
            var taskId = this.CurrentShortMeasTask.Id;
            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Run the meas task with ID #{taskId}", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            SVC.SdrnsControllerWcfClient.RunMeasTask(taskId);
        }
        private void OnStopMeasTaskCommand(object parameter)
        {
            if (this.CurrentShortMeasTask == null)
            {
                return;
            }

            var taskId = this.CurrentShortMeasTask.Id;
            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Stop the meas task with ID #{taskId}", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            SVC.SdrnsControllerWcfClient.StopMeasTask(taskId);
        }

        private void OnRefreshShortTasksCommand(object parameter)
        {
            this._dataStore.Reset();
            this.ReloadShortMeasTasks();
        }

        private void OnRefreshShortSensorsCommand(object parameter)
        {
            this.ReloadShortSensors(this.CurrentMeasTask);
        }

        private void OnShowHideMeasTaskDetailCommand(object parameter)
        {
            switch (this._measTaskDetailVisibility)
            {
                case Visibility.Visible:
                    this.MeasTaskDetailVisibility = Visibility.Hidden;
                    break;
                case Visibility.Hidden:
                    this.MeasTaskDetailVisibility = Visibility.Visible;
                    break;
                case Visibility.Collapsed:
                    break;
                default:
                    break;
            }
        }

        private void OnShowHideMeasResultsDetailCommand(object parameter)
        {
            switch (this._measResultsDetailVisibility)
            {
                case Visibility.Visible:
                    this.MeasResultsDetailVisibility = Visibility.Hidden;
                    break;
                case Visibility.Hidden:
                    this.MeasResultsDetailVisibility = Visibility.Visible;
                    break;
                case Visibility.Collapsed:
                    break;
                default:
                    break;
            }
        }

        private void UpdateCurrentChartOption(MeasurementResultsViewModel measurementResults)
        {
            if (this.MeasResultsDetailVisibility != Visibility.Visible)
            {
                return;
            }

            CS.ChartOption chartOption;

            if (measurementResults == null)
            {
                chartOption = this.GetDefaultChartOption();
            }
            else
            {
                chartOption = this.GetChartOption(measurementResults); 
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.CurrentChartOption = chartOption;
            }));
        }

        private CS.ChartOption GetChartOption(MeasurementResultsViewModel result)
        {
            if (result.TypeMeasurements == SDR.MeasurementType.MonitoringStations)
            {
                return this.GetChartOptionByMonitoringStations(result);
            }
            else if (result.TypeMeasurements == SDR.MeasurementType.SpectrumOccupation)
            {
                return this.GetChartOptionBySpectrumOccupation(result);
            }
            else if (result.TypeMeasurements == SDR.MeasurementType.Level)
            {
                return this.GetChartOptionByLevel(result);
            }
            return this.GetDefaultChartOption();
            
        }

        private CS.ChartOption GetChartOptionByMonitoringStations(MeasurementResultsViewModel result)
        {
            var option = new CS.ChartOption
            {
                Title = "Monitoring Stations",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            //var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(this.CurrentResultsMeasurementsStation.Id);
            //this._currentMeasurementResults = Mappers.Map(sdrMeasResults);

            var measStation = this._currentResultsMeasurementsStationData;
            if (measStation == null)
            {
                return option;
            }

            //var spectrumLevels = this._currentMeasurementResults.ge; //measStation.GeneralResultLevelsSpecrum;
            var spectrumLevels = measStation.GeneralResultLevelsSpecrum;
            if (spectrumLevels == null || spectrumLevels.Length == 0)
            {
                return option;
            }

            var count = spectrumLevels.Length;
            var points = new Point[count];
            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);
            for (int i = 0; i < count; i++)
            {
                var valX = Convert.ToDouble(measStation.GeneralResultSpecrumStartFreq + i * measStation.GeneralResultSpecrumSteps / 1000);
                var valY = spectrumLevels[i];

                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    maxX = valX;
                    minX = valX;
                    maxY = valY;
                    minY = valY;
                }
                else
                {
                    if (maxX < valX)
                        maxX = valX;
                    if (minX > valX)
                        minX = valX;

                    if (maxY < valY)
                        maxY = valY;
                    if (minY > valY)
                        minY = valY;
                }
                points[i] = point;
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
        }

        private CS.ChartOption GetChartOptionBySpectrumOccupation(MeasurementResultsViewModel result)
        {
            var option = new CS.ChartOption
            {
                Title = "Spectrum Occupation",
                YLabel = "Occupation (%)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Columns,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = 0,
                YMax = 100,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            var sdrMeasResults = _dataStore.GetMeasurementResultByResId(result.MeasSdrResultsId); // SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(result.MeasSdrResultsId);

            var count = sdrMeasResults.FrequenciesMeasurements.Length;
            var points = new Point[count];
            var max = default(double);
            var min = default(double);
            for (int i = 0; i < count; i++)
            {
                var ms = sdrMeasResults.MeasurementsResults[i] as SDR.SpectrumOccupationMeasurementResult;
                var valX = sdrMeasResults.FrequenciesMeasurements[i].Freq;
                var valY = ms.Value ?? 0;
                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    max = valX;
                    min = valX;
                }
                else
                {
                    if (max < valX)
                        max = valX;
                    if (min > valX)
                        min = valX;
                }
                points[i] = point;
            }

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(min, max, 6);
            option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;
            return option;
        }

        private CS.ChartOption GetChartOptionByLevel(MeasurementResultsViewModel result)
        {
            var option = new CS.ChartOption
            {
                Title = "Level",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            var sdrMeasResults = _dataStore.GetMeasurementResultByResId(result.MeasSdrResultsId);  //SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(result.MeasSdrResultsId);

            var count = sdrMeasResults.FrequenciesMeasurements.Length;
            var points = new Point[count];
            
            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            for (int i = 0; i < count; i++)
            {
                var ms = sdrMeasResults.MeasurementsResults[i] as SDR.LevelMeasurementResult;
                var valX = sdrMeasResults.FrequenciesMeasurements[i].Freq;
                var valY = ms.Value ?? 0;
                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    maxX = valX;
                    minX = valX;
                    maxY = valY;
                    minY = valY;
                }
                else
                {
                    if (maxX < valX)
                        maxX = valX;
                    if (minX > valX)
                        minX = valX;

                    if (maxY < valY)
                        maxY = valY;
                    if (minY > valY)
                        minY = valY;
                }
                points[i] = point;
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            //var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            //option.XTick = preparedDataX.Step;
            //option.XMin = preparedDataX.MinValue;
            //option.XMax = preparedDataX.MaxValue;

            var preparedDataX = Environment.Utitlity.CalcLevelRange(minX - 5, maxX + 5);
            option.XTick = 50;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
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
            var routes = new List<MP.MapDrawingDataRoute>();
            var polygons = new List<MP.MapDrawingDataPolygon>();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this._currentMeasurementResults == null)
            {
                this.CurrentMapData = null;
                return;
            }

            var sdrRoutes = SVC.SdrnsControllerWcfClient.GetRoutes(this._currentMeasurementResults.MeasSdrResultsId);
            if (sdrRoutes != null && sdrRoutes.Length > 0)
            {
                var routePoints = new List<Location>();
                sdrRoutes.ToList().ForEach(sdrRoute =>
                {
                    if (sdrRoute.RoutePoints != null && sdrRoute.RoutePoints.Length > 0)
                    {
                        sdrRoute.RoutePoints.OrderBy(c => c.StartTime).ToList().ForEach(point =>
                        {
                            routePoints.Add(new Location(point.Lon, point.Lat));
                        });
                    }
                });
                routes.Add(new MP.MapDrawingDataRoute() { Points = routePoints.ToArray(), Color = System.Windows.Media.Colors.Black, Fill = System.Windows.Media.Colors.Black });
            }

            var sdrPolygonPoints = SVC.SdrnsControllerWcfClient.GetSensorPoligonPoint(this._currentMeasurementResults.MeasSdrResultsId);
            if (sdrPolygonPoints != null && sdrPolygonPoints.Length > 0)
            {
                var polygonPoints = new List<Location>();
                sdrPolygonPoints.ToList().ForEach(sdrPolygonPoint =>
                {
                    if (sdrPolygonPoint.Lon.HasValue && sdrPolygonPoint.Lat.HasValue)
                    {
                        polygonPoints.Add(new Location(sdrPolygonPoint.Lon.Value, sdrPolygonPoint.Lat.Value));
                    }
                });
                polygons.Add(new MP.MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
            }

            if (this.LevelMeasurements != null && this.LevelMeasurements.Source != null)
            {
                foreach (var levelMeasurement in LevelMeasurements.Source)
                {
                    if (levelMeasurement.Lon.HasValue && levelMeasurement.Lat.HasValue)
                    {
                        System.Windows.Media.Brush pointBrush = System.Windows.Media.Brushes.GreenYellow;
                        if (levelMeasurement.LeveldBmkVm.HasValue && levelMeasurement.LeveldBmkVm.Value != 0)
                        {
                            pointBrush = GetBrushColor(10, 80, levelMeasurement.LeveldBmkVm.Value);
                        }
                        else if (levelMeasurement.LeveldBm.HasValue && levelMeasurement.LeveldBm.Value != 0)
                        {
                            pointBrush = GetBrushColor(-100, -30, levelMeasurement.LeveldBm.Value);
                        }
                        points.Add(new MP.MapDrawingDataPoint() { Location = new Location(levelMeasurement.Lon.Value, levelMeasurement.Lat.Value), Opacity = 0.5, Height = 5, Width = 5, Fill = pointBrush, Color = pointBrush });
                    }
                }
            }

            if (this.CurrentShortSensor != null)
            {
                var svcSensor = _dataStore.GetSensorById(this.CurrentShortSensor.Id);// SVC.SdrnsControllerWcfClient.GetSensorById(this.CurrentShortSensor.Id);
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

            var currentMeasTaskResultStation = this.CurrentResultsMeasurementsStation;
            var currentMeasTaskResult = this.CurrentMeasurementResults;
            var currentMeasTaskStation = this.CurrentMeasTaskStation;
            var currentMeasTask = this.CurrentMeasTask;

            // To define station points
            
            if (currentMeasTaskResultStation != null)
            {
                if (currentMeasTask != null && currentMeasTaskResultStation.StationId!=null)
                {
                    //var measTaskStations = currentMeasTask.StationsForMeasurements;
                    var measTaskStations = _dataStore.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id); // SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
                    if (measTaskStations != null && measTaskStations.Length > 0)
                    {
                        var stationForShow = measTaskStations
                            .Where(measTaskStation =>
                                   measTaskStation.IdStation.ToString() == currentMeasTaskResultStation.StationId 
                                && measTaskStation.Site != null 
                                && measTaskStation.Site.Lon.HasValue
                                && measTaskStation.Site.Lat.HasValue)
                            .FirstOrDefault(); 

                        if (stationForShow != null)
                        {
                            points.Add(this.MakeDrawingPointForStation(stationForShow.Site.Lon.Value, stationForShow.Site.Lat.Value));
                        }
                    }
                }
            }
            else if(currentMeasTaskResult != null)
            {
                var measTaskResultStations = currentMeasTaskResult.ResultsMeasStation;
                if (measTaskResultStations != null && measTaskResultStations.Length > 0)
                {
                    if (currentMeasTask != null)
                    {
                        //var measTaskStations = currentMeasTask.StationsForMeasurements;
                        var measTaskStations = _dataStore.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id); //SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
                        if (measTaskStations != null && measTaskStations.Length > 0)
                        {
                            var stationsForShow = measTaskStations
                                .Where(measTaskStation => 
                                        measTaskResultStations.Where(s => s.Idstation == measTaskStation.IdStation.ToString()).FirstOrDefault() != null)
                                .ToArray();

                            if (stationsForShow.Length > 0)
                            {
                                var stationPoints = stationsForShow
                                    .Where(s => s.Site != null && s.Site.Lon.HasValue && s.Site.Lat.HasValue)
                                    .Select(s => this.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
                                    .ToArray();

                                if (stationPoints.Length > 0)
                                {
                                    points.AddRange(stationPoints);
                                }
                            }
                        }
                    }
                }
            }
            else if (currentMeasTaskStation != null)
            {
                if (currentMeasTaskStation.SiteLon.HasValue && currentMeasTaskStation.SiteLat.HasValue)
                {
                    points.Add(this.MakeDrawingPointForStation(currentMeasTaskStation.SiteLon.Value, currentMeasTaskStation.SiteLat.Value));
                }
            }
            else if (currentMeasTask != null )
            {
                //var taskStations = currentMeasTask.StationsForMeasurements;
                var taskStations = _dataStore.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);// SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
                if (taskStations != null && taskStations.Length > 0)
                {
                    var stationPoints = taskStations
                        .Where(s => s.Site != null && s.Site.Lon.HasValue && s.Site.Lat.HasValue)
                        .Select(s => this.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
                        .ToArray();

                    if (stationPoints.Length > 0)
                    {
                        points.AddRange(stationPoints);
                    }
                }
            }

            data.Routes = routes.ToArray();
            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.CurrentMapData = data;
            }));

        }
        string GetTimeVal(TimeSpan val)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", val.Hours, val.Minutes, val.Seconds, val.Milliseconds / 10);

        }
        private System.Windows.Media.Brush GetBrushColor(double minVal, double maxVal, double val)
        {
            byte id;

            if (val <= minVal)
                id = 0;
            else if (val >= maxVal)
                id = 255;
            else
            {
                double oneprcVal = (maxVal - minVal) / 100;
                double rezVal = (val - minVal) / oneprcVal * 2.55;
                byte.TryParse(Math.Round(rezVal, 0).ToString(), out id);
            }

            byte.TryParse(id.ToString(), out byte a);
            byte.TryParse((255 - id).ToString(), out byte b);

            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(a, a, b));
        }
        public SDR.MeasurementType TypeMeasurements
        {
            get
            {
                if(this.CurrentMeasTask != null)
                {
                    return this.CurrentMeasTask.MeasDtParamTypeMeasurements;
                }

                return SDR.MeasurementType.SpectrumOccupation;
            }
        }

        public string StatusBarTitle
        {
            get => this._statusBarTitle;
            set => this.Set(ref this._statusBarTitle, value);
        }

        public string StatusBarDescription
        {
            get => this._statusBarDecription;
            set => this.Set(ref this._statusBarDecription, value);
        }

        public bool StatusBarIsIndeterminate
        {
            get => this._statusBarIsIndeterminate;
            set => this.Set(ref this._statusBarIsIndeterminate, value);
        }
    }
}
