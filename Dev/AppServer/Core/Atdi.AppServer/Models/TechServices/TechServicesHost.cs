using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;

namespace Atdi.AppServer.Models.TechServices
{
    internal class TechServicesHost : LoggedObject, ITechServicesHost, IDisposable
    {
        private readonly IWindsorContainer _container;
        private DefaultServiceHostFactory _hostFactory;
        private ITechServiceHostsSlot _hostsSlot;

        public TechServicesHost(ITechServiceHostsSlot hostsSlot, IWindsorContainer container, ILogger logger) : base(logger)
        {
            this._hostsSlot = hostsSlot;
            this._container = container;
            this._hostFactory = new DefaultServiceHostFactory(this._container.Kernel);

            foreach (var host in this._hostsSlot.Hosts)
            {
                host.Install(container);
            } 
        }


        public void Open()
        {
            foreach (var host in this._hostsSlot.Hosts)
            {
                host.Open();
            }
        }

        public void Close()
        {
            this.Dispose();
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
                    if (this._hostsSlot != null)
                    {
                        foreach (var host in this._hostsSlot.Hosts)
                        {
                            host.Close();
                        }
                        this._hostsSlot = null;
                    }
                    if (this._hostFactory != null)
                    {
                        this._hostFactory = null;
                        this.Logger.Info("AppServer: The tech service host has been closed");
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TechServiceHost() {
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
}
