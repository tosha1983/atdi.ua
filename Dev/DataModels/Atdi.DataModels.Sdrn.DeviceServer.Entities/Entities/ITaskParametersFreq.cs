using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface ITaskParametersFreq
    {
        int? Id { get; set; }
        double? FreqCH { get; set; }
        int? IdTaskParameters { get; set; }
        ITaskParameters TASKPARAMETERS { get; set; }
    }
}
