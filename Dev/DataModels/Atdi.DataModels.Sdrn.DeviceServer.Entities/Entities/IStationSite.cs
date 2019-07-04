using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IStationSite_PK
    {
        long? Id { get; set; }

    }

    [Entity]
    public interface IStationSite : IStationSite_PK
    {
        double? Lon { get; set; }
        double Lat { get; set; }
        string Address { get; set; }
        string Region { get; set; }
    }
}
