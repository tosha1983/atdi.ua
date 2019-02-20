using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IFSemples
    {
        int? Id { get; set; }
        double? Freq { get; set; }
        double? LeveldBm { get; set; }
        double? LeveldBmkVm { get; set; }
        double? LevelMindBm { get; set; }
        double? LevelMaxdBm { get; set; }
        double? OcupationPt { get; set; }
        int? IdMeasSdrResults { get; set; }
        IMeasSDRResults MEASSDRRESULTS { get; set; }
    }
}
