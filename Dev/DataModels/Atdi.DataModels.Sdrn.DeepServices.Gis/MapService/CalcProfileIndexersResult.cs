using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis.MapService
{
    [Serializable]
    public struct CalcProfileIndexersResult
	{
		/// <summary>
		/// Подготовленый вызываемой стороной буфер индексаторов для заполнения результатами расчета профиля
		/// </summary>
		public ProfileIndexer[] Indexers;

		/// <summary>
		/// Стартовая позиция с которой расчету необходимо заполнять буфер индексаторов
		/// </summary>
		public int StartPosition;

		/// <summary>
		/// Количество элементво профиля
		/// </summary>
		public int IndexerCount;
	}
}
