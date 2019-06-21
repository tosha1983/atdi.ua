using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IEntityPart_PK
    {
        string EntityId { get; set; }
        int PartIndex { get; set; }
    }

    [Entity]
    public interface IEntityPart: IEntityPart_PK
    {
        bool? Eof { get; set; }
        Byte[] Content{ get; set; }
        IEntity ENTITY { get; set; }
    }
}
