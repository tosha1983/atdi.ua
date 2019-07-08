using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResStMaskElement_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IResStMaskElement : IResStMaskElement_PK
    {
        double? Bw { get; set; }
        double? Level { get; set; }
        IResStGeneral RES_STGENERAL { get; set; }
    }
}
