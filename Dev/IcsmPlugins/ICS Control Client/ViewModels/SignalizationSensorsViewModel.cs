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
using System.Collections;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace XICSM.ICSControlClient.ViewModels
{
    class Emitt
    {
        public long Id;
        public double StartFrequency_MHz;
        public double StopFrequency_MHz;
        public EmittWorkTimes[] WorkTimes;
        public EmittingViewModel Emitting;
    }
    class EmittWorkTimes
    {
        public long Id;
        public DateTime StartEmitt;
        public DateTime StopEmitt;
    }
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
        private int _startType;
        private EmittingViewModel[] _emittings;
        private FRM.Form _form;
        private string _endpointUrls;
        private List<long> _sensorIds;
        private DateTime? _timeMeas;

        public WpfCommand StartCommand { get; set; }

        public SignalizationSensorsViewModel(int startType, FRM.Form form, EmittingViewModel[] emittings, DateTime? timeMeas)
        {
            this._startType = startType;
            this._form = form;
            this._emittings = emittings;
            this._timeMeas = timeMeas;
            this._shortSensors = new ShortSensorDataAdatper();
            this.StartCommand = new WpfCommand(this.OnStartCommand);

            _endpointUrls = PluginHelper.GetRestApiEndPoint();

            if (string.IsNullOrEmpty(_endpointUrls))
                return;

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

        public int StartType
        {
            get => this._startType;
            set => this.Set(ref this._startType, value);
        }

        private bool _isUseTime = false;
        public bool IsUseTime
        {
            get => this._isUseTime;
            set => this.Set(ref this._isUseTime, value);
        }

        private double _timeIntersection = 10;
        public double TimeIntersection
        {
            get => this._timeIntersection;
            set => this.Set(ref this._timeIntersection, value);
        }

        private double _freqIntersection = 50;
        public double FreqIntersection
        {
            get => this._freqIntersection;
            set => this.Set(ref this._freqIntersection, value);
        }

        private DateTime _dateFrom = DateTime.Today.AddMonths(-1);
        public DateTime DateFrom
        {
            get => this._dateFrom;
            set => this.Set(ref this._dateFrom, value);
        }

        private DateTime _timeFrom = DateTime.Today;
        public DateTime TimeFrom
        {
            get => this._timeFrom;
            set => this.Set(ref this._timeFrom, value);
        }

        private DateTime _dateTo = DateTime.Today;
        public DateTime DateTo
        {
            get => this._dateTo;
            set => this.Set(ref this._dateTo, value);
        }

        private DateTime _timeTo = DateTime.Today.AddHours(23).AddMinutes(59);
        public DateTime TimeTo
        {
            get => this._timeTo;
            set => this.Set(ref this._timeTo, value);
        }
        private DateTime _dateStart;
        private DateTime _dateStop;

        private double? _freqFrom;
        public double? FreqFrom
        {
            get => this._freqFrom;
            set => this.Set(ref this._freqFrom, value);
        }

        private double? _freqTo;
        public double? FreqTo
        {
            get => this._freqTo;
            set => this.Set(ref this._freqTo, value);
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
                                .Select(l => MapsDrawingHelper.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
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
                _sensorIds = new List<long>();
                if (this._currentShortSensor != null)
                {
                    foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                        _sensorIds.Add(shortSensor.Id);
                }

                if (!ValidateData())
                    return;

                SDR.Emitting[] emittings = null;

                if (this._startType == 1)
                    emittings = GetEmittingsForType1();
                else if (this._startType == 2)
                    emittings = GetEmittingsForType2();

                if (emittings == null)
                    return;

                _form.Close();
                var dlgForm = new FM.MeasResultSignalizationForm(0, this._startType, emittings, this._timeMeas);
                dlgForm.ShowDialog();
                dlgForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private SDR.Emitting[] GetEmittingsForType1()
        {
            var tempEmittings = new List<Emitt>();

            if (!this._timeMeas.HasValue && this._emittings.Length != 1)
            {
                MessageBox.Show("Please select one emission!");
                return null;
            }

            if (!this._timeMeas.HasValue)
            {
                using (var wc = new HttpClient())
                {
                    string filter = $"(Id in ({this._emittings[0].Id}))";
                    string fields = "RES_MEAS.TimeMeas";
                    string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Emitting?select={fields}&filter={filter}";
                    var response = wc.GetAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var dicFields = new Dictionary<string, int>();
                        var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                        foreach (var field in data.Fields)
                            dicFields[field.Path] = field.Index;

                        foreach (object[] record in data.Records)
                        {
                            this._timeMeas = (DateTime)record[dicFields["RES_MEAS.TimeMeas"]];
                        }
                    }
                }
            }
            if (!this._timeMeas.HasValue)
                return null;

            using (var wc = new HttpClient())
            {
                var minFreq = _emittings.Length == 0 ? 0 : _emittings.Min(d => d.StartFrequency_MHz);
                var maxFreq = _emittings.Length == 0 ? 0 : _emittings.Max(d => d.StopFrequency_MHz);
                long emittingId = 0;
                string filter = $"(EMITTING.StartFrequency_MHz le {maxFreq.ToString().Replace(",", ".")})and(EMITTING.StopFrequency_MHz ge {minFreq.ToString().Replace(",", ".")})and(EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id in ({string.Join(",", _sensorIds)}))and(EMITTING.RES_MEAS.TimeMeas Ge {_timeMeas.Value.Date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")})and(EMITTING.RES_MEAS.TimeMeas Le {_timeMeas.Value.Date.AddDays(1).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")})";
                string fields = "Id,StartEmitting,StopEmitting,EMITTING.Id,EMITTING.StartFrequency_MHz,EMITTING.StopFrequency_MHz,EMITTING.RES_MEAS.TimeMeas";
                string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/WorkTime?select={fields}&filter={filter}&orderBy=EMITTING.Id";
                var response = wc.GetAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    var emitt = new Emitt();
                    var emittWorkTimes = new List<EmittWorkTimes>();
                    bool isOkEmitt = false;

                    foreach (object[] record in data.Records)
                    {
                        var newEmmittingId = Convert.ToInt64(record[dicFields["EMITTING.Id"]]);
                        if (emittingId != newEmmittingId)
                        {
                            emittingId = newEmmittingId;
                            isOkEmitt = false;
                            var startFrequency_MHz = (double)record[dicFields["EMITTING.StartFrequency_MHz"]];
                            var stopFrequency_MHz = (double)record[dicFields["EMITTING.StopFrequency_MHz"]];

                            var resultDate = (DateTime)record[dicFields["EMITTING.RES_MEAS.TimeMeas"]];

                            if (_timeMeas.Value.Date != resultDate.Date)
                                continue;

                            foreach (var emitting in _emittings)
                            {
                                if (100 * (Math.Min(stopFrequency_MHz, emitting.StopFrequency_MHz) - Math.Max(startFrequency_MHz, emitting.StartFrequency_MHz)) / (Math.Max(emitting.StopFrequency_MHz - emitting.StartFrequency_MHz, stopFrequency_MHz - startFrequency_MHz)) >= _freqIntersection)
                                {
                                    emitt = new Emitt() { Id = emittingId, StartFrequency_MHz = startFrequency_MHz, StopFrequency_MHz = stopFrequency_MHz, Emitting = emitting };
                                    emittWorkTimes = new List<EmittWorkTimes>();
                                    tempEmittings.Add(emitt);
                                    isOkEmitt = true;
                                }

                                if (isOkEmitt)
                                {
                                    var emittWorkTime = new EmittWorkTimes()
                                    {
                                        Id = Convert.ToInt64(record[dicFields["Id"]]),
                                        StartEmitt = (DateTime)record[dicFields["StartEmitting"]],
                                        StopEmitt = (DateTime)record[dicFields["StopEmitting"]]
                                    };

                                    emittWorkTimes.Add(emittWorkTime);
                                    emitt.WorkTimes = emittWorkTimes.ToArray();
                                }
                            }
                        }
                    }
                }
            }

            var ids = new Dictionary<long, long>();
            if (IsUseTime)
            {
                foreach (var emitt in tempEmittings)
                    foreach (var workTime in emitt.WorkTimes)
                        foreach (var wt in emitt.Emitting.WorkTimes)
                            if (100 * (DateMin(wt.StopEmitting, workTime.StopEmitt) - DateMax(wt.StartEmitting, workTime.StartEmitt)).TotalSeconds / (Math.Max((workTime.StopEmitt - workTime.StartEmitt).TotalSeconds, (wt.StopEmitting - wt.StartEmitting).TotalSeconds)) >= _timeIntersection)
                                if (!ids.ContainsKey(emitt.Id))
                                    ids.Add(emitt.Id, emitt.Id);
            }
            else
            {
                foreach (var emitt in tempEmittings)
                    foreach (var time in emitt.WorkTimes)
                        if (!ids.ContainsKey(emitt.Id))
                            ids.Add(emitt.Id, emitt.Id);
            }
            
            return GetEmittingsByEmittingIds(ids.Keys.ToArray());
        }
        private SDR.Emitting[] GetEmittingsForType2()
        {
            try
            {
                var emittings = new List<SDR.Emitting>();
                //using (var wc = new HttpClient())
                //{
                //    long emittingId = 0;
                //    string filter = $"(EMITTING.StartFrequency_MHz le {_freqTo.ToString().Replace(",",".")})and(EMITTING.StopFrequency_MHz ge {_freqFrom.ToString().Replace(",", ".")})and(StartEmitting le {_dateStop.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")})and(StopEmitting ge {_dateStart.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")})and(EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id in ({string.Join(",", _sensorIds)}))";
                //    string fields = "StartEmitting,StopEmitting,HitCount,PersentAvailability,EMITTING.Id,EMITTING.StartFrequency_MHz,EMITTING.StopFrequency_MHz,EMITTING.CurentPower_dBm,EMITTING.ReferenceLevel_dBm,EMITTING.MeanDeviationFromReference,EMITTING.TriggerDeviationFromReference,EMITTING.RollOffFactor,EMITTING.StandardBW,EMITTING.StationID,EMITTING.StationTableName,EMITTING.LevelsDistributionCount,EMITTING.LevelsDistributionLvl,EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId,EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name";
                //    string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/WorkTime?select={fields}&filter={filter}&orderBy=EMITTING.Id";
                //    var response = wc.GetAsync(request).Result;
                //    if (response.StatusCode == HttpStatusCode.OK)
                //    {
                //        var dicFields = new Dictionary<string, int>();
                //        var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                //        foreach (var field in data.Fields)
                //            dicFields[field.Path] = field.Index;

                //        var emitting = new SDR.Emitting();
                //        var workTimes = new List<SDR.WorkTime>();

                //        foreach (object[] record in data.Records)
                //        {
                //            var newEmmittingId = Convert.ToInt64(record[dicFields["EMITTING.Id"]]);
                //            if (emittingId != newEmmittingId)
                //            {
                //                emittingId = newEmmittingId;
                //                emitting = new SDR.Emitting()
                //                {
                //                    Id = newEmmittingId,
                //                    StartFrequency_MHz = (double)record[dicFields["EMITTING.StartFrequency_MHz"]],
                //                    StopFrequency_MHz = (double)record[dicFields["EMITTING.StopFrequency_MHz"]],
                //                    CurentPower_dBm = (double)record[dicFields["EMITTING.CurentPower_dBm"]],
                //                    ReferenceLevel_dBm = (double)record[dicFields["EMITTING.ReferenceLevel_dBm"]],
                //                    MeanDeviationFromReference = (double)record[dicFields["EMITTING.MeanDeviationFromReference"]],
                //                    TriggerDeviationFromReference = (double)record[dicFields["EMITTING.TriggerDeviationFromReference"]],
                //                    EmittingParameters = new SDR.EmittingParameters()
                //                    {
                //                        RollOffFactor = (double)record[dicFields["EMITTING.RollOffFactor"]],
                //                        StandardBW = (double)record[dicFields["EMITTING.StandardBW"]]
                //                    },
                //                    AssociatedStationID = Convert.ToInt64(record[dicFields["EMITTING.StationID"]]),
                //                    AssociatedStationTableName = (string)record[dicFields["EMITTING.StationTableName"]],
                //                    SensorName = (string)record[dicFields["EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name"]],
                //                    SensorTechId = (string)record[dicFields["EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId"]]
                //                };

                //                workTimes = new List<SDR.WorkTime>();

                //                var levelDistr = new SDR.LevelsDistribution();
                //                if (record[dicFields["EMITTING.LevelsDistributionCount"]] != null)
                //                {
                //                    var items = (record[dicFields["EMITTING.LevelsDistributionCount"]] as JArray).Select(jv => (int)jv).ToArray();
                //                    levelDistr.Count = items;
                //                }
                //                if (record[dicFields["EMITTING.LevelsDistributionLvl"]] != null)
                //                {
                //                    var items = (record[dicFields["EMITTING.LevelsDistributionLvl"]] as JArray).Select(jv => (int)jv).ToArray();
                //                    levelDistr.Levels = items;
                //                }
                //                emitting.LevelsDistribution = levelDistr;
                //                emitting.Spectrum = GetSpectrumByEmittingId(emittingId);
                //                emittings.Add(emitting);
                //            }

                //            workTimes.Add(new SDR.WorkTime()
                //            {
                //                StartEmitting = (DateTime)record[dicFields["StartEmitting"]],
                //                StopEmitting = (DateTime)record[dicFields["StopEmitting"]],
                //                HitCount = Convert.ToInt32(record[dicFields["HitCount"]]),
                //                PersentAvailability = (float)Convert.ChangeType(record[dicFields["HitCount"]], typeof(float)) // (float)record[dicFields["PersentAvailability"]]
                //            });
                //            emitting.WorkTimes = workTimes.ToArray();
                //        }
                //    }
                //}

                var emittingsIds = new Dictionary<long, List<SDR.WorkTime>>();
                using (var wc = new HttpClient())
                {
                    string filter = $"(EMITTING.StartFrequency_MHz le {_freqTo.ToString().Replace(",",".")})and(EMITTING.StopFrequency_MHz ge {_freqFrom.ToString().Replace(",", ".")})and(StartEmitting le {_dateStop.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")})and(StopEmitting ge {_dateStart.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")})and(EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id in ({string.Join(",", _sensorIds)}))";
                    string fields = "EMITTING.Id,StartEmitting,StopEmitting,HitCount,PersentAvailability";
                    string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/WorkTime?select={fields}&filter={filter}";
                    var response = wc.GetAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var dicFields = new Dictionary<string, int>();
                        var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                        foreach (var field in data.Fields)
                            dicFields[field.Path] = field.Index;

                        var emitting = new SDR.Emitting();
                        var workTimes = new List<SDR.WorkTime>();

                        foreach (object[] record in data.Records)
                        {
                            var emmittingId = Convert.ToInt64(record[dicFields["EMITTING.Id"]]);

                            if (!emittingsIds.ContainsKey(emmittingId))
                                emittingsIds.Add(emmittingId, new List<SDR.WorkTime>());

                            emittingsIds[emmittingId].Add(new SDR.WorkTime()
                            {
                                StartEmitting = (DateTime)record[dicFields["StartEmitting"]],
                                StopEmitting = (DateTime)record[dicFields["StopEmitting"]],
                                HitCount = Convert.ToInt32(record[dicFields["HitCount"]]),
                                PersentAvailability = (float)Convert.ChangeType(record[dicFields["PersentAvailability"]], typeof(float))
                            });
                        }
                    }
                }

                if (emittingsIds.Count > 5000 && emittingsIds.Count < 20000)
                {
                    if (MessageBox.Show("Attention! Received a large amount of radiation, it will take a long time to get the result. Are you sure you want to continue?", PluginHelper.MessageBoxCaption, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        return null;
                }

                if (emittingsIds.Count > 20000)
                {
                    MessageBox.Show("Attention! A large number of emissions have been received, please reduce the amount of data obtaining.");
                    return null;
                }


                int counter = 0;
                var b = emittingsIds.Keys.GroupBy(_ => counter++ / 500).Select(v => v.ToArray());

                foreach (var ids in b.Select(v => String.Join(",", v)))
                {
                    using (var wc = new HttpClient())
                    {
                        string filter = $"(Id in ({ids}))";
                        string fields = "Id,StartFrequency_MHz,StopFrequency_MHz,CurentPower_dBm,ReferenceLevel_dBm,MeanDeviationFromReference,TriggerDeviationFromReference,RollOffFactor,StandardBW,StationID,StationTableName,LevelsDistributionCount,LevelsDistributionLvl,RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId,RES_MEAS.SUBTASK_SENSOR.SENSOR.Name,RES_MEAS.SUBTASK_SENSOR.SENSOR.Title";
                        string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Emitting?select={fields}&filter={filter}";
                        var response = wc.GetAsync(request).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var dicFields = new Dictionary<string, int>();
                            var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                            foreach (var field in data.Fields)
                                dicFields[field.Path] = field.Index;

                            foreach (object[] record in data.Records)
                            {
                                var emitting = new SDR.Emitting()
                                {
                                    Id = Convert.ToInt64(record[dicFields["Id"]]),
                                    StartFrequency_MHz = (double)record[dicFields["StartFrequency_MHz"]],
                                    StopFrequency_MHz = (double)record[dicFields["StopFrequency_MHz"]],
                                    CurentPower_dBm = (double)record[dicFields["CurentPower_dBm"]],
                                    ReferenceLevel_dBm = (double)record[dicFields["ReferenceLevel_dBm"]],
                                    MeanDeviationFromReference = (double)record[dicFields["MeanDeviationFromReference"]],
                                    TriggerDeviationFromReference = (double)record[dicFields["TriggerDeviationFromReference"]],
                                    EmittingParameters = new SDR.EmittingParameters()
                                    {
                                        RollOffFactor = (double)record[dicFields["RollOffFactor"]],
                                        StandardBW = (double)record[dicFields["StandardBW"]]
                                    },
                                    AssociatedStationID = Convert.ToInt64(record[dicFields["StationID"]]),
                                    AssociatedStationTableName = (string)record[dicFields["StationTableName"]],
                                    SensorName = (string)record[dicFields["RES_MEAS.SUBTASK_SENSOR.SENSOR.Name"]],
                                    SensorTitle = (string)record[dicFields["RES_MEAS.SUBTASK_SENSOR.SENSOR.Title"]],
                                    SensorTechId = (string)record[dicFields["RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId"]]
                                };

                                var levelDistr = new SDR.LevelsDistribution();
                                if (record[dicFields["LevelsDistributionCount"]] != null)
                                {
                                    var items = (record[dicFields["LevelsDistributionCount"]] as JArray).Select(jv => (int)jv).ToArray();
                                    levelDistr.Count = items;
                                }
                                if (record[dicFields["LevelsDistributionLvl"]] != null)
                                {
                                    var items = (record[dicFields["LevelsDistributionLvl"]] as JArray).Select(jv => (int)jv).ToArray();
                                    levelDistr.Levels = items;
                                }
                                emitting.LevelsDistribution = levelDistr;
                                emitting.Spectrum = GetSpectrumByEmittingId(emitting.Id.Value);
                                emitting.WorkTimes = emittingsIds[emitting.Id.Value].ToArray();
                                emittings.Add(emitting);
                            }
                        }
                    }
                }

                return emittings.ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        private SDR.Emitting[] GetEmittingsByEmittingIds(long[] ids)
        {
            try
            {
                var emittings = new List<SDR.Emitting>();
                using (var wc = new HttpClient())
                {
                    long emittingId = 0;
                    string filter = $"(EMITTING.Id in ({string.Join(",", ids)}))";
                    string fields = "StartEmitting,StopEmitting,HitCount,PersentAvailability,EMITTING.Id,EMITTING.StartFrequency_MHz,EMITTING.StopFrequency_MHz,EMITTING.CurentPower_dBm,EMITTING.ReferenceLevel_dBm,EMITTING.MeanDeviationFromReference,EMITTING.TriggerDeviationFromReference,EMITTING.RollOffFactor,EMITTING.StandardBW,EMITTING.StationID,EMITTING.StationTableName,EMITTING.LevelsDistributionCount,EMITTING.LevelsDistributionLvl,EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId,EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name,EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Title,EMITTING.RES_MEAS.Id";
                    string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/WorkTime?select={fields}&filter={filter}&orderBy=EMITTING.Id";
                    var response = wc.GetAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var dicFields = new Dictionary<string, int>();
                        var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                        foreach (var field in data.Fields)
                            dicFields[field.Path] = field.Index;

                        var emitting = new SDR.Emitting();
                        var workTimes = new List<SDR.WorkTime>();

                        foreach (object[] record in data.Records)
                        {
                            var newEmmittingId = Convert.ToInt64(record[dicFields["EMITTING.Id"]]);
                            if (emittingId != newEmmittingId)
                            {
                                emittingId = newEmmittingId;
                                emitting = new SDR.Emitting()
                                {
                                    Id = newEmmittingId,
                                    StartFrequency_MHz = (double)record[dicFields["EMITTING.StartFrequency_MHz"]],
                                    StopFrequency_MHz = (double)record[dicFields["EMITTING.StopFrequency_MHz"]],
                                    CurentPower_dBm = (double)record[dicFields["EMITTING.CurentPower_dBm"]],
                                    ReferenceLevel_dBm = (double)record[dicFields["EMITTING.ReferenceLevel_dBm"]],
                                    MeanDeviationFromReference = (double)record[dicFields["EMITTING.MeanDeviationFromReference"]],
                                    TriggerDeviationFromReference = (double)record[dicFields["EMITTING.TriggerDeviationFromReference"]],
                                    EmittingParameters = new SDR.EmittingParameters()
                                    {
                                        RollOffFactor = (double)record[dicFields["EMITTING.RollOffFactor"]],
                                        StandardBW = (double)record[dicFields["EMITTING.StandardBW"]]
                                    },
                                    AssociatedStationID = Convert.ToInt64(record[dicFields["EMITTING.StationID"]]),
                                    AssociatedStationTableName = (string)record[dicFields["EMITTING.StationTableName"]],
                                    SensorName = (string)record[dicFields["EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name"]],
                                    SensorTitle = (string)record[dicFields["EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.Title"]],
                                    SensorTechId = (string)record[dicFields["EMITTING.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId"]],
                                    MeasResultId = Convert.ToInt64(record[dicFields["EMITTING.RES_MEAS.Id"]])
                                };

                                workTimes = new List<SDR.WorkTime>();

                                var levelDistr = new SDR.LevelsDistribution();
                                if (record[dicFields["EMITTING.LevelsDistributionCount"]] != null)
                                {
                                    var items = (record[dicFields["EMITTING.LevelsDistributionCount"]] as JArray).Select(jv => (int)jv).ToArray();
                                    levelDistr.Count = items;
                                }
                                if (record[dicFields["EMITTING.LevelsDistributionLvl"]] != null)
                                {
                                    var items = (record[dicFields["EMITTING.LevelsDistributionLvl"]] as JArray).Select(jv => (int)jv).ToArray();
                                    levelDistr.Levels = items;
                                }
                                emitting.LevelsDistribution = levelDistr;
                                emitting.Spectrum = GetSpectrumByEmittingId(emittingId);
                                emittings.Add(emitting);
                            }

                            workTimes.Add(new SDR.WorkTime()
                            {
                                StartEmitting = (DateTime)record[dicFields["StartEmitting"]],
                                StopEmitting = (DateTime)record[dicFields["StopEmitting"]],
                                HitCount = Convert.ToInt32(record[dicFields["HitCount"]]),
                                PersentAvailability = (float)Convert.ChangeType(record[dicFields["HitCount"]], typeof(float)) // (float)record[dicFields["PersentAvailability"]]
                            });

                            emitting.WorkTimes = workTimes.ToArray();
                        }
                    }
                }
                return emittings.ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        private SDR.Spectrum GetSpectrumByEmittingId(long emittingId)
        {
            try
            {
                var spectrum = new SDR.Spectrum();
                using (var wc = new HttpClient())
                {
                    string filter = $"(EMITTING.Id eq {emittingId})";
                    string fields = "SpectrumStartFreq_MHz,SpectrumSteps_kHz,T1,T2,MarkerIndex,Bandwidth_kHz,CorrectnessEstimations,Contravention,TraceCount,SignalLevel_dBm,Levels_dBm";
                    string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/Spectrum?select={fields}&filter={filter}";
                    var response = wc.GetAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var dicFields = new Dictionary<string, int>();
                        var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                        foreach (var field in data.Fields)
                            dicFields[field.Path] = field.Index;

                        foreach (object[] record in data.Records)
                        {
                            spectrum.Bandwidth_kHz = (double)record[dicFields["Bandwidth_kHz"]];
                            spectrum.Contravention = Convert.ToInt32(record[dicFields["Contravention"]]) == 1 ? true : false;
                            spectrum.CorrectnessEstimations = Convert.ToInt32(record[dicFields["CorrectnessEstimations"]]) == 1 ? true : false; ;
                            spectrum.MarkerIndex = Convert.ToInt32(record[dicFields["MarkerIndex"]]);
                            spectrum.SignalLevel_dBm = (float)Convert.ChangeType(record[dicFields["SignalLevel_dBm"]], typeof(float));
                            spectrum.SpectrumStartFreq_MHz = (double)record[dicFields["SpectrumStartFreq_MHz"]];
                            spectrum.SpectrumSteps_kHz = (double)record[dicFields["SpectrumSteps_kHz"]];
                            spectrum.T1 = Convert.ToInt32(record[dicFields["T1"]]);
                            spectrum.T2 = Convert.ToInt32(record[dicFields["T2"]]);
                            spectrum.TraceCount = Convert.ToInt32(record[dicFields["TraceCount"]]);
                            if (record[dicFields["Levels_dBm"]] != null)
                            {
                                var items = (record[dicFields["Levels_dBm"]] as JArray).Select(jv => (float)jv).ToArray();
                                spectrum.Levels_dBm = items;
                            }
                        }
                    }
                }
                return spectrum;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        private bool ValidateData()
        {
            if (_sensorIds.Count == 0)
            {
                MessageBox.Show("Undefined sensors!");
                return false;
            }
            if (this._startType == 1)
            {
                if (IsUseTime == true && (TimeIntersection < 0.01 || TimeIntersection > 100))
                {
                    MessageBox.Show("Incorrect value 'Mandatory time intersection'!");
                    return false;
                }
                if (FreqIntersection < 0.01 || FreqIntersection > 100)
                {
                    MessageBox.Show("Incorrect value 'Mandatory frequency intersection'!");
                    return false;
                }
                return true;
            }
            else if (this._startType == 2)
            {
                _dateStart = DateFrom.AddHours(TimeFrom.Hour).AddMinutes(TimeFrom.Minute);
                _dateStop = DateTo.AddHours(TimeTo.Hour).AddMinutes(TimeTo.Minute);
                //if (_dateStart > _dateStop || _dateStart.AddMonths(1) < _dateStop)
                //{
                //    MessageBox.Show("Incorrect value 'Date From' or 'Date To'!");
                //    return false;
                //}
                if (!FreqFrom.HasValue)
                {
                    MessageBox.Show("Undefined value 'Frequency from, MHz'!");
                    return false;
                }
                if (!FreqTo.HasValue)
                {
                    MessageBox.Show("Undefined value 'Frequency to, MHz'!");
                    return false;
                }
                if (FreqFrom > FreqTo || FreqTo - FreqFrom > 100)
                {
                    MessageBox.Show("Incorrect value 'Frequency from, MHz' or 'Frequency to, MHz'!");
                    return false;
                }
                if (FreqTo - FreqFrom > 10 && MessageBox.Show("You have select a large frequency range, it may take a long time to process the request. Continue?", "ICS Control Client", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return false;

                return true;
            }
            else
                return false;
        }
        private DateTime DateMin(DateTime d1, DateTime d2)
        {
            if (d1 > d2)
                return d2;
            else
                return d1;
        }
        private DateTime DateMax(DateTime d1, DateTime d2)
        {
            if (d1 < d2)
                return d2;
            else
                return d1;
        }
    }
}
