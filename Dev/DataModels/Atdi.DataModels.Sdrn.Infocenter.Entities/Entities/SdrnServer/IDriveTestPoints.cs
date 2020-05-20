using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities.SdrnServer
{
	public interface IDriveTestPoints_PK
	{
		long Id { get; set; }
	}

	public interface IDriveTestPoints : IDriveTestPoints_PK
	{
		IDriveTest DRIVE_TEST { get; set; }

		/// <summary>
		/// byte[] == DriveTestPoint[]
		/// </summary>
		byte[] Data { get; set; }

		int Count { get; set; }
	}

	public struct DriveTestPoint
	{
		public Wgs84Coordinate Coordinate;
		public int Height_m;
		public float FieldStrength_dBmkVm;
		public float Level_dBm;
	}
}
