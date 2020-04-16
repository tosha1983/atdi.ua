using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using System.Windows.Controls;
using System.Collections;
using fm = System.Windows.Forms;
using Atdi.Common;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using INP = System.Windows.Input;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System.Net.Http;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace XICSM.ICSControlClient.ViewModels
{
    public class CreateMeasTaskViewModel : WpfViewModelBase
    {
        #region Current Objects
        private int? _allotId;
        private int planId = IM.NullI;
        private SDR.MeasurementType _measType = SDR.MeasurementType.Signaling;
        private MeasTaskViewModel _currentMeasTask;
        private IList _currentShortSensor;
        private MP.MapDrawingData _currentMapData;
        #endregion

        public FM.MeasTaskForm _measTaskForm;
        private ShortSensorDataAdatper _shortSensors;
        private double? minFq = null;
        private double? maxFq = null;

        #region Commands
        public WpfCommand CreateMeasTaskCommand { get; set; }
        public WpfCommand DoubleClickSensorCommand { get; set; }
        #endregion

        public CreateMeasTaskViewModel(int? allotId, SDR.MeasurementType measType)
        {
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);
            this.DoubleClickSensorCommand = new WpfCommand(this.OnDoubleClickSensorCommand);
            this._shortSensors = new ShortSensorDataAdatper();
            this._currentMeasTask = new MeasTaskViewModel();
            this._allotId = allotId;
            this._measType = measType;
            this.SetDefaultVaues();
            this.ReloadShortSensors();
            this.RedrawMap();
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public MeasTaskViewModel CurrentMeasTask
        {
            get => this._currentMeasTask;
            set => this.Set(ref this._currentMeasTask, value, () => { ReloadShortSensors(); RedrawMap(); });
        }
        public IList CurrentShortSensor
        {
            get => this._currentShortSensor;
            set
            {
                this._currentShortSensor = value;
                RedrawMap();
            }
        }
        //public bool IsAutoMeasDtParamMeasTimeProp
        //{
        //    get => this._currentMeasTask.IsAutoMeasDtParamMeasTime;
        //    set 
        //    {
        //        this._currentMeasTask.IsAutoMeasDtParamMeasTime = value;
        //        if (value)
        //            this._currentMeasTask.MeasDtParamMeasTime = null;
        //    }
        //}

        #region Sources (Adapters)
        public ShortSensorDataAdatper ShortSensors => this._shortSensors;

        #endregion
        private void SetDefaultVaues()
        {
            if (_allotId.HasValue && (_measType == SDR.MeasurementType.Signaling || _measType == SDR.MeasurementType.SpectrumOccupation))
            {
                IMRecordset rsAllot = new IMRecordset("CH_ALLOTMENTS", IMRecordset.Mode.ReadOnly);
                rsAllot.SetWhere("ID", IMRecordset.Operation.Eq, _allotId.Value);
                rsAllot.Select("ID,CUST_TXT1,CUST_DAT1,CUST_DAT2,PLAN_ID,Plan.BANDWIDTH,Plan.CHANNEL_SEP");
                for (rsAllot.Open(); !rsAllot.IsEOF(); rsAllot.MoveNext())
                {
                    this._currentMeasTask.Name = rsAllot.GetS("CUST_TXT1");
                    if (rsAllot.GetT("CUST_DAT1") != IM.NullT)
                        this._currentMeasTask.MeasTimeParamListPerStart = rsAllot.GetT("CUST_DAT1");
                    else
                        this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;

                    if (rsAllot.GetT("CUST_DAT2") != IM.NullT)
                        this._currentMeasTask.MeasTimeParamListPerStop = rsAllot.GetT("CUST_DAT2");
                    else
                        this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);

                    if (rsAllot.GetD("Plan.BANDWIDTH") != IM.NullD)
                        this._currentMeasTask.MeasFreqParamStep = rsAllot.GetD("Plan.BANDWIDTH");
                    else
                        this._currentMeasTask.MeasFreqParamStep = 100;
                    planId = rsAllot.GetI("PLAN_ID");
                }

                if (planId != IM.NullI)
                {
                    var measFreqParamMeasFreqs = new List<double>();

                    IMRecordset rs = new IMRecordset("FREQ_PLAN_CHAN", IMRecordset.Mode.ReadOnly);
                    rs.SetWhere("PLAN_ID", IMRecordset.Operation.Eq, planId);
                    rs.Select("PLAN_ID,FREQ");
                    for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                    {
                        if (rs.GetD("FREQ") != IM.NullD)
                        {
                            measFreqParamMeasFreqs.Add(rs.GetD("FREQ"));
                            if (!minFq.HasValue || minFq.Value > rs.GetD("FREQ"))
                                minFq = rs.GetD("FREQ");
                            if (!maxFq.HasValue || maxFq.Value < rs.GetD("FREQ"))
                                maxFq = rs.GetD("FREQ");
                        }
                    }
                    if (minFq.HasValue)
                        this._currentMeasTask.MeasFreqParamRgL = minFq.Value;
                    else
                        this._currentMeasTask.MeasFreqParamRgL = 900;

                    if (maxFq.HasValue)
                        this._currentMeasTask.MeasFreqParamRgU = maxFq.Value;
                    else
                        this._currentMeasTask.MeasFreqParamRgU = 1000;

                    this._currentMeasTask.MeasFreqParams = minFq.ToString() + " - " + maxFq.ToString();
                    this._currentMeasTask.MeasFreqParamMeasFreqs = measFreqParamMeasFreqs.ToArray();
                }
                else
                {
                    this._currentMeasTask.MeasFreqParamRgL = 900;
                    this._currentMeasTask.MeasFreqParamRgU = 1000;
                    this._currentMeasTask.MeasFreqParamStep = 100;
                }
                this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyList;
            }
            else
            {
                this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;
                this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);
                this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyList;
                this._currentMeasTask.MeasFreqParamRgL = 900;
                this._currentMeasTask.MeasFreqParamRgU = 1000;
                this._currentMeasTask.MeasFreqParamStep = 100;
            }

            this._currentMeasTask.MeasDtParamTypeMeasurements = _measType;
            this._currentMeasTask.MeasTimeParamListTimeStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListTimeStop = DateTime.Today.AddDays(1).AddMinutes(-1);
            if (_measType == SDR.MeasurementType.Signaling || _measType == SDR.MeasurementType.SpectrumOccupation)
            {
                this._currentMeasTask.MeasTimeParamListPerInterval = 600;
                this._currentMeasTask.IsAutoMeasDtParamRBW = true;
                this._currentMeasTask.IsAutoMeasDtParamVBW = true;
                this._currentMeasTask.IsAutoMeasDtParamMeasTime = true;
                this._currentMeasTask.IsAutoMeasDtParamPreamplification = true;
                this._currentMeasTask.IsAutoMeasDtParamReferenceLevel = true;
                this._currentMeasTask.IsAutoMeasDtParamRfAttenuation = true;

                //this._currentMeasTask.MeasDtParamRBW = -1;
                //this._currentMeasTask.MeasDtParamVBW = -1;
                //this._currentMeasTask.MeasDtParamMeasTime = 0.001;
                this._currentMeasTask.MeasDtParamDetectType = SDR.DetectingType.MaxPeak;
                //this._currentMeasTask.MeasDtParamRfAttenuation = 0;
                //this._currentMeasTask.MeasDtParamPreamplification = 0;
            }
            else
            {
                this._currentMeasTask.IsAutoMeasDtParamRBW = false;
                this._currentMeasTask.IsAutoMeasDtParamVBW = false;
                this._currentMeasTask.IsAutoMeasDtParamMeasTime = false;
                this._currentMeasTask.IsAutoMeasDtParamPreamplification = false;
                this._currentMeasTask.IsAutoMeasDtParamReferenceLevel = false;
                this._currentMeasTask.IsAutoMeasDtParamRfAttenuation = false;
                this._currentMeasTask.MeasDtParamRBW = 100;
                this._currentMeasTask.MeasDtParamVBW = 100;
                this._currentMeasTask.MeasDtParamMeasTime = 0.001;
                this._currentMeasTask.MeasDtParamDetectType = SDR.DetectingType.Average;
                this._currentMeasTask.MeasDtParamRfAttenuation = 0;
                this._currentMeasTask.MeasDtParamPreamplification = 0;
            }

            if (_measType == SDR.MeasurementType.Signaling)
            {
                this._currentMeasTask.IsAutoTriggerLevel_dBm_Hz = true;
                this._currentMeasTask.CollectEmissionInstrumentalEstimation = false;
            }

            this._currentMeasTask.MeasOtherTypeSpectrumOccupation = SDR.SpectrumOccupationType.FreqChannelOccupation;
            this._currentMeasTask.MeasOtherLevelMinOccup = -75;
            this._currentMeasTask.MeasOtherSwNumber = 1;
            this._currentMeasTask.MeasOtherNCount = 1000;
            this._currentMeasTask.MeasOtherNChenal = 10;
            this._currentMeasTask.SupportMultyLevel = false;
            this._currentMeasTask.MeasOtherTypeSpectrumScan = SDR.SpectrumScanType.Sweep;
            this._currentMeasTask.ResultType = SDR.MeasTaskResultType.MeasurementResult;
            this._currentMeasTask.ExecutionMode = SDR.MeasTaskExecutionMode.Automatic;
            this._currentMeasTask.Task = SDR.MeasTaskType.Scan;
            this._currentMeasTask.DateCreated = DateTime.Now;
            this._currentMeasTask.CreatedBy = IM.ConnectedUser();

            this._currentMeasTask.FiltrationTrace = false;
            this._currentMeasTask.windowBW = 1.1;
            this._currentMeasTask.AllowableExcess_dB = 10;
            //this._currentMeasTask.triggerLevel_dBm_Hz = -999;
            this._currentMeasTask.CrossingBWPercentageForGoodSignals = 70;
            this._currentMeasTask.CrossingBWPercentageForBadSignals = 40;
            this._currentMeasTask.DiffLevelForCalcBW = 25;
            this._currentMeasTask.CorrelationAnalize = false;
            this._currentMeasTask.CorrelationFactor = 0.75;
            this._currentMeasTask.SignalizationNCount = 1000;
            this._currentMeasTask.SignalizationNChenal = 100;
            this._currentMeasTask.AnalyzeByChannel = false;

            this._currentMeasTask.CompareTraceJustWithRefLevels = false;
            this._currentMeasTask.AutoDivisionEmitting = true;
            this._currentMeasTask.DifferenceMaxMax = 20;
            this._currentMeasTask.NumberPointForChangeExcess = 10;
            this._currentMeasTask.DetailedMeasurementsBWEmission = false;
            this._currentMeasTask.MinPointForDetailBW = 300;

            this._currentMeasTask.CheckFreqChannel = false;
            this._currentMeasTask.MaxFreqDeviation = 0.0001;
            this._currentMeasTask.CheckLevelChannel = true;
            this._currentMeasTask.Standard = "GSM";
            this._currentMeasTask.StandardInstEstim = "GSM";

            this.CurrentMeasTask.CollectEmissionInstrumentalEstimation = false;
            this._currentMeasTask.AnalyzeSysInfoEmission = false;
            this._currentMeasTask.nDbLevel_dB = 15;
            this._currentMeasTask.NumberIgnoredPoints = 1;
            this._currentMeasTask.MinExcessNoseLevel_dB = 5;
            this._currentMeasTask.TimeBetweenWorkTimes_sec = null; // 60;
            this._currentMeasTask.TypeJoinSpectrum = 0;
        }
        private void ReloadShortSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
            this._shortSensors.Source = sdrSensors;
            if (sdrSensors.Length == 1)
            {
                this._currentShortSensor = new List<ShortSensorViewModel>() { Mappers.Map(sdrSensors[0]) };
                RedrawMap();
            }
        }
        private void OnDoubleClickSensorCommand(object parameter)
        {
            var param = new OnlineMeasurementParameters();
            if (!this.CurrentMeasTask.IsAutoMeasDtParamMeasTime)
                param.SweepTime_s = this.CurrentMeasTask.MeasDtParamMeasTime;

            if (this.CurrentMeasTask.MeasFreqParamStep.HasValue)
            {
                if (this.CurrentMeasTask.MeasFreqParamRgL.HasValue)
                    param.FreqStart_MHz = this.CurrentMeasTask.MeasFreqParamRgL - this.CurrentMeasTask.MeasFreqParamStep / 2000;

                if (this.CurrentMeasTask.MeasFreqParamRgU.HasValue)
                    param.FreqStop_MHz = this.CurrentMeasTask.MeasFreqParamRgU + this.CurrentMeasTask.MeasFreqParamStep / 2000;
            }
            else
            {
                param.FreqStart_MHz = this.CurrentMeasTask.MeasFreqParamRgL;
                param.FreqStop_MHz = this.CurrentMeasTask.MeasFreqParamRgU;
            }

            if (!this.CurrentMeasTask.IsAutoMeasDtParamRfAttenuation)
                param.Att_dB = (int?)this.CurrentMeasTask.MeasDtParamRfAttenuation;

            if (!this.CurrentMeasTask.IsAutoMeasDtParamPreamplification)
                param.PreAmp_dB = this.CurrentMeasTask.MeasDtParamPreamplification;

            if (!this.CurrentMeasTask.IsAutoMeasDtParamReferenceLevel)
                param.RefLevel_dBm = (int?)this.CurrentMeasTask.MeasDtParamReferenceLevel;

            param.DetectorType = (Enum.TryParse<DetectorType>(this.CurrentMeasTask.MeasDtParamDetectType.ToString(), out DetectorType outResType)) ? outResType : DetectorType.MaxPeak;

            var dlgForm = new FM.OnlineMeasurementForm(parameter as ShortSensorViewModel, param);
            dlgForm.ShowDialog();
            dlgForm.Dispose();
        }
        private void OnCreateMeasTaskCommand(object parameter)
        {
            try
            {
                bool result = true;
                //var dateBg = this._currentMeasTask.MeasTimeParamListPerStart;
                //var dateEd = this._currentMeasTask.MeasTimeParamListPerStop;

                //if (this._currentMeasTask.MeasTimeParamListTimeStart.HasValue)
                //    dateBg = dateBg.AddHours(this._currentMeasTask.MeasTimeParamListTimeStart.Value.Hour).AddMinutes(this._currentMeasTask.MeasTimeParamListTimeStart.Value.Minute);
                //if (this._currentMeasTask.MeasTimeParamListTimeStop.HasValue)
                //    dateEd = dateEd.AddHours(this._currentMeasTask.MeasTimeParamListTimeStop.Value.Hour).AddMinutes(this._currentMeasTask.MeasTimeParamListTimeStop.Value.Minute);

                //if (!this.CurrentMeasTask.ValidateStateModel())
                //    return;

                //if (dateBg > dateEd)
                //{
                //    MessageBox.Show("Date Stop should be great of the Date Start!");
                //    return;
                //}

                if (this._currentMeasTask.SupportMultyLevel.HasValue && this._currentMeasTask.SupportMultyLevel.Value)
                {
                    if (MessageBox.Show($"{Properties.Resources.Attention} {Properties.Resources.YouSelectedOption}: “{Properties.Resources.CollectDataForChangeInTheMinLevel}”. {Properties.Resources.Message_ThisOptionWillRequireAdditionalCalculationsOnTheServersAndWillSignificantlyIncreaseTheRequiredAmountOfMemoryToStoreTheResults} {Properties.Resources.Message_DoYouWantContinue}?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    {
                        result = false;
                        return;
                    }
                }

                if (this._currentMeasTask.MeasTimeParamListPerStart > this._currentMeasTask.MeasTimeParamListPerStop)
                {
                    MessageBox.Show("Date Stop should be great of the Date Start!");
                    return;
                }
                if (this._currentMeasTask.MeasTimeParamListTimeStart > this._currentMeasTask.MeasTimeParamListTimeStop)
                {
                    MessageBox.Show("Time Stop should be great of the Time Start!");
                    return;
                }

                if (this.CurrentMeasTask.DetailedMeasurementsBWEmission.GetValueOrDefault(false) && this.CurrentMeasTask.MinPointForDetailBW.HasValue && this.CurrentMeasTask.SignalizationNChenal.HasValue && (this.CurrentMeasTask.MinPointForDetailBW.Value < this.CurrentMeasTask.SignalizationNChenal.Value || this.CurrentMeasTask.MinPointForDetailBW.Value > 5000))
                {
                    MessageBox.Show("The value “The minimum number of points a spectrum must contain in order not to measure bandwith” must be in the range from " + this.CurrentMeasTask.SignalizationNChenal.Value + " to 5000!");
                    return;
                }

                if (string.IsNullOrEmpty(this._currentMeasTask.Name))
                {
                    MessageBox.Show("Undefined task name!");
                    return;
                }

                if (this._currentShortSensor == null || this._currentShortSensor.Count == 0)
                {
                    MessageBox.Show("Undefined sensor!");
                    return;
                }

                if (!this._currentMeasTask.IsAutoMeasDtParamRfAttenuation && (this._currentMeasTask.MeasDtParamRfAttenuation.HasValue && (this._currentMeasTask.MeasDtParamRfAttenuation < 0 || this._currentMeasTask.MeasDtParamRfAttenuation > 40)))
                {
                    MessageBox.Show("The value 'Attenuation, dB' must be in the range from 0 to 40");
                    return;
                }
                if (!this._currentMeasTask.IsAutoMeasDtParamRfAttenuation && (this._currentMeasTask.MeasDtParamRfAttenuation.HasValue && this._currentMeasTask.MeasDtParamRfAttenuation.Value != Math.Round(this._currentMeasTask.MeasDtParamRfAttenuation.Value, 0)))
                {
                    MessageBox.Show("The value 'Attenuation, dB' must be an integer");
                    return;
                }

                if (!this._currentMeasTask.IsAutoMeasDtParamPreamplification && (this._currentMeasTask.MeasDtParamPreamplification.HasValue && (this._currentMeasTask.MeasDtParamPreamplification < 0 || this._currentMeasTask.MeasDtParamPreamplification > 40)))
                {
                    MessageBox.Show("The value 'Gain of preamplifier, dB' must be in the range from 0 to 40");
                    return;
                }

                if (!this._currentMeasTask.IsAutoMeasDtParamReferenceLevel && (this._currentMeasTask.MeasDtParamReferenceLevel.HasValue && (this._currentMeasTask.MeasDtParamReferenceLevel < -200 || this._currentMeasTask.MeasDtParamReferenceLevel > 10)))
                {
                    MessageBox.Show("The value 'Gain of preamplifier, dB' must be in the range from -200 to 10");
                    return;
                }

                if (!this._currentMeasTask.IsAutoTriggerLevel_dBm_Hz && (this._currentMeasTask.triggerLevel_dBm_Hz.HasValue && (this._currentMeasTask.triggerLevel_dBm_Hz < -200 || this._currentMeasTask.triggerLevel_dBm_Hz > -100)))
                {
                    MessageBox.Show("The value 'Noise Level' must be in the range from -200 to -100");
                    return;
                }

                if (planId != IM.NullI)
                {
                    var points = this._currentMeasTask.MeasOtherNChenal * (maxFq - minFq) / this._currentMeasTask.MeasFreqParamStep;
                    if (points > 200)
                    {
                        MessageBox.Show("Lot of points, please change “Number of steps for measurements in channel” or number channel in plan");
                        result = false;
                    }
                }

                if (!minFq.HasValue || minFq < 0.009 || minFq > 6000 || !maxFq.HasValue || maxFq < 0.009 || maxFq > 6000)
                {
                    MessageBox.Show("Wrong frequency list values!");
                    return;
                }

                if (!this._currentMeasTask.MeasFreqParamStep.HasValue)
                {
                    MessageBox.Show("Step with cannot be null!");
                    return;
                }

                var measFreqParam = new SDR.MeasFreqParam() { Mode = this._currentMeasTask.MeasFreqParamMode, Step = this._currentMeasTask.MeasFreqParamStep };

                switch (measFreqParam.Mode)
                {
                    case SDR.FrequencyMode.SingleFrequency:
                        if (this.CurrentMeasTask.MeasFreqParam.HasValue)
                        {
                            measFreqParam.MeasFreqs = new SDR.MeasFreq[] { new SDR.MeasFreq() { Freq = this.CurrentMeasTask.MeasFreqParam.Value } };
                        }
                        break;
                    case SDR.FrequencyMode.FrequencyList:
                        var frqList = new List<SDR.MeasFreq>();
                        if (_measType == SDR.MeasurementType.SpectrumOccupation || _measType == SDR.MeasurementType.Signaling)
                        {
                            if (this._currentMeasTask.MeasFreqParamMeasFreqs != null)
                            {
                                foreach (var freq in this._currentMeasTask.MeasFreqParamMeasFreqs)
                                {
                                    frqList.Add(new SDR.MeasFreq() { Freq = freq });
                                }
                                measFreqParam.MeasFreqs = frqList.ToArray();
                                measFreqParam.RgL = this._currentMeasTask.MeasFreqParamRgL;
                                measFreqParam.RgU = this._currentMeasTask.MeasFreqParamRgU;
                            }
                        }
                        //else
                        //{
                        //    if (!string.IsNullOrEmpty(this._currentMeasTask.MeasFreqParams))
                        //    {
                        //        var freqArray = this._currentMeasTask.MeasFreqParams.Replace(';', ',').Split(',');
                        //        foreach (var freq in freqArray)
                        //        {
                        //            if (double.TryParse(freq, out double freqD))
                        //            {
                        //                measFreqParam.MeasFreqs = measFreqParam.MeasFreqs.Concat(new SDR.MeasFreq[] { new SDR.MeasFreq() { Freq = freqD } }).ToArray();
                        //            }
                        //        }
                        //    }
                        //}
                        break;
                    case SDR.FrequencyMode.FrequencyRange:
                        measFreqParam.RgL = this._currentMeasTask.MeasFreqParamRgL;
                        measFreqParam.RgU = this._currentMeasTask.MeasFreqParamRgU;
                        //measFreqParam.Step = this._currentMeasTask.MeasFreqParamStep;
                        break;
                }

                if (_measType == SDR.MeasurementType.SpectrumOccupation)
                {
                    var val = (measFreqParam.MeasFreqs == null ? 1000 : measFreqParam.MeasFreqs.Length) * ((maxFq - minFq) / this._currentMeasTask.MeasFreqParamStep) * this._currentMeasTask.MeasOtherNChenal;

                    if (val > 50000)
                    {
                        MessageBox.Show("Attention!!! The “Number of steps for measurements in channel” have big value. That task will require lot of sensors resources.", "ISC Control Client");
                        result = false;
                    }
                    else if (val > 10000)
                    {
                        if (MessageBox.Show("Attention!!! The “Number of steps for measurements in channel” have big value. That task will require lot of sensors resources. Perhaps this will be to the detriment of other tasks. Do you want continue?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            result = false;
                    }

                    if (this._currentMeasTask.MeasOtherNCount.HasValue && this._currentMeasTask.MeasOtherNCount > 10000)
                    {
                        MessageBox.Show("Attention!!! The “Number total scan” have big value. That task will require lot of sensors resources.", "ISC Control Client");
                        result = false;
                    }
                    else if (this._currentMeasTask.MeasOtherNCount.HasValue && this._currentMeasTask.MeasOtherNCount > 1000)
                    {
                        if (MessageBox.Show("Attention!!! The “Number total scan” have big value. That task will require lot of sensors resources. Perhaps this will be to the detriment of other tasks. Do you want continue?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            result = false;
                    }

                    if (this._currentMeasTask.MeasOtherNChenal.HasValue && this._currentMeasTask.MeasOtherNChenal > 100)
                    {
                        MessageBox.Show("Attention!!! The “Number of steps for measurements in channel” have big value. That task will require lot of sensors resources.", "ISC Control Client");
                        result = false;
                    }
                    else if (this._currentMeasTask.MeasOtherNChenal.HasValue && this._currentMeasTask.MeasOtherNChenal > 50)
                    {
                        if (MessageBox.Show("Attention!!! The “Number of steps for measurements in channel” have big value. That task will require lot of sensors resources. Perhaps this will be to the detriment of other tasks. Do you want continue?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            result = false;
                    }
                }
                if (_measType == SDR.MeasurementType.Signaling)
                {
                    double? val = 0;
                    if (measFreqParam.Mode == SDR.FrequencyMode.FrequencyList)
                        val = this.CurrentMeasTask.SignalizationNChenal * ((maxFq - minFq) / this._currentMeasTask.MeasFreqParamStep);
                    else if (measFreqParam.Mode == SDR.FrequencyMode.FrequencyRange)
                        val = this.CurrentMeasTask.SignalizationNChenal * ((this._currentMeasTask.MeasFreqParamRgU - this._currentMeasTask.MeasFreqParamRgL) / this._currentMeasTask.MeasFreqParamStep);
                    if (val > 500)
                    {
                        MessageBox.Show("Attention!!! The “The number of point in the channel during scanning” have big value. That task will require lot of sensors resources.", "ISC Control Client");
                        result = false;
                    }
                    else if (val > 100)
                    {
                        if (MessageBox.Show("Attention!!! The “The number of point in the channel during scanning” have big value. That task will require lot of sensors resources. Perhaps this will be to the detriment of other tasks. Do you want continue?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            result = false;
                    }

                    if (this._currentMeasTask.SignalizationNCount.HasValue && this._currentMeasTask.SignalizationNCount > 1000000)
                    {
                        MessageBox.Show("Attention!!! The “The maximum number of scan per day” have big value. That task will require lot of sensors resources.", "ISC Control Client");
                        result = false;
                    }
                    else if (this._currentMeasTask.SignalizationNCount.HasValue && this._currentMeasTask.SignalizationNCount > 1000)
                    {
                        if (MessageBox.Show("Attention!!! The “The maximum number of scan per day” have big value. That task will require lot of sensors resources. Perhaps this will be to the detriment of other tasks. Do you want continue?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            result = false;
                    }

                    if (this._currentMeasTask.CorrelationAnalize.HasValue && this._currentMeasTask.CorrelationAnalize.Value)
                    {
                        if (this._currentMeasTask.CorrelationFactor.HasValue && this._currentMeasTask.CorrelationFactor > 1)
                        {
                            MessageBox.Show("Attention!!! The “Correlation coefficient” have big value. Results will have lot of emissions.", "ISC Control Client");
                            result = false;
                        }
                        else if (this._currentMeasTask.CorrelationFactor.HasValue && this._currentMeasTask.CorrelationFactor > 0.8)
                        {
                            if (MessageBox.Show("Attention!!! The “Correlation coefficient” have big value. Results will have lot of emissions. Do you want continue?", "ISC Control Client", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                                result = false;
                        }
                    }
                    if (this._currentMeasTask.CollectEmissionInstrumentalEstimation.HasValue && this._currentMeasTask.CollectEmissionInstrumentalEstimation.Value)
                    {
                        switch ((this.CurrentMeasTask.CollectEmissionInstrumentalEstimation.Value && !this.CurrentMeasTask.AnalyzeByChannel.Value) ? this._currentMeasTask.StandardInstEstim : this._currentMeasTask.Standard)
                        {
                            case "GSM":
                                this._currentMeasTask.windowBW = 1.4;
                                this._currentMeasTask.CrossingBWPercentageForGoodSignals = 80;
                                this._currentMeasTask.CrossingBWPercentageForBadSignals = 60;
                                this._currentMeasTask.CorrelationAnalize = true;
                                this._currentMeasTask.CorrelationFactor = 0.99;
                                this._currentMeasTask.AnalyzeByChannel = true;
                                this._currentMeasTask.CompareTraceJustWithRefLevels = false;
                                this._currentMeasTask.DetailedMeasurementsBWEmission = false;
                                break;
                            case "CDMA":
                                this._currentMeasTask.windowBW = 1.1;
                                this._currentMeasTask.CrossingBWPercentageForGoodSignals = 80;
                                this._currentMeasTask.CrossingBWPercentageForBadSignals = 60;
                                this._currentMeasTask.CorrelationAnalize = true;
                                this._currentMeasTask.CorrelationFactor = 0.99;
                                this._currentMeasTask.AnalyzeByChannel = true;
                                this._currentMeasTask.CompareTraceJustWithRefLevels = false;
                                this._currentMeasTask.DetailedMeasurementsBWEmission = false;
                                break;
                            case "LTE":
                                this._currentMeasTask.windowBW = 1.1;
                                this._currentMeasTask.CrossingBWPercentageForGoodSignals = 80;
                                this._currentMeasTask.CrossingBWPercentageForBadSignals = 60;
                                this._currentMeasTask.CorrelationAnalize = true;
                                this._currentMeasTask.CorrelationFactor = 0.99;
                                this._currentMeasTask.AnalyzeByChannel = false;
                                this._currentMeasTask.CompareTraceJustWithRefLevels = false;
                                this._currentMeasTask.DetailedMeasurementsBWEmission = false;
                                break;
                            case "UMTS":
                                this._currentMeasTask.windowBW = 1.1;
                                this._currentMeasTask.CrossingBWPercentageForGoodSignals = 80;
                                this._currentMeasTask.CrossingBWPercentageForBadSignals = 60;
                                this._currentMeasTask.CorrelationAnalize = true;
                                this._currentMeasTask.CorrelationFactor = 0.99;
                                this._currentMeasTask.AnalyzeByChannel = false;
                                this._currentMeasTask.CompareTraceJustWithRefLevels = false;
                                this._currentMeasTask.DetailedMeasurementsBWEmission = false;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (!result)
                    return;

                if (!GetOthersMeastask())
                    return;

                List<SDR.ReferenceSituation> listRef = new List<SDR.ReferenceSituation>();
                if (this._currentMeasTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.Signaling)
                {
                    string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                    foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                    {
                        fm.OpenFileDialog openFile = new fm.OpenFileDialog() { Filter = "Текстовые файлы(*.csv)|*.csv", Title = shortSensor.Name };
                        if (openFile.ShowDialog() == fm.DialogResult.OK)
                        {
                            var _waitForm = new FM.WaitForm();
                            _waitForm.SetMessage("Loading file. Please wait...");
                            _waitForm.TopMost = true;
                            _waitForm.Show();
                            _waitForm.Refresh();

                            SDR.ReferenceSituation refSit = new SDR.ReferenceSituation();
                            List<SDR.ReferenceSignal> listRefSig = new List<SDR.ReferenceSignal>();

                            var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id);
                            SDR.SensorLocation sensorLocation = null;
                            if (svcSensor.Locations != null && svcSensor.Locations.Length > 0)
                                sensorLocation = svcSensor.Locations[svcSensor.Locations.Length - 1];

                            using (TextFieldParser parser = new TextFieldParser(openFile.FileName))
                            {
                                int i = 0;
                                try
                                {
                                    parser.TextFieldType = FieldType.Delimited;
                                    parser.SetDelimiters(";");
                                    while (!parser.EndOfData)
                                    {
                                        i++;
                                        var record = parser.ReadFields();

                                        if (i >= 4)
                                        {
                                            SDR.ReferenceSignal refSig = new SDR.ReferenceSignal();

                                            double? f = record[9].Replace(".", sep).TryToDouble();
                                            double? l = record[4].Replace(".", sep).TryToDouble();
                                            double? d = record[11].Replace(".", sep).TryToDouble();
                                            double? a = record[12].Replace(".", sep).TryToDouble();

                                            if (f.HasValue)
                                            {
                                                refSig.Frequency_MHz = f.Value;

                                                if (l.HasValue)
                                                    refSig.LevelSignal_dBm = l.Value;

                                                if (d.HasValue && a.HasValue)
                                                {
                                                    refSig.IcsmTable = "MOB_STATION";
                                                    if (!this.GetRefSignalBySensor(ref refSig, sensorLocation, d.Value, a.Value))
                                                    {
                                                        refSig.IcsmTable = "MOB_STATION2";
                                                        this.GetRefSignalBySensor(ref refSig, sensorLocation, d.Value, a.Value);
                                                    }
                                                }
                                            }
                                            listRefSig.Add(refSig);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("Incorrect format file: " + openFile.FileName + "!\r\n" + "Line: " + i.ToString() + "\r\n" + e.Message);
                                    _waitForm.Close();
                                    return;
                                }
                            }

                            refSit.ReferenceSignal = listRefSig.ToArray();
                            refSit.SensorId = shortSensor.Id;
                            listRef.Add(refSit);
                            _waitForm.Close();
                        }
                    }
                }

                List<SDR.MeasSensor> stationsList = new List<SDR.MeasSensor>();
                foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                {
                    stationsList.Add(new SDR.MeasSensor() { SensorId = new SDR.MeasSensorIdentifier() { Value = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id).Id.Value } });
                }

                var measTask = new SDR.MeasTask()
                {
                    Name = this._currentMeasTask.Name,
                    MaxTimeBs = this._currentMeasTask.MaxTimeBs,
                    MeasFreqParam = measFreqParam,
                    //MeasFreqParam = new SDR.MeasFreqParam()
                    //{
                    //    Mode = this._currentMeasTask.MeasFreqParamMode,
                    //    RgL = this._currentMeasTask.MeasFreqParamRgL, 
                    //    RgU = this._currentMeasTask.MeasFreqParamRgU,
                    //    Step = this._currentMeasTask.MeasFreqParamStep
                    //    //MeasFreqs = new SDR.MeasFreq()
                    //},
                    //MeasLocParams = ,
                    MeasOther = new SDR.MeasOther()
                    {
                        TypeSpectrumOccupation = this._currentMeasTask.MeasOtherTypeSpectrumOccupation,
                        LevelMinOccup = this._currentMeasTask.MeasOtherLevelMinOccup,
                        SwNumber = this._currentMeasTask.MeasOtherSwNumber,
                        TypeSpectrumScan = this._currentMeasTask.MeasOtherTypeSpectrumScan,
                        NChenal = this._currentMeasTask.MeasOtherNChenal,
                        SupportMultyLevel = this._currentMeasTask.SupportMultyLevel                        
                    },
                    //MeasSubTasks = ,
                    OrderId = this._currentMeasTask.OrderId,
                    Prio = this._currentMeasTask.Prio,
                    ResultType = this._currentMeasTask.ResultType,
                    Status = this._currentMeasTask.Status,
                    Type = this._currentMeasTask.Type,
                    MeasDtParam = new SDR.MeasDtParam()
                    {
                        TypeMeasurements = this._currentMeasTask.MeasDtParamTypeMeasurements,
                        RBW = this._currentMeasTask.IsAutoMeasDtParamRBW == true ? -1 : this._currentMeasTask.MeasDtParamRBW,
                        VBW = this._currentMeasTask.IsAutoMeasDtParamVBW == true ? -1 : this._currentMeasTask.MeasDtParamVBW,
                        MeasTime = this._currentMeasTask.IsAutoMeasDtParamMeasTime == true ? 0.001 : this._currentMeasTask.MeasDtParamMeasTime,
                        DetectType = this._currentMeasTask.MeasDtParamDetectType,
                        RfAttenuation = this._currentMeasTask.IsAutoMeasDtParamRfAttenuation == true ? -1 : this._currentMeasTask.MeasDtParamRfAttenuation,
                        Preamplification = this._currentMeasTask.IsAutoMeasDtParamPreamplification == true ? -1 : this._currentMeasTask.MeasDtParamPreamplification,
                        Mode = this._currentMeasTask.MeasDtParamMode,
                        Demod = this._currentMeasTask.MeasDtParamDemod,
                        ReferenceLevel = this._currentMeasTask.IsAutoMeasDtParamReferenceLevel == true ? 1000000000 : this._currentMeasTask.MeasDtParamReferenceLevel,
                        IfAttenuation = this._currentMeasTask.MeasDtParamIfAttenuation,
                        NumberTotalScan = this._currentMeasTask.MeasOtherNCount,

                    },
                    MeasTimeParamList = new SDR.MeasTimeParamList()
                    {
                        PerStart = this._currentMeasTask.MeasTimeParamListPerStart,
                        PerStop = this._currentMeasTask.MeasTimeParamListPerStop,
                        TimeStart = this._currentMeasTask.MeasTimeParamListTimeStart,
                        TimeStop = this._currentMeasTask.MeasTimeParamListTimeStop,
                        Days = this._currentMeasTask.MeasTimeParamListDays,
                        PerInterval = this._currentMeasTask.MeasTimeParamListPerInterval
                    },
                    SignalingMeasTaskParameters = new SDR.SignalingMeasTask()
                    {
                        InterruptionParameters = new SDR.SignalingInterruptionParameters()
                        {
                            AutoDivisionEmitting = this._currentMeasTask.AutoDivisionEmitting,
                            DifferenceMaxMax = this._currentMeasTask.DifferenceMaxMax,
                            CheckLevelChannel = this._currentMeasTask.CheckLevelChannel,
                            DiffLevelForCalcBW = this._currentMeasTask.DiffLevelForCalcBW,
                            MaxFreqDeviation = this._currentMeasTask.MaxFreqDeviation,
                            MinExcessNoseLevel_dB = this._currentMeasTask.MinExcessNoseLevel_dB,
                            MinPointForDetailBW = this._currentMeasTask.MinPointForDetailBW,
                            nDbLevel_dB = this._currentMeasTask.nDbLevel_dB,
                            NumberIgnoredPoints = this._currentMeasTask.NumberIgnoredPoints,
                            NumberPointForChangeExcess = this._currentMeasTask.NumberPointForChangeExcess,
                            windowBW = this._currentMeasTask.windowBW
                        },
                        GroupingParameters = new SDR.SignalingGroupingParameters()
                        {
                            CrossingBWPercentageForBadSignals = this._currentMeasTask.CrossingBWPercentageForBadSignals,
                            CrossingBWPercentageForGoodSignals = this._currentMeasTask.CrossingBWPercentageForGoodSignals,
                            TimeBetweenWorkTimes_sec = this._currentMeasTask.TimeBetweenWorkTimes_sec,
                            TypeJoinSpectrum = this._currentMeasTask.TypeJoinSpectrum
                        },
                        allowableExcess_dB = this._currentMeasTask.AllowableExcess_dB,
                        CompareTraceJustWithRefLevels = this._currentMeasTask.CompareTraceJustWithRefLevels,
                        FiltrationTrace = this._currentMeasTask.FiltrationTrace,
                        SignalizationNChenal = this._currentMeasTask.SignalizationNChenal,
                        SignalizationNCount = this._currentMeasTask.SignalizationNCount,
                        AnalyzeByChannel = this._currentMeasTask.AnalyzeByChannel,
                        AnalyzeSysInfoEmission = this._currentMeasTask.AnalyzeSysInfoEmission,
                        CheckFreqChannel = this._currentMeasTask.CheckFreqChannel,
                        CorrelationAnalize = this._currentMeasTask.CorrelationAnalize,
                        CorrelationFactor = this._currentMeasTask.CorrelationFactor,
                        DetailedMeasurementsBWEmission = this._currentMeasTask.DetailedMeasurementsBWEmission,
                        Standard = (this.CurrentMeasTask.CollectEmissionInstrumentalEstimation.Value && !this.CurrentMeasTask.AnalyzeByChannel.Value) ? this._currentMeasTask.StandardInstEstim : this._currentMeasTask.Standard,
                        triggerLevel_dBm_Hz = this.CurrentMeasTask.IsAutoTriggerLevel_dBm_Hz == true ? -999 : this._currentMeasTask.triggerLevel_dBm_Hz,
                        CollectEmissionInstrumentalEstimation = this.CurrentMeasTask.CollectEmissionInstrumentalEstimation
                    },
                    Sensors = stationsList.ToArray(),
                    Task = SDR.MeasTaskType.Scan,
                    ExecutionMode = SDR.MeasTaskExecutionMode.Automatic,
                    DateCreated = DateTime.Now,
                    CreatedBy = IM.ConnectedUser(),
                    RefSituation = listRef.ToArray()
                };

                var measTaskId = WCF.SdrnsControllerWcfClient.CreateMeasTask(measTask);
                if (measTaskId == IM.NullI)
                {
                    throw new InvalidOperationException($"Could not create a meas task");
                }
                else
                {
                    if (_allotId.HasValue && _allotId.Value > 0)
                    {
                        IMRecordset rsAllot = new IMRecordset("CH_ALLOTMENTS", IMRecordset.Mode.ReadWrite);
                        rsAllot.Select("ID,STATUS,CUST_NBR1");
                        rsAllot.SetWhere("ID", IMRecordset.Operation.Eq, _allotId.Value);
                        using (rsAllot.OpenWithScope())
                        {
                            rsAllot.Edit();
                            rsAllot.Put("STATUS", "dur");
                            rsAllot.Put("CUST_NBR1", measTaskId);
                            rsAllot.Update();
                        }
                    }
                }
                _measTaskForm.TaskId = measTaskId;
                _measTaskForm.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private bool GetOthersMeastask()
        {
            var prevTaskData = new List<MeasTask>();
            var taskData = new List<MeasTask>();

            var appSettings = ConfigurationManager.AppSettings;
            string endpointUrls = appSettings["SdrnServerRestEndpoint"];

            if (string.IsNullOrEmpty(endpointUrls))
            {
                MessageBox.Show("Undefined value for SdrnServerRestEndpoint in file ICSM3.exe.config.");
                return false;
            }

            var dateBg = this._currentMeasTask.MeasTimeParamListPerStart;
            var dateEd = this._currentMeasTask.MeasTimeParamListPerStop;
            if (this._currentMeasTask.MeasTimeParamListTimeStart.HasValue)
                dateBg = dateBg.AddHours(this._currentMeasTask.MeasTimeParamListTimeStart.Value.Hour).AddMinutes(this._currentMeasTask.MeasTimeParamListTimeStart.Value.Minute);
            if (this._currentMeasTask.MeasTimeParamListTimeStop.HasValue)
                dateEd = dateEd.AddHours(this._currentMeasTask.MeasTimeParamListTimeStop.Value.Hour).AddMinutes(this._currentMeasTask.MeasTimeParamListTimeStop.Value.Minute);

            using (var wc = new HttpClient())
            {
                var sensorsIds = new List<long>();
                foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                    sensorsIds.Add(shortSensor.Id);

                string fields = "SUBTASK.MEAS_TASK.Id,SUBTASK.MEAS_TASK.Type,SUBTASK.MEAS_TASK.Name,SUBTASK.MEAS_TASK.PerStart,SUBTASK.MEAS_TASK.TimeStart,SUBTASK.MEAS_TASK.PerStop,SUBTASK.MEAS_TASK.TimeStop,SUBTASK.MEAS_TASK.DateCreated,SUBTASK.MEAS_TASK.CreatedBy,SUBTASK.MEAS_TASK.Status,SENSOR.Id";
                string filter = $"((SENSOR.Id in ({string.Join(",", sensorsIds)}))and(SUBTASK.MEAS_TASK.Type eq '{this._measType.ToString()}')and(SUBTASK.MEAS_TASK.PerStop Ge {this._currentMeasTask.MeasTimeParamListPerStart.Date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff")}))";

                var response = wc.GetAsync(endpointUrls + $"api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/SubTaskSensor?select={fields}&filter={filter}&orderBy=SUBTASK.MEAS_TASK.Id").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;

                    long lastMeasTaskId = 0; 

                    foreach (object[] record in data.Records)
                    {
                        long measTaskId = Convert.ToInt64(record[dicFields["SUBTASK.MEAS_TASK.Id"]]);
                        var measTask = new MeasTask();
                        var sensors = new List<long>();

                        if (lastMeasTaskId != measTaskId)
                        {
                            lastMeasTaskId = measTaskId;
                            sensors = new List<long>();

                            measTask = new MeasTask();
                            measTask.MeasTaskId = measTaskId;
                            measTask.TaskType = (string)record[dicFields["SUBTASK.MEAS_TASK.Type"]];
                            measTask.TaskName = (string)record[dicFields["SUBTASK.MEAS_TASK.Name"]];
                            measTask.Status = (string)record[dicFields["SUBTASK.MEAS_TASK.Status"]];

                            measTask.DateStart = (DateTime)record[dicFields["SUBTASK.MEAS_TASK.PerStart"]];
                            var timeStart = (DateTime?)record[dicFields["SUBTASK.MEAS_TASK.TimeStart"]];
                            if (timeStart.HasValue)
                                measTask.DateStart.AddHours(timeStart.Value.Hour).AddMinutes(timeStart.Value.Minute);

                            measTask.DateStop = (DateTime)record[dicFields["SUBTASK.MEAS_TASK.PerStop"]];
                            var timeStop = (DateTime?)record[dicFields["SUBTASK.MEAS_TASK.TimeStop"]];
                            if (timeStop.HasValue)
                                measTask.DateStop.AddHours(timeStop.Value.Hour).AddMinutes(timeStop.Value.Minute);

                            measTask.DateCreated = (DateTime?)record[dicFields["SUBTASK.MEAS_TASK.DateCreated"]];
                            measTask.CreatedBy = (string)record[dicFields["SUBTASK.MEAS_TASK.CreatedBy"]];

                            prevTaskData.Add(measTask);
                        }
                        sensors.Add(Convert.ToInt64(record[dicFields["SENSOR.Id"]]));
                        measTask.SensorIds = string.Join(",", sensors);
                    }
                }
            }

            foreach (var task in prevTaskData)
            {
                if (task.DateStop > dateBg && task.DateStart < dateEd && !"Z".Equals(task.Status, StringComparison.OrdinalIgnoreCase))
                {
                    if (this._currentMeasTask.MeasFreqParamRgL.HasValue && this._currentMeasTask.MeasFreqParamRgU.HasValue)
                    {
                        using (var wc = new HttpClient())
                        {
                            string fields = "Rgl,Rgu";
                            string filter = $"(MEAS_TASK.Id Eq {task.MeasTaskId})";

                            double? freqMin = null;
                            double? freqMax = null;

                            var response = wc.GetAsync(endpointUrls + $"api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/MeasFreqParam?select={fields}&filter={filter}").Result;
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var dicFields = new Dictionary<string, int>();
                                var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                                foreach (var field in data.Fields)
                                    dicFields[field.Path] = field.Index;

                                foreach (object[] record in data.Records)
                                {
                                    var fqMin = (double?)record[dicFields["Rgl"]];
                                    var fqMax = (double?)record[dicFields["Rgu"]];

                                    if (fqMin.HasValue && (!freqMin.HasValue || freqMin.Value > fqMin))
                                        freqMin = fqMin;

                                    if (fqMax.HasValue && (!freqMax.HasValue || freqMax.Value < fqMax))
                                        freqMax = fqMax;
                                }
                            }
                            if (freqMin.HasValue && freqMax.HasValue)
                            {
                                var df_min = Math.Min((this._currentMeasTask.MeasFreqParamRgU.Value - this._currentMeasTask.MeasFreqParamRgL.Value), (freqMax.Value - freqMin.Value));
                                var intrseption = Math.Min(this._currentMeasTask.MeasFreqParamRgU.Value, freqMax.Value) - Math.Max(this._currentMeasTask.MeasFreqParamRgL.Value, freqMin.Value);

                                if (intrseption > 0)
                                {
                                    var p_calc = 100 * intrseption / df_min;
                                    if (p_calc >= 10)
                                    {
                                        task.FqMin = freqMin;
                                        task.FqMax = freqMax;
                                        taskData.Add(task);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (taskData.Count > 0)
            {
                var dlgForm = new FM.MeasTaskListForm(taskData.ToArray());
                dlgForm.ShowDialog();
                dlgForm.Dispose();

                if (!dlgForm.IsPresOK)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }
        private bool GetRefSignalBySensor(ref SDR.ReferenceSignal refSig, SDR.SensorLocation sensorLocation, double d, double a)
        {
            bool result = false;
            if (sensorLocation == null || !sensorLocation.Lon.HasValue || !sensorLocation.Lat.HasValue) 
                return false;

            double lonSensor = sensorLocation.Lon.Value;
            double latSensor = sensorLocation.Lat.Value;

            double lon = lonSensor - d * Math.Sin(a * Math.PI / 180) / (111315 * Math.Cos(latSensor * Math.PI / 180));
            double lat = latSensor - d * Math.Cos(a * Math.PI / 180) / 111315;

            double mod = double.MaxValue;
            int eqpId = 0;

            string freqTableName = "";

            if (refSig.IcsmTable == "MOB_STATION")
                freqTableName = "MOBSTA_FREQS";
            else
                freqTableName = "MOBSTA_FREQS2";

            IMRecordset rs = new IMRecordset(freqTableName, IMRecordset.Mode.ReadOnly);
            rs.SetWhere("TX_FREQ", IMRecordset.Operation.Lt, refSig.Frequency_MHz + 0.0001);
            rs.SetWhere("TX_FREQ", IMRecordset.Operation.Gt, refSig.Frequency_MHz - 0.0001);
            rs.SetWhere("Station.Position.LONGITUDE", IMRecordset.Operation.Lt, lon + 0.1);
            rs.SetWhere("Station.Position.LONGITUDE", IMRecordset.Operation.Gt, lon - 0.1);
            rs.SetWhere("Station.Position.LATITUDE", IMRecordset.Operation.Lt, lat + 0.1);
            rs.SetWhere("Station.Position.LATITUDE", IMRecordset.Operation.Gt, lat - 0.1);
            rs.Select("ID,Station.BW, Station.Position.LONGITUDE,Station.Position.LATITUDE,Station.EQUIP_ID,Station.ID");
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                double staLon = rs.GetD("Station.Position.LONGITUDE");
                double staLat = rs.GetD("Station.Position.LATITUDE");

                if (Math.Abs(staLon - lon) + Math.Abs(staLat - lat) < mod)
                {
                    mod = Math.Abs(staLon - lon) + Math.Abs(staLat - lat);

                    double bw = rs.GetD("Station.BW");
                    int id = rs.GetI("ID");
                    eqpId = rs.GetI("Station.EQUIP_ID");

                    if (bw != 0 && bw != IM.NullD)
                        refSig.Bandwidth_kHz = bw;

                    refSig.IcsmId = rs.GetI("Station.ID");
                }
                result = true;
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            if (eqpId > 0)
            {
                var listFreq = new List<double>();
                var listLoss = new List<float>();
                string table;

                if (refSig.IcsmTable == "MOB_STATION")
                    table = "EQUIP_PMR_MPT";
                else
                    table = "EQUIP_MOB2_MPT";

                IMRecordset rsEqp = new IMRecordset(table, IMRecordset.Mode.ReadOnly);
                rsEqp.SetWhere("EQUIP_ID", IMRecordset.Operation.Eq, eqpId);
                rsEqp.SetWhere("TYPE", IMRecordset.Operation.Eq, "TS");
                rsEqp.Select("ATTN,FREQ");
                for (rsEqp.Open(); !rsEqp.IsEOF(); rsEqp.MoveNext())
                {
                    listLoss.Add((float)rsEqp.GetD("ATTN"));
                    listFreq.Add(1000 * rsEqp.GetD("FREQ"));
                }

                if (rsEqp.IsOpen())
                    rsEqp.Close();
                rsEqp.Destroy();

                if (listFreq.Count > 0 && listLoss.Count > 0)
                {
                    var signal = new SDR.SignalMask() { Freq_kHz = listFreq.ToArray(), Loss_dB = listLoss.ToArray() };
                    refSig.SignalMask = signal;
                }
            }
            return result;
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
                foreach (ShortSensorViewModel sensor in this._currentShortSensor)
                    DrawSensor(points, sensor.Id);
            else if (this._shortSensors.Source != null && this._shortSensors.Source.Length > 0)
                foreach (var sensor in this._shortSensors.Source)
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
                        .Select(l => this.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
                        .ToArray();
                    points.AddRange(sensorPoints);
                }
            }
        }
    }
}
