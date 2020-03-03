﻿using System;
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

		public int AxisXNumber;

		public int AxisXStep;

		public int AxisYNumber;

		public int AxisYStep;

		public int UpperLeftX;

		public int UpperLeftY;

		public int LowerRightX;

		public int LowerRightY;

		public int StepsNumber => this.AxisXNumber * this.AxisYNumber;

		public int RectArea => (this.LowerRightX - this.UpperLeftX) * (this.UpperLeftY - this.LowerRightY);

		public bool Has(int x, int y)
		{
			return x >= this.UpperLeftX
			       && x <= this.LowerRightX
			       && y <= this.UpperLeftY
			       && y >= this.LowerRightY;
		}
	}
}