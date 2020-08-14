using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using VM = Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels;
//using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Data;
using System.Windows;
using FRM = System.Windows.Forms;
using System.IO;
using Atdi.WpfControls.EntityOrm.Controls;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries;
using System.Globalization;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration
{
    [ViewXaml("PivotTableConfiguration.xaml")]
    [ViewCaption("Pivot table configuration")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private SensorModel _currentSensor;
        private StationModel _currentStation;
        private IList _currentSensors;
        private IList _currentStations;
        private MapDrawingData _currentMapData;
        private long _resultId;

        private IEventHandlerToken<Events.OnCreatedCalcTask> _onCreatedCalcTaskToken;
        private IEventHandlerToken<Events.OnRunedCalcTask> _onRunedCalcTaskToken;

        private PivotTableConfiguration _pivotTableConfiguration;

        public SensorsDataAdapter Sensors { get; set; }
        public StationsDataAdapter Stations { get; set; }

        public ViewCommand SelectFileCommand { get; set; }
        public ViewCommand StartCalculationCommand { get; set; }
        public ViewCommand RefreshCommand { get; set; }

        public View(
            SensorsDataAdapter sensorDataAdapter,
            StationsDataAdapter stationDataAdapter,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            _onCreatedCalcTaskToken = _eventBus.Subscribe<Events.OnCreatedCalcTask>(this.OnCreatedCalcTaskHandle);
            _onRunedCalcTaskToken = _eventBus.Subscribe<Events.OnRunedCalcTask>(this.OnRunedCalcTaskHandle);

            this.SelectFileCommand = new ViewCommand(this.OnSelectFileCommand);
            this.StartCalculationCommand = new ViewCommand(this.OnStartCalculationCommand);
            this.RefreshCommand = new ViewCommand(this.OnRefreshCommand);

            this.Sensors = sensorDataAdapter;
            this.Stations = stationDataAdapter;
            this._pivotTableConfiguration = new PivotTableConfiguration() { IsUseDefaultThreshold = true, Threshold = -90 };
        }
        public long ResultId
        {
            get => this._resultId;
            set => this.Set(ref this._resultId, value, () => { this.OnChangedResultId(value); });
        }

        private bool _isSaveDataToCSV = false;
        public bool IsSaveDataToCSV
        {
            get => this._isSaveDataToCSV;
            set => this.Set(ref this._isSaveDataToCSV, value, () => { IsEnabledSelectFile = value; });
        }

        private bool _isEnabledSelectFile = false;
        public bool IsEnabledSelectFile
        {
            get => this._isEnabledSelectFile;
            set => this.Set(ref this._isEnabledSelectFile, value);
        }

        private bool _isUseDefaultThreshold = true;
        public bool IsUseDefaultThreshold
        {
            get => this._isUseDefaultThreshold;
            set => this.Set(ref this._isUseDefaultThreshold, value, () => { this._pivotTableConfiguration.IsUseDefaultThreshold = value; IsEnabledThreshold = !value; });
        }

        private bool _isEnabledThreshold = false;
        public bool IsEnabledThreshold
        {
            get => this._isEnabledThreshold;
            set => this.Set(ref this._isEnabledThreshold, value);
        }

        private string _filePath = "";
        public string FilePath
        {
            get => this._filePath;
            set => this.Set(ref this._filePath, value);
        }
        public PivotTableConfiguration PivotTableConfiguration
        {
            get => this._pivotTableConfiguration;
            set => this.Set(ref this._pivotTableConfiguration, value);
        }
        public MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public SensorModel CurrentSensor
        {
            get => this._currentSensor;
            set => this.Set(ref this._currentSensor, value/*, () => { this.OnChangedCurrentProject(value); }*/);
        }
        public StationModel CurrentStation
        {
            get => this._currentStation;
            set => this.Set(ref this._currentStation, value/*, () => { this.OnChangedCurrentProject(value); }*/);
        }
        public IList CurrentSensors
        {
            get => this._currentSensors;
            set
            {
                this._currentSensors = value;
                RedrawMap();
            }
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
        private void OnChangedResultId(long resultId)
        {
            ReloadSensors();
            ReloadStations();
        }
        private void ReloadSensors()
        {
            this.Sensors.SensorIds = _objectReader.Read<long[]>().By(new SensorIdsByDriveTestIds { DriveTestIds = _objectReader.Read<long[]>().By(new DriveTestByResultIds { ResultId = this.ResultId }) });
            this.Sensors.Refresh();
            this.CurrentSensor = null;
        }
        private void ReloadStations()
        {
            this.Stations.resultId = this._resultId;
            this.Stations.GSID = PivotTableConfiguration.GSID;
            this.Stations.CorrelationThreshold = PivotTableConfiguration.CorrelationThreshold;
            this.Stations.Status = PivotTableConfiguration.Status;
            this.Stations.Refresh();
            this.CurrentStation = null;
        }
        private void OnRefreshCommand(object parameter)
        {
            ReloadStations();
        }
        private void OnSelectFileCommand(object parameter)
        {
            try
            {
                FRM.SaveFileDialog sfd = new FRM.SaveFileDialog() { Filter = "CSV (*.csv)|*.csv", FileName = $"RefSpectrum_{this._resultId}.csv" };
                if (sfd.ShowDialog() == FRM.DialogResult.OK)
                {
                    this.FilePath = sfd.FileName;
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnStartCalculationCommand(object parameter)
        {
            try
            {
                var calcTask = _objectReader.Read<CalcTaskModel>().By(new CalcTaskByResultId { ResultId = this._resultId });

                var stationIds = new List<long>();
                if (this._currentStations != null)
                    foreach (StationModel v in this._currentStations)
                        stationIds.Add(v.Id);

                if (!ValidateData())
                    return;

                var modifier = new Modifiers.CreateCalcTask
                {
                    MapName = calcTask.MapName,
                    ContextId = calcTask.ContextId,
                    OwnerId = Guid.NewGuid(),
                    Comments = this._pivotTableConfiguration.Comments,
                    PowerThreshold_dBm = this._pivotTableConfiguration.IsUseDefaultThreshold ? -90 : this._pivotTableConfiguration.Threshold,
                    ResultId = this._resultId,
                    StationIds = stationIds.ToArray()
                };
                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private bool ValidateData()
        {
            if (this._pivotTableConfiguration.Threshold < -200 || this._pivotTableConfiguration.Threshold > 50)
            {
                _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.PowerThresholdDBm}'"));
                return false;
            }
            if (this._isSaveDataToCSV && string.IsNullOrEmpty(this._filePath))
            {
                _starter.ShowException("Warning!", new Exception($"Undefined path to csv file"));
                return false;
            }

            return true;
        }
        private void OnCreatedCalcTaskHandle(Events.OnCreatedCalcTask data)
        {
            var modifier = new Modifiers.RunCalcTask
            {
                Id = data.CalcTaskId
            };
            _commandDispatcher.Send(modifier);
        }
        private void OnRunedCalcTaskHandle(Events.OnRunedCalcTask data)
        {
            if (this.IsSaveDataToCSV)
            {
                var resultId = _objectReader.Read<long?>().By(new GetResultIdByTaskId { TaskId = data.Id });
                if (resultId.HasValue)
                {
                    if (WaitForCalcResult(data.Id, resultId.Value))
                    {
                        var refSpectrumResultId = _objectReader.Read<long?>().By(new RefSpectrumResultIdByResultId { ResultId = resultId.Value });
                        if (refSpectrumResultId.HasValue)
                        {

                            if (File.Exists(this._filePath))
                            {
                                try
                                {
                                    File.Delete(this._filePath);
                                }
                                catch (IOException ex)
                                {
                                    _starter.ShowException(Exceptions.StationCalibrationCalculation, new Exception("It wasn't possible to write the data to the disk." + ex.Message));
                                }
                            }
                            var output = new List<string>();
                            output.Add("#;OrderId;Table ICSM Name;ID ICSM;ID Sensor;Global SID;Freq MHz;Level dBm;Percent;DateMeas");

                            long i = 0;


                            var refSpectrums = _objectReader.Read<RefSpectrumResultModel[]>().By(new RefSpectrumByRefSpectrumResultId { RefSpectrumResultId = refSpectrumResultId.Value });

                            foreach (var sp in refSpectrums)
                            {
                                output.Add($"{++i};{sp.OrderId};{sp.TableIcsmName};{sp.IdIcsm};{sp.IdSensor};{sp.GlobalCID};{sp.Freq_MHz};{sp.Level_dBm};{sp.Percent};{sp.DateMeas}");
                            }

                            System.IO.File.WriteAllLines(this._filePath, output.ToArray(), System.Text.Encoding.UTF8);

                            _starter.Stop(this);
                        }
                        else
                        {
                            this._logger.Exception(Exceptions.StationCalibrationCalculation, new Exception($"For selected task not found information in RefSpectrumResultIdByResultId!"));
                            //_starter.ShowException("Warning!", new Exception($"For selected task not found information in IGn06Result!"));
                        }
                    }
                }
                else
                {
                    this._logger.Exception(Exceptions.StationCalibrationCalculation, new Exception($"For selected task not found information in ICalcResults!"));
                    //_starter.ShowException("Warning!", new Exception($"For selected task not found information in ICalcResults!"));
                }
            }
            else
            {
                _starter.Stop(this);
            }
        }
        private bool WaitForCalcResult(long calcTaskId, long calcResultId)
        {
            bool result = false;
            _starter.StartLongProcess(
                new LongProcessOptions()
                {
                    CanStop = false,
                    CanAbort = true,
                    UseProgressBar = true,
                    UseLog = true,
                    IsModal = true,
                    MinValue = 0,
                    MaxValue = 1000,
                    ValueKind = LongProcessValueKind.Infinity,
                    Title = "Calculating task ...",
                    Note = "Please control the log processes below."
                },
                token =>
                {
                    var cancel = false;
                    long eventId = 0;

                    while (!cancel)
                    {
                        var status = _objectReader.Read<byte?>().By(new GetResultStatusById { ResultId = calcResultId });

                        if (status.HasValue)
                        {
                            if (status == (byte)CalcResultStatusCode.Completed)
                            {
                                result = true;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }

                            if (status == (byte)CalcResultStatusCode.Failed)
                            {
                                //_starter.ShowException("Warning!", new Exception($"Task calculation completed with status '{CalcResultStatusCode.Failed.ToString()}'!"));
                                this._logger.Exception(Exceptions.StationCalibrationCalculation, new Exception($"Task calculation completed with status '{CalcResultStatusCode.Failed.ToString()}'!"));
                                result = false;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }
                            if (status == (byte)CalcResultStatusCode.Aborted)
                            {
                                //_starter.ShowException("Warning!", new Exception($"Task calculation completed with status '{CalcResultStatusCode.Aborted.ToString()}'!"));
                                this._logger.Exception(Exceptions.StationCalibrationCalculation, new Exception($"Task calculation completed with status '{CalcResultStatusCode.Aborted.ToString()}'!"));
                                result = false;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }
                            if (status == (byte)CalcResultStatusCode.Canceled)
                            {
                                //_starter.ShowException("Warning!", new Exception($"Task calculation completed with status '{CalcResultStatusCode.Canceled.ToString()}'!"));
                                this._logger.Exception(Exceptions.StationCalibrationCalculation, new Exception($"Task calculation completed with status '{CalcResultStatusCode.Canceled.ToString()}'!"));
                                result = false;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }
                        }

                        var events = _objectReader.Read<CalcResultEventsModel[]>().By(new GetResultEventsByEventIdAndResultId { ResultId = calcResultId, EventId = eventId });
                        foreach (var item in events)
                        {
                            eventId = item.Id;
                            var message = item.Message;

                            if (item.State != null)
                                message = $"{item.Message}: {item.State.State.ToString()}%";

                            _eventBus.Send(new LongProcessLogEvent
                            {
                                ProcessToken = token,
                                Message = message
                            });

                            if (item.LevelCode == 2)
                            {
                                _starter.ShowException("Error!", new Exception(item.Message));
                                result = false;
                                cancel = true;
                            }

                        }
                        System.Threading.Thread.Sleep(5 * 1000);

                        token.AbortToken.ThrowIfCancellationRequested();
                    }
                    //Created = 0, // Фаза создания и подготовки окружения к запуску процесса расчета
                    //Pending = 1, // Фаза ожидания запуска процесса расчета
                    //Accepted = 2, // Фаза ожидания запуска процесса расчета
                    //Processing = 3, // Расчет выполняется
                    //Completed = 4, // Расчет завершен
                    //Canceled = 5, // Расчет был отменен по внешней причине
                    //Aborted = 6, // Расчет был прерван по внутреней причине
                    //Failed = 7  // Попытка запуска завершилась не удачей
                });
            return result;
        }
        private void RedrawMap()
        {
            var data = new MapDrawingData();
            var polygons = new List<MapDrawingDataPolygon>();
            var points = new List<MapDrawingDataPoint>();

            if (this._currentStations != null)
            {
                foreach (StationModel v in this._currentStations)
                {
                    points.Add(new MapDrawingDataPoint()
                    {
                        Location = new Location()
                        {
                            Lon = v.Old_Lon_dec_deg,
                            Lat = v.Old_Lat_dec_deg
                        },
                        Color = System.Windows.Media.Brushes.Black,
                        Fill = System.Windows.Media.Brushes.Black,

                        Opacity = 0.85,
                        Width = 10,
                        Height = 10
                    });
                }
            }

            if (this._currentSensors != null)
            {
                foreach (SensorModel v in this._currentSensors)
                {
                    var loc = _objectReader.Read<Location>().By(new SensorLocationBySensorId { SensorId = v.Id });
                    if (loc != null)
                    {
                        points.Add(new MapDrawingDataPoint()
                        {
                            Location = loc,
                            Color = System.Windows.Media.Brushes.Blue,
                            Fill = System.Windows.Media.Brushes.DarkBlue,

                            Opacity = 0.85,
                            Width = 10,
                            Height = 10
                        });
                    }
                }
            }

            var countours = _objectReader.Read<string[]>().By(new ContoursByResultId { ResultId = this._resultId });
            if (countours != null && countours.Length > 0)
            {
                foreach (var item in countours)
                {
                    var polygonPoints = new List<Location>();
                    var contour = GetCountourFromString(item);

                    if (contour.Length > 0)
                    {
                        contour.ToList().ForEach(areaPoint =>
                        {
                            polygonPoints.Add(new Location() { Lat = areaPoint.Lat, Lon = areaPoint.Lon });
                        });
                        polygons.Add(new MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
                    }
                }
            }

            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();

            this.CurrentMapData = data;
        }
        private Location[] GetCountourFromString(string contour)
        {
            string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var listLocation = new List<Location>();
            var coords = contour.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var coord in coords)
            {
                if (!string.IsNullOrEmpty(coord))
                {
                    var cord = coord.Split(' ');
                    if (cord.Length == 2)
                    {
                        if (double.TryParse(cord[0].Replace(".", sep).Replace(",", sep), out double lon) && double.TryParse(cord[1].Replace(".", sep).Replace(",", sep), out double lat))
                            listLocation.Add(new Location() { Lon = lon, Lat = lat });
                    }
                }
            }
            return listLocation.ToArray();
        }
        public override void Dispose()
        {
            _onCreatedCalcTaskToken?.Dispose();
            _onCreatedCalcTaskToken = null;
            _onRunedCalcTaskToken?.Dispose();
            _onRunedCalcTaskToken = null;
        }
    }
}
