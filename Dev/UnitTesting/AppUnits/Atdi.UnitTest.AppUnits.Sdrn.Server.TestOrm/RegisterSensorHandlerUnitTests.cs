using System;
//using Atdi.AppUnits.Sdrn.MessageController;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.TestOrm
{
    [TestClass]
    public class RegisterSensorHandlerUnitTests
    {
        private IBusGate _busGate;
        private IDataLayer<EntityDataOrm> _dataLayer;
        private ISdrnServerEnvironment _environment;
        private IEventEmitter _eventEmitter;
        private ILogger _logger;
        private IEntityOrm _entityOrm;

        [TestInitialize]
        public void InitEnvironment()
        {
            this._busGate = new FakeBusGate();
            this._dataLayer = new FakeDataLayer<EntityDataOrm>();
            this._eventEmitter = new FakeEventEmitter();
            this._logger = new FakeLogger();
        }

        [TestMethod]
        public void Test_CheckMessageType()
        {
            //var handler = new Atdi.AppUnits.Sdrn.MessageController.RegisterSensorFromDeviceHandler(this._busGate, _dataLayer, _environment, _eventEmitter, _logger);
            //handler.Handle();
        }

   
    }
}
