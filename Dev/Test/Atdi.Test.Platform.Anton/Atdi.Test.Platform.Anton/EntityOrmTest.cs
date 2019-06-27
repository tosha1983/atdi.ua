using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SB = Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus;

namespace Atdi.Test.Platform
{
    class EventEmitterFake : IEventEmitter
    {
        public void Emit(DataModels.Api.EventSystem.IEvent @event, EventEmittingOptions options)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EventEmitterFake() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
    class SdrnMessagePublisherFake : ISdrnMessagePublisher
    {
        public string Tag => throw new NotImplementedException();

        public ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject> CreateOutgoingEnvelope<TMessageType, TDeliveryObject>() where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
        {
            throw new NotImplementedException();
        }

        public string Send<TMessageType, TDeliveryObject>(ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject> envelope) where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SdrnMessagePublisherFake() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
    class MessagesSiteFake : IMessagesSite
    {
        void IMessagesSite.ChangeStatus(long messageId, byte oldCode, byte newCode, string statusNote)
        {
            throw new NotImplementedException();
        }

        (long, string)[] IMessagesSite.GetMessagesForNotification()
        {
            throw new NotImplementedException();
        }

        IMessageProcessingScope<TDeliveryObject> IMessagesSite.StartProcessing<TDeliveryObject>(long messageId)
        {
            return new ProccesingScope<TDeliveryObject>();
        }
    }
    class ProccesingScope<TDeliveryObject> : IMessageProcessingScope<TDeliveryObject>
    {
        public string SensorName => "";

        public string SensorTechId => "";

        public TDeliveryObject Delivery
        {
            get
            {
                var type = typeof(TDeliveryObject);
                if (type == typeof(MeasResults))
                {
                    var measResultTest = new TestMeasResult();
                    var deliveryObject = measResultTest.BuildTestMeasResults();
                    return (TDeliveryObject)(object)deliveryObject;
                }
                else
                {
                    return default(TDeliveryObject);
                }
            }
        }

        public MessageProcessingStatus Status { get; set ; }
        public string ResultNote { get ; set ; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    class SdrnServerEnvironmentFake : ISdrnServerEnvironment
    {
        string ISdrnServerEnvironment.ServerInstance => throw new NotImplementedException();

        string ISdrnServerEnvironment.LicenseNumber => throw new NotImplementedException();

        DateTime ISdrnServerEnvironment.LicenseDateStop => throw new NotImplementedException();

        ServerRole ISdrnServerEnvironment.ServerRoles => throw new NotImplementedException();

        string ISdrnServerEnvironment.MasterServerInstance => throw new NotImplementedException();
    }
    class LoggerFake : ILogger
    {
        void IEventWriter.Critical(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        void IEventWriter.Debug(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        void IEventWriter.Error(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        void IEventWriter.Exception(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        void IEventWriter.Info(EventContext context, EventCategory category, EventText eventText)
        {
            throw new NotImplementedException();
        }

        bool ILogger.IsAllowed(EventLevel level)
        {
            throw new NotImplementedException();
        }

        ITraceScope IEventWriter.StartTrace(EventContext context, EventCategory category, TraceScopeName scopeName, string source, IReadOnlyDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        void IEventWriter.Verbouse(EventContext context, EventCategory category, EventText eventText)
        {
            throw new NotImplementedException();
        }

        void IEventWriter.Warning(EventContext context, EventCategory category, EventText eventText)
        {
            throw new NotImplementedException();
        }
    }
    class EntityOrmTest
    {
        public static void Run(IServicesResolver servicesResolver, ILogger logger)
        {
            var dataLayer = servicesResolver.Resolve<IDataLayer<EntityDataOrm>>();

            Test_Subscribes(dataLayer, logger);
        }

        private static void Test_Subscribes(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            var measResultTest = new TestMeasResult();
            var measResult = measResultTest.BuildTestMeasResults();

            
            //scope.

            var subscriber = new SB.SendMeasResultsSubscriber(new EventEmitterFake(), new SdrnMessagePublisherFake(), new MessagesSiteFake(), dataLayer, new SdrnServerEnvironmentFake(), logger);
            var event1 = new DevicesBusEvent
            {
                BusMessageId = 1
            };
            subscriber.Notify(event1);

        }
    }
}
