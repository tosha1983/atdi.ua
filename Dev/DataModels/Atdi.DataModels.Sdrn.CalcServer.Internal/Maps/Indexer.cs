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
	public struct Indexer
	{
		public int XIndex;
		public int YIndex;

		public override string ToString()
		{
			return $"[{YIndex:D4}:{XIndex:D4}]";
		}
	}
}
