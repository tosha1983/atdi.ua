﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public class MesureSysInfoDeviceProperties : DevicePropertiesBase
    {
        public string[] AvailableStandards;
        public StandardDeviceProperties StandardDeviceProperties;
    }
}
