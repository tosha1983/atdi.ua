using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasDtParam
    {
        int Id { get; set; }
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
        int? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
