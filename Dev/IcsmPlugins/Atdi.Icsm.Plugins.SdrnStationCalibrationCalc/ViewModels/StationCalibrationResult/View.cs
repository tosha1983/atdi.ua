using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using MP = Atdi.WpfControls.EntityOrm.Controls;
using System.Data;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Adapters;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries;
using VM = Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels;
using FRM = System.Windows.Forms;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using System.ComponentModel;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.Reports;
using System.IO;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    [ViewXaml("StationCalibrationResult.xaml")]
    [ViewCaption("Station Calibration result")]
    public class View : ViewBase
    {
        private DateTime? _dateStartLoadResults;
        private DateTime? _dateStopLoadResults;

        private long _resultId;
        private MP.MapDrawingData _currentMapData;

        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public ViewCommand LoadResultsCommand { get; set; }
        public ViewCommand ExportResultsCommand { get; set; }
        public ViewCommand CreatePivotTableCommand { get; set; }

        private CalcServerDataLayer _dataLayer { get; set; }

        private StationCalibrationResultModel _currentStationCalibrationResult;
        private IList _currentStationCalibrationResultModel;
        public StationCalibrationResultDataAdapter  StationCalibrationResultDataAdapter { get; set; }

        private IList _currentStationCalibrationDriveTestsModel;
        public StationCalibrationDriveTestsDataAdapter StationCalibrationDriveTestsDataAdapter { get; set; }


        private IList _currentStationCalibrationStaModel;
        public StationCalibrationStaDataAdapter  StationCalibrationStaDataAdapter { get; set; }

        public View(
            CalcServerDataLayer dataLayer,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            StationCalibrationResultDataAdapter stationCalibrationResultDataAdapter,
            StationCalibrationDriveTestsDataAdapter stationCalibrationDriveTestsDataAdapter,
            StationCalibrationStaDataAdapter stationCalibrationStaDataAdapter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            this.StationCalibrationResultDataAdapter = stationCalibrationResultDataAdapter;
            this.StationCalibrationDriveTestsDataAdapter = stationCalibrationDriveTestsDataAdapter;
            this.StationCalibrationStaDataAdapter = stationCalibrationStaDataAdapter;


            this.LoadResultsCommand = new ViewCommand(this.OnLoadResultsCommand);
            this.ExportResultsCommand = new ViewCommand(this.OnExportResultsCommand);
            this.CreatePivotTableCommand = new ViewCommand(this.OnCreatePivotTableCommand);

            this._dataLayer = dataLayer;

            this.DateStartLoadResults = DateTime.Now.AddDays(-30);
            this.DateStopLoadResults = DateTime.Now;

            this.RedrawMap();
        }

        public DateTime? DateStartLoadResults
        {
            get => this._dateStartLoadResults;
            set => this.Set(ref this._dateStartLoadResults, value);
        }
        public DateTime? DateStopLoadResults
        {
            get => this._dateStopLoadResults;
            set => this.Set(ref this._dateStopLoadResults, value);
        }

        private void OnLoadResultsCommand(object parameter)
        {
            try
            {
                this.StationCalibrationStaDataAdapter.resultId = -1;
                this.StationCalibrationStaDataAdapter.Refresh();

                this.StationCalibrationDriveTestsDataAdapter.resultId = -1;
                this.StationCalibrationDriveTestsDataAdapter.Refresh();

                
                this.StationCalibrationResultDataAdapter.taskId = GetTaskIdByCalcResultId(ResultId).Value;
                this.StationCalibrationResultDataAdapter.dateTimeStart = new DateTimeOffset(DateStartLoadResults.Value);
                this.StationCalibrationResultDataAdapter.dateTimeStop = new DateTimeOffset(DateStopLoadResults.Value);
                this.StationCalibrationResultDataAdapter.Refresh();

            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnExportResultsCommand(object parameter)
        {
            try
            {
                FRM.SaveFileDialog sfd = new FRM.SaveFileDialog() { Filter = "CSV (*.csv)|*.csv", FileName = $"StationCalibrationResult_{this._resultId}.xlsx" };
                if (sfd.ShowDialog() == FRM.DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            _starter.ShowException("Warning!", new Exception($"It wasn't possible to write the data to the disk." + ex.Message));
                        }
                    }

                    ReportExcelFast rep = new ReportExcelFast();
                    var line = new List<string>();
                    rep.Init(sfd.FileName, "Table with stations", "");
                    var cellStyle = rep.GetCellStyleBorder();

                    line.Add(Properties.Resources.Id);
                    line.Add(Properties.Resources.ResultStationStatus);
                    line.Add(Properties.Resources.MaxCorellation);
                    line.Add(Properties.Resources.ExternalCode);
                    line.Add(Properties.Resources.ExternalSource);
                    rep.SetColumnWidth(4, 15);
                    line.Add(Properties.Resources.LicenseGsid);
                    rep.SetColumnWidth(5, 20);
                    line.Add(Properties.Resources.RealGsid);
                    rep.SetColumnWidth(6, 20);
                    line.Add(Properties.Resources.New_Tilt_deg);
                    line.Add(Properties.Resources.Old_Tilt_deg);
                    line.Add(Properties.Resources.New_Azimuth_deg);
                    line.Add(Properties.Resources.Old_Azimuth_deg);
                    line.Add(Properties.Resources.New_Altitude_m);
                    line.Add(Properties.Resources.Old_Altitude_m);
                    line.Add(Properties.Resources.New_Lat_deg);
                    line.Add(Properties.Resources.Old_Lat_deg);
                    line.Add(Properties.Resources.New_Lon_deg);
                    line.Add(Properties.Resources.Old_Lon_deg);
                    line.Add(Properties.Resources.New_Power_dB);
                    line.Add(Properties.Resources.Old_Power_dB);
                    line.Add(Properties.Resources.OldFreq_MHz);
                    line.Add(Properties.Resources.FreqLinkDriveTest_MHz);
                    line.Add(Properties.Resources.Standard);
                    rep.WriteLine(line.ToArray());

                    int i = 1;
                    this.StationCalibrationStaDataAdapter.Reset();
                    foreach (StationCalibrationStaModel item in this.StationCalibrationStaDataAdapter)
                    {
                        rep.WriteLine();
                        rep.SetCellValue(0, i, item.Id, cellStyle);
                        rep.SetCellValue(1, i, item.ResultStationStatus, cellStyle);
                        rep.SetCellValue(2, i, item.MaxCorellation, cellStyle);
                        rep.SetCellValue(3, i, item.ExternalCode, cellStyle);
                        rep.SetCellValue(4, i, item.ExternalSource, cellStyle);
                        rep.SetCellValue(5, i, item.LicenseGsid, cellStyle);
                        rep.SetCellValue(6, i, item.RealGsid, cellStyle);
                        rep.SetCellValue(7, i, item.New_Tilt_deg, cellStyle);
                        rep.SetCellValue(8, i, item.Old_Tilt_deg, cellStyle);
                        rep.SetCellValue(9, i, item.New_Azimuth_deg, cellStyle);
                        rep.SetCellValue(10, i, item.Old_Azimuth_deg, cellStyle);
                        rep.SetCellValue(11, i, item.New_Altitude_m, cellStyle);
                        rep.SetCellValue(12, i, item.Old_Altitude_m, cellStyle);
                        rep.SetCellValue(13, i, item.New_Lat_dec_deg, cellStyle);
                        rep.SetCellValue(14, i, item.Old_Lat_dec_deg, cellStyle);
                        rep.SetCellValue(15, i, item.New_Lon_dec_deg, cellStyle);
                        rep.SetCellValue(16, i, item.Old_Lon_dec_deg, cellStyle);
                        rep.SetCellValue(17, i, item.New_Power_dB, cellStyle);
                        rep.SetCellValue(18, i, item.Old_Power_dB, cellStyle);
                        rep.SetCellValue(19, i, item.Old_Freq_MHz, cellStyle);
                        rep.SetCellValue(20, i, item.Freq_MHz, cellStyle);
                        rep.SetCellValue(21, i, item.Standard, cellStyle);
                        i++;
                    }

                    rep.AddSheet("Drive tests");
                    line.Clear();
                    line.Add(Properties.Resources.Id);
                    line.Add(Properties.Resources.DriveTestId);
                    line.Add(Properties.Resources.ExternalSource);
                    line.Add(Properties.Resources.ExternalCode);
                    line.Add(Properties.Resources.StationGcid);
                    rep.SetColumnWidth(4, 20);
                    line.Add(Properties.Resources.MeasGcid);
                    rep.SetColumnWidth(5, 20);
                    line.Add(Properties.Resources.ResultDriveTestStatus);
                    line.Add(Properties.Resources.CountPointsInDriveTest);
                    line.Add(Properties.Resources.MaxPercentCorellation);
                    line.Add(Properties.Resources.Freq_MHz);
                    line.Add(Properties.Resources.Standard);
                    rep.WriteLine(line.ToArray());

                    i = 1;
                    this.StationCalibrationDriveTestsDataAdapter.Reset();
                    foreach (StationCalibrationDriveTestsModel item in this.StationCalibrationDriveTestsDataAdapter)
                    {
                        rep.WriteLine();
                        rep.SetCellValue(0, i, item.Id, cellStyle);
                        rep.SetCellValue(1, i, item.DriveTestId, cellStyle);
                        rep.SetCellValue(2, i, item.ExternalSource, cellStyle);
                        rep.SetCellValue(3, i, item.ExternalCode, cellStyle);
                        rep.SetCellValue(4, i, item.StationGcid, cellStyle);
                        rep.SetCellValue(5, i, item.MeasGcid, cellStyle);
                        rep.SetCellValue(6, i, item.ResultDriveTestStatus, cellStyle);
                        rep.SetCellValue(7, i, item.CountPointsInDriveTest, cellStyle);
                        rep.SetCellValue(8, i, item.MaxPercentCorellation, cellStyle);
                        rep.SetCellValue(9, i, item.Freq_MHz, cellStyle);
                        rep.SetCellValue(10, i, item.Standard, cellStyle);
                        i++;
                    }

                    rep.Save();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnCreatePivotTableCommand(object parameter)
        {
            try
            {
                if (this._currentStationCalibrationResult != null)
                    _starter.Start<VM.PivotTableConfiguration.View>(isModal: true, c => c.ResultId = this._currentStationCalibrationResult.Id);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        public long ResultId
        {
            get => this._resultId;
            set => this.Set(ref this._resultId, value, () => { this.OnChangeResultId(value); });
        }
        public StationCalibrationResultModel CurrentStationCalibrationResult
        {
            get => this._currentStationCalibrationResult;
            set => this.Set(ref this._currentStationCalibrationResult, value, () => { });
        }
        private void OnChangeResultId(long resId)
        {
            this.StationCalibrationResultDataAdapter.taskId = GetTaskIdByCalcResultId(resId).Value;
            this.StationCalibrationResultDataAdapter.Refresh();
        }

        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }

        /// <summary>
        /// Координаты станции
        /// </summary>
        private void RedrawMapStationCalibrationStaModel()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygonsByStations = new List<MP.MapDrawingDataPolygon>();

            if (this._currentStationCalibrationStaModel != null)
            {
                foreach (StationCalibrationStaModel v in this._currentStationCalibrationStaModel)
                {
                    var sta = GetStationCalibrationStaById(v.Id);
                    if (sta != null)
                    {
                        for (int i = 0; i < sta.Length; i++)
                        {
                            points.Add(new MP.MapDrawingDataPoint()
                            {
                                Location = new MP.Location()
                                {
                                    Lon = sta[i].Old_Lon_dec_deg,
                                    Lat = sta[i].Old_Lat_dec_deg
                                },
                                Color = System.Windows.Media.Brushes.Blue,
                                Fill = System.Windows.Media.Brushes.DarkBlue,

                                Opacity = 0.85,
                                Width = 10,
                                Height = 10
                            });
                        }
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        /// <summary>
        /// Маршруты драйв тестов
        /// </summary>
        private void RedrawStationCalibrationDriveTestsModel()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygonsByStations = new List<MP.MapDrawingDataPolygon>();

            if (this._currentStationCalibrationDriveTestsModel != null)
            {
                foreach (StationCalibrationDriveTestsModel v in this._currentStationCalibrationDriveTestsModel)
                {
                    if (v.DriveTestId > 0)
                    {
                        var driveTestPoints = GetDriveTestPoints(v.DriveTestId);
                        if (driveTestPoints != null)
                        {
                            for (int i = 0; i < driveTestPoints.Length; i++)
                            {
                                points.Add(new MP.MapDrawingDataPoint()
                                {
                                    Location = new MP.Location()
                                    {
                                        Lat = driveTestPoints[i].Coordinate.Latitude,
                                        Lon = driveTestPoints[i].Coordinate.Longitude
                                    },
                                    Color = System.Windows.Media.Brushes.Orange,
                                    Fill = System.Windows.Media.Brushes.OrangeRed,
                                    Opacity = 0.85,
                                    Width = 4,
                                    Height = 4
                                });
                            }
                        }

                        if (v.LinkToStationMonitoringId > 0)
                        {
                            var sta = GetStationCalibrationStaByResId(v.LinkToStationMonitoringId);
                            if (sta != null)
                            {
                                for (int i = 0; i < sta.Length; i++)
                                {
                                    points.Add(new MP.MapDrawingDataPoint()
                                    {
                                        Location = new MP.Location()
                                        {
                                            Lon = sta[i].Old_Lon_dec_deg,
                                            Lat = sta[i].Old_Lat_dec_deg
                                        },
                                        Color = System.Windows.Media.Brushes.Blue,
                                        Fill = System.Windows.Media.Brushes.DarkBlue,

                                        Opacity = 0.85,
                                        Width = 10,
                                        Height = 10
                                    });
                                }
                            }
                        }
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        /// <summary>
        /// Координаты станции и маршруты драйв тестов
        /// </summary>
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygonsByStations = new List<MP.MapDrawingDataPolygon>();

            if (this._currentStationCalibrationResultModel!=null)
            {
                foreach (StationCalibrationResultModel v in this._currentStationCalibrationResultModel)
                {
                    var sta = GetStationCalibrationStaByResId(v.Id);
                    if (sta != null)
                    {
                        for (int i = 0; i < sta.Length; i++)
                        {
                            points.Add(new MP.MapDrawingDataPoint()
                            {
                                Location = new MP.Location()
                                {
                                    Lon = sta[i].Old_Lon_dec_deg,
                                    Lat = sta[i].Old_Lat_dec_deg
                                },
                                Color = System.Windows.Media.Brushes.Blue,
                                Fill = System.Windows.Media.Brushes.DarkBlue,

                                Opacity = 0.85,
                                Width = 10,
                                Height = 10
                            });
                        }
                    }
                }
            }
            
            //data.Polygons = polygonsByStations.ToArray();
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }


        public IList CurrentStationCalibrationResultModel
        {
            get { return this._currentStationCalibrationResultModel; }
            set
            {
                this._currentStationCalibrationResultModel = value;

                if (this._currentStationCalibrationResultModel != null)
                {
                    foreach (StationCalibrationResultModel v in this._currentStationCalibrationResultModel)
                    {
                        this.StationCalibrationStaDataAdapter.resultId = v.Id;
                        this.StationCalibrationStaDataAdapter.Refresh();
                        this.StationCalibrationDriveTestsDataAdapter.resultId = v.Id;
                        this.StationCalibrationDriveTestsDataAdapter.Refresh();
                        this.RedrawMap();
                    }
                }
            }
        }


        public IC_ES.DriveTestPoint[] GetDriveTestPoints(long Id)
        {
            var resDriveTestPoints = _objectReader
                .Read<IC_ES.DriveTestPoint[]>()
                .By(new DriveTestPointsResultsModelById()
                {
                    Id = Id
                });
            return resDriveTestPoints;
        }
        
        public StationCalibrationStaModel[] GetStationCalibrationStaByResId(long ResId)
        {
            var resRoutes = _objectReader
                .Read<StationCalibrationStaModel[]>()
                .By(new StationCalibrationStaModelByResultId()
                {
                    ResultId = ResId
                });
            return resRoutes;
        }

        public StationCalibrationStaModel[] GetStationCalibrationStaById(long Id)
        {
            var resRoutes = _objectReader
                .Read<StationCalibrationStaModel[]>()
                .By(new StationCalibrationStaModelById()
                {
                     Id = Id
                });
            return resRoutes;
        }

        public long? GetTaskIdByCalcResultId(long ResId)
        {
            var resRoutes = _objectReader
                .Read<long?>()
                .By(new CalcResultsModelById()
                {
                    ResultId = ResId
                });
            return resRoutes;
        }


        public IList CurrentStationCalibrationDriveTestsModel
        {
            get { return this._currentStationCalibrationDriveTestsModel; }
            set
            {
                this._currentStationCalibrationDriveTestsModel = value;
                this.RedrawStationCalibrationDriveTestsModel();
            }
        }


        public IList CurrentStationCalibrationStaModel
        {
            get { return this._currentStationCalibrationStaModel; }
            set
            {
                this._currentStationCalibrationStaModel = value;
                this.RedrawMapStationCalibrationStaModel();
            }
        }

        

        public override void Dispose()
        {

        }

    }

}
