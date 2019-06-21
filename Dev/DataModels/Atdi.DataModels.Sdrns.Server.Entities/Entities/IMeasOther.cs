using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IMeasOther_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasOther: IMeasOther_PK
    {
        int? SwNumber { get; set; }
        string TypeSpectrumscan { get; set; }
        string TypeSpectrumOccupation { get; set; }
        double? LevelMinOccup { get; set; }
        int? Nchenal { get; set; }
        long? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
