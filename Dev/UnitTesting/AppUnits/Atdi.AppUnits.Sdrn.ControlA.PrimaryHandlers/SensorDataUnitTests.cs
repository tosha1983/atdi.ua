using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;
using Atdi.AppUnits.Sdrn.ControlA.Bus;



namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers
{
    [TestClass]
    public class SensorDataUnitTests
    {
        [TestMethod]
        public void Test_UpdateStatus()
        {
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            Launcher._logger = fakeLogger;
            SensorDb sensorDb = new SensorDb();
            sensorDb.UpdateStatus();
        }

        [TestMethod]
        public void Test_CreateNewObjectSensor()
        {
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            Launcher._logger = fakeLogger;
            SensorDb sensorDb = new SensorDb();
            Atdi.AppServer.Contracts.Sdrns.Sensor sensor = sensorDb.GetSensorFromDefaultConfigFile();
            bool res = sensorDb.CreateNewObjectSensor(sensor);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_AddNewLocations()
        {
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            Launcher._logger = fakeLogger;
            SensorDb sensorDb = new SensorDb();
            Atdi.AppServer.Contracts.Sdrns.Sensor sensor = sensorDb.GetSensorFromDefaultConfigFile();
            sensorDb.AddNewLocations(1, 20, 20, 100, DateTime.Now, DateTime.Now.AddHours(1), AllStatusLocation.A);
        }
    }
}
