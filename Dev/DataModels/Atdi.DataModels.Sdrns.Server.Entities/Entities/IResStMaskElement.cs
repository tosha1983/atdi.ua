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
        int Id { get; set; }
        int? ResStGeneralId { get; set; }
        double? Bw { get; set; }
        double? Level { get; set; }
        IResStGeneral RESSTGENERAL { get; set; }
    }
}
