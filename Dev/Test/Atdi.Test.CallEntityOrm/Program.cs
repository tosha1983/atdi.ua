using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.UnitTest.AppUnits.Sdrn.Server.TestOrm;

namespace Atdi.Test.CallEntityOrm
{
    class Program
    {
        static void Main(string[] args)
        {
            Atdi.UnitTest.AppUnits.Sdrn.Server.TestOrm.RegisterSensorHandlerUnitTests registerSensorHandlerUnitTests = new Atdi.UnitTest.AppUnits.Sdrn.Server.TestOrm.RegisterSensorHandlerUnitTests();
            registerSensorHandlerUnitTests.InitEnvironment();
            registerSensorHandlerUnitTests.Test_CheckMessageType();
/*
            Atdi.UnitTest.AppUnits.Sdrn.Serve
                r.PrimaryHandlers.RegisterSensorHandlerUnitTests.RegisterSensorHandlerUnitTests registerSensorHandlerUnitTests = new RegisterSensorHandlerUnitTests();
            registerSensorHandlerUnitTests.InitEnvironment();
            registerSensorHandlerUnitTests.Test_CheckMessageType();
            */
        }
    }
}
