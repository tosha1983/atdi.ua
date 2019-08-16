using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.PipelineHandlers
{
    public class ClientMeasTasksPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            return context.GoAhead(data);
        }
    }
}
