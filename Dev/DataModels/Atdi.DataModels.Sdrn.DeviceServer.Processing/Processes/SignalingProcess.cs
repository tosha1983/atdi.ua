﻿using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SignalingProcess : ProcessBase
    {
        public Emitting[] Emittings;
        public SensorParameters sensorParameters;
        public SignalingProcess() : base("Signaling process")
        {
            
        }
    }
}
