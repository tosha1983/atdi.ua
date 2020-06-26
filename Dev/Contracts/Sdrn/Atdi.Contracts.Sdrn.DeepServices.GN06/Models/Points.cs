using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	
	/// <summary>
	/// 
	/// </summary>
	public struct Points
    {
        public PointEarthGeometric[]  PointEarthGeometrics;
        public int SizeResultBuffer;
    }
}
