﻿using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers
{
    public class SdrnServerPrimaryHandlersComponent : AppUnitComponent
    {

        public SdrnServerPrimaryHandlersComponent() 
            : base("SdrnServerPrimaryHandlersAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
        }

        protected override void OnActivateUnit()
        {
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}