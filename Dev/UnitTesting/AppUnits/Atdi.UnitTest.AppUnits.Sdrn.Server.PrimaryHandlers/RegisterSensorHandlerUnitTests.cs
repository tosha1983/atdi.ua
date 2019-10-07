using System;
//using Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers
{
    [TestClass]
    public class RegisterSensorHandlerUnitTests
    {
        private IBusGate _busGate;
        private IDataLayer<EntityDataOrm> _dataLayer;
        private ISdrnServerEnvironment _environment;
        private IEventEmitter _eventEmitter;
        private ILogger _logger;

        [TestInitialize]
        public void InitEnvironment()
        {
            this._busGate = new Fake.FakeBusGate();
            this._dataLayer = new Fake.FakeDataLayer<EntityDataOrm>();
            this._environment = new Fake.FakeSdrnServerEnvironment();
            this._eventEmitter = new Fake.FakeEventEmitter();
            this._logger = new Fake.FakeLogger();
        }

        [TestMethod]
        public void Test_CheckMessageType()
        {
            //var handler = new RegisterSensorHandler(this._busGate, _dataLayer, _environment, _eventEmitter, _logger);
            //var expected = DeviceBusMessages.RegisterSensorMessage.Name;

            //Assert.AreEqual(expected, handler.MessageType);
        }

        [TestMethod]
        public void Test_CallOnHandler_Confirmed()
        {
            //var handler = new RegisterSensorHandler(this._busGate, _dataLayer, _environment, _eventEmitter, _logger);

            //var message = new Fake.FakeSdrnReceivedMessage<Sensor>(Guid.NewGuid().ToString(), DeviceBusMessages.RegisterSensorMessage.Name)
            //{
            //    Created = DateTime.Now,
            //    DeviceSensorName = "Name",
            //    DeviceSensorTechId = "TechId",
            //    Data = new Sensor
            //    {
            //        Name = "Name",
            //        Equipment = new SensorEquipment
            //        {
            //            TechId = "TechId"
            //        }
            //    },
            //    Result = MessageHandlingResult.Received
            //};

            //handler.OnHandle(message);

            //Assert.AreEqual(MessageHandlingResult.Confirmed, message.Result);
        }
    }
}
