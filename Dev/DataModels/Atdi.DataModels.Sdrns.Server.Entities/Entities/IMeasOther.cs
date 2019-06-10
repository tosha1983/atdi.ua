using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasOther
    {
        long Id { get; set; }
        int? SwNumber { get; set; }
        string TypeSpectrumscan { get; set; }
        string TypeSpectrumOccupation { get; set; }
        double? LevelMinOccup { get; set; }
        int? Nchenal { get; set; }
        long? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
