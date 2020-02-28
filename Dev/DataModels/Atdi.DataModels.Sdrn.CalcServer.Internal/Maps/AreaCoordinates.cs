using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public Coordinate UpperLeft;
		public Coordinate LowerRight;

		public Coordinate LowerLeft => new Coordinate
		{
			X = UpperLeft.X,
			Y = LowerRight.Y
		};

		public Coordinate UpperRight => new Coordinate
		{
			X = LowerRight.X,
			Y = UpperLeft.Y
		};

		public int AxisX => LowerRight.X - UpperLeft.X;

		public int AxisY => UpperLeft.Y - LowerRight.Y;

		public long Area => (this.LowerRight.X - this.UpperLeft.X) * (this.UpperLeft.Y - this.LowerRight.Y);

		public override string ToString()
		{
			return $"[{UpperLeft}-{LowerRight}]({AxisY}x{AxisX})";
		}

		/// <summary>
		/// Принадлежность точки области. Координаты взодящие в область
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Has(int x, int y)
		{
			return !(x >= LowerLeft.X || x < UpperLeft.X || y >= UpperLeft.Y || y < LowerRight.Y);
		}
	}
}
