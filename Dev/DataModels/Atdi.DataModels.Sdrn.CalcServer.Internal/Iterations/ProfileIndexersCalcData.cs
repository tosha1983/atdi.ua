using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public sealed class ProfileIndexersCalcData
	{

		/// <summary>
		/// Буффер, содержащий индексаторы профиля 
		/// </summary>
		public Maps.Indexer[] Result;

		/// <summary>
		/// Позиция бокса: Lower Left x and Y
		/// </summary>
		public Maps.MapArea Area;

		/// <summary>
		/// Опорная точка, от которой необходмио считать профели до целевых
		/// </summary>
		public Maps.Coordinate Point;

		/// <summary>
		/// Коодината целевйо точки
		/// </summary>
		public Maps.Coordinate Target;

	}
}
