using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis.Maps
{
    [Serializable]
    public class MapMetadata
	{
		/// <summary>
		/// Описание карты
		/// Внимание: для карт ATDI 160 символов
		/// </summary>
		public string Note;

		public string Projection;

		public string StepUnit;

		public int AxisXNumber;

		public int AxisXStep;

		public int AxisYNumber;

		public int AxisYStep;

		public int UpperLeftX;

		public int UpperLeftY;

		public int UpperRightX;

		public int UpperRightY;

		public int LowerLeftX;

		public int LowerLeftY;

		public int LowerRightX;

		public int LowerRightY;
	}
    [Serializable]
    public enum MapContentType
	{
		Unknown = 0,
		Relief = 1,
		Clutter = 2,
		Building = 3,
		ClutterDesc = 4
	}
}
