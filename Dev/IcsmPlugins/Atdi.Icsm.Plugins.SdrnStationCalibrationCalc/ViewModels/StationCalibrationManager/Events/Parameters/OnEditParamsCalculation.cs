﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Events
{
    public class OnEditParamsCalculation
    {
        public long ClientContextId { get; set; }
        public bool IsSuccessUpdateParameters;
    }
}
