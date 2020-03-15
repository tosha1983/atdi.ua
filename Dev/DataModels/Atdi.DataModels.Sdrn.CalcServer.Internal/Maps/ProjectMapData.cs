using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Maps
{
	public class ProjectMapData
	{
		public long Id { get; set; }

		public Axis AxisX { get; set; }

		public Axis AxisY { get; set; }

		public Coordinate UpperLeft { get; set; }

		public Coordinate LowerRight { get; set; }

		public short[] ReliefContent { get; set; }

		public byte[] ClutterContent { get; set; }

		public byte[] BuildingContent { get; set; }

		public bool Has(int x, int y)
		{
			return !(x >= LowerRight.X || x < UpperLeft.X || y >= UpperLeft.Y || y < LowerRight.Y);
		}

		public MapArea Area => new MapArea
		{
			AxisX = this.AxisX,
			AxisY = this.AxisY,
			UpperLeft = this.UpperLeft
		};

		public override string ToString()
		{
			return $"ID={Id}; Area=[{UpperLeft}:{LowerRight}]; AxisX='{this.AxisX}'; AxisY='{this.AxisY}'";
		}
	}
}
