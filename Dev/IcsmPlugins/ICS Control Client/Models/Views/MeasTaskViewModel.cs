using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

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
        public double MeasDtParamRfAttenuation { get; set; }

        public double MeasDtParamIfAttenuation { get; set; }

        public bool IsAutoMeasDtParamMeasTime { get; set; }
        public double? MeasDtParamMeasTime { get; set; }

        public DetectingType MeasDtParamDetectType { get; set; }

        public IList<DetectingType> MeasDtParamDetectTypeValues
        {
            get { return Enum.GetValues(typeof(DetectingType)).Cast<DetectingType>().ToList<DetectingType>(); }
        }

        public string MeasDtParamDemod { get; set; }

        public bool IsAutoMeasDtParamPreamplification { get; set; }
        public int MeasDtParamPreamplification { get; set; }

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
                    case "MeasTimeParamListPerInterval":
                        if ((MeasTimeParamListPerInterval < 0) || (MeasTimeParamListPerInterval > 3600))
                        {
                            error = "The value must be greater than 0 and less than 3600";
                        }
                        break;
                    case "MeasFreqParamRgL":
                        if ((MeasFreqParamRgL < 1) || (MeasFreqParamRgL > 6000))
                        {
                            error = "The value must be greater than 1 and less than 6000";
                        }
                        break;
                    case "MeasFreqParamRgU":
                        if ((MeasFreqParamRgU < 1) || (MeasFreqParamRgU > 6000))
                        {
                            error = "The value must be greater than 1 and less than 6000";
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

    }
}
