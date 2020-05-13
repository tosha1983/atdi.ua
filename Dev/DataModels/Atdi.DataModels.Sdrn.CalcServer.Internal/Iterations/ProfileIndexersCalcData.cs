using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public sealed class ProfileIndexersCalcData
	{

		/// <summary>
		/// Буффер, содержащий индексаторы профиля 
		/// </summary>
		public ProfileIndexer[] Result;

		/// <summary>
		/// Позиция бокса: Lower Left x and Y
		/// </summary>
		public AtdiMapArea Area;

		/// <summary>
		/// Опорная точка, от которой необходмио считать профели до целевых
		/// </summary>
		public AtdiCoordinate Point;

		/// <summary>
		/// Коодината целевйо точки
		/// </summary>
		public AtdiCoordinate Target;

		public bool CheckReverse = true;

		public bool HasError = false;

	}
}
