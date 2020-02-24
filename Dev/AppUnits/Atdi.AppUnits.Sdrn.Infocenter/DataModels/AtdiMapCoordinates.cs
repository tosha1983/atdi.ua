using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.DataModels
{
	internal struct AtdiMapCoordinates
	{
		public AtdiMapCoordinate UpperLeft;

		public AtdiMapCoordinate UpperRight;

		public AtdiMapCoordinate LowerLeft;

		public AtdiMapCoordinate LowerRight;

		public int AxisX => UpperRight.X - UpperLeft.X;

		public int AxisY => UpperLeft.Y - LowerLeft.Y;

		public override string ToString()
		{
			return $"{UpperLeft} x {LowerRight} : axis.X = '{AxisX}', axis.Y = '{AxisY}'";
		}
	}
}
