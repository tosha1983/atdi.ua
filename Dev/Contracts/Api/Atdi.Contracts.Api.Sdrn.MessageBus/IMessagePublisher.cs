﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IMessagePublisher : IDisposable
    {
        IMessageToken Send<TObject>(string messageType, TObject messageObject, string correlationToken = null);
    }
}
