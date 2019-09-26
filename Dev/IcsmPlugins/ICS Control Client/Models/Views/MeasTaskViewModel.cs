using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows.Forms;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasTaskViewModel : IDataErrorInfo
    {

        public long Id { get; set; }

        public int? OrderId { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public MeasTaskExecutionMode ExecutionMode { get; set; }

        public MeasTaskType Task { get; set; }

        public int? Prio { get; set; }

        public MeasTaskResultType ResultType { get; set; }

        public IList<MeasTaskResultType> ResultTypeValues
        {
            get { return Enum.GetValues(typeof(MeasTaskResultType)).Cast<MeasTaskResultType>().ToList<MeasTaskResultType>(); }
        }

        public int? MaxTimeBs { get; set; }

        public DateTime? DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public MeasurementType MeasDtParamTypeMeasurements { get; set; }

        public IList<MeasurementType> MeasDtParamTypeMeasurementsValues
        {
            //get { return Enum.GetValues(typeof(MeasurementType)).Cast<MeasurementType>().ToList<MeasurementType>(); }
            get { return new List<MeasurementType>() { MeasurementType.Level, MeasurementType.SpectrumOccupation, MeasurementType.Signaling }; }
        }

        public bool IsAutoMeasDtParamRBW { get; set; }
        public double? MeasDtParamRBW { get; set; }

        public bool IsAutoMeasDtParamVBW { get; set; }
        public double? MeasDtParamVBW { get; set; }

        public bool IsAutoMeasDtParamRfAttenuation { get; set; }
        public double? MeasDtParamRfAttenuation { get; set; }

        public double? MeasDtParamIfAttenuation { get; set; }

        public bool IsAutoMeasDtParamMeasTime { get; set; }
        public double? MeasDtParamMeasTime { get; set; }

        public DetectingType MeasDtParamDetectType { get; set; }

        public IList<DetectingType> MeasDtParamDetectTypeValues
        {
            get { return Enum.GetValues(typeof(DetectingType)).Cast<DetectingType>().ToList<DetectingType>(); }
        }

        public string MeasDtParamDemod { get; set; }

        public bool IsAutoMeasDtParamPreamplification { get; set; }
        public int? MeasDtParamPreamplification { get; set; }

        public bool IsAutoMeasDtParamReferenceLevel { get; set; }
        public int? MeasDtParamReferenceLevel { get; set; }

        public MeasurementMode MeasDtParamMode { get; set; }

        public FrequencyMode MeasFreqParamMode { get; set; }

        public IList<FrequencyMode> MeasFreqParamModeValues
        {
            get { return Enum.GetValues(typeof(FrequencyMode)).Cast<FrequencyMode>().ToList<FrequencyMode>(); }
        }
        public double? MeasFreqParamRgL { get; set; }

        public double? MeasFreqParamRgU { get; set; }

        public double? MeasFreqParamStep { get; set; }

        public double? MeasFreqParam { get; set; }
        public string MeasFreqParams { get; set; }

        public double[] MeasFreqParamMeasFreqs { get; set; }

        public int? MeasOtherSwNumber { get; set; }

        public SpectrumScanType MeasOtherTypeSpectrumScan { get; set; }

        public IList<SpectrumScanType> MeasOtherTypeSpectrumScanValues
        {
            get { return Enum.GetValues(typeof(SpectrumScanType)).Cast<SpectrumScanType>().ToList<SpectrumScanType>(); }
        }

        public SpectrumOccupationType MeasOtherTypeSpectrumOccupation { get; set; }

        public IList<SpectrumOccupationType> MeasOtherTypeSpectrumOccupationValues
        {
            get { return Enum.GetValues(typeof(SpectrumOccupationType)).Cast<SpectrumOccupationType>().ToList<SpectrumOccupationType>(); }
        }

        public double? MeasOtherLevelMinOccup { get; set; }

        public int? MeasOtherNCount { get; set; }
        public int? MeasOtherNChenal { get; set; }


        public DateTime MeasTimeParamListPerStart { get; set; }

        public DateTime MeasTimeParamListPerStop { get; set; }

        public DateTime? MeasTimeParamListTimeStart { get; set; }

        public DateTime? MeasTimeParamListTimeStop { get; set; }

        public string MeasTimeParamListDays { get; set; }

        public double? MeasTimeParamListPerInterval { get; set; }

        public StationDataForMeasurements[] StationsForMeasurements { get; set; }

        public MeasSensor[] Sensors { get; set; }

        public SignalingMeasTask SignalingMeasTaskParameters { get; set; }


        public bool? FiltrationTrace { get; set; }
        public double? windowBW { get; set; }
        public double? AllowableExcess_dB { get; set; }
        public bool IsAutoTriggerLevel_dBm_Hz { get; set; }
        public double? triggerLevel_dBm_Hz { get; set; }
        public double? CrossingBWPercentageForGoodSignals { get; set; }
        public double? CrossingBWPercentageForBadSignals { get; set; }
        public double? DiffLevelForCalcBW { get; set; }
        public bool? CorrelationAnalize { get; set; }
        public double? CorrelationFactor { get; set; }
        public int? SignalizationNCount { get; set; }
        public int? SignalizationNChenal { get; set; }
        public bool? AnalyzeByChannel { get; set; }

        public bool? CompareTraceJustWithRefLevels { get; set; }
        public bool? AutoDivisionEmitting { get; set; }
        public double? DifferenceMaxMax { get; set; }
        public int? NumberPointForChangeExcess { get; set; }
        public bool? DetailedMeasurementsBWEmission { get; set; }
        public int? MinPointForDetailBW { get; set; }

        public bool? CheckFreqChannel { get; set; }
        public double? MaxFreqDeviation { get; set; }
        public bool? CheckLevelChannel { get; set; }
        public string Standard { get; set; }

        public bool? AnalyzeSysInfoEmission { get; set; }
        public double? nDbLevel_dB { get; set; } 
        public int? NumberIgnoredPoints { get; set; } 
        public double? MinExcessNoseLevel_dB { get; set; }
        public int? TimeBetweenWorkTimes_sec { get; set; }
        public int? TypeJoinSpectrum { get; set; }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "MeasOtherSwNumber":
                        if ((MeasOtherSwNumber < 1))
                        {
                            error = "The value must be greater than zero";
                        }
                        break;
                    case "MeasTimeParamListPerStart":
                        if (MeasTimeParamListPerStart < DateTime.Today)
                        {
                            error = "Date cannot be less than current";
                        }
                        //if (MeasTimeParamListPerStart > MeasTimeParamListPerStop)
                        //{
                        //    error = "Start date cannot be greater than end date";
                        //}
                        break;
                    case "MeasTimeParamListPerStop":
                        if (MeasTimeParamListPerStop < DateTime.Today)
                        {
                            error = "Date cannot be less than current";
                        }
                        //if (MeasTimeParamListPerStart > MeasTimeParamListPerStop)
                        //{
                        //    error = "Start date cannot be greater than end date";
                        //}
                        break;
                    case "MeasDtParamMeasTime":
                        if (MeasDtParamMeasTime.HasValue && (MeasDtParamMeasTime.Value < 0.001 || MeasDtParamMeasTime.Value > 1))
                        {
                            error = "The value must be in the range from 0.001 to 1";
                        }
                        break;
                    case "MeasDtParamRfAttenuation":
                        if (MeasDtParamRfAttenuation.HasValue && (MeasDtParamRfAttenuation.Value < 0 || MeasDtParamRfAttenuation.Value > 40))
                        {
                            error = "The value must be in the range from 0 to 40";
                        }
                        if (MeasDtParamRfAttenuation.HasValue && (MeasDtParamRfAttenuation.Value != Math.Round(MeasDtParamRfAttenuation.Value, 0)))
                        {
                            error = "The value must be an integer";
                        }
                        break;
                    case "MeasDtParamPreamplification":
                        if (MeasDtParamPreamplification.HasValue && (MeasDtParamPreamplification.Value < 0 || MeasDtParamPreamplification.Value > 40))
                        {
                            error = "The value must be in the range from 0 to 40";
                        }
                        break;
                    case "MeasDtParamReferenceLevel":
                        if (MeasDtParamReferenceLevel.HasValue && (MeasDtParamReferenceLevel.Value < -200 || MeasDtParamReferenceLevel.Value > 10))
                        {
                            error = "The value must be in the range from -200 to 10";
                        }
                        break;
                    case "MeasOtherLevelMinOccup":
                        if (MeasOtherLevelMinOccup.HasValue && (MeasOtherLevelMinOccup.Value < -160 || MeasOtherLevelMinOccup.Value > -30))
                        {
                            error = "The value must be in the range from -160 to -30";
                        }
                        break;
                    case "MeasOtherNCount":
                        if (MeasOtherNCount.HasValue && (MeasOtherNCount.Value < 1 || MeasOtherNCount.Value > 1000000))
                        {
                            error = "The value must be in the range from 1 to 1000000";
                        }
                        break;
                    case "MeasOtherNChenal":
                        if (MeasOtherNChenal.HasValue && (MeasOtherNChenal.Value < 10 || MeasOtherNChenal.Value > 400))
                        {
                            error = "The value must be in the range from 10 to 400";
                        }
                        break;
                    case "windowBW":
                        if (windowBW.HasValue && (windowBW.Value < 1 || windowBW.Value > 2))
                        {
                            error = "The value must be in the range from 1 to 2";
                        }
                        break;
                    case "AllowableExcess_dB":
                        if (AllowableExcess_dB.HasValue && (AllowableExcess_dB.Value < 0 || AllowableExcess_dB.Value > 50))
                        {
                            error = "The value must be in the range from 0 to 50";
                        }
                        break;
                    case "triggerLevel_dBm_Hz":
                        if (triggerLevel_dBm_Hz.HasValue && (triggerLevel_dBm_Hz.Value < -200 || triggerLevel_dBm_Hz.Value > -100))
                        {
                            error = "The value must be in the range from -200 to -100";
                        }
                        break;
                    case "CrossingBWPercentageForGoodSignals":
                        if (CrossingBWPercentageForGoodSignals.HasValue && (CrossingBWPercentageForGoodSignals.Value < 1 || CrossingBWPercentageForGoodSignals.Value > 99))
                        {
                            error = "The value must be in the range from 1 to 99";
                        }
                        break;
                    case "CrossingBWPercentageForBadSignals":
                        if (MeasOtherNChenal.HasValue && (CrossingBWPercentageForBadSignals.Value < 1 || CrossingBWPercentageForBadSignals.Value > 99))
                        {
                            error = "The value must be in the range from 1 to 99";
                        }
                        break;
                    case "DiffLevelForCalcBW":
                        if (DiffLevelForCalcBW.HasValue && (DiffLevelForCalcBW.Value < 6 || DiffLevelForCalcBW.Value > 36))
                        {
                            error = "The value must be in the range from 6 to 36";
                        }
                        break;
                    case "CorrelationFactor":
                        if (CorrelationFactor.HasValue && (CorrelationFactor.Value < 0 || CorrelationFactor.Value > 1))
                        {
                            error = "The value must be in the range from 0 to 1";
                        }
                        break;
                    case "SignalizationNCount":
                        if (SignalizationNCount.HasValue && (SignalizationNCount.Value < 1 || SignalizationNCount.Value > 1000000))
                        {
                            error = "The value must be in the range from 1 to 1000000";
                        }
                        break;
                    case "SignalizationNChenal":
                        if (SignalizationNChenal.HasValue && (SignalizationNChenal.Value < 2 || SignalizationNChenal.Value > 5000))
                        {
                            error = "The value must be in the range from 2 to 5000";
                        }
                        break;
                    case "DifferenceMaxMax":
                        if (DifferenceMaxMax.HasValue && (DifferenceMaxMax.Value < 5 || DifferenceMaxMax.Value > 40))
                        {
                            error = "The value must be in the range from 5 to 40";
                        }
                        break;
                    case "NumberPointForChangeExcess":
                        if (NumberPointForChangeExcess.HasValue && (NumberPointForChangeExcess.Value < 0 || NumberPointForChangeExcess.Value > 100))
                        {
                            error = "The value must be in the range from 0 to 100";
                        }
                        break;
                    case "MaxFreqDeviation":
                        if (MaxFreqDeviation.HasValue && (MaxFreqDeviation.Value < 0.000000001 || MaxFreqDeviation.Value > 0.0001))
                        {
                            error = "The value must be in the range from 0.000000001 to 0.0001";
                        }
                        break;
                }
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        public bool ValidateStateModel()
        {
            var errors = new StringBuilder();

            var properties = new string[]
            {
                "MeasTimeParamListPerStart",
                "MeasTimeParamListPerStop",
                "MeasTimeParamListTimeStart",
                "MeasTimeParamListTimeStop",
                "MeasDtParamMeasTime",
                "MeasDtParamRfAttenuation",
                "MeasDtParamPreamplification",
                "MeasDtParamReferenceLevel",
                "MeasOtherSwNumber",
                "MeasOtherLevelMinOccup",
                "MeasOtherNCount",
                "MeasOtherNChenal",
                "windowBW",
                "AllowableExcess_dB",
                "triggerLevel_dBm_Hz",
                "CrossingBWPercentageForGoodSignals",
                "CrossingBWPercentageForBadSignals",
                "DiffLevelForCalcBW",
                "CorrelationFactor",
                "SignalizationNCount",
                "SignalizationNChenal",
                "DifferenceMaxMax",
                "NumberPointForChangeExcess",
                "MaxFreqDeviation"
            };

            for (int i = 0; i < properties.Length; i++)
            {
                var propertyName = properties[i];
                var error = this[propertyName];
                if (!string.IsNullOrEmpty(error))
                {
                    errors.AppendLine($" - invalid value of the {propertyName}: {error}");
                }
            }

            if (errors.Length > 0)
            {
                //throw new InvalidOperationException("Invalid input task properties: \n" + errors.ToString());
                MessageBox.Show("Invalid input task properties: \n" + errors.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
