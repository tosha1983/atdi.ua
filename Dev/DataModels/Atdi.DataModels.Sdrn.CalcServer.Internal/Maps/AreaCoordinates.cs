using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Maps
{
	/// <summary>
	/// Координаты области карты
	/// Важно: граничные точки принадлежат другой области
	///  - UpperLeft.Y / LowerRight.X - эти значения являються началом другой смежной области
	/// и определяют ее точки
	/// </summary>
	public struct AreaCoordinates
	{
		public AtdiCoordinate UpperLeft;
		public AtdiCoordinate LowerRight;

		public AtdiCoordinate LowerLeft => new AtdiCoordinate
		{
			X = UpperLeft.X,
			Y = LowerRight.Y
		};

		public AtdiCoordinate UpperRight => new AtdiCoordinate
		{
			X = LowerRight.X,
			Y = UpperLeft.Y
		};

		public int AxisX => LowerRight.X - UpperLeft.X;

		public int AxisY => UpperLeft.Y - LowerRight.Y;

		public long Area => (this.LowerRight.X - this.UpperLeft.X) * (this.UpperLeft.Y - this.LowerRight.Y);

		public override string ToString()
		{
			return $"[{UpperLeft}-{LowerRight}]; {AxisY}x{AxisX}; Area='{Area}'";
		}

		/// <summary>
		/// Принадлежность точки области. Координаты взодящие в область
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Has(int x, int y)
		{
			return !(x >= LowerRight.X || x < UpperLeft.X || y >= UpperLeft.Y || y < LowerRight.Y);
		}
	}
}
