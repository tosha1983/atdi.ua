using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Diagnostics;

namespace XICSM.ICSControlClient.Environment
{
    public class WcfServiceClientBase<TContract, TClient>
        where TContract: class
        where TClient : WcfServiceClientBase<TContract, TClient>, new()
    {
        private readonly string _configName;
        private readonly ChannelFactory<TContract> _channelFactory = null;

        private static TClient _instance;


        public WcfServiceClientBase(string configName)
        {
            this._configName = configName;
            this._channelFactory = new ChannelFactory<TContract>(configName);
        }

        public static WcfService<TContract> Open()
        {
            var factory = GetChannelFactory();
            var service = factory.CreateChannel();
            var channel = new WcfService<TContract>(service);
            return channel;
        }

        public static TResult Execute<TResult>(Func<TContract, TResult> action)
        {
            using (var service = Open())
            {
                return service.Execute(action);
            }
        }

        public static void Execute(Action<TContract> action)
        {
            using (var service = Open())
            {
                service.Execute(action);
            }
        }


        private static ChannelFactory<TContract> GetChannelFactory()
        {
            return GetInstance()._channelFactory;
        }

        private static TClient GetInstance()
        {
            if(_instance == null)
            {
                lock (typeof(TClient))
                {
                    if (_instance == null)
                    {
                        TClient temp = new TClient();
                        // Office 365 has a restriction of System.Threading usage, but regular sandbox - no.
                        //System.Threading.Thread.MemoryBarrier();
                        _instance = temp;
                    }
                }
            }
            return _instance;
        }
    }

    public class WcfService<TContract> : IDisposable
        where TContract : class
    {
        private readonly IClientChannel _channel;
        private readonly TContract _service;

        public WcfService(TContract service)
        {
            this._channel = service as IClientChannel;
            this._service = service;
        }

        public TContract Contract => this._service;

        public IClientChannel Channel => this._channel;

        public TResult Execute<TResult>(Func<TContract, TResult> action)
        {
            return Invoke<TResult>(action);
        }

        public void Execute(Action<TContract> action)
        {
            Invoke<bool>(service => { action(service); return true; });
        }

        private TResult Invoke<TResult>(Func<TContract, TResult> action)
        {
            try
            {
                Debug.Print($"Invoke service method: {action.Method.Name}");

                return action(this._service);
            }
            catch (FaultException e)
            {
                Logger.WriteError(PluginMetadata.Processes.InvokeWcfOperation, $"There was a service problem. {e.Message}");
                Logger.WriteExeption(PluginMetadata.Processes.InvokeWcfOperation, e);
                throw new InvalidOperationException("WCF operation call was aborted");
            }
            catch (CommunicationException e)
            {
                Logger.WriteError(PluginMetadata.Processes.InvokeWcfOperation, $"There was a communication problem. {e.Message}");
                Logger.WriteExeption(PluginMetadata.Processes.InvokeWcfOperation, e);
                throw new InvalidOperationException("WCF operation call was aborted");
            }
            catch (TimeoutException e)
            {
                Logger.WriteError(PluginMetadata.Processes.InvokeWcfOperation, $"The service operation timed out. {e.Message}");
                Logger.WriteExeption(PluginMetadata.Processes.InvokeWcfOperation, e);
                throw new InvalidOperationException("WCF operation call was aborted");
            }
            catch (Exception e)
            {
                Logger.WriteError(PluginMetadata.Processes.InvokeWcfOperation, $"There was a problem. {e.Message}");
                Logger.WriteExeption(PluginMetadata.Processes.InvokeWcfOperation, e);
                throw new InvalidOperationException("WCF operation call was aborted");
            }
            finally
            {
                if (this._channel.State != CommunicationState.Opened)
                {
                    this._channel.Abort();
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this._channel.State == CommunicationState.Opened || this._channel.State == CommunicationState.Opening)
                    {
                        this._channel.Close();
                    }
                    else if (this._channel.State == CommunicationState.Faulted)
                    {
                        this._channel.Abort();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
