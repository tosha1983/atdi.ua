using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILinkSectorFreq
    {
        int Id { get; set; }
        int? SectorFreqId { get; set; }
        int? SectorId { get; set; }
        ISectorFreq SECTORFREQ { get; set; }
        ISector SECTOR { get; set; }
    }
}
