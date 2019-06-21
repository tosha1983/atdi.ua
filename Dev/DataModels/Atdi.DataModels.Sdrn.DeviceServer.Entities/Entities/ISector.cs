﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    public interface ISector_PK
    {
       long?  Id { get; set; }
    }

        [Entity]
    public interface ISector : ISector_PK
    {
        string SectorId { get; set; }
        double? Agl { get; set; }
        double? Eirp { get; set; }
        double? Azimut { get; set; }
        double? Bw { get; set; }
        string ClassEmission { get; set; }
        long? StationId { get; set; }
        IMeasStation STATION { get; set; }
    }
}
