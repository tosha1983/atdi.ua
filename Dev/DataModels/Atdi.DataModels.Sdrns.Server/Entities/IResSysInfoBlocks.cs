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
        int Id { get; set; }
        string Data { get; set; }
        string Type { get; set; }
        int? ResSysInfoId { get; set; }
        IResSysInfo RESSYSINFO { get; set; }
    }
}
