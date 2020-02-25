using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IProtocols_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IProtocols : IProtocols_PK
    {
        string PermissionNumber { get; set; }
        DateTime? PermissionStart { get; set; }
        DateTime? PermissionStop { get; set; }
        string GlobalSID { get; set; }
        double Freq_MHz { get; set; }
        double Level_dBm { get; set; }
        DateTime DateMeas { get; set; }
        double SensorLon { get; set; }
        double SensorLat { get; set; }
        string SensorName { get; set; }
        double? DispersionLow { get; set; }
        double? DispersionUp { get; set; }
        double? Percent { get; set; }
        long? SensorId { get; set; }
        IStationExtended STATION_EXTENDED { get; set; }
        ISynchroProcess SYNCHRO_PROCESS { get; set; }
    }
}
