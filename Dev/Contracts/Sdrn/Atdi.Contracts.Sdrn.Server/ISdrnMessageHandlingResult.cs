using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnMessageHandlingResult
    {
        SdrnMessageHandlingStatus Status { get; set; }

        string ReasonFailure { get; set; }
    }
}
