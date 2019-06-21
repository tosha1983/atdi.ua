using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    public interface ITaskParametersFreq_PK
    {
        long? Id { get; set; }
    }


        [Entity]
    public interface ITaskParametersFreq : ITaskParametersFreq_PK
    {
        double? FreqCH { get; set; }
        long? IdTaskParameters { get; set; }
        ITaskParameters TASKPARAMETERS { get; set; }
    }
}
