using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IXvResLevels
    {
        long Id { get; }
        long SensorId { get; }
        float? FreqMeas { get; set; }
        string TypeMeasurements { get; set; }
        DateTime? TimeMeas { get; set; }
        string TypeSpectrumOccupation { get; set; }
        float? OccupancySpect { get; set; }
        float? VMinLvl { get; set; }
        float? ValueLvl { get; set; }
        float? VMMaxLvl { get; set; }
        int? ScansNumber { get; set; }
        double? LevelMinOccup { get; set; }
        long TaskId { get; }
        string SensorName { get; }
        string SensorTitle { get; }
        double? Longitude { get; set; }
        double? Latitude { get; set; }
        double? Step { get; set; }
    }
}
