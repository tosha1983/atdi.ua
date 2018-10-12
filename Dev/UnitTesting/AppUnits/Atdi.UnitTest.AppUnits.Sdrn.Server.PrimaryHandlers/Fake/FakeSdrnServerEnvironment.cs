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
    }
}
