using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingGroupingParameters
    {
        [DataMember]
        public int? TimeBetweenWorkTimes_sec { get; set; } // время для группировки записей workTime по умолчанию 60
        [DataMember]
        public int? TypeJoinSpectrum { get; set; } // принцип объединения спектра 0 - Best Emmiting (ClearWrite), 1 - MaxHold, 2 - Avarage по умолчанию 0
        [DataMember]
        public double? CrossingBWPercentageForGoodSignals { get; set; } //определяет насколько процентов должно совпадать излучение если BW определен по умолчанию 70
        [DataMember]
        public double? CrossingBWPercentageForBadSignals { get; set; } // определяет насколько процентов должно совпадать излучение если BW не определен по умолчанию 40
    }
}
