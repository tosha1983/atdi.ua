using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Maps
{
    [Serializable]
    public enum ProjectMapType
	{
		Unknown = 0,
		Relief = 1,
		Clutter = 2,
		Building = 3
	}
}
