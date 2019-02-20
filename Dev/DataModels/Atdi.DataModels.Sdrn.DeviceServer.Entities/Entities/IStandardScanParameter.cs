using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IStandardScanParameter
    {
        int Id { get; set; }
        string Standard { get; set; }
        double? MeasFreqRelative { get; set; }
        double? LevelDbm { get; set; }
        double? DetectLevelDbm { get; set; }
        double? MaxPermissBW { get; set; }
        double? RBW_kHz { get; set; }
        double? VBW_kHz { get; set; }
        double? ScanBW_kHz { get; set; }
        double? MeasTime_sec { get; set; }
        double? RefLevel_dBm { get; set; }
        string DetectType { get; set; }
        int? Preamplification_dB { get; set; }
        int? RfAttenuation_dB { get; set; }
        int? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
