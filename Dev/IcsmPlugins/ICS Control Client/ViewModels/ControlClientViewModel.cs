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
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Globalization;
using TR = System.Threading;
using Microsoft.VisualBasic;
using Atdi.Modules.Sdrn.Calculation;

namespace XICSM.ICSControlClient.ViewModels
{
    public class ControlClientViewModel : WpfViewModelBase, IDisposable
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
        private IList _currentShortMeasTasks;

        // Task -> TaskStation
        private MeasTaskDetailStationViewModel _currentMeasTaskStation;

        // Task -> Sensors
        private ShortSensorViewModel _currentShortSensor;

        // Task -> Results
        private MeasurementResultsViewModel _currentMeasurementResult;
        private IList _currentMeasurementResults;
        private GeneralResultViewModel _currentGeneralResult;

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
        private GeneralResultDataAdapter _generalResults;
        private ResultsMeasurementsStationFilters _currentStationsGolbalFilter;

        private Visibility _measResultsDetailVisibility = Visibility.Hidden;
        private Visibility _taskStationsVisibility = Visibility.Hidden;
        private Visibility _resultStationsVisibility = Visibility.Hidden;
        private Visibility _resFreq1Visibility = Visibility.Hidden;
        private Visibility _resFreq2Visibility = Visibility.Hidden;
        private Visibility _resIdStationVisibility = Visibility.Hidden;
        private Visibility _resSpecVisibility = Visibility.Hidden;
        private Visibility _resTimeMeasVisibility = Visibility.Hidden;
        private Visibility _resLevelMes1Visibility = Visibility.Hidden;
        private Visibility _resGetCSVVisibility = Visibility.Hidden;
        private Visibility _resGetGraphicVisibility = Visibility.Hidden;
        private Visibility _resExportRSVisibility = Visibility.Hidden;

        private double? _LowFreq;
        private double? _UpFreq;
        private string _specLabelText = "0 of 0";
        private int _selectedSpectrum = 0;

        private bool _statusBarIsIndeterminate = false;
        private string _statusBarTitle = "";
        private string _statusBarDecription = "";


        #region Commands
        public WpfCommand GetCSVCommand { get; set; }
        public WpfCommand GetSOCSVCommand { get; set; }
        public WpfCommand ExportRSCommand { get; set; }
        public WpfCommand GetGraphicCommand { get; set; }
        public WpfCommand SearchStationCommand { get; set; }
        public WpfCommand PrevSpecCommand { get; set; }
        public WpfCommand NextSpecCommand { get; set; }
        public WpfCommand CreateMeasTaskCommand { get; set; }
        public WpfCommand DeleteMeasTaskCommand { get; set; }
        public WpfCommand RunMeasTaskCommand { get; set; }
        public WpfCommand StopMeasTaskCommand { get; set; }
        public WpfCommand RefreshShortTasksCommand { get; set; }
        public WpfCommand RefreshShortSensorsCommand { get; set; }
        public WpfCommand ShowHideMeasResultsDetailCommand { get; set; }
        public WpfCommand MeasResultCommand { get; set; }
        public WpfCommand FilterApplyCommand { get; set; }
        public WpfCommand DoubleClickSensorCommand { get; set; }
        public WpfCommand EditSensorTitleCommand { get; set; }
        public WpfCommand ChangeLevelOfMinOccupationCommand { get; set; }
        public WpfCommand SaveTasksCommand { get; set; }
        public WpfCommand SelectSavedTasksCommand { get; set; }
        public WpfCommand DoubleClickResultCommand { get; set; }

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
            this._currentStationsGolbalFilter = new ResultsMeasurementsStationFilters();

