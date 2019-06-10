using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResSysInfoBlocks
    {
        long Id { get; set; }
        string Data { get; set; }
        byte[]  BinData { get; set; }
        string Type { get; set; }
        long? ResSysInfoId { get; set; }
        IResSysInfo RESSYSINFO { get; set; }
    }
}
