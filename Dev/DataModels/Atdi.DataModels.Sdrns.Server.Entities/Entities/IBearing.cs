using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IBearing
    {
        int Id { get; set; }
        int? ResMeasStaId { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        double? Agl { get; set; }
        double? Level_dBm { get; set; }
        double? Level_dBmkVm { get; set; }
        DateTime MeasurementTime { get; set; }
        double? Bandwidth_kHz { get; set; }
        double? Quality { get; set; }
        double CentralFrequency_MHz { get; set; }
        double Bearing { get; set; }
        double? AntennaAzimut { get; set; }
        IResMeasStaRaw RESMEASSTA { get; set; }
    }
}
