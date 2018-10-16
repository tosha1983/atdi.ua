using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnServerEnvironment
    {
        string ServerInstance { get;  }

        string LicenseNumber { get; }

        DateTime LicenseDateStop { get; }
    }

    public static class SdrnServerEnvironmentExtentions
    {

        public static string GetAppName(this ISdrnServerEnvironment environment)
        {
            return $"[SDRN.Server].[{environment.ServerInstance}]";
        }
    }
}
