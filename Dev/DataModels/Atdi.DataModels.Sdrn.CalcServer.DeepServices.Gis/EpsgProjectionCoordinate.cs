﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.DeepServices.Gis
{
	/// <summary>
	/// Метровые координаты с проекцией
	/// </summary>
	public struct EpsgProjectionCoordinate
	{
		public uint Projection;
		public double X;
		public double Y;
	}
}
