using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IResLevels_PK
    {
        long Id { get; set; }
    }


    [Entity]
    public interface IResLevels : IResLevels_PK
    {
        float? ValueLvl { get; set; }
        double? StddevLev { get; set; }
        float? VMinLvl { get; set; }
        float? VMMaxLvl { get; set; }
        double? LimitLvl { get; set; }
        double? OccupancyLvl { get; set; }
        double? PMinLvl { get; set; }
        double? PMaxLvl { get; set; }
        double? PDiffLvl { get; set; }
        float? FreqMeas { get; set; }
        float? ValueSpect { get; set; }
        double? StdDevSpect { get; set; }
        double? VMinSpect { get; set; }
        double? VMMaxSpect { get; set; }
        double? LimitSpect { get; set; }
        float? OccupancySpect { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
