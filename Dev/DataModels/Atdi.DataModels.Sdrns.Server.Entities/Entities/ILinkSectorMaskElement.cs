using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface ILinkSectorMaskElement_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkSectorMaskElement: ILinkSectorMaskElement_PK
    {
        long? SectorMaskElementId { get; set; }
        long? SectorId { get; set; }
        ISectorMaskElement SECTORMASKELEMENT { get; set; }
        ISector SECTOR { get; set; }
    }
}
