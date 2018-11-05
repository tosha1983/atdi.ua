using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities2
{
    [Entity]
    public interface IEntity
    {
        int Id { get; set; }

        string EntityId { get; set; }

        string Name { get; set; }

        byte[] Content { get; set; }
    }
}
