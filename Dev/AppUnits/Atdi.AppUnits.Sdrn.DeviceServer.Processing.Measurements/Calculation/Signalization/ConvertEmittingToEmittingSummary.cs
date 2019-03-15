using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class ConvertEmittingToEmittingSummary
    {
        public static Emitting Convert(Emitting EmittingRaw, Emitting EmittingDetailed, Emitting EmittingSummary)
        {
            Emitting emittingSummary = new Emitting();

          
            return emittingSummary;
        }
    }
}
