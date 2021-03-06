﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public interface IEventSite : IDisposable
    {
        IEventSiteConfig Config { get; }

        IEventEmitter GetEmitter();

        IEventDispatcher GetDispatcher();
    }
}
