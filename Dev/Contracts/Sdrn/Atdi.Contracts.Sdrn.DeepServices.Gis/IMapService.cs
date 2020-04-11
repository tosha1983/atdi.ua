using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;

namespace Atdi.Contracts.Sdrn.DeepServices.Gis
{
	public interface IMapService
	{
		void CalcProfileIndexers(in CalcProfileIndexersArgs args, ref CalcProfileIndexersResult result);
	}
}
