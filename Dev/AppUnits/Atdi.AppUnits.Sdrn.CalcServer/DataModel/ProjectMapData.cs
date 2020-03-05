using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.DataModel
{
	internal class ProjectMapData
	{
		public long ProjectMapId;

		public string Projection;

		public string StepUnit;

		public int OwnerAxisXNumber;

		public int OwnerAxisXStep;

		public int OwnerAxisYNumber;

		public int OwnerAxisYStep;

		public int OwnerUpperLeftX;

		public int OwnerUpperLeftY;

		public int AxisXNumber;

		public int AxisXStep;

		public int AxisYNumber;

		public int AxisYStep;

		public int UpperLeftX;

		public int UpperLeftY;

		public int LowerRightX;

		public int LowerRightY;

		public int OwnerStepsNumber => this.OwnerAxisXNumber * this.OwnerAxisYNumber;

		public long RectArea => (this.LowerRightX - this.UpperLeftX) * (this.UpperLeftY - this.LowerRightY);

		public bool Has(int x, int y)
		{
			return x >= this.UpperLeftX
			       && x <= this.LowerRightX
			       && y <= this.UpperLeftY
			       && y >= this.LowerRightY;
		}

		public Coordinate IndexToUpperLeftCoordinate(int xIndex, int yIndex)
		{
			return new Coordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex,
				Y = this.UpperLeftY - this.AxisYStep * yIndex
			};
		}

		public Coordinate IndexToLowerRightCoordinate(int xIndex, int yIndex)
		{
			return new Coordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex + this.AxisXStep,
				Y = this.UpperLeftY - this.AxisYStep * yIndex - this.AxisYStep
			};
		}

		public AreaCoordinates IndexToArea(int xIndex, int yIndex)
		{
			return new AreaCoordinates
			{
				UpperLeft = IndexToUpperLeftCoordinate(xIndex, yIndex),
				LowerRight = IndexToLowerRightCoordinate(xIndex, yIndex)
			};
		}
	}
}
