using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface ILinkSectorFreq_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkSectorFreq: ILinkSectorFreq_PK
    {
        long? SectorFreqId { get; set; }
        long? SectorId { get; set; }
        ISectorFreq SECTORFREQ { get; set; }
        ISector SECTOR { get; set; }
    }
}
