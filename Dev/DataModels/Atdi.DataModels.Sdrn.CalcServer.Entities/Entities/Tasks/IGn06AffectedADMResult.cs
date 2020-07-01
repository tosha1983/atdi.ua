using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
    [EntityPrimaryKey]
    public interface IGn06AffectedADMResult_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IGn06AffectedADMResult : IGn06AffectedADMResult_PK
    {
        long Gn06ResultId { get; set; }
        string ADM { get; set; }
        string TypeAffected { get; set; }
        string AffectedServices { get; set; }
    }
}