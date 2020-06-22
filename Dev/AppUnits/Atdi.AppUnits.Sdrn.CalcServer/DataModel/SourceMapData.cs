using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
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
		public string PriorityName;

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

		public void RecalculateCoverage(ProjectMapData data)
		{
			this.CoverageUpperLeftX = Math.Max(data.UpperLeftX, this.UpperLeftX);
			this.CoverageUpperLeftY = Math.Min(data.UpperLeftY, this.UpperLeftY);

			this.CoverageLowerRightX = Math.Min(data.LowerRightX, this.LowerRightX);
			this.CoverageLowerRightY = Math.Max(data.LowerRightY, this.LowerRightY);

			this.CoverageArea = 0;
			this.CoveragePercent = 0;

			if ((CoverageLowerRightX - CoverageUpperLeftX) <= 0
			    || (CoverageUpperLeftY - CoverageLowerRightY) <= 0)
			{
				throw new InvalidOperationException($"Something went wrong during the recount of coverage maps. Suddenly, the map coverage disappeared. Map ID #{MapId}; {MapName}; Project map ID #{data.ProjectMapId}");
			}

			this.CoverageArea = (CoverageLowerRightX - CoverageUpperLeftX) * (CoverageUpperLeftY - CoverageLowerRightY);
			this.CoveragePercent = (this.CoverageArea / data.RectArea) * 100;
		}

		// перекрываем прямоуголники
		public bool IntersectWith(ProjectMapData data)
		{

			this.CoverageUpperLeftX = Math.Max(data.UpperLeftX, this.UpperLeftX);
			this.CoverageUpperLeftY = Math.Min(data.UpperLeftY, this.UpperLeftY);

			this.CoverageLowerRightX = Math.Min(data.LowerRightX, this.LowerRightX);
			this.CoverageLowerRightY = Math.Max(data.LowerRightY, this.LowerRightY);

			if ((CoverageLowerRightX - CoverageUpperLeftX) <= 0
			||  (CoverageUpperLeftY - CoverageLowerRightY) <= 0)
			{
				return false;
			}

			this.CoverageArea = (CoverageLowerRightX - CoverageUpperLeftX) * (CoverageUpperLeftY - CoverageLowerRightY);
			this.CoveragePercent = (this.CoverageArea / data.RectArea) * 100;

			this.Priority = 4;
			this.PriorityName = "Priority 4";

			if ((   (this.AxisXStep == data.AxisXStep && this.AxisYStep == data.AxisYStep) 
			     || (IsAliquot(this.AxisXStep, data.AxisXStep) && IsAliquot(this.AxisYStep, data.AxisYStep)) 
			     ) && this.CoveragePercent >= 30)
			{
				this.Priority = 0;
				this.PriorityName = "Master Map";
			}
			else if (this.CoveragePercent >= 60)
			{
				this.Priority = 1;
				this.PriorityName = "Priority 1";
			}
			else if (this.AxisXStep == data.AxisXStep && this.AxisYStep == data.AxisYStep)
			{
				this.Priority = 2;
				this.PriorityName = "Priority 2";
			}
			else if (this.AxisXStep * this.AxisYStep < data.AxisXStep * data.AxisYStep)
			{
				this.Priority = 3;
				this.PriorityName = "Priority 3";
			}

			return this.CoverageArea > 0;
		}

		public static bool IsAliquot(int value1, int value2)
		{
			if (value1 > value2)
			{
				return value1 % value2 == 0;
			}
			if (value1 < value2)
			{
				return value2 % value1 == 0;
			}
			return true;
		}
		public bool IntersectWith(AreaCoordinates area, out AreaCoordinates coverageArea)
		{
			coverageArea.UpperLeft = new AtdiCoordinate()
			{
				X = Math.Max(area.UpperLeft.X, this.UpperLeftX),
				Y = Math.Min(area.UpperLeft.Y, this.UpperLeftY)
			};

			coverageArea.LowerRight = new AtdiCoordinate()
			{
				X = Math.Min(area.LowerRight.X, this.LowerRightX),
				Y = Math.Max(area.LowerRight.Y, this.LowerRightY)
			};

            if ((coverageArea.LowerRight.X - coverageArea.UpperLeft.X) <= 0
            || (coverageArea.UpperLeft.Y - coverageArea.LowerRight.Y) <= 0)
            {
                return false;
            }

            return coverageArea.Area > 0;
		}

		public AtdiCoordinate IndexToUpperLeftCoordinate(int xIndex, int yIndex)
		{
			return new AtdiCoordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex,
				Y = this.UpperLeftY - this.AxisYStep * yIndex
			};
		}

		public AtdiCoordinate IndexToLowerLeftCoordinate(int xIndex, int yIndex)
		{
			return new AtdiCoordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex,
				Y = this.UpperLeftY - this.AxisYStep * yIndex - this.AxisYStep
			};
		}

		public AtdiCoordinate IndexToLowerRightCoordinate(int xIndex, int yIndex)
		{
			return new AtdiCoordinate
			{
				X = this.UpperLeftX + this.AxisXStep * xIndex + this.AxisXStep,
				Y = this.UpperLeftY - this.AxisYStep * yIndex - this.AxisYStep
			};
		}

		public ProfileIndexer CoordinateToIndexes(int x, int y)
		{
			return new ProfileIndexer
			{
				XIndex = (int)Math.Ceiling((x - this.UpperLeftX + 1) / (double)this.AxisXStep) - 1,
				YIndex = this.AxisYNumber - ((int)Math.Ceiling((y - (this.UpperLeftY - (this.AxisYNumber * this.AxisYStep)) + 1) / (double)this.AxisYStep))

			};


			//var xIndex = (int)Math.Ceiling((x - upperLeftX + 1) / (double)xStep) - 1;
			//var yIndex = yNumber - ((int)Math.Ceiling((y - (upperLeftY - (yNumber * yStep)) + 1) / (double)yStep));
		}


		public bool HitIndexes(int xIndex, int yIndex)
		{
			return (xIndex >= 0 && yIndex >= 0 && xIndex < this.AxisXNumber && yIndex < this.AxisYNumber);
		}
	}
}
