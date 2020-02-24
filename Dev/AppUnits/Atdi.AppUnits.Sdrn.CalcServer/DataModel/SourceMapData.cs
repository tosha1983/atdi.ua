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

			if (this.AxisXStep * this.AxisYStep == data.AxisXStep * data.AxisYStep && this.CoverageArea >= 30)
			{
				this.Priority = 1;
			}
			else if (this.CoverageArea >= 60)
			{
				this.Priority = 2;
			}
			else if (this.AxisXStep * this.AxisYStep < data.AxisXStep * data.AxisYStep)
			{
				this.Priority = 3;
			}

			return this.CoverageArea > 0;
		}
	}
}
