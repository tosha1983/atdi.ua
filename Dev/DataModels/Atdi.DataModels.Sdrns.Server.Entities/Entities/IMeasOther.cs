using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasOther_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasOther: IMeasOther_PK
    {
        string TypeSpectrumOccupation { get; set; }
        double? LevelMinOccup { get; set; }
        int? Nchenal { get; set; }
        IMeasTask MEAS_TASK { get; set; }
    }
}
