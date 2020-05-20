using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Icsm.Plugins.Core;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
	public class InfocenterDataLayer : DataLayerBase
	{
		public InfocenterDataLayer(AppComponentConfig config) 
			: base (
					new WebApiEndpoint( 
						new Uri(config.InfocenterEntityOrmEndpointBaseAddress),
						config.InfocenterEntityOrmEndpointApiUri),
					new WebApiDataContext( config.InfocenterEntityOrmDataContext)
				)
		{
		}
	}
}
