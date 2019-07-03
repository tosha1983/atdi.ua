using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasStation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasStation: IMeasStation_PK
    {
        string StationType { get; set; }
        IStation STATION { get; set; }
        IMeasTask MEAS_TASK { get; set; }
    }
}
