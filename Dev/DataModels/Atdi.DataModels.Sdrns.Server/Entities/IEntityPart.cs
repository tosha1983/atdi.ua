using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IEntityPart
    {
        int Id { get; set; }
        int PartIndex { get; set; }
        bool? Eof { get; set; }
        Byte[] Content{ get; set; }
        IEntity ENTITY { get; set; }
    }
}
