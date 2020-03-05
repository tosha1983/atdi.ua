using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.DataModels
{
	internal struct MapCoordinates
	{
		public MapCoordinate UpperLeft;

		public MapCoordinate UpperRight;

		public MapCoordinate LowerLeft;

		public MapCoordinate LowerRight;

		public int AxisX => UpperRight.X - UpperLeft.X;

		public int AxisY => UpperLeft.Y - LowerLeft.Y;

		public override string ToString()
		{
			return $"{UpperLeft} x {LowerRight} : axis.X = '{AxisX}', axis.Y = '{AxisY}'";
		}
	}
}
