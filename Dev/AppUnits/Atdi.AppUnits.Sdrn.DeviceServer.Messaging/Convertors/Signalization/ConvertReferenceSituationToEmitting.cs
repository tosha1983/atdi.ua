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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor
{
    public static class ConvertReferenceSituationToEmitting
    {
        public static Emitting Convert(this ReferenceSituation referenceLevels)
        {
            Emitting emitting = new Emitting();

          
            return emitting;
        }
    }
}
