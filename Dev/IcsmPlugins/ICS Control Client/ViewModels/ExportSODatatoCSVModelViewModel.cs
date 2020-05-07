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
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using SDRI = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using System.Collections;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using Atdi.Common;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrns.Server.Entities;
using System.Security.Cryptography;
using Atdi.DataModels.Sdrns.Server;
using Model = XICSM.ICSControlClient.Models;





namespace XICSM.ICSControlClient.ViewModels
{
    public class ExportSODatatoCSVModelViewModel : WpfViewModelBase
    {
        public enum SpectrumOccupationType
        {
            FreqBandwidthOccupation,
            FreqChannelOccupation,
            All
        }

        private IList _currentSensors;
        private long[] _currentSensorsIndexes;
        private MP.MapDrawingData _currentMapData;
        private DateTime? _dateStartExport;
        private DateTime? _dateStopExport;
        private double? _freqStartExport;
        private double? _freqStopExport;
        private string _exportDirectoryName;
        private bool _isEnabledExport = false;
        private SpectrumOccupationType _spectrumOccupationType;
        private FM.WaitForm _waitForm = null;



        private ShortSensorDataAdatper _sensors;
        public WpfCommand ExportCommand { get; set; }



        public ExportSODatatoCSVModelViewModel()
        {
            this.ExportCommand = new WpfCommand(this.OnExportCommand);
            this._sensors = new ShortSensorDataAdatper();
            this.ReloadData();
            this.RedrawMap();
        }
        public ShortSensorDataAdatper Sensors => this._sensors;
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }

        public SpectrumOccupationType SpectrumOccupationTypeVal
        {
            get => this._spectrumOccupationType;
            set => this.Set(ref this._spectrumOccupationType, value);
        }

        public IList<SpectrumOccupationType> SpectrumOccupationTypeValues => Enum.GetValues(typeof(SpectrumOccupationType)).Cast<SpectrumOccupationType>().ToList();


        public IList CurrentSensors
        {
            get => this._currentSensors;
            set
            {
                this._currentSensors = value;
                RedrawMap();
                CheckEnabledStart();
            }
        }

        public long[] CurrentSensorsIndexes
        {
            get => this._currentSensorsIndexes;
            set => this.Set(ref this._currentSensorsIndexes, value);
        }

        public string ExportDirectoryName
        {
            get => this._exportDirectoryName;
            set => this.Set(ref this._exportDirectoryName, value);
        }


        public double? FreqStartExport
        {
            get => this._freqStartExport;
            set => this.Set(ref this._freqStartExport, value);
        }
        public double? FreqStopExport
        {
            get => this._freqStopExport;
            set => this.Set(ref this._freqStopExport, value);
        }

