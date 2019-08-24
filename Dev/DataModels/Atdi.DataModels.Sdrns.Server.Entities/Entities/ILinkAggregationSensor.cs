using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkAggregationSensor_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkAggregationSensor : ILinkAggregationSensor_PK
    {
        ISensor SENSOR { get; set; }
        string AggregationServerName { get; set; }
    }
}
