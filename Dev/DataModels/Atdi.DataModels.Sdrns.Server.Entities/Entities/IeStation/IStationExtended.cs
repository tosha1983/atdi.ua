using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface IStationExtended_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IStationExtended : IStationExtended_PK
    {
        int TableId { get; set; }
        string TableName { get; set; }
        string Standard { get; set; }
        string StandardName { get; set; }
        string OwnerName { get; set; }
        string PermissionNumber { get; set; }
        DateTime? PermissionStart { get; set; }
        DateTime? PermissionStop { get; set; }
        string Address { get; set; }
        double Longitude { get; set; }
        double Latitude { get; set; }
        double BandWidth { get; set; }
        string DesigEmission { get; set; }
        string Province { get; set; }
        string DocNum { get; set; }
        string CurentStatusStation { get; set; }
        string StatusMeas { get; set; }
        DateTime? TestStartDate { get; set; }
        DateTime? TestStopDate { get; set; }
        DateTime? PermissionCancelDate { get; set; }
        string PermissionGlobalSID { get; set; }
        float[] StationTxFreq { get; set; }
        float[] StationRxFreq { get; set; }
        string[] StationChannel { get; set; }
        string StationName { get; set; }
        string OKPO { get; set; }
    }
}
