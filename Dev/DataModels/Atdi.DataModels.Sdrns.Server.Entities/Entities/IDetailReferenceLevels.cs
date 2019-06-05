using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IDetailReferenceLevels
    {
        long Id { get; set; }
        double? level { get; set; }
        long? ReferenceLevelId { get; set; }
        IReferenceLevels REFLEVELS { get; set; }
    }
}

