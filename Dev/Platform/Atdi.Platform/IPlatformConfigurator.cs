﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IPlatformConfigurator
    {
        DependencyInjection.IServicesContainer BuildContainer();
    }
}
