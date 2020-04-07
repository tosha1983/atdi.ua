using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis.MapService
{
	public struct CalcProfileIndexersArgs
	{
		/// <summary>
		/// Координата опорной точки с которой нужно начать построить профиль
		/// </summary>
		public AtdiCoordinate Point;

		/// <summary>
		/// Координата целевой точки до которой нужно построить профиль
		/// </summary>
		public AtdiCoordinate Target;

		/// <summary>
		/// Координата нижней левой точки начала карты для которой расчитываються индексы
		/// </summary>
		public AtdiCoordinate Location;

		/// <summary>
		/// Размер шага в метрах по оси Х
		/// </summary>
		public decimal AxisXStep;

		/// <summary>
		/// Размер шага в метрах по оси Y
		/// </summary>
		public decimal AxisYStep;

		/// <summary>
		/// Количество шагов по оcи Y
		/// </summary>
		public int AxisYNumber;
	}
}
