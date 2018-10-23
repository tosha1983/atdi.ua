using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISensorAntenna
    {
        int Id { get; set; }

        int SensorId { get; set; }

        string Code { get; set; }

        string Name { get; set; }

        string TechId { get; set; }

        ISensor SENSOR { get; set; }
    }
}
