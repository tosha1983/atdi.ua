using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer.Internal
{
	public interface  IClientContextService
	{
		void PrepareContext(IDataLayerScope dbScope, long contextId);

		ClientContext GetContextById(IDataLayerScope dbScope, long contextId);

		ClientContextStation GetContextStation(IDataLayerScope dbScope, long contextId, long stationId);

		PropagationModel GetPropagationModel(IDataLayerScope dbScope, long contextId);
	}
}
