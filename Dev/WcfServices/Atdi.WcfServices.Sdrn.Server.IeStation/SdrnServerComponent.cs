﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.AppServer;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class SdrnServerComponent : WcfServicesComponent
    {
        public SdrnServerComponent() : base("SdrnServerWcfServicesIeStation", ComponentBehavior.SingleInstance)
        {

        }

        protected override void OnInstall()
        {
            var exampleConfig = this.Config.Extract<ComponentConfig>();
            this.Container.RegisterInstance(exampleConfig, ServiceLifetime.Singleton);

            base.OnInstall();
            this.Container.Register<RunSynchroProcess, RunSynchroProcess>(ServiceLifetime.Singleton);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

            hostLoader.RegisterTrigger("Check running  process synchronize refspectrum and emittings", () =>
            {
                var messagesProcessing = this.Resolver.Resolve<RunSynchroProcess>();
                messagesProcessing.RecoveryDataSynchronizationProcess();
            });
        }

        protected override void OnUninstall()
        {
            base.OnUninstall();
        }
    }
}
