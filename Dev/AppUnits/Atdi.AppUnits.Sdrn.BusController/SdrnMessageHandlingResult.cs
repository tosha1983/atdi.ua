using Atdi.Contracts.Sdrn.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnMessageHandlingResult : ISdrnMessageHandlingResult
    {
        public SdrnMessageHandlingResult()
        {
            this.Status = SdrnMessageHandlingStatus.Unprocessed;
        }

        public SdrnMessageHandlingStatus Status { get; set; }

        public string ReasonFailure { get; set; }
    }
}
