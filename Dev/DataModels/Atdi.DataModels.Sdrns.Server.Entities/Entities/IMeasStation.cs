﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasStation
    {
        int Id { get; set; }
        int? StationId { get; set; }
        string StationType { get; set; }
        int? MeasTaskId { get; set; }
        IStation STATION { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}