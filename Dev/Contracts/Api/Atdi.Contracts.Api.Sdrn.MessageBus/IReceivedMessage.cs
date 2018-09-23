using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IReceivedMessage<TObject>
    {
        IMessageToken Token { get; }

        string CorrelationToken { get; }

        DateTime Created { get; }

        TObject Data { get; }

        MessageHandlingResult Result { get; set; }

        string ReasonFailure { get; set; }
    }
}
