using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILastUpdate_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface ILastUpdate : ILastUpdate_PK
    {
        string TableName { get; set; }
        DateTime? LastUpdate { get; set; }
        string Status { get; set; }
    }
}
