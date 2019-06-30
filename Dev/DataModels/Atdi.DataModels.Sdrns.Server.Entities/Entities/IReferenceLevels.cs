using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IReferenceLevels_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IReferenceLevels : IReferenceLevels_PK
    {
        double? StartFrequency_Hz { get; set; }
        double? StepFrequency_Hz { get; set; }
        float[] RefLevels { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}