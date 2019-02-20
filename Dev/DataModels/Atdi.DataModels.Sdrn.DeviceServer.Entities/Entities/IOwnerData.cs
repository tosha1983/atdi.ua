using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IOwnerData
    {
        int Id { get; set; }
        string OwnerName { get; set; }
        string OKPO { get; set; }
        string ZIP { get; set; }
        string CODE { get; set; }
        string Address { get; set; }
    }
}
