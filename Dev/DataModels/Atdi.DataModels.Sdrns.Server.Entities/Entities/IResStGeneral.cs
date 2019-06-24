using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResStGeneral_PK
    {
        long Id { get; set; }
    }
    [Entity]
    public interface IResStGeneral : IResStGeneral_PK
    {
        double? BW { get; set; }
        double? Rbw { get; set; }
        double? Vbw { get; set; }
        long? ResMeasStaId { get; set; }
        double? CentralFrequency { get; set; }
        double? CentralFrequencyMeas { get; set; }
        double? DurationMeas { get; set; }
        int? MarkerIndex { get; set; }
        int? T1 { get; set; }
        int? T2 { get; set; }
        DateTime? TimeStartMeas { get; set; }
        DateTime? TimeFinishMeas { get; set; }
        double? OffsetFrequency { get; set; }
        double? SpecrumStartFreq { get; set; }
        double? SpecrumSteps { get; set; }
        int? Correctnessestim { get; set; }
        int? TraceCount { get; set; }
        float[] LevelsSpectrumdBm { get; set; }
        IResMeasStation RESMEASSTA { get; set; }
    }
}
