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
        long Id { get; set; }
        long? SectorFreqId { get; set; }
        long? SectorId { get; set; }
        ISectorFreq SECTORFREQ { get; set; }
        ISector SECTOR { get; set; }
    }
}
