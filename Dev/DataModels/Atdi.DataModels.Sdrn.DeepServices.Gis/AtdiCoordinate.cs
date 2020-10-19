using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis
{
    /// <summary>
    /// Метровые целочисленные координаты 
    /// </summary>
    [Serializable]
    public struct AtdiCoordinate
	{
		public int X;
		public int Y;

		public static implicit operator AtdiCoordinate(EpsgCoordinate source) => new AtdiCoordinate
		{
			X = (int)source.X,
			Y = (int)source.Y
		};
	}
}
