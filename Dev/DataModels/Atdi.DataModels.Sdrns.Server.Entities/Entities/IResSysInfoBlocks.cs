using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IResSysInfoBlocks_PK
    {
        long Id { get; set; }
    }
        [Entity]
    public interface IResSysInfoBlocks : IResSysInfoBlocks_PK
    {
        string Data { get; set; }
        string Type { get; set; }
        long? ResSysInfoId { get; set; }
        IResSysInfo RESSYSINFO { get; set; }
    }
}
