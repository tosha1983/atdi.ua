using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasTaskViewModel
    {

        public int Id { get; set; }

        public int? OrderId { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public MeasTaskExecutionMode ExecutionMode { get; set; }

        public MeasTaskType Task { get; set; }

        public int? Prio { get; set; }

        public MeasTaskResultType ResultType { get; set; }

        public int? MaxTimeBs { get; set; }

        public DateTime? DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public MeasurementType MeasDtParamTypeMeasurements { get; set; }

        public double? MeasDtParamRBW { get; set; }

        public double? MeasDtParamVBW { get; set; }

        public double MeasDtParamRfAttenuation { get; set; }

        public double MeasDtParamIfAttenuation { get; set; }

        public double? MeasDtParamMeasTime { get; set; }

        public DetectingType MeasDtParamDetectType { get; set; }

        public string MeasDtParamDemod { get; set; }

        public int MeasDtParamPreamplification { get; set; }

        public MeasurementMode MeasDtParamMode { get; set; }


        public FrequencyMode MeasFreqParamMode { get; set; }

        public double? MeasFreqParamRgL { get; set; }

        public double? MeasFreqParamRgU { get; set; }

        public double? MeasFreqParamStep { get; set; }

        public double[] MeasFreqParamMeasFreqs { get; set; }


        public int? MeasOtherSwNumber { get; set; }

        public SpectrumScanType MeasOtherTypeSpectrumScan { get; set; }

        public SpectrumOccupationType MeasOtherTypeSpectrumOccupation { get; set; }

        public double? MeasOtherLevelMinOccup { get; set; }

        public int? MeasOtherNChenal { get; set; }


        public DateTime MeasTimeParamListPerStart { get; set; }

        public DateTime MeasTimeParamListPerStop { get; set; }

        public DateTime? MeasTimeParamListTimeStart { get; set; }

        public DateTime? MeasTimeParamListTimeStop { get; set; }

        public string MeasTimeParamListDays { get; set; }

        public double? MeasTimeParamListPerInterval { get; set; }

        public StationDataForMeasurements[] StationsForMeasurements { get; set; }

        public MeasStation[] Stations { get; set; }

    }
}
