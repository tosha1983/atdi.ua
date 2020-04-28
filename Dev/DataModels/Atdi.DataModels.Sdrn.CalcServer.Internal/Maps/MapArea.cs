//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Maps
//{
//	public struct MapArea
//	{
//		public Axis AxisX { get; set; }

//		public Axis AxisY { get; set; }

//		public Coordinate UpperLeft { get; set; }

//		public Coordinate UpperRight => new Coordinate()
//		{
//			X = this.UpperLeft.X + this.AxisX.Size,
//			Y = this.UpperLeft.Y
//		};

//		public Coordinate LowerLeft => new Coordinate()
//		{
//			X = this.UpperLeft.X,
//			Y = this.UpperLeft.Y - (this.AxisY.Size)
//		};
//		public Coordinate LowerRight => new Coordinate()
//		{
//			X = this.UpperLeft.X + this.AxisX.Size,
//			Y = this.UpperLeft.Y - this.AxisY.Size
//		};
//	}
//}
