using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IReferenceLevels
    {
        long Id { get; set; }
        double? StartFrequency_Hz { get; set; }
        double? StepFrequency_Hz { get; set; }
        //byte[] ReferenceLevels { get; set; }
        float[] RefLevels { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}