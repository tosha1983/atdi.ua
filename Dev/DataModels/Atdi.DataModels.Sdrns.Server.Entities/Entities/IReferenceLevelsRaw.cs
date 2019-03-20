using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IReferenceLevelsRaw
    {
        int Id { get; set; }
        double? StartFrequency_Hz { get; set; }
        double? StepFrequency_Hz { get; set; }
        int? ResMeasId { get; set; }
        IResMeasRaw RESMEASRAW { get; set; }
    }
}