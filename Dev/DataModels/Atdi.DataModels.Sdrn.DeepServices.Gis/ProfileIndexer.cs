using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis
{
    [Serializable]
    public struct ProfileIndexer
	{
		public int XIndex;
		public int YIndex;

		public override string ToString()
		{
			return $"[{YIndex:D5}:{XIndex:D5}]";
		}
	}
}
