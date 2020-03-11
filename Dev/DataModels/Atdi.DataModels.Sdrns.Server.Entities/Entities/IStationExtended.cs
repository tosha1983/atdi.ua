using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
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
    }
}
