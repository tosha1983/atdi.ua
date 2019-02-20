﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface ISensorPolygon
    {
        int Id { get; set; }
        int? SensorId { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        ISensor SENSOR { get; set; }
    }
}