            this.GetCSVCommand = new WpfCommand(this.OnGetCSVCommand);
            this.GetGraphicCommand = new WpfCommand(this.OnGetGraphicCommand);
            this.ExportRSCommand = new WpfCommand(this.OnExportRSCommand);
            this.SearchStationCommand = new WpfCommand(this.OnSearchStationCommand);
            this.PrevSpecCommand = new WpfCommand(this.OnPrevSpecCommand);
            this.NextSpecCommand = new WpfCommand(this.OnNextSpecCommand);
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);
            this.DeleteMeasTaskCommand = new WpfCommand(this.OnDeleteMeasTaskCommand);
            this.RunMeasTaskCommand = new WpfCommand(this.OnRunMeasTaskCommand);
            this.StopMeasTaskCommand = new WpfCommand(this.OnStopMeasTaskCommand);

            this.RefreshShortTasksCommand = new WpfCommand(this.OnRefreshShortTasksCommand);
            this.RefreshShortSensorsCommand = new WpfCommand(this.OnRefreshShortSensorsCommand);
            this.ShowHideMeasResultsDetailCommand = new WpfCommand(this.OnShowHideMeasResultsDetailCommand);
            this.DoubleClickSensorCommand = new WpfCommand(this.OnDoubleClickSensorCommand);
            this.EditSensorTitleCommand = new WpfCommand(this.OnEditSensorTitleCommand);
            this.MeasResultCommand = new WpfCommand(this.OnMeasResultCommand);
            this.ChangeLevelOfMinOccupationCommand = new WpfCommand(this.OnChangeLevelOfMinOccupationCommand);
            this.SaveTasksCommand = new WpfCommand(this.OnSaveTasksCommand);
            this.SelectSavedTasksCommand = new WpfCommand(this.OnSelectSavedTasksCommand);
            this.DoubleClickResultCommand = new WpfCommand(this.OnDoubleClickResultCommand);

            this.ShortMeasTasks = new ShortMeasTaskDataAdatper();
            this.MeasResults = new MeasurementResultsDataAdatper();
            this.ShortSensors = new ShortSensorDataAdatper();
            this.MeasTaskDetailStations = new MeasTaskDetailStationDataAdapter();
            this.ResultsMeasurementsStations = new ResultsMeasurementsStationDataAdapter();
            this.LevelMeasurements = new LevelMeasurementsCarDataAdapter();
            this._generalResults = new GeneralResultDataAdapter();

            this.FilterApplyCommand = new WpfCommand(this.OnFilterApplyCommand);
            this.LoadSettings();

            this._dataStore.OnBeginInvoke += _dataStore_OnBeginInvoke;
            this._dataStore.OnEndInvoke += _dataStore_OnEndInvoke;

            this.ReloadShortMeasTasks();
            this.ReloadShortSensors(this.CurrentMeasTask);
            this.RedrawMap();

            this.ResFreq1Visibility = Visibility.Collapsed;
            this.ResFreq2Visibility = Visibility.Collapsed;
            this.ResIdStationVisibility = Visibility.Collapsed;
            this.ResSpecVisibility = Visibility.Collapsed;
            this.ResTimeMeasVisibility = Visibility.Collapsed;
            this.ResLevelMes1Visibility = Visibility.Collapsed;
            this.ResGetCSVVisibility = Visibility.Collapsed;

        }
        private Timer _timer = null;
        private WaitForm _waitForm = null;

        private void _dataStore_OnEndInvoke(string description)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
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
                }
                catch (Exception mes)
                {
                    MessageBox.Show("_dataStore_OnEndInvoke:" + mes.Message);
                }

            }));
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (_timer != null)
                    {
                        _timer.Enabled = false;
                        if (_waitForm == null)
                        {
                            _waitForm = new FM.WaitForm();
                            _waitForm.SetMessage("Please wait...");
                            _waitForm.TopMost = true;
                            _waitForm.ShowDialog();
                            //_waitForm.FormBorderStyle = FRM.FormBorderStyle.FixedSingle;
                            //_waitForm.Refresh();
                        }
                    }
                }
                catch (Exception mes)
                {
                    MessageBox.Show("_timer_Elapsed: " + mes.Message);
                }
            }));
        }

        private void _dataStore_OnBeginInvoke(string description)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    this.StatusBarIsIndeterminate = true;
                    this.StatusBarTitle = $"Loading data of {description} ...";

                    this._timer = new Timer(300);
                    this._timer.Elapsed += _timer_Elapsed;
                    this._timer.Enabled = true;
                }
                catch (Exception mes)
                {
                    MessageBox.Show("_dataStore_OnBeginInvoke: " + mes.Message);
                }

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
        public ResultsMeasurementsStationFilters CurrentStationsGolbalFilter
        {
            get => this._currentStationsGolbalFilter;
            set => this.Set(ref this._currentStationsGolbalFilter, value);
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
            this.CurrentGeneralResult = null;
            this.CurrentResultsMeasurementsStationData = null;
            ResGetGraphicVisibility = Visibility.Hidden;
            ResGetCSVVisibility = Visibility.Hidden;
        }

        private void OnChangedCurrentShortMeasTaskAction(ShortMeasTaskViewModel shortMeasTask)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string currentUICulture = appSettings["UICulture"];
            if (!string.IsNullOrEmpty(currentUICulture))
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(currentUICulture);

            if (shortMeasTask != null)
            {
                Debug.Print($"OnChangedCurrentShortMeasTaskAction: ID = '{shortMeasTask.Id}'");
            }
            else
            {
                Debug.Print($"OnChangedCurrentShortMeasTaskAction: ID = 'is null'");
            }

            if (shortMeasTask.TypeMeasurements == SDR.MeasurementType.SpectrumOccupation || shortMeasTask.TypeMeasurements == SDR.MeasurementType.Signaling)
                MeasResultsDetailVisibility = Visibility.Hidden;
            else
                MeasResultsDetailVisibility = Visibility.Visible;

            ReloadShortMeasResults(shortMeasTask);
            var currentMeasTask = ReloadCurrentMeasTask(shortMeasTask);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ReloadMeasTaskDetail(currentMeasTask);
            }));

            if (currentMeasTask.SupportMultyLevel.HasValue && currentMeasTask.SupportMultyLevel.Value)
                ChangeLevelOfMinOccupationEnabled = true;
            else
                ChangeLevelOfMinOccupationEnabled = false;

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
        public IList CurrentMeasurementResults
        {
            get => this._currentMeasurementResults;
            set => this.Set(ref this._currentMeasurementResults, value);
        }
        public IList CurrentShortMeasTasks
        {
            get => this._currentShortMeasTasks;
            set => this.Set(ref this._currentShortMeasTasks, value);
        }
        public MeasurementResultsViewModel CurrentMeasurementResult
        {
            get => this._currentMeasurementResult;
            set => this.Set(ref this._currentMeasurementResult, value,() => { OnChangedCurrentMeasurementResult(value); });
        }
        public GeneralResultViewModel CurrentGeneralResult
        {
            get => this._currentGeneralResult;
            set => this.Set(ref this._currentGeneralResult, value, () => { UpdateCurrentChartOption(this._currentMeasurementResult); RedrawMap(); });
        }
        private void OnChangedCurrentMeasurementResult(MeasurementResultsViewModel measurementResults)
        {
            if (_userActionTask == null)
            {
                var task = new Task(() => this.OnChangedCurrentMeasurementResultAction(measurementResults));
                _userActionTask = task;
                task.Start();
            }
            else
            {
                _userActionTask = _userActionTask.ContinueWith(t => this.OnChangedCurrentMeasurementResultAction(measurementResults));
            }
            this.CurrentGeneralResult = null;
            this.CurrentResultsMeasurementsStationData = null;
            this.ShortSensors.ClearFilter();
            this.ShortSensors.ApplyFilter(c => c.Name == measurementResults.SensorName);

            if (this._currentMeasurementResult.TypeMeasurements == SDR.MeasurementType.SpectrumOccupation && this.CurrentMeasurementResult != null)
            {
                ResExportRSVisibility = Visibility.Hidden;
                ResGetGraphicVisibility = Visibility.Visible;
                ResGetCSVVisibility = Visibility.Visible;
            }
            else if (this._currentMeasurementResult.TypeMeasurements == SDR.MeasurementType.MonitoringStations && this.CurrentMeasurementResult != null)
            {
                ResExportRSVisibility = Visibility.Hidden;
                ResGetGraphicVisibility = Visibility.Hidden;
                ResGetCSVVisibility = Visibility.Visible;
            }
            else if (this._currentMeasurementResult.TypeMeasurements == SDR.MeasurementType.Signaling && this.CurrentMeasurementResult != null)
            {
                ResExportRSVisibility = Visibility.Visible;
                ResGetGraphicVisibility = Visibility.Hidden;
                ResGetCSVVisibility = Visibility.Hidden;
            }
            else
            {
                ResExportRSVisibility = Visibility.Hidden;
                ResGetGraphicVisibility = Visibility.Hidden;
                ResGetCSVVisibility = Visibility.Hidden;
            }
        }

        private void OnChangedCurrentMeasurementResultAction(MeasurementResultsViewModel measurementResults)
        {
            if (measurementResults != null)
            {
                Debug.Print($"OnChangedCurrentMeasurementResultAction: ID = '{measurementResults.MeasSdrResultsId}'");
            }
            else
            {
                Debug.Print($"OnChangedCurrentMeasurementResultAction: ID = 'is null'");
            }

            this.ReloadMeasResaltDetail(measurementResults);
            this.UpdateCurrentChartOption(measurementResults);
            this.RedrawMap();
        }

        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStation
        {
            get => this._currentResultsMeasurementsStation;
            set => this.Set(ref this._currentResultsMeasurementsStation, value, () => { ReloadMeasResultStationDetail(); ReloadLevelMeasurements(); UpdateCurrentChartOption(this.CurrentMeasurementResult); RedrawMap(); });
        }
        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStationData
        {
            get => this._currentResultsMeasurementsStationData;
            set => this.Set(ref this._currentResultsMeasurementsStationData, value);
        }
        #region Sources (Adapters)

        public LevelMeasurementsCarDataAdapter LevelMeasurements { get; }

        public ShortMeasTaskDataAdatper ShortMeasTasks { get; }

        public MeasurementResultsDataAdatper MeasResults { get; }

        public ShortSensorDataAdatper ShortSensors { get; }

        public MeasTaskDetailStationDataAdapter MeasTaskDetailStations { get; }

        public ResultsMeasurementsStationDataAdapter ResultsMeasurementsStations { get; }

        #endregion

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
            ReloadMeasResaltDetail(this.CurrentMeasurementResult);
            UpdateCurrentChartOption(this.CurrentMeasurementResult);
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
        public Visibility ResTimeMeasVisibility
        {
            get => this._resTimeMeasVisibility;
            set => this.Set(ref this._resTimeMeasVisibility, value);
        }

        public Visibility ResLevelMes1Visibility
        {
            get => this._resLevelMes1Visibility;
            set => this.Set(ref this._resLevelMes1Visibility, value);
        }
        public Visibility ResGetCSVVisibility
        {
            get => this._resGetCSVVisibility;
            set => this.Set(ref this._resGetCSVVisibility, value);
        }
        public Visibility ResGetGraphicVisibility
        {
            get => this._resGetGraphicVisibility;
            set => this.Set(ref this._resGetGraphicVisibility, value);
        }
        public Visibility ResExportRSVisibility
        {
            get => this._resExportRSVisibility;
            set => this.Set(ref this._resExportRSVisibility, value);
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
        public string SpecLabelText
        {
            get => this._specLabelText;
            set => this.Set(ref this._specLabelText, value);
        }

        bool _showMeasTaskDetailEnabled = true;
        public bool ShowMeasTaskDetailEnabled
        {
            get => this._showMeasTaskDetailEnabled;
            set => this.Set(ref this._showMeasTaskDetailEnabled, value);
        }
        bool _hideMeasTaskDetailEnabled = false;
        public bool HideMeasTaskDetailEnabled
        {
            get => this._hideMeasTaskDetailEnabled;
            set => this.Set(ref this._hideMeasTaskDetailEnabled, value);

        }
        bool _showMeasResultsDetailEnabled = true;
        public bool ShowMeasResultsDetailEnabled
        {
            get => this._showMeasResultsDetailEnabled;
            set => this.Set(ref this._showMeasResultsDetailEnabled, value);

        }
        bool _hideMeasResultsDetailEnabled = false;
        public bool HideMeasResultsDetailEnabled
        {
            get => this._hideMeasResultsDetailEnabled;
            set => this.Set(ref this._hideMeasResultsDetailEnabled, value);

        }
        bool _changeLevelOfMinOccupationEnabled = false;
        public bool ChangeLevelOfMinOccupationEnabled
        {
            get => this._changeLevelOfMinOccupationEnabled;
            set => this.Set(ref this._changeLevelOfMinOccupationEnabled, value);

        }
        private void ReloadShortMeasTasks()
        {
            var sdrTasks = SVC.SdrnsControllerWcfClient.GetShortMeasTasks();
            this.ShortMeasTasks.Source = sdrTasks.OrderByDescending(c => c.Id.Value).ToArray();
        }

        private void ReloadShortMeasResults(ShortMeasTaskViewModel shortMeasTask)
        {
            long taskId = 0;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (shortMeasTask != null)
                    {
                        taskId = shortMeasTask.Id;
                    }

                    var sdrMeasResults = _dataStore.GetMeasResultsHeaderByTaskId(taskId); // SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(taskId);
                    this.MeasResults.Source = sdrMeasResults.OrderByDescending(c => c.Id.MeasSdrResultsId).ToArray();
                }
                catch (Exception mes)
                {
                    MessageBox.Show("ReloadShortMeasResults: " + mes.Message);
                    this._dataStore_OnEndInvoke("");
                }
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

            if (this.MeasTaskDetailStations != null)
            {
                if (measTask != null)
                {
                    this.MeasTaskDetailStations.Source = _dataStore.GetStationDataForMeasurementsByTaskId(measTask.Id);
                }
                else
                {
                    this.MeasTaskDetailStations.Source = null;
                }
            }
            if (measTask == null)
            {
                this.TaskStationsVisibility = Visibility.Hidden;
                return;
            }
            if (measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations)
            {
                if (this.MeasTaskDetailStations.Source != null && this.MeasTaskDetailStations.Source.Length > 0)
                    this.TaskStationsVisibility = Visibility.Visible;
                else
                    this.TaskStationsVisibility = Visibility.Hidden;
            }
            else
            {
                this.TaskStationsVisibility = Visibility.Hidden;
            }
            //ReloadDetailVisible(measTask);
        }
        private void ReloadDetailVisible(MeasTaskViewModel measTask)
        {
            if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations)
            {
                this.ResFreq1Visibility = Visibility.Collapsed;
                this.ResFreq2Visibility = Visibility.Visible;
                this.ResIdStationVisibility = Visibility.Visible;
                this.ResSpecVisibility = Visibility.Visible;
                this.ResLevelMes1Visibility = Visibility.Visible;
                this.ResGetCSVVisibility = Visibility.Visible;
            }
            else if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.SpectrumOccupation)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResTimeMeasVisibility = Visibility.Collapsed;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.Level)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else if (measTask != null && measTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.Signaling)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResTimeMeasVisibility = Visibility.Collapsed;
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
            var appSettings = ConfigurationManager.AppSettings;
            string currentUICulture = appSettings["UICulture"];
            if (!string.IsNullOrEmpty(currentUICulture))
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(currentUICulture);

            if (measurementResults != null)
            {
                if (measurementResults.TypeMeasurements == SDR.MeasurementType.MonitoringStations)
                {
                    //var sdrMeasResults = _dataStore.GetResMeasStationHeaderByResId(measurementResults.MeasSdrResultsId);
                    var filter = new SDR.ResultsMeasurementsStationFilters()
                    {
                        FreqBg = PluginHelper.ConvertStringToDouble(this._currentStationsGolbalFilter.FreqBg),
                        FreqEd = PluginHelper.ConvertStringToDouble(this._currentStationsGolbalFilter.FreqEd),
                        MeasGlobalSid = this._currentStationsGolbalFilter.MeasGlobalSid,
                        Standard = this._currentStationsGolbalFilter.Standard
                    };
                    var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(measurementResults.MeasSdrResultsId, filter);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //this._resultsMeasurementsStations.Source = sdrMeasResults.OrderByDescending(c => c.Id).ToArray();
                        this.ResultsMeasurementsStations.Source = sdrMeasResults;
                    }));
                }

                if (this.MeasResultsDetailVisibility == Visibility.Visible)
                {
                    double? vLowFreq = 0;
                    double? vUpFreq = 0;

                    var sdrMeasResultsDetail = _dataStore.GetMeasurementResultByResId(measurementResults.MeasSdrResultsId); // SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(this._currentMeasurementResult.MeasSdrResultsId);
                    if (this._currentMeasTask != null && this._currentMeasTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.Signaling)
                    {
                        vLowFreq = this._currentMeasTask.MeasFreqParamRgL;
                        vUpFreq = this._currentMeasTask.MeasFreqParamRgU;
                    }
                    else
                    {
                        vLowFreq = sdrMeasResultsDetail.FrequenciesMeasurements == null ? (double?)null : (sdrMeasResultsDetail.FrequenciesMeasurements.Length == 0 ? 0 : sdrMeasResultsDetail.FrequenciesMeasurements.Min(f => f.Freq));
                        vUpFreq = sdrMeasResultsDetail.FrequenciesMeasurements == null ? (double?)null : (sdrMeasResultsDetail.FrequenciesMeasurements.Length == 0 ? 0 : sdrMeasResultsDetail.FrequenciesMeasurements.Max(f => f.Freq));
                    }
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        LowFreq = vLowFreq;
                        UpFreq = vUpFreq;
                    }));
                }
            }
            else
            {
                //this.CurrentMeasurementResult = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.ResultsMeasurementsStations.Source = null;
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
                this._selectedSpectrum = 0;
                this.GenerateSpecLabelText();
                if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0)
                {
                    this.CurrentGeneralResult = Mappers.Map(this.CurrentResultsMeasurementsStationData.GeneralResults[0]);
                }
                else
                    this.CurrentGeneralResult = null;

                if (this._currentMeasurementResult.TypeMeasurements == SDR.MeasurementType.MonitoringStations && this.CurrentResultsMeasurementsStation != null && this.CurrentGeneralResult != null && this.CurrentGeneralResult.LevelsSpecrum != null)
                    ResGetGraphicVisibility = Visibility.Visible;
            }
        }

        private void ReloadShortSensors(MeasTaskViewModel measTask)
        {
            var sensors = SVC.SdrnsControllerWcfClient.GetShortSensors();

            if (measTask != null)
            {
                if (measTask.Sensors != null && measTask.Sensors.Length > 0)
                {
                    sensors = sensors.Where(sdrSensor => measTask.Sensors.FirstOrDefault(s => s.SensorId.Value == sdrSensor.Id.Value) != null).ToArray();
                }
                else
                {
                    sensors = new SDR.ShortSensor[] { };
                }
            }

            //var sensors = _dataStore.GetShortSensorsByTask(measTask);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.ShortSensors.Source = sensors.OrderByDescending(c => c.Id.Value).ToArray();
            }));
        }

        private void ReloadLevelMeasurements()
        {
            if (this.CurrentResultsMeasurementsStation == null)
            {
                this.LevelMeasurements.Source = null;
                return;
            }
            this.LevelMeasurements.Source = this._currentResultsMeasurementsStationData.LevelMeasurements;
        }
        private void OnCreateMeasTaskCommand(object parameter)
        {
            try
            {
                var measTaskForm = new FM.MeasTaskForm(0, SDR.MeasurementType.Signaling);
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
                this.ReloadShortMeasTasks();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnSaveTasksCommand(object parameter)
        {
            if(_currentShortMeasTask != null)
            {
                var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(_currentShortMeasTask.Id);
                task.Status = "S";
                task.DateCreated = DateTime.Now;
                task.MeasSubTasks = null;
                task.Id = null;
                var newTaskId = SVC.SdrnsControllerWcfClient.CreateMeasTask(task);
            }
            
        }
        private void OnSelectSavedTasksCommand(object parameter)
        {
            try
            {
                var form = new FM.WpfStandardForm("SavedTaskForm.xaml", "SavedTaskViewModel");
                form.ShowDialog();
                form.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnGetCSVCommand(object parameter)
        {
            if (this._currentMeasTask != null)
            {
                if (this._currentMeasTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.SpectrumOccupation)
                    OnGetSOCSVCommand(parameter);
                else if (this._currentMeasTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations)
                    OnGetMSCSVCommand(parameter);
            }
        }
        private void OnExportRSCommand(object parameter)
        {
            if (this._currentMeasurementResult != null && this._currentMeasurementResult != null)
            {
                var measResult = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(this._currentMeasurementResult.MeasSdrResultsId, null, null);

                FRM.SaveFileDialog sfd = new FRM.SaveFileDialog() { Filter = "CSV (*.csv)|*.csv", FileName = $"RS_Result_{this._currentMeasurementResult.MeasSdrResultsId}.csv" };
                if (sfd.ShowDialog() == FRM.DialogResult.OK)
                {
                    var output = new List<string>();
                    output.Add("MaskLevels_dB;MaskFrequencies_kHz");

                    foreach (var emitting in measResult.Emittings)
                    {
                        //var result = CreateMaskFromSpectrum.CreateMaskFromEmitting(emitting.Spectrum.Levels_dBm, emitting.Spectrum.SpectrumStartFreq_MHz, emitting.Spectrum.SpectrumSteps_kHz, 6, 6, 8);

                        //for (int i = 0; i < result.MaskLevels_dB.Length - 1; i++)
                        //{
                        //    output.Add($"{result.MaskLevels_dB[i]};{result.MaskFrequencies_kHz[i]}");
                        //}
                    }
                    System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                    MessageBox.Show("Your file was generated and its ready for use.");
                }
            }
            else
            {
                MessageBox.Show("No data for export.");
                return;
            }
        }
        private void OnDoubleClickResultCommand(object parameter)
        {
            if (this._currentMeasurementResult != null && this._currentMeasurementResult.TypeMeasurements == SDR.MeasurementType.Signaling)
            {
                var dlgForm = new FM.MeasResultSignalizationForm(this._currentMeasurementResult.MeasSdrResultsId, 0, null, null);
                dlgForm.ShowDialog();
                dlgForm.Dispose();
            }
        }
        private void OnGetSOCSVCommand(object parameter)
        {
            try
            {
                using (var wc = new HttpClient())
                {
                    {
                        string endpointUrls = PluginHelper.GetRestApiEndPoint();

                        if (string.IsNullOrEmpty(endpointUrls))
                            return;

                        if (this._currentMeasurementResults == null || this._currentMeasTask == null)
                            return;

                        var ids = new List<long>();
                        foreach (MeasurementResultsViewModel result in this._currentMeasurementResults)
                            ids.Add(result.MeasSdrResultsId);

                        var response = wc.GetAsync(endpointUrls + $"api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/ResLevels?select=RES_MEAS.Id,FreqMeas,OccupancySpect,VMinLvl,VMMaxLvl,RES_MEAS.StartTime,RES_MEAS.StopTime,RES_MEAS.ScansNumber&filter=(RES_MEAS.Id in ({string.Join(",", ids)}))&orderBy=RES_MEAS.Id").Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var so_type = this._currentMeasTask.MeasOtherTypeSpectrumOccupation == SDR.SpectrumOccupationType.FreqChannelOccupation ? "FCO" : "FBO";
                            FRM.SaveFileDialog sfd = new FRM.SaveFileDialog() { Filter = "CSV (*.csv)|*.csv", FileName = $"SO{so_type}_{this._currentMeasTask.MeasFreqParamRgL??0}-{this._currentMeasTask.MeasFreqParamRgU??0}.csv" };
                            if (sfd.ShowDialog() == FRM.DialogResult.OK)
                            {
                                //MessageBox.Show("Data will be exported and you will be notified when it is ready.");
                                if (File.Exists(sfd.FileName))
                                {
                                    try
                                    {
                                        File.Delete(sfd.FileName);
                                    }
                                    catch (IOException ex)
                                    {
                                        MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                                    }
                                }
                                var output = new List<string>();
                                output.Add("Id = " + this._currentMeasTask.Id);
                                output.Add("Status = " + this._currentMeasTask.Status);
                                output.Add("Name = " + this._currentMeasTask.Name);
                                output.Add("Date Start = " + this._currentMeasTask.MeasTimeParamListPerStart);
                                output.Add("Date Stop = " + this._currentMeasTask.MeasTimeParamListPerStop);
                                if (this._currentMeasTask.MeasTimeParamListTimeStart.HasValue)
                                    output.Add("Time Start = " + this._currentMeasTask.MeasTimeParamListTimeStart);
                                if (this._currentMeasTask.MeasTimeParamListTimeStop.HasValue)
                                    output.Add("Time Stop = " + this._currentMeasTask.MeasTimeParamListTimeStop);
                                if (this._currentMeasTask.MeasTimeParamListPerInterval.HasValue)
                                    output.Add("Duration = " + this._currentMeasTask.MeasTimeParamListPerInterval);
                                if (this._currentMeasTask.MeasFreqParamRgL.HasValue)
                                    output.Add("Start freq = " + this._currentMeasTask.MeasFreqParamRgL);
                                if (this._currentMeasTask.MeasFreqParamRgU.HasValue)
                                    output.Add("Stop freq = " + this._currentMeasTask.MeasFreqParamRgU);
                                if (this._currentMeasTask.MeasFreqParamStep.HasValue)
                                    output.Add("Step whith = " + this._currentMeasTask.MeasFreqParamStep);
                                output.Add("Mode = " + this._currentMeasTask.MeasFreqParamMode);

                                if(this._currentMeasTask.IsAutoMeasDtParamRBW || this._currentMeasTask.MeasDtParamRBW.HasValue)
                                    output.Add("RBW = " + (this._currentMeasTask.IsAutoMeasDtParamRBW ? "Auto" : this._currentMeasTask.MeasDtParamRBW.ToString()));
                                if (this._currentMeasTask.IsAutoMeasDtParamVBW || this._currentMeasTask.MeasDtParamVBW.HasValue)
                                    output.Add("VBW = " + (this._currentMeasTask.IsAutoMeasDtParamVBW ? "Auto" : this._currentMeasTask.MeasDtParamVBW.ToString()));
                                if (this._currentMeasTask.IsAutoMeasDtParamMeasTime || this._currentMeasTask.MeasDtParamMeasTime.HasValue)
                                    output.Add("Sweep time = " + (this._currentMeasTask.IsAutoMeasDtParamMeasTime ? "Auto" : this._currentMeasTask.MeasDtParamMeasTime.ToString()));
                                output.Add("Type of detecting = " + this._currentMeasTask.MeasDtParamDetectType);
                                if (this._currentMeasTask.IsAutoMeasDtParamRfAttenuation || this._currentMeasTask.MeasDtParamRfAttenuation.HasValue)
                                    output.Add("Attenuation = " + (this._currentMeasTask.IsAutoMeasDtParamRfAttenuation ? "Auto" : this._currentMeasTask.MeasDtParamRfAttenuation.ToString()));
                                if (this._currentMeasTask.IsAutoMeasDtParamPreamplification || this._currentMeasTask.MeasDtParamPreamplification.HasValue)
                                    output.Add("Gain of preamplifier = " + (this._currentMeasTask.IsAutoMeasDtParamPreamplification ? "Auto" : this._currentMeasTask.MeasDtParamPreamplification.ToString()));
                                if (this._currentMeasTask.IsAutoMeasDtParamReferenceLevel || this._currentMeasTask.MeasDtParamReferenceLevel.HasValue)
                                    output.Add("Reference Level = " + (this._currentMeasTask.IsAutoMeasDtParamReferenceLevel ? "Auto" : this._currentMeasTask.MeasDtParamReferenceLevel.ToString()));
                                if (this._currentMeasTask.MeasOtherSwNumber.HasValue)
                                    output.Add("Sweep number = " + this._currentMeasTask.MeasOtherSwNumber);

                                output.Add("TypeSpectrumOccupation = " + this._currentMeasTask.MeasOtherTypeSpectrumOccupation);
                                if (this._currentMeasTask.MeasOtherLevelMinOccup.HasValue)
                                    output.Add("Level of min. occupation = " + this._currentMeasTask.MeasOtherLevelMinOccup);
                                if (this._currentMeasTask.MeasOtherSwNumber.HasValue)
                                    output.Add("Number of scans at a time = " + this._currentMeasTask.MeasOtherSwNumber);
                                output.Add("Type of spectrums scan = " + this._currentMeasTask.MeasOtherTypeSpectrumScan);
                                output.Add("Type of Measurement = " + this._currentMeasTask.MeasDtParamTypeMeasurements);
                                output.Add("");
                                output.Add("RESULT_ID;FREQ_MEAS;OCCUPANCY_SPECT;VMIN_LVL;VMAX_LVL;START_TIME;STOP_TIME;SCANS_NUMBER");

                                var dicFields = new Dictionary<string, int>();
                                var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                                foreach (var field in data.Fields)
                                    dicFields[field.Path] = field.Index;

                                foreach (object[] record in data.Records)
                                {
                                    var RESULT_ID = Convert.ToInt64(record[dicFields["RES_MEAS.Id"]]);
                                    var FREQ_MEAS = (double)record[dicFields["FreqMeas"]];
                                    var OCCUPANCY_SPECT = (double)record[dicFields["OccupancySpect"]];
                                    var VMIN_LVL = (double)record[dicFields["VMinLvl"]];
                                    var VMAX_LVL = (double)record[dicFields["VMMaxLvl"]];
                                    var START_TIME = (DateTime)record[dicFields["RES_MEAS.StartTime"]];
                                    var STOP_TIME = (DateTime)record[dicFields["RES_MEAS.StopTime"]];
                                    var SCANS_NUMBER = Convert.ToInt32(record[dicFields["RES_MEAS.ScansNumber"]]);
                                    output.Add($"{RESULT_ID};{FREQ_MEAS};{OCCUPANCY_SPECT};{VMIN_LVL};{VMAX_LVL};{START_TIME};{STOP_TIME};{SCANS_NUMBER}");
                                }

                                System.IO.File.WriteAllLines(sfd.FileName, output.ToArray(), System.Text.Encoding.UTF8);
                                MessageBox.Show("Your file was generated and its ready for use.");
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnGetMSCSVCommand(object parameter)
        {
            try
            {
                string filename = "";
                if (this._currentMeasurementResult == null || this._currentResultsMeasurementsStationData == null)
                    return;

                int recCount = LevelMeasurements.Source.Count();
                if (recCount == 0)
                {
                    MessageBox.Show("No data for export.");
                    return;
                }

                long taskId = this._currentMeasurementResult.MeasTaskId;
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
                            if (this._currentGeneralResult.CentralFrequencyMeas.HasValue && this._currentGeneralResult.CentralFrequencyMeas > 0.01)
                            {
                                freq = this._currentGeneralResult.CentralFrequencyMeas.Value;
                            }
                            if (this._currentGeneralResult.CentralFrequency.HasValue && this._currentGeneralResult.CentralFrequency > 0.01)
                            {
                                freq = this._currentGeneralResult.CentralFrequency.Value;
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
        private void OnGetGraphicCommand(object parameter)
        {
            try
            {
                SDR.MeasurementResults _measResult = null;
                GeneralResultViewModel _generalResult = null;

                if (this._currentShortMeasTask.TypeMeasurements == SDR.MeasurementType.MonitoringStations)
                {
                    _generalResult = this._currentGeneralResult;
                }
                else if (this._currentShortMeasTask.TypeMeasurements == SDR.MeasurementType.SpectrumOccupation)
                {
                    _measResult = _dataStore.GetMeasurementResultByResId(this.CurrentMeasurementResult.MeasSdrResultsId);
                }
                else if (this._currentShortMeasTask.TypeMeasurements == SDR.MeasurementType.Level)
                {
                    _measResult = _dataStore.GetMeasurementResultByResId(this.CurrentMeasurementResult.MeasSdrResultsId);
                }

                var form = new FM.GraphicForm(this._currentShortMeasTask.TypeMeasurements, _measResult, _generalResult, 1);
                form.ShowDialog();
                form.Dispose();
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
            if (this.CurrentShortMeasTask == null && this.CurrentShortMeasTasks == null)
                return;

            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Delete the meas task(s)", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (result != System.Windows.Forms.DialogResult.Yes)
                return;

            if (this.CurrentShortMeasTasks != null)
            {
                foreach (ShortMeasTaskViewModel task in this.CurrentShortMeasTasks)
                    SVC.SdrnsControllerWcfClient.DeleteMeasTaskById(task.Id);
            }
            else
            {
                SVC.SdrnsControllerWcfClient.DeleteMeasTaskById(this.CurrentShortMeasTask.Id);
            }

            this.ReloadShortMeasTasks();
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
        private void OnChangeLevelOfMinOccupationCommand(object parameter)
        {
            var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(this.CurrentShortMeasTask.Id);

            var newLevel = Interaction.InputBox(Properties.Resources.LevelOfMinOccupationDBm, "ICS Control Client", task.MeasOther.LevelMinOccup.ToString());
            var level = PluginHelper.ConvertStringToDouble(newLevel, true);

            if (level.HasValue)
            {
                if (level.Value < -150 || level.Value > 120)
                {
                    PluginHelper.ShowMessageValueMustBeInTheRange(Properties.Resources.LevelOfMinOccupationDBm, "-150", "120");
                    return;
                }

                task.MeasOther.LevelMinOccup = level;
                SVC.SdrnsControllerWcfClient.UpdateMeasTaskParametersAndRecalcResults(task);

                var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(this.CurrentShortMeasTask.Id);
                this.MeasResults.Source = sdrMeasResults.OrderByDescending(c => c.Id.MeasSdrResultsId).ToArray();
            }
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
        private void OnShowHideMeasResultsDetailCommand(object parameter)
        {
            switch (this._measResultsDetailVisibility)
            {
                case Visibility.Visible:
                    this.MeasResultsDetailVisibility = Visibility.Hidden;
                    this.ShowMeasResultsDetailEnabled = true;
                    this.HideMeasResultsDetailEnabled = false;
                    break;
                case Visibility.Hidden:
                    this.MeasResultsDetailVisibility = Visibility.Visible;
                    this.ShowMeasResultsDetailEnabled = false;
                    this.HideMeasResultsDetailEnabled = true;
                    break;
                case Visibility.Collapsed:
                    break;
                default:
                    break;
            }
        }
        private void OnDoubleClickSensorCommand(object parameter)
        {
            if (this._currentShortSensor == null)
            {
                MessageBox.Show(Properties.Resources.Message_SelectASensorToSwitchToOnLineMeasurementMode);
                return;
            }
            var dlgForm = new FM.OnlineMeasurementForm(this._currentShortSensor, null);
            dlgForm.ShowDialog();
            dlgForm.Dispose();
        }
        private void OnEditSensorTitleCommand(object parameter)
        {
            if (this._currentShortSensor != null)
            {
                var newTitle = Interaction.InputBox("Enter title", "ICS Control Client");
                if (!string.IsNullOrEmpty(newTitle))
                {
                    SVC.SdrnsControllerWcfClient.UpdateSensorTitle(this._currentShortSensor.Id, newTitle);
                    this.ReloadShortSensors(this.CurrentMeasTask);
                }
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
                return this.GetChartOptionByMonitoringStations();
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

        private CS.ChartOption GetChartOptionByMonitoringStations()
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

            
            if (this._currentGeneralResult == null || this._currentGeneralResult.LevelsSpecrum == null || this._currentGeneralResult.LevelsSpecrum.Length == 0)
            {
                return option;
            }
            var generalResult = this._currentGeneralResult;
            var spectrumLevels = generalResult.LevelsSpecrum;
            var count = spectrumLevels.Length;
            var points = new Point[count];
            var linesList = new List<CS.ChartLine>();
            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);
            for (int i = 0; i < count; i++)
            {
                var valX = Convert.ToDouble(generalResult.SpecrumStartFreq + i * generalResult.SpecrumSteps / 1000);
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

            if (generalResult.T1.GetValueOrDefault(0) != 0)
            {
                linesList.Add(new CS.ChartLine() { Point = new Point { X = generalResult.T1.Value, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = "T1", LabelLeft = 5, LabelTop = -25 });
            }
            if (generalResult.MarkerIndex.GetValueOrDefault(0) != 0)
            {
                linesList.Add(new CS.ChartLine() { Point = new Point { X = generalResult.MarkerIndex.Value, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = "M", LabelLeft = 5, LabelTop = -35 });
            }
            if (generalResult.T2.GetValueOrDefault(0) != 0)
            {
                linesList.Add(new CS.ChartLine() { Point = new Point { X = generalResult.T2.Value, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = "T2", LabelLeft = 5, LabelTop = -45 });
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
            option.LinesArray = linesList.ToArray();

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

            if (sdrMeasResults.FrequenciesMeasurements == null)
                return option;

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
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;
            if (min != max)
                option.XTick = preparedDataX.Step;

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
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var routes = new List<MP.MapDrawingDataRoute>();
            var polygons = new List<MP.MapDrawingDataPolygon>();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this.CurrentShortSensor != null)
                DrawSensor(points, this.CurrentShortSensor.Id);
            else if (this.ShortSensors.Source != null && this.ShortSensors.Source.Length > 0)
                foreach (var sensor in this.ShortSensors.Source)
                    DrawSensor(points, sensor.Id.Value);

            if (this._currentMeasurementResult != null)
            {
                var sdrRoutes = SVC.SdrnsControllerWcfClient.GetRoutes(this._currentMeasurementResult.MeasSdrResultsId);
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

                var sdrPolygonPoints = SVC.SdrnsControllerWcfClient.GetSensorPoligonPoint(this._currentMeasurementResult.MeasSdrResultsId);
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

                var currentMeasTaskResultStation = this.CurrentResultsMeasurementsStation;
                var currentMeasTaskResult = this.CurrentMeasurementResult;
                var currentMeasTaskStation = this.CurrentMeasTaskStation;
                var currentMeasTask = this.CurrentMeasTask;

                // To define station points

                if (currentMeasTaskResultStation != null)
                {
                    if (currentMeasTask != null && currentMeasTaskResultStation.StationId != null)
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
                                points.Add(MapsDrawingHelper.MakeDrawingPointForStation(stationForShow.Site.Lon.Value, stationForShow.Site.Lat.Value));
                            }
                        }
                    }
                }
                else if (currentMeasTaskResult != null)
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
                                        .Select(s => MapsDrawingHelper.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
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
                        points.Add(MapsDrawingHelper.MakeDrawingPointForStation(currentMeasTaskStation.SiteLon.Value, currentMeasTaskStation.SiteLat.Value));
                    }
                }
                else if (currentMeasTask != null)
                {
                    //var taskStations = currentMeasTask.StationsForMeasurements;
                    var taskStations = _dataStore.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);// SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
                    if (taskStations != null && taskStations.Length > 0)
                    {
                        var stationPoints = taskStations
                            .Where(s => s.Site != null && s.Site.Lon.HasValue && s.Site.Lat.HasValue)
                            .Select(s => MapsDrawingHelper.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
                            .ToArray();

                        if (stationPoints.Length > 0)
                        {
                            points.AddRange(stationPoints);
                        }
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
        private void DrawSensor(List<MP.MapDrawingDataPoint> points, long sensorId)
        {
            var svcSensor = _dataStore.GetSensorById(sensorId);
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
                        .Select(l => MapsDrawingHelper.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
                        .ToArray();
                    points.AddRange(sensorPoints);
                }
            }
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
        private void OnPrevSpecCommand(object parameter)
        {
            if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0 && _selectedSpectrum > 0)
            {
                this.CurrentGeneralResult = Mappers.Map(this.CurrentResultsMeasurementsStationData.GeneralResults[--_selectedSpectrum]);
                this.GenerateSpecLabelText();
            }
        }
        private void OnNextSpecCommand(object parameter)
        {
            if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0 && _selectedSpectrum < this.CurrentResultsMeasurementsStationData.GeneralResults.Length - 1)
            {
                this.CurrentGeneralResult = Mappers.Map(this.CurrentResultsMeasurementsStationData.GeneralResults[++_selectedSpectrum]);
                this.GenerateSpecLabelText();
            }
        }
        private void OnFilterApplyCommand(object parameter)
        {
            Properties.Settings.Default.GlobalFilterMeasGlobalSid = this._currentStationsGolbalFilter.MeasGlobalSid;
            Properties.Settings.Default.GlobalFilterStandard = this._currentStationsGolbalFilter.Standard;
            Properties.Settings.Default.GlobalFilterFreqBg = this._currentStationsGolbalFilter.FreqBg;
            Properties.Settings.Default.GlobalFilterFreqEd = this._currentStationsGolbalFilter.FreqEd;
            Properties.Settings.Default.Save();
            ReloadMeasResaltDetail(this.CurrentMeasurementResult);
        }
        private void LoadSettings()
        {
            this._currentStationsGolbalFilter.MeasGlobalSid = Properties.Settings.Default.GlobalFilterMeasGlobalSid;
            this._currentStationsGolbalFilter.Standard = Properties.Settings.Default.GlobalFilterStandard;
            this._currentStationsGolbalFilter.FreqBg = Properties.Settings.Default.GlobalFilterFreqBg;
            this._currentStationsGolbalFilter.FreqEd = Properties.Settings.Default.GlobalFilterFreqEd;
        }
        private void GenerateSpecLabelText()
        {
            if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0)
                SpecLabelText = (_selectedSpectrum + 1).ToString() + " of " + this.CurrentResultsMeasurementsStationData.GeneralResults.Length.ToString();
            else
                SpecLabelText = "0 of 0";
        }


        //private void ShortSensorsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    MessageBox.Show($"ShortSensorsDataGrid_MouseDoubleClick: {e.ClickCount}");
        //}

        public void Dispose()
        {
            this._dataStore.OnBeginInvoke -= _dataStore_OnBeginInvoke;
            this._dataStore.OnEndInvoke -= _dataStore_OnEndInvoke;

            _timer?.Dispose();
            _waitForm?.Dispose();
            _userActionTask?.Dispose();
        }
    }
}
