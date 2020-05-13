using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Core;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
	public class CalcServerDataLayer : DataLayerBase
	{
		public CalcServerDataLayer(AppComponentConfig config) 
			: base (
					new WebApiEndpoint( 
						new Uri(config.CalcServerEntityOrmEndpointBaseAddress),
						config.CalcServerEntityOrmEndpointApiUri),
					new WebApiDataContext( config.CalcServerEntityOrmDataContext)
				)
		{
		}
	}
}
