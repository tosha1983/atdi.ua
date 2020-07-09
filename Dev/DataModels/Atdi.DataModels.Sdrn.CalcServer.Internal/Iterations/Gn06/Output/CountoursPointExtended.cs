using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    public class CountoursPointExtended: CountoursPoint
    {
        public string administration;
        public BroadcastingTypeContext  broadcastingTypeContext;
        public BroadcastingTypeCalculation broadcastingTypeCalculation;
    }
}
    
