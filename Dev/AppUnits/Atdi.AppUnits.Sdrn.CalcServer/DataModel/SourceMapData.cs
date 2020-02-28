using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.DataModel
{
	internal class SourceMapData
	{
		public long MapId;

		public string MapName;

		public byte StepDataSize;

		public int AxisXNumber;

		public int AxisXStep;

		public int AxisYNumber;

		public int AxisYStep;

		public int UpperLeftX;

		public int UpperLeftY;

		public int LowerRightX;

		public int LowerRightY;

		public int CoverageUpperLeftX;

		public int CoverageUpperLeftY;

		public int CoverageLowerRightX;

		public int CoverageLowerRightY;

		public long CoverageArea;

		public decimal CoveragePercent;

		public byte Priority;

		public bool Used;

		public int SectorsCount;

		public int StepsNumber => this.AxisXNumber * this.AxisYNumber;

		public bool Has(int x, int y)
		{
			return x >= this.UpperLeftX
		       &&  x <= this.LowerRightX
		       &&  y <= this.UpperLeftY
		       &&  y >= this.LowerRightY;
		}

		// перекрываем прямоуголники
		public bool IntersectWith(ProjectMapData data)
		{

			this.CoverageUpperLeftX = Math.Max(data.UpperLeftX, this.UpperLeftX);
			this.CoverageUpperLeftY = Math.Min(data.UpperLeftY, this.UpperLeftY);

			this.CoverageLowerRightX = Math.Min(data.LowerRightX, this.LowerRightX);
			this.CoverageLowerRightY = Math.Max(data.LowerRightY, this.LowerRightY);

			this.CoverageArea = (CoverageLowerRightX - CoverageUpperLeftX) * (CoverageUpperLeftY - CoverageLowerRightY);
			this.CoveragePercent = (this.CoverageArea / data.RectArea) * 100;

			this.Priority = 4;

			if (this.AxisXStep == data.AxisXStep && this.AxisYStep == data.AxisYStep && this.CoverageArea >= 30)
			{
				this.Priority = 0;
			}
			else if (this.CoverageArea >= 60)
			{
				this.Priority = 1;
			}
			else if (this.AxisXStep == data.AxisXStep && this.AxisYStep == data.AxisYStep)
			{
				this.Priority = 2;
			}
			else if (this.AxisXStep * this.AxisYStep < data.AxisXStep * data.AxisYStep)
			{
				this.Priority = 3;
			}

			return this.CoverageArea > 0;
		}

		public bool IntersectWith(AreaCoordinates area, out AreaCoordinates coverageArea)
		{
			coverageArea.UpperLeft = new Coordinate()
			{
				X = Math.Max(area.UpperLeft.X, this.UpperLeftX),
				Y = Math.Min(area.UpperLeft.Y, this.UpperLeftY)
			};

			coverageArea.LowerRight = new Coordinate()
			{
				X = Math.Min(area.LowerRight.X, this.LowerRightX),
				Y = Math.Max(area.LowerRight.Y, this.LowerRightY)
			};

			return coverageArea.Area > 0;
		}

		public Coordinate IndexToUpperLeftCoordinate(int xIndex, int yIndex)
		{
			return new Coordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex,
				Y = this.UpperLeftY - this.AxisYStep * yIndex
			};
		}

		public Coordinate IndexToLowerLeftCoordinate(int xIndex, int yIndex)
		{
			return new Coordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex,
				Y = this.UpperLeftY - this.AxisYStep * yIndex - this.AxisYStep
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

		public Indexer CoordinateToIndexes(int x, int y)
		{
			return new Indexer
			{
				XIndex = (int)Math.Ceiling((this.UpperLeftX - x) / (double)this.AxisXStep) - 1,
				YIndex = this.AxisYNumber -((int)Math.Ceiling((this.UpperLeftY - (this.AxisYNumber * this.AxisYStep) - y) / (double)this.AxisXStep) - 1)

			};
		}
	}
}
