using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface IArea_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IArea : IArea_PK
    {
        string Name { get; set; }
        string TypeOfArea { get; set; }
        string CreatedBy { get; set; }
        DateTime? CreatedDate { get; set; }
        int IdentifierFromICSM { get; set; }
    }

}
