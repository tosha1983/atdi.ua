using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResStLevelsSpectRaw
    {
        int Id { get; set; }
        int? ResStGeneralId { get; set; }
        float? LevelSpecrum { get; set; }
        IResStGeneralRaw RESSTGENERAL { get; set; }
    }
}
