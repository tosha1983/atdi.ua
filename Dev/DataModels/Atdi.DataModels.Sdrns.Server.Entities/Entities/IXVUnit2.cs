using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IXVUnit2
    {
        double? Lon { get; }
        double? Lat { get; }
        double? LeveldBm { get; }
        double? CentralFrequency { get; }
        DateTime? TimeOfMeasurements { get; }
        double? BW { get; }
        int? IdStation { get; }
        double? SpecrumSteps { get; }
        int? T1 { get; }
        int? T2 { get; }
        long Id { get; }
        string MeasGlobalSid { get; }
    }
}
