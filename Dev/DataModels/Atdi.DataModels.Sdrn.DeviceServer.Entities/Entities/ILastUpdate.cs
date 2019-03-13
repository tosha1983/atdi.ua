using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface ILastUpdate
    {
        int? Id { get; set; }
        string TableName { get; set; }
        DateTime? LastUpdate { get; set; }
        string Status { get; set; }
    }
}
