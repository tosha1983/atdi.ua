using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class SignalingGroupingParameters
    {
        public int? TimeBetweenWorkTimes_sec { get; set; } // время для группировки записей workTime по умолчанию 60
        public int? TypeJoinSpectrum { get; set; } // принцип объединения спектра 0 - Best Emmiting (ClearWrite), 1 - MaxHold, 2 - Avarage по умолчанию 0
        public double? CrossingBWPercentageForGoodSignals { get; set; } //определяет насколько процентов должно совпадать излучение если BW определен по умолчанию 70
        public double? CrossingBWPercentageForBadSignals { get; set; } // определяет насколько процентов должно совпадать излучение если BW не определен по умолчанию 40
    }
}
