using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface ISectorMaskElement_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISectorMaskElement : ISectorMaskElement_PK
    {
        double? Level { get; set; }
        double? Bw { get; set; }
    }
}
