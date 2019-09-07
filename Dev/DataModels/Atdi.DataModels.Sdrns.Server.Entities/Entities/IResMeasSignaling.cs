using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResMeasSignaling_PK
    {
        long Id { get; set; }
    }
    [Entity]
    public interface IResMeasSignaling : IResMeasSignaling_PK
    {
        bool IsSend { get; set; }
        IResMeas RES_MEAS { get; set; }
    }
}
