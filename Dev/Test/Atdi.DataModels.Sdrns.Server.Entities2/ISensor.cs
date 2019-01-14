using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities2
{
    [Entity]
    public interface ISensor
    {
        int Id { get; set; }

        int SensorIdentifierId { get; set; }

        string Status { get; set; }

        string Name { get; set; }

        string TechId { get; set; }
    }
}
