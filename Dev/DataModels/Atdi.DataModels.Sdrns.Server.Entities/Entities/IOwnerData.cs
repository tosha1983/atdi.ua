using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IOwnerData_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IOwnerData : IOwnerData_PK
    {
        string OwnerName { get; set; }
        string OKPO { get; set; }
        string ZIP { get; set; }
        string CODE { get; set; }
        string Address { get; set; }
        long? StationDatFormId { get; set; }
    }
}
