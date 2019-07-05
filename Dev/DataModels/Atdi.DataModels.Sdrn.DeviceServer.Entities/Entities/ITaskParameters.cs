using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ITaskParameters_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface ITaskParameters : ITaskParameters_PK
    {
        string SDRTaskId { get; set; }
        string MeasurementType { get; set; }
        double? MinFreq_MHz { get; set; }
        double? MaxFreq_MHz { get; set; }
        double? RBW_Hz { get; set; }
        double? VBW_Hz { get; set; }
        double? StepSO_kHz { get; set; }
        double? SweepTime_ms { get; set; }
        int? NChenal { get; set; }
        double? LevelMinOccup_dBm { get; set; }
        string Type_of_SO { get; set; }
        string Status { get; set; }
        double? ReceivedIQStreemDuration_sec { get; set; }
        string TypeTechnology { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? StopTime { get; set; }
        int? NCount { get; set; }
        int SensorId { get; set; }
        ITaskParametersFreq[] ListTaskParametersFreq { get; set; }
        bool CompareTraceJustWithRefLevels { get; set; }
        bool AutoDivisionEmitting { get; set; }
        double? DifferenceMaxMax { get; set; }
        bool FiltrationTrace { get; set; }
        double? AllowableExcess_dB { get; set; }
        double? PercentForCalcNoise { get; set; }
        int? SignalizationNChenal { get; set; }
        int? SignalizationNCount { get; set; }
    }
}
