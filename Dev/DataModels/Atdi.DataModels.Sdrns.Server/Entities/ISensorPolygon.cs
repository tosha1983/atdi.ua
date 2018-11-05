﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISensorPolygon
    {
        int Id { get; set; }
        int? SensorId { get; set; }
        int? Lon { get; set; }
        int? Lat { get; set; }
        ISensor SENSOR { get; set; }
    }
}