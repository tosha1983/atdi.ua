using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkMeasStation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkMeasStation: ILinkMeasStation_PK
    {
        IMeasTask MEAS_TASK { get; set; }
        IStation STATION { get; set; }
    }
}
