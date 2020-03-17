using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface IAreaLocation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IAreaLocation : IAreaLocation_PK
    {
        double Longitude { get; set; }
        double Latitude { get; set; }
        IArea AREA { get; set; }
    }
}
