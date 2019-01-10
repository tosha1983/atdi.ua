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
using SDR = Atdi.AppServer.Contracts.Sdrns;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;

namespace XICSM.ICSControlClient.ViewModels
{
    
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

        #region Corrent Objects
        
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
        private ResultsMeasurementsStationExtentedViewModel _currentResultsMeasurementsStation;
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
        private ResultsMeasurementsStationExtentedDataAdapter _resultsMeasurementsStations;
        private LevelMeasurementsCarDataAdapter _levelMeasurements;

        private Visibility _measTaskDetailVisibility = Visibility.Hidden;
        private Visibility _measResultsDetailVisibility = Visibility.Hidden;

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

        public ControlClientViewModel()
        {
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
            this._resultsMeasurementsStations = new ResultsMeasurementsStationExtentedDataAdapter();
            this._levelMeasurements = new LevelMeasurementsCarDataAdapter();


            this.ReloadShortMeasTasks();
            this.ReloadShorSensors();
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
            set => this.Set(ref this._currentMeasTask, value, () => { ReloadShorSensors(); RedrawMap(); });
        }

        public ShortMeasTaskViewModel CurrentShortMeasTask
        {
            get => this._currentShortMeasTask;
            set => this.Set(ref this._currentShortMeasTask, value, () => { ReloadShortMeasResults(); ReloadMeasTaskDetail(); ReloadMeasResaltDetail(); });
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
            set => this.Set(ref this._currentMeasurementResults, value,() => { ReloadMeasResaltDetail(); UpdateCurrentChartOption(); this.RedrawMap(); });
        }
        public ResultsMeasurementsStationExtentedViewModel CurrentResultsMeasurementsStation
        {
            get => this._currentResultsMeasurementsStation;
            set => this.Set(ref this._currentResultsMeasurementsStation, value, () => { ReloadMeasResultStationDetail(); ReloadLevelMeasurements(); UpdateCurrentChartOption(); });
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

        public ResultsMeasurementsStationExtentedDataAdapter ResultsMeasurementsStations => this._resultsMeasurementsStations;

        #endregion

        public Visibility MeasTaskDetailVisibility
        {
            get => this._measTaskDetailVisibility;
            set => this.Set(ref this._measTaskDetailVisibility, value, ReloadMeasTaskDetail);
        }

        public Visibility MeasResultsDetailVisibility
        {
            get => this._measResultsDetailVisibility;
            set => this.Set(ref this._measResultsDetailVisibility, value, ReloadMeasResaltDetail);
        }

        private void ReloadShortMeasTasks()
        {
            var sdrTasks = SVC.SdrnsControllerWcfClient.GetShortMeasTasks(new Atdi.AppServer.Contracts.DataConstraint());
            this._shortMeasTasks.Source = sdrTasks;
        }

        private void ReloadShortMeasResults()
        {
            int taskId = 0;
            if (this._currentShortMeasTask != null)
            {
                taskId = this._currentShortMeasTask.Id;
            }

            var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(taskId);
            this._measResults.Source = sdrMeasResults;
        }

        private void ReloadMeasTaskDetail()
        {
            int taskId = 0;
            if (this._currentShortMeasTask != null)
            {
                taskId = this._currentShortMeasTask.Id;
            }
            var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(taskId);
            var taskViewModel = Mappers.Map(task);
            this.CurrentMeasTask = taskViewModel;

            if (this.MeasTaskDetailVisibility == Visibility.Visible && taskId > 0)
            {

                if (task != null)
                {
                    //this._measTaskDetailStations.Source = task.StationsForMeasurements;
                    this._measTaskDetailStations.Source = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(taskId);
                }
                else
                {
                    this._measTaskDetailStations.Source = null;
                }
            }
        }
        private void ReloadMeasResaltDetail()
        {
            if (this._currentMeasurementResults != null)
            {
                var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(this._currentMeasurementResults.MeasSdrResultsId);
                this._resultsMeasurementsStations.Source = sdrMeasResults;
            }
            else
            {
                this.CurrentMeasurementResults = null;
                this._resultsMeasurementsStations.Source = null;
            }
        }
        private void ReloadMeasResultStationDetail()
        {
            if (this._currentResultsMeasurementsStation != null)
            {
                var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationById(this._currentResultsMeasurementsStation.Id);
                this.CurrentResultsMeasurementsStationData = Mappers.Map(sdrMeasResults);
            }
        }

        private void ReloadShorSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors(new Atdi.AppServer.Contracts.DataConstraint());

            var measTask = this.CurrentMeasTask;
            if (measTask != null )
            {
                if (measTask.Stations != null && measTask.Stations.Length > 0)
                {
                    sdrSensors = sdrSensors
                        .Where(sdrSensor => measTask.Stations
                                .FirstOrDefault(s => s.StationId.Value == sdrSensor.Id.Value) != null
                            )
                        .ToArray();
                }
                else
                {
                    sdrSensors = new SDR.ShortSensor[] { };
                }
            }
            this._shortSensors.Source = sdrSensors;
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

                int taskId = this._currentMeasurementResults.MeasTaskId;
                string stationId = this._currentResultsMeasurementsStationData.StationId;

                FRM.SaveFileDialog sfd = new FRM.SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "FS_Meas_Res_" + taskId.ToString() + "_" + stationId + ".csv";
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

                string stationName = this._currentResultsMeasurementsStationData.MeasGlobalSID;
                double? lonMin = null;
                double? lonMax = null;
                double? latMin = null;
                double? latMax = null;
                decimal? freq = null;

                stationName = stationName.Substring(stationName.Length - 5, 4).TrimStart('0');

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

                    if (minFreq.HasValue && maxFreq.HasValue && minFreq - 0.2 < (double)freq && (double)freq < maxFreq + 0.2)
                        stations.Add(source.GetI("ID"), source.GetI("ID"));

                    //MessageBox.Show(source.GetS("ID") + " - " + source.GetS("NAME") + "(" + source.GetD("Position.LONGITUDE").ToString() + ":" + source.GetD("Position.LATITUDE").ToString() + ")");
                }

                if (stations.Count() > 0)
                {
                    var dlgForm = new FM.StationListForm();
                    dlgForm.stationIDs = string.Join(",", stations.Keys.ToArray());
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
            this.ReloadShortMeasTasks();
        }

        private void OnRefreshShortSensorsCommand(object parameter)
        {
            this.ReloadShorSensors();
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


        private void UpdateCurrentChartOption()
        {
            if (this.MeasResultsDetailVisibility != Visibility.Visible)
            {
                return;
            }

            if (this.CurrentMeasurementResults == null)
            {
                this.CurrentChartOption = this.GetDefaultChartOption();
            }
            else
            {
                this.CurrentChartOption = this.GetChartOption(this.CurrentMeasurementResults); 
            }
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

            var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(result.MeasSdrResultsId);

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

            var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(result.MeasSdrResultsId);

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

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            option.XTick = preparedDataX.Step;
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
            var points = new List<MP.MapDrawingDataPoint>();

            if (this.CurrentShortSensor != null)
            {
                var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(this.CurrentShortSensor.Id);
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
                    var measTaskStations = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
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
                        var measTaskStations = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
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
                var taskStations = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id);
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


            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        string GetTimeVal(TimeSpan val)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", val.Hours, val.Minutes, val.Seconds, val.Milliseconds / 10);

        }
    }
}
