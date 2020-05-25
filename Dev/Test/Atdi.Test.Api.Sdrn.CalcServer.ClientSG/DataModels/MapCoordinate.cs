using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.DataModels
{
	internal struct MapCoordinate
	{
		public int X;
		public int Y;

		public override string ToString()
		{
			return $"({X},{Y})";
		}
	}
}
