using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    [Flags]
    public enum ServerRole
    {
        SdrnServer = 1,
        MasterServer = 2,
        AggregationServer = 4
    }

    public interface ISdrnServerEnvironment
    {
        string ServerInstance { get;  }

        string LicenseNumber { get; }

        DateTime LicenseStopDate { get; }

        DateTime LicenseStartDate { get; }


        ServerRole ServerRoles { get; }

        string MasterServerInstance { get; }
    }

    public static class SdrnServerEnvironmentExtentions
    {

        public static string GetAppName(this ISdrnServerEnvironment environment)
        {
            return $"[SDRN.Server].[{environment.ServerInstance}]";
        }
    }
}
