using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILinkResSensor
    {
        int Id { get; set; }
        int? SensorId { get; set; }
        int? ResMeasStaId { get; set; }
        ISensor SENSOR { get; set; }
        IResMeasStation RESMEASSTA { get; set; }
    }
}
