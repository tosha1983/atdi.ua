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
        int Id { get; set; }
        double? StartFrequency_Hz { get; set; }
        double? StepFrequency_Hz { get; set; }
        byte[] ReferenceLevels { get; set; }
        int? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}