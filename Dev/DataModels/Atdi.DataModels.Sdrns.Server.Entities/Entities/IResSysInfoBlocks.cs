using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResSysInfoBlocks_PK
    {
        long Id { get; set; }
    }
    [Entity]
    public interface IResSysInfoBlocks : IResSysInfoBlocks_PK
    {
        string Data { get; set; }
        string Type { get; set; }
        IResSysInfo RES_SYS_INFO { get; set; }
    }
}
