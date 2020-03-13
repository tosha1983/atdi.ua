using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkArea_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkArea : ILinkArea_PK
    {
        IArea AREA { get; set; }
        ISynchroProcess SYNCHRO_PROCESS { get; set; }
    }

}
