﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IBusGateConfig
    {
        object this[string name] { get; set; }

        T GetValue<T>(string name, T defaultValue = default(T));

        bool TryGetValue<T>(string name, out T value);
    }
}
