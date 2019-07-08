using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResMeasStation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IResMeasStation : IResMeasStation_PK
    {
        string GlobalSID { get; set; }
        string MeasGlobalSID { get; set; }
        long? IdStation { get; set; }
        string Status { get; set; }
        string Standard { get; set; }
        ISector SECTOR { get; set; }
        IResMeas RES_MEAS { get; set; }
        IStation STATION { get; set; }
    }
}
