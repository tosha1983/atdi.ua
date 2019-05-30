using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IReferenceSituation
    {
        int Id { get; set; }
        int? SensorId { get; set; }
        int? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
        ISensor SENSOR { get; set; }
    }
}

