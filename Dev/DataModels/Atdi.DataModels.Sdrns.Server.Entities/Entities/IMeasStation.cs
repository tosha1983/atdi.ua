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
        long? IdStation { get; set; }
        string StationType { get; set; }
        IMeasTask MEAS_TASK { get; set; }
    }
}
