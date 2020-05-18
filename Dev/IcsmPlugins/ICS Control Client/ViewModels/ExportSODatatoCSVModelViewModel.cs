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

        private const string TypeMeasurements = "SpectrumOccupation";

        private IList _currentSensors;
        private long[] _currentSensorsIndexes;
        private MP.MapDrawingData _currentMapData;
        private DateTime? _dateStartExport;
        private DateTime? _dateStopExport;
        private float? _freqStartExport;
        private float? _freqStopExport;
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


        public float? FreqStartExport
        {
            get => this._freqStartExport;
            set => this.Set(ref this._freqStartExport, value);
        }
        public float? FreqStopExport
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
            MessageBox.Show("Export results can be large. If necessary, the export will take place in several files/tables!", PluginHelper.MessageBoxCaption, MessageBoxButton.OK);
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

                var sensorIds = new long[CurrentSensors.Count];

                if (CurrentSensors != null)
                {
                    sensorIds = ((IList)CurrentSensors).Cast<ShortSensorViewModel>().Select(o => o.Id).ToArray();
                }
                long recordCount = 0;
                var endpoint = new WebApiEndpoint(new Uri(PluginHelper.GetWebAPIBaseAddress()), PluginHelper.GetWebAPIUrl());
                var dataContext = new WebApiDataContext(PluginHelper.GetDataContext());
                var dataLayer = new WebApiDataLayer(endpoint, dataContext);
                var listSensors = BreakDownElemBlocks.BreakDown(sensorIds);
                for (int i = 0; i < listSensors.Count; i++)
                {
                    var webQueryXvResLevels = dataLayer.GetBuilder<IXvResLevels>().Read()
                        .Select(c => c.Id).OrderByAsc(c => c.Id);
                    var filter = webQueryXvResLevels.BeginFilter()
                       .Condition(c => c.TimeMeas, FilterOperator.LessEqual, DateStopExport.Value)
                           .And()
                       .Condition(c => c.TimeMeas, FilterOperator.GreaterEqual, DateStartExport.Value)
                           .And()
                       .Condition(c => c.TypeMeasurements, FilterOperator.Equal, TypeMeasurements)
                           .And()
                       .Condition(c => c.SensorId, FilterOperator.In, listSensors[i].ToArray())
                           .And()
                       .Condition(c => c.FreqMeas, FilterOperator.Between, FreqStartExport.Value, FreqStopExport.Value);
                        if (SpectrumOccupationTypeVal != SpectrumOccupationType.All)
                        {
                         filter
                        .And()
                        .Condition(c => c.TypeSpectrumOccupation, FilterOperator.Equal, SpectrumOccupationTypeVal.ToString());
                        }
                    filter.EndFilter();
                    recordCount+= dataLayer.Executor.Execute(webQueryXvResLevels);
                }

                if (recordCount > 100000)
                {
                    if (MessageBox.Show($"The result of this query in the database will be a set of data with the number of records {recordCount}. Start exporting data?", PluginHelper.MessageBoxCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
                        if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
                        {
                            if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                            {
                                ExportDirectoryName = folderDialog.SelectedPath;
                                ReadData(recordCount);
                            }
                        }
                    }
                }
                else if (recordCount > 0)
                {
                    FRM.FolderBrowserDialog folderDialog = new FRM.FolderBrowserDialog();
                    if (folderDialog.ShowDialog() == FRM.DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(folderDialog.SelectedPath))
                        {
                            ExportDirectoryName = folderDialog.SelectedPath;
                            ReadData(recordCount);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"The result of this query in the database is 0 records!", PluginHelper.MessageBoxCaption, MessageBoxButton.OK);
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


        private void ReadData(long recordCount)
        {
            bool isSuccess = false;

            var isThreshHoldExportSO = false;
            var countMaxRecInPage = PluginHelper.GetMaxCountRecordInPage();
            var countMaxRecordInCsvFile = PluginHelper.GetMaxCountRecordInCsvFile();

            var endpoint = new WebApiEndpoint(new Uri(PluginHelper.GetWebAPIBaseAddress()), PluginHelper.GetWebAPIUrl());
            var dataContext = new WebApiDataContext(PluginHelper.GetDataContext());
            var dataLayer = new WebApiDataLayer(endpoint, dataContext);


            string currFileName = string.Empty;
            var countAllRec = 0;
            var countRecInOneFile = 0;
            var indexNumFile = 0;
            var sensorIds = new long[CurrentSensors.Count];


            if (CurrentSensors != null)
            {
                sensorIds = ((IList)CurrentSensors).Cast<ShortSensorViewModel>().Select(o => o.Id).ToArray();
            }

            _waitForm = new FM.WaitForm();
            _waitForm.SetMessage($"Please wait...");
            _waitForm.TopMost = true;

            Task.Run(() =>
            {
                try
                {
                    var listSensors = BreakDownElemBlocks.BreakDown(sensorIds);
                    for (int i = 0; i < listSensors.Count; i++)
                    {
                        long countResLevels = 0;
                        long offsetResLevels = 0;
                        do
                        {
                            if ((recordCount - countMaxRecInPage) < 0)
                            {
                                countMaxRecInPage = recordCount;
                            }
                            var webQueryResLevels = dataLayer.GetBuilder<IXvResLevels>().Read()
                                  .Select(c => c.Id,
                                          c => c.FreqMeas,
                                          c => c.Latitude,
                                          c => c.LevelMinOccup,
                                          c => c.Longitude,
                                          c => c.OccupancySpect,
                                          c => c.ScansNumber,
                                          c => c.SensorId,
                                          c => c.SensorName,
                                          c => c.Step,
                                          c => c.TaskId,
                                          c => c.TimeMeas,
                                          c => c.TypeMeasurements,
                                          c => c.TypeSpectrumOccupation,
                                          c => c.ValueLvl,
                                          c => c.VMinLvl,
                                          c => c.VMMaxLvl)
                                  .OrderByAsc(c => c.Id)
                                  .Paginate(offsetResLevels, countMaxRecInPage);
                            var filter = webQueryResLevels.BeginFilter()
                                .Condition(c => c.TimeMeas, FilterOperator.LessEqual, DateStopExport.Value)
                                    .And()
                                .Condition(c => c.TimeMeas, FilterOperator.GreaterEqual, DateStartExport.Value)
                                    .And()
                                .Condition(c => c.TypeMeasurements, FilterOperator.Equal, TypeMeasurements)
                                    .And()
                                .Condition(c => c.SensorId, FilterOperator.In, listSensors[i].ToArray())
                                    .And()
                                .Condition(c => c.FreqMeas, FilterOperator.Between, FreqStartExport.Value, FreqStopExport.Value);
                            if (SpectrumOccupationTypeVal != SpectrumOccupationType.All)
                            {
                                filter
                               .And()
                               .Condition(c => c.TypeSpectrumOccupation, FilterOperator.Equal, SpectrumOccupationTypeVal.ToString());
                            }
                            filter.EndFilter();


                            var recordsResLevels = dataLayer.Executor.ExecuteAndFetch(webQueryResLevels, readerResLevels =>
                            {
                                countResLevels = readerResLevels.Count;
                                if (countResLevels > 0)
                                {
                                    while (readerResLevels.Read())
                                    {
                                        var taskId = readerResLevels.GetValue(c => c.TaskId);
                                        var sensorId = readerResLevels.GetValue(c => c.SensorId);
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
                                                       $"{readerResLevels.GetValue(c => c.TimeMeas).Value.ToString("dd.MM.yyyy")};" +
                                                       $"{readerResLevels.GetValue(c => c.TimeMeas).Value.ToString("MM:HH:ss")};" +
                                                       $"{readerResLevels.GetValue(c => c.ScansNumber)};" +
                                                       $"{readerResLevels.GetValue(c => c.Step)};" +
                                                       $"{readerResLevels.GetValue(c => c.TypeSpectrumOccupation)};" +
                                                       $"{readerResLevels.GetValue(c => c.LevelMinOccup)};" +
                                                       $"{readerResLevels.GetValue(c => c.TaskId)};" +
                                                       $"{readerResLevels.GetValue(c => c.SensorName)};" +
                                                       $"{XICSM.ICSControlClient.ViewModels.Reports.ConvertCoordinates.DecToDmsToString2(readerResLevels.GetValue(c => c.Longitude).Value, Coordinates.EnumCoordLine.Lon)};" +
                                                       $"{XICSM.ICSControlClient.ViewModels.Reports.ConvertCoordinates.DecToDmsToString2(readerResLevels.GetValue(c => c.Latitude).Value, Coordinates.EnumCoordLine.Lat)}" }, System.Text.Encoding.UTF8);

                                        countRecInOneFile++;
                                        countAllRec++;
                                        isSuccess = true;
                                        if (recordCount == countAllRec)
                                        {
                                            isThreshHoldExportSO = true;
                                            break;
                                        }
                                    }
                                }
                                return true;
                            });

                            var subValue = recordCount - countAllRec;
                            if (subValue <= countMaxRecInPage)
                            {
                                countMaxRecInPage = subValue;
                            }
                            offsetResLevels += countResLevels;
                        }
                        while ((countMaxRecInPage > 0) && (countResLevels > 0) && (isThreshHoldExportSO == false));
                        if (isThreshHoldExportSO == true)
                        {
                            break;
                        }
                    }
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

            if (isSuccess)
            {
                MessageBox.Show("Your file(s) was generated and its ready for use.");
            }
            else
            {
                MessageBox.Show("Errors occurred during export!");
            }
        }
    }
}
