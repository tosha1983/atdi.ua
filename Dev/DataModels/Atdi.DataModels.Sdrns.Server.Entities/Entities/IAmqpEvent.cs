using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IAmqpEvent
    {
        long Id { get; set; }

        string PropType { get; set; }

        DateTimeOffset CreatedDate { get; set; }

    }
}