        public DateTime? DateStartExport
        {
            get => this._dateStartExport;
            set => this.Set(ref this._dateStartExport, value);
        }
        public DateTime? DateStopExport
        {
            get => this._dateStopExport;
            set => this.Set(ref this._dateStopExport, value);
        }
        public bool IsEnabledExport
        {
            get => this._isEnabledExport;
            set => this.Set(ref this._isEnabledExport, value);
        }
        private void CheckEnabledStart()
        {
            if (this._currentSensors.Count > 0)
                IsEnabledExport = true;
            else
                IsEnabledExport = false;
        }
        private void ReloadData()
        {
            this.DateStartExport = DateAndTime.Now.AddDays(-30);
            this.DateStopExport = DateAndTime.Now;
            this.FreqStartExport = 700;
            this.FreqStopExport = 2400;
            this.ReloadSensors();
        }
        private void ReloadSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
            this._sensors.Source = sdrSensors;
        }

        private void OnExportCommand(object parameter)
        {
            MessageBox.Show("Результати експорту можуть мати великій розмір. За необхідністю експорт відбудеться в декілька файлів/таблиць!", PluginHelper.MessageBoxCaption, MessageBoxButton.OK);
            {
                if (!DateStartExport.HasValue)
                {
                    MessageBox.Show("Undefined Date Start!");
                    return;
                }
                if (!DateStopExport.HasValue)
                {
                    MessageBox.Show("Undefined Date Stop!");
                    return;
                }
                if (DateStartExport > DateStopExport)
                {
                    MessageBox.Show("Date Stop should be great of the Date Start!");
                    return;
                }


                if (!FreqStartExport.HasValue)
                {
                    MessageBox.Show("Undefined Channel Frequency Min, MHz!");
                    return;
                }
                if (!FreqStopExport.HasValue)
                {
                    MessageBox.Show("Undefined Channel Frequency Max, MHz!");
                    return;
                }
                if (FreqStartExport > FreqStopExport)
                {
                    MessageBox.Show("Frequency Max should be great of the Frequency Min!");
                    return;
                }


                FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
                if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                    {
                        ExportDirectoryName = folderDialog.SelectedPath;
                        ReadData();
                    }
                }
            }
        }

        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            if (CurrentSensors != null)
                foreach (ShortSensorViewModel sensor in CurrentSensors)
                    DrawSensor(points, sensor.Id);
            else if (this._sensors.Source != null && this._sensors.Source.Length > 0)
                foreach (var sensor in this._sensors.Source)
                    DrawSensor(points, sensor.Id.Value);

            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        private void DrawSensor(List<MP.MapDrawingDataPoint> points, long sensorId)
        {
            var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(sensorId);
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


        private void ReadData()
        {
            bool isSuccess = true;
            try
            {
                var isThreshHoldExportSO = false;
                var threshHoldExportSO = PluginHelper.GetThreshHoldExportSO();
                var countMaxRecInPage = PluginHelper.GetMaxCountRecordInPage();
                var countMaxRecordInCsvFile = PluginHelper.GetMaxCountRecordInCsvFile();
                var endpoint = new WebApiEndpoint(new Uri(PluginHelper.GetWebAPIBaseAddress()), PluginHelper.GetWebAPIUrl());
                var dataContext = new WebApiDataContext(PluginHelper.GetDataContext());
                var dataLayer = new WebApiDataLayer();
                var executor = dataLayer.GetExecutor(endpoint, dataContext);

                string currFileName = string.Empty;
                var countAllRec = 0; 
                var countRecInOneFile = 0; var indexNumFile = 0;
                var lstMeasTask = new List<MeasTaskSpectrumOccupation>();
                var lstMeasTaskIds = new List<long>();
                var lstAllSensors = new List<Sensor>();
                var typeMeasurements = "SpectrumOccupation";
                var sensorIds = new long[CurrentSensors.Count];

                if (CurrentSensors != null)
                {
                    int idx = 0;
                    foreach (ShortSensorViewModel sensor in CurrentSensors)
                    {
                        sensorIds[idx] = sensor.Id;
                        idx++;
                    }
                }

                _waitForm = new FM.WaitForm();
                _waitForm.SetMessage($"Please wait...");
                _waitForm.TopMost = true;

                Task.Run(() =>
                {
                    var listSensors = BreakDownElemBlocks.BreakDown(sensorIds);
                    for (int i = 0; i < listSensors.Count; i++)
                    {
                        long countResLevels = 0;
                        long offsetResLevels = 0;
                        do
                        {
                            if ((threshHoldExportSO - countMaxRecInPage) < 0)
                            {
                                countMaxRecInPage = threshHoldExportSO;
                            }

                            var webQueryResLevels = dataLayer.GetBuilder<IResLevels>()
                            .Read()
                            .Select(
                               c => c.Id, c => c.FreqMeas, c => c.OccupancySpect, c => c.VMinLvl, c => c.VMMaxLvl, c => c.ValueLvl, c => c.RES_MEAS.Id, c => c.RES_MEAS.TimeMeas, c => c.RES_MEAS.ScansNumber, c => c.RES_MEAS.TypeMeasurements, c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id,
                               c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Name, c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id, c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name, c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId, c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Title)
                            .OrderByAsc(c => c.Id)
                            .Paginate(offsetResLevels, countMaxRecInPage)
                            .BeginFilter()
                                .Condition(c => c.RES_MEAS.TimeMeas, FilterOperator.LessEqual, DateStopExport.Value)
                                 .And()
                                .Condition(c => c.RES_MEAS.TimeMeas, FilterOperator.GreaterEqual, DateStartExport.Value)
                                 .And()
                                .Condition(c => c.RES_MEAS.TypeMeasurements, FilterOperator.Equal, typeMeasurements)
                                 .And()
                                .Condition(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id, FilterOperator.In, listSensors[i].ToArray())
                                 .And()
                                .Condition(c => c.FreqMeas, FilterOperator.Between, (float)FreqStartExport.Value, (float)FreqStopExport.Value)
                            .EndFilter();


                            var recordsResLevels = executor.ExecuteAndFetch(webQueryResLevels, readerResLevels =>
                            {
                                countResLevels = readerResLevels.Count;
                                if (countResLevels > 0)
                                {
                                    while (readerResLevels.Read())
                                    {
                                        var taskId = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                                        var sensorId = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);

                                        if (!lstMeasTaskIds.Contains(taskId))
                                        {
                                            lstMeasTaskIds.Add(taskId);
                                            var measTask = new MeasTaskSpectrumOccupation();
                                            measTask.Id = new MeasTaskIdentifier();
                                            measTask.Id.Value = taskId;
                                            measTask.Name = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Name);

                                            MeasurementType typeMeas;
                                            if (Enum.TryParse<MeasurementType>(readerResLevels.GetValue(c => c.RES_MEAS.TypeMeasurements), out typeMeas))
                                            {
                                                measTask.TypeMeasurements = typeMeas;
                                            }
                                            IReadQuery<IMeasOther> webQueryMeasOther = null;

                                            if (SpectrumOccupationTypeVal != SpectrumOccupationType.All)
                                            {
                                                webQueryMeasOther = dataLayer.GetBuilder<IMeasOther>()
                                               .Read()
                                               .Select(
                                               c => c.Id, c => c.LevelMinOccup, c => c.TypeSpectrumOccupation, c => c.Nchenal)
                                               .OrderByAsc(c => c.Id)
                                               .BeginFilter()
                                                   .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, taskId)
                                                   .And()
                                                   .Condition(c => c.TypeSpectrumOccupation, FilterOperator.Equal, SpectrumOccupationTypeVal.ToString())
                                               .EndFilter()
                                               .OnTop(1);
                                            }
                                            else
                                            {
                                                webQueryMeasOther = dataLayer.GetBuilder<IMeasOther>()
                                                .Read()
                                                .Select(
                                                c => c.Id, c => c.LevelMinOccup, c => c.TypeSpectrumOccupation, c => c.Nchenal)
                                                .OrderByAsc(c => c.Id)
                                                .BeginFilter()
                                                    .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, taskId)
                                                .EndFilter()
                                                .OnTop(1);
                                            }

                                            long cntMeasOther = 0;
                                            measTask.SpectrumOccupationParameters = new SpectrumOccupationParameters();
                                            var recordsMeasOther = executor.ExecuteAndFetch(webQueryMeasOther, readerMeasOther =>
                                                {
                                                    cntMeasOther = readerMeasOther.Count;
                                                    while (readerMeasOther.Read())
                                                    {
                                                        Atdi.DataModels.Sdrns.Server.SpectrumOccupationType spectrumOccupationType;
                                                        if (Enum.TryParse<Atdi.DataModels.Sdrns.Server.SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out spectrumOccupationType))
                                                        {
                                                            measTask.SpectrumOccupationParameters.TypeSpectrumOccupation = spectrumOccupationType;
                                                        }
                                                        measTask.SpectrumOccupationParameters.NChenal = readerMeasOther.GetValue(c => c.Nchenal);
                                                        measTask.SpectrumOccupationParameters.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                                                        break;
                                                    }
                                                    return true;
                                                });

                                            if (cntMeasOther == 0)
                                            {
                                                continue;
                                            }



                                            var webQueryMeasFreqParam = dataLayer.GetBuilder<IMeasFreqParam>()
                                               .Read()
                                               .Select(
                                               c => c.Id, c => c.Mode, c => c.Rgl, c => c.Rgu, c => c.Step)
                                               .OrderByAsc(c => c.Id)
                                               .BeginFilter()
                                                   .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, taskId)
                                               .EndFilter()
                                               .OnTop(1);


                                            var recordsMeasFreqParam = executor.ExecuteAndFetch(webQueryMeasFreqParam, readerMeasFreqParam =>
                                                {
                                                    measTask.MeasFreqParam = new MeasFreqParam();
                                                    while (readerMeasFreqParam.Read())
                                                    {
                                                        measTask.MeasFreqParam.Step = readerMeasFreqParam.GetValue(c => c.Step);
                                                        measTask.MeasFreqParam.RgU = readerMeasFreqParam.GetValue(c => c.Rgu);
                                                        measTask.MeasFreqParam.RgL = readerMeasFreqParam.GetValue(c => c.Rgl);
                                                        FrequencyMode frequencyMode;
                                                        if (Enum.TryParse<FrequencyMode>(readerMeasFreqParam.GetValue(c => c.Mode), out frequencyMode))
                                                        {
                                                            measTask.MeasFreqParam.Mode = frequencyMode;
                                                        }
                                                        break;
                                                    }
                                                    return true;
                                                });


                                            var webQueryMeasDtParam = dataLayer.GetBuilder<IMeasDtParam>()
                                           .Read()
                                           .Select(
                                           c => c.Id, c => c.Mode, c => c.Demod, c => c.DetectType, c => c.Ifattenuation, c => c.MeasTime, c => c.NumberTotalScan, c => c.Preamplification, c => c.Rbw, c => c.ReferenceLevel, c => c.Rfattenuation, c => c.SwNumber, c => c.Vbw)
                                           .OrderByAsc(c => c.Id)
                                           .BeginFilter()
                                               .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, taskId)
                                           .EndFilter()
                                           .OnTop(1);


                                            var recordsMeasDtParam = executor.ExecuteAndFetch(webQueryMeasDtParam, readerMeasDtParam =>
                                            {
                                                measTask.MeasDtParam = new MeasDtParam();
                                                while (readerMeasDtParam.Read())
                                                {
                                                    measTask.MeasDtParam.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                                                    MeasurementMode measurementMode;
                                                    if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out measurementMode))
                                                    {
                                                        measTask.MeasDtParam.Mode = measurementMode;
                                                    }
                                                    DetectingType detectingType;
                                                    if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectingType))
                                                    {
                                                        measTask.MeasDtParam.DetectType = detectingType;
                                                    }
                                                    measTask.MeasDtParam.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation);
                                                    measTask.MeasDtParam.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                                                    measTask.MeasDtParam.NumberTotalScan = readerMeasDtParam.GetValue(c => c.NumberTotalScan);
                                                    measTask.MeasDtParam.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification);
                                                    measTask.MeasDtParam.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                                                    measTask.MeasDtParam.ReferenceLevel = readerMeasDtParam.GetValue(c => c.ReferenceLevel);
                                                    measTask.MeasDtParam.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation);
                                                    measTask.MeasDtParam.SwNumber = readerMeasDtParam.GetValue(c => c.SwNumber);
                                                    measTask.MeasDtParam.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                                                    break;
                                                }
                                                return true;
                                            });


                                            var lstMeasSensor = new List<MeasSensor>();
                                            var lstSensors = new List<Sensor>();
                                            var lstSensorIds = new List<long>();

                                            var webQuerySensor = dataLayer.GetBuilder<ISubTaskSensor>()
                                                .Read()
                                                .Select(
                                                c => c.Id, c => c.SUBTASK.MEAS_TASK.Id, c => c.SENSOR.Id, c => c.SENSOR.Name, c => c.SENSOR.TechId)
                                                .OrderByAsc(c => c.Id)
                                                .BeginFilter()
                                                .Condition(c => c.SUBTASK.MEAS_TASK.Id, FilterOperator.Equal, taskId)
                                                .EndFilter();
                                            var recordswebQuerySensor = executor.ExecuteAndFetch(webQuerySensor, readerSubTaskSensor =>
                                                {
                                                    while (readerSubTaskSensor.Read())
                                                    {
                                                        if (!lstSensorIds.Contains(readerSubTaskSensor.GetValue(c => c.SENSOR.Id)))
                                                        {
                                                            lstSensorIds.Add(readerSubTaskSensor.GetValue(c => c.SENSOR.Id));

                                                            var sensor = new Sensor();
                                                            sensor.Id = new SensorIdentifier();
                                                            sensor.Id.Value = readerSubTaskSensor.GetValue(c => c.SENSOR.Id);
                                                            sensor.Name = readerSubTaskSensor.GetValue(c => c.SENSOR.Name);
                                                            sensor.Equipment = new SensorEquip();
                                                            sensor.Equipment.TechId = readerSubTaskSensor.GetValue(c => c.SENSOR.TechId);

                                                            var sensorLocation = new SensorLocation();
                                                            var webQuerySensorLocation = dataLayer.GetBuilder<ISensorLocation>()
                                                            .Read()
                                                            .Select(
                                                            c => c.Id, c => c.Lon, c => c.Lat, c => c.Status, c => c.Asl, c => c.DateCreated)
                                                            .OrderByAsc(c => c.Id)
                                                            .BeginFilter()
                                                                .Condition(c => c.SENSOR.Id, FilterOperator.Equal, sensor.Id.Value)
                                                                .And()
                                                                .Condition(c => c.SENSOR.Status, FilterOperator.Equal, "A")
                                                            .EndFilter()
                                                            .OnTop(1);

                                                            var recordsSensorLocation = executor.ExecuteAndFetch(webQuerySensorLocation, readerSensorLocation =>
                                                            {
                                                                while (readerSensorLocation.Read())
                                                                {
                                                                    sensorLocation.ASL = readerSensorLocation.GetValue(c => c.Asl);
                                                                    sensorLocation.DataCreated = readerSensorLocation.GetValue(c => c.DateCreated);
                                                                    sensorLocation.Lon = readerSensorLocation.GetValue(c => c.Lon);
                                                                    sensorLocation.Lat = readerSensorLocation.GetValue(c => c.Lat);
                                                                    sensorLocation.Status = readerSensorLocation.GetValue(c => c.Status);
                                                                    break;
                                                                }
                                                                return true;
                                                            });

                                                            sensor.Locations = new SensorLocation[1] { sensorLocation };
                                                            lstSensors.Add(sensor);

                                                            if (lstAllSensors.Find(x => x.Id.Value == sensor.Id.Value) == null)
                                                            {
                                                                if (listSensors[i].Contains(sensor.Id.Value))
                                                                {
                                                                    lstAllSensors.Add(sensor);
                                                                }
                                                            }
                                                            if (listSensors[i].Contains(sensor.Id.Value))
                                                            {

                                                                for (int j = 0; j < lstSensors.Count; j++)
                                                                {
                                                                    var measSensor = new MeasSensor()
                                                                    {
                                                                        SensorName = lstSensors[j].Name,
                                                                        SensorId = new MeasSensorIdentifier()
                                                                        {
                                                                            Value = lstSensors[j].Id.Value
                                                                        },
                                                                        SensorTechId = lstSensors[j].Equipment.TechId,
                                                                    };
                                                                    lstMeasSensor.Add(measSensor);
                                                                }
                                                            }
                                                        }
                                                    };
                                                    return true;
                                                });

                                            if (lstMeasSensor.Count > 0)
                                            {
                                                measTask.Sensors = lstMeasSensor.ToArray();
                                                lstMeasTask.Add(measTask);
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        var tskFind = lstMeasTask.Find(x => x.Id.Value == taskId);
                                        if (tskFind != null)
                                        {
                                            if ((tskFind.SpectrumOccupationParameters != null) && (tskFind.MeasFreqParam != null))
                                            {
                                                for (int j = 0; j < tskFind.Sensors.Length; j++)
                                                {
                                                    double lon = -1;
                                                    double lat = -1;
                                                    string sensorName = "";
                                                    var sensorFnd = lstAllSensors.Find(x => x.Id.Value == tskFind.Sensors[j].SensorId.Value);
                                                    if ((sensorFnd != null) && (sensorFnd.Locations != null) && (sensorFnd.Locations.Length > 0))
                                                    {
                                                        if (sensorFnd.Locations[0].Lon != null)
                                                        {
                                                            lon = sensorFnd.Locations[0].Lon.Value;
                                                        }
                                                        if (sensorFnd.Locations[0].Lat != null)
                                                        {
                                                            lat = sensorFnd.Locations[0].Lat.Value;
                                                        }
                                                        sensorName = sensorFnd.Name;
                                                    }
                                                    if ((countRecInOneFile == 0) || (countRecInOneFile == countMaxRecordInCsvFile))
                                                    {
                                                        currFileName = ExportDirectoryName + $"\\SpectrumOccupation_{Guid.NewGuid().ToString()}_{DateTime.Now.ToString("dd_MM_yyyy")}_{indexNumFile}.csv";
                                                        indexNumFile++;
                                                        countRecInOneFile = 0;
                                                        System.IO.File.AppendAllLines(currFileName, new string[] {"Id;"+
                                                                                            "Частота, МГц;" +
                                                                                            "Занятість спектру %;" +
                                                                                            "Рівень Мінімальний, дБм;" +
                                                                                            "Рівень Середній, дБм;" +
                                                                                            "Рівень Максимальний, дБм;" +
                                                                                            "Дата (число);" +
                                                                                            "Дата (години);" +
                                                                                            "Кількість вимірів;" +
                                                                                            "Ширина каналу, кГц;" +
                                                                                            "Тип ЗС;" +
                                                                                            "Мінімальний рівень ЗС, дБм;" +
                                                                                            "ID завдання;" +
                                                                                            "Назва сенсора;" +
                                                                                            "Довгота сенсора, град;" +
                                                                                            "Широта сенсора, град" }, System.Text.Encoding.UTF8);
                                                    }

                                                    System.IO.File.AppendAllLines(currFileName, new string[] {
                                                       $"{readerResLevels.GetValue(c => c.Id)};" +
                                                       $"{readerResLevels.GetValue(c => c.FreqMeas)};" +
                                                       $"{readerResLevels.GetValue(c => c.OccupancySpect)};" +
                                                       $"{readerResLevels.GetValue(c => c.VMinLvl)};" +
                                                       $"{readerResLevels.GetValue(c => c.ValueLvl)};" +
                                                       $"{ readerResLevels.GetValue(c => c.VMMaxLvl)};" +
                                                       $"{readerResLevels.GetValue(c => c.RES_MEAS.TimeMeas).Value.ToString("dd.MM.yyyy")};" +
                                                       $"{readerResLevels.GetValue(c => c.RES_MEAS.TimeMeas).Value.ToString("MM:HH:ss")};" +
                                                       $"{readerResLevels.GetValue(c => c.RES_MEAS.ScansNumber)};{tskFind.MeasFreqParam.Step};" +
                                                       $"{tskFind.SpectrumOccupationParameters.TypeSpectrumOccupation.ToString()};" +
                                                       $"{tskFind.SpectrumOccupationParameters.LevelMinOccup};" +
                                                       $"{taskId};" +
                                                       $"{sensorName};" +
                                                       $"{XICSM.ICSControlClient.ViewModels.Reports.ConvertCoordinates.DecToDmsToString2(lon, Coordinates.EnumCoordLine.Lon)};" +
                                                       $"{XICSM.ICSControlClient.ViewModels.Reports.ConvertCoordinates.DecToDmsToString2(lat, Coordinates.EnumCoordLine.Lat)}" }, System.Text.Encoding.UTF8);

                                                    countRecInOneFile++;
                                                    countAllRec++;
                                                    isSuccess = true;
                                                    if (threshHoldExportSO == countAllRec)
                                                    {
                                                        isThreshHoldExportSO = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (isThreshHoldExportSO)
                                        {
                                            break;
                                        }
                                    }
                                }
                                return true;
                            });

                            var subValue = threshHoldExportSO - countAllRec;
                            if (subValue<= countMaxRecInPage)
                            {
                                countMaxRecInPage = subValue;
                            }
                            offsetResLevels += countResLevels;
                        }
                        while ((countMaxRecInPage>0) && (isThreshHoldExportSO == false));
                        if (isThreshHoldExportSO==true)
                        {
                            break;
                        }
                    }
                }).ContinueWith(task =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (_waitForm != null)
                        {
                            _waitForm.Close();
                            _waitForm = null;
                        }
                    }));
                });
                _waitForm.ShowDialog();
            }
            catch (EntityOrmWebApiException e)
            {
                isSuccess = false;
                MessageBox.Show(e.ToString());
            }
            catch (Exception e)
            {
                isSuccess = false;
                MessageBox.Show(e.ToString());
            }
            finally
            {
                if (_waitForm != null)
                {
                    _waitForm.Close();
                    _waitForm = null;
                }
            }
            if (isSuccess)
            {
                MessageBox.Show("Your file(s) was generated and its ready for use.");
            }
        }
    }
}
