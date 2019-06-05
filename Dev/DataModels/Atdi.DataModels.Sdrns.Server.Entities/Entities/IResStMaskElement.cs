using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResStMaskElement
    {
        long Id { get; set; }
        long? ResStGeneralId { get; set; }
        double? Bw { get; set; }
        double? Level { get; set; }
        IResStGeneral RESSTGENERAL { get; set; }
    }
}
