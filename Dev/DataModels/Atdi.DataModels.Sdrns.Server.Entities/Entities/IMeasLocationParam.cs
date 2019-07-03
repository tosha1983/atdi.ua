using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasLocationParam_PK
    {
        long Id { get; set; }
    }
    [Entity]
    public interface IMeasLocationParam: IMeasLocationParam_PK
    {
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        double? MaxDist { get; set; }
        IMeasTask MEAS_TASK { get; set; }
    }
}
