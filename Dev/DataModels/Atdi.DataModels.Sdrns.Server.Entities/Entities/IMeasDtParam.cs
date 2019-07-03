using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasDtParam_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasDtParam: IMeasDtParam_PK
    {
        string TypeMeasurements { get; set; }
        string DetectType { get; set; }
        double? Rfattenuation { get; set; }
        double? Ifattenuation { get; set; }
        double? MeasTime { get; set; }
        string Demod { get; set; }
        int? Preamplification { get; set; }
        string Mode { get; set; }
        double? Rbw { get; set; }
        double? Vbw { get; set; }
        IMeasTask MEAS_TASK { get; set; }
    }
}
