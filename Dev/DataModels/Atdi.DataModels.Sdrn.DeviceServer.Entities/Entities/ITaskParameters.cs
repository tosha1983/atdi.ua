using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface ITaskParameters
    {
        int Id { get; set; }
        string SDRTaskId { get; set; }
        string MeasurementType { get; set; }
        double? MinFreq_MHz { get; set; }
        double? MaxFreq_MHz { get; set; }
        double? RBW_Hz { get; set; }
        double? VBW_Hz { get; set; }
        double? StepSO_kHz { get; set; }
        int? NChenal { get; set; }
        double? LevelMinOccup_dBm { get; set; }
        string Type_of_SO { get; set; }
        string Status { get; set; }
        double? ReceivedIQStreemDuration_sec { get; set; }
        string TypeTechnology { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? StopTime { get; set; }
        ITaskParametersFreq[] ListTaskParametersFreq { get; set; }
    }
}
