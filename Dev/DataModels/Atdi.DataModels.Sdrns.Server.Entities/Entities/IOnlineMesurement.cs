using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IOnlineMesurement_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IOnlineMesurement : IOnlineMesurement_PK
    {
        DateTimeOffset CreatedDate { get; set; }

        Guid ServerToken { get; set; }

        byte StatusCode { get; set; }

        string StatusNote { get; set; }

        ISensor SENSOR { get; set; }

        int PeriodMinutes { get; set; }

        DateTimeOffset? StartTime { get; set; }

        DateTimeOffset? FinishTime { get; set; }

        byte[] SensorToken { get; set; }

        string WebSocketUrl { get; set; }
    }
}
