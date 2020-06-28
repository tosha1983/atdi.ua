using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.Contracts.Sdrn.DeepServices.GN06
{
	
	/// <summary>
	/// 
	/// </summary>
	public struct PointWithAzimuthResult
    {
       public PointWithAzimuth[] PointWithAzimuth;
       public int sizeResultBuffer;
    }
}
