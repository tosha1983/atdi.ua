using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IResultsLocation
    {
        int Id { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        int? IdMeasSdrResults { get; set; }
        IMeasSDRResults MEASSDRRESULTS { get; set; }
    }
}
