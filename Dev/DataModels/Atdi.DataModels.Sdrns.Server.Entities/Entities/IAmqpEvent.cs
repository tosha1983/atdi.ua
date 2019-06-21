using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IAmqpEvent_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IAmqpEvent : IAmqpEvent_PK
    {
        string PropType { get; set; }

        DateTimeOffset CreatedDate { get; set; }

    }

}
