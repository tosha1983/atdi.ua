using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResLevMeasOnline_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IResLevMeasOnline : IResLevMeasOnline_PK
    {
        double? Value { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
