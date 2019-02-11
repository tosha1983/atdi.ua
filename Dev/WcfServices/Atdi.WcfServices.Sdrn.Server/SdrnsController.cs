using Atdi.Contracts.WcfServices.Sdrn.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SdrnsController: WcfServiceBase<ISdrnsController>, ISdrnsController
    {
    }
}
