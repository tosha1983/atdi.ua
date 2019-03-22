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
    public static class CalcGroupingEmitting
    {
        public static Emitting[] Convert(Emitting[] EmittingRaw, Emitting[] EmittingTemp, Emitting[] EmittingSummary)
        {

            
            Emitting[] emittingSummary = new Emitting[1];
            return emittingSummary;
        }
    }
}
