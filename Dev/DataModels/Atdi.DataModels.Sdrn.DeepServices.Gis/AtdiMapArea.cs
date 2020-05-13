using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis
{
	public struct AtdiMapArea
	{
		public AtdiAxis AxisX { get; set; }

		public AtdiAxis AxisY { get; set; }

		public AtdiCoordinate UpperLeft { get; set; }

		public AtdiCoordinate UpperRight => new AtdiCoordinate()
		{
			X = this.UpperLeft.X + this.AxisX.Size,
			Y = this.UpperLeft.Y
		};

		public AtdiCoordinate LowerLeft => new AtdiCoordinate()
		{
			X = this.UpperLeft.X,
			Y = this.UpperLeft.Y - (this.AxisY.Size)
		};
		public AtdiCoordinate LowerRight => new AtdiCoordinate()
		{
			X = this.UpperLeft.X + this.AxisX.Size,
			Y = this.UpperLeft.Y - this.AxisY.Size
		};
	}
}
