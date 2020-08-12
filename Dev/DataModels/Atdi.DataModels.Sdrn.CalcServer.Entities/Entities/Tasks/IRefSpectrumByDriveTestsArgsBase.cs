using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{

    [Entity]
    public interface IRefSpectrumByDriveTestsArgsBase
    {
        long ResultId { get; set; }
        long[] StationIds { get; set; }
        float PowerThreshold_dBm { get; set; }
        string Comments { get; set; }
    }
    
}
