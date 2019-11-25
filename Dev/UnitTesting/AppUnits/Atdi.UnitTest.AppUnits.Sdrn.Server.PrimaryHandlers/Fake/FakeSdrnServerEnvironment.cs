using Atdi.Contracts.Sdrn.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake
{
    class FakeSdrnServerEnvironment : ISdrnServerEnvironment
    {
        public string ServerInstance => "TestInstance";

        public string LicenseNumber => throw new NotImplementedException();
        public DateTime LicenseStopDate { get; }
        public DateTime LicenseStartDate { get; }

        public DateTime LicenseDateStop => throw new NotImplementedException();

        public ServerRole ServerRoles => throw new NotImplementedException();

        public string MasterServerInstance => throw new NotImplementedException();
        public string GetSharedSecretKey(string context)
        {
            throw new NotImplementedException();
        }
    }
}
