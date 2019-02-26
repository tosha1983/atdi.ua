using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IWorkScheduler
    {
        Task Run(string workContext, Action action, int delay = 0);
    }
}
