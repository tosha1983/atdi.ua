﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResLevels
    {
        int Id { get; set; }
        double? ValueLvl { get; set; }
        double? StddevLev { get; set; }
        double? VMinLvl { get; set; }
        double? VMMaxLvl { get; set; }
        double? LimitLvl { get; set; }
        double? OccupancyLvl { get; set; }
        double? PMinLvl { get; set; }
        double? PMaxLvl { get; set; }
        double? PDiffLvl { get; set; }
        double? FreqMeas { get; set; }
        double? ValueSpect { get; set; }
        double? StdDevSpect { get; set; }
        double? VMinSpect { get; set; }
        double? VMMaxSpect { get; set; }
        double? LimitSpect { get; set; }
        double? OccupancySpect { get; set; }
        int? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}