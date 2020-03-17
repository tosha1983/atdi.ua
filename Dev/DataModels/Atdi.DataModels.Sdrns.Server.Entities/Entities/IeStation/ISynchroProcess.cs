using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface ISynchroProcess_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISynchroProcess : ISynchroProcess_PK
    {
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        string Status { get; set; }
        DateTime DateStart { get; set; }
        DateTime DateEnd { get; set; }
        int? CountRecordsImported { get; set; }
        int? CountRecordsOutput { get; set; }
        int? CountRecordsOutputWithoutEmitting { get; set; }
    }
}
