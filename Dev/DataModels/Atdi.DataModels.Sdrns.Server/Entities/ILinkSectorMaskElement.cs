using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILinkSectorMaskElement
    {
        int Id { get; set; }
        int? SectorMaskElementId { get; set; }
        int? SectorId { get; set; }
        ISectorMaskElement SECTORMASKELEMENT { get; set; }
        ISector SECTOR { get; set; }
    }
}
