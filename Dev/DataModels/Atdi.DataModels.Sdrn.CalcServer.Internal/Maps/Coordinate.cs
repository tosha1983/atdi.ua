using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Maps
{
	/// <summary>
	/// Координаты
	/// </summary>
	public struct Coordinate
	{
		public int X;
		public int Y;

		public override string ToString()
		{
			return $"({X},{Y})";
		}
	}
}
