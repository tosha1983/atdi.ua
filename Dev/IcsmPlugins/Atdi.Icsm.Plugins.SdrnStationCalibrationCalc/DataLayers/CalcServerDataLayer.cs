using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Icsm.Plugins.Core;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc
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
