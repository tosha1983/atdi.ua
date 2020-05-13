using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Maps
{
	public class ProjectMapData
	{
		public long Id { get; set; }

		public AtdiAxis AxisX { get; set; }

		public AtdiAxis AxisY { get; set; }

		public AtdiCoordinate UpperLeft { get; set; }

		public AtdiCoordinate LowerRight { get; set; }

		public short[] ReliefContent { get; set; }

		public byte[] ClutterContent { get; set; }

		public byte[] BuildingContent { get; set; }

		public bool Has(int x, int y)
		{
			return !(x >= LowerRight.X || x < UpperLeft.X || y >= UpperLeft.Y || y < LowerRight.Y);
		}

		public AtdiMapArea Area => new AtdiMapArea
		{
			AxisX = this.AxisX,
			AxisY = this.AxisY,
			UpperLeft = this.UpperLeft
		};

		public override string ToString()
		{
			return $"ID={Id}; Area=[{UpperLeft}:{LowerRight}]; AxisX=[{this.AxisX}]; AxisY=[{this.AxisY}]";
		}
	}
}
