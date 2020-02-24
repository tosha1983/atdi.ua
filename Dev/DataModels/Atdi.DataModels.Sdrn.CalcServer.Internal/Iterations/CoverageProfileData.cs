using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public sealed class CoverageProfileData
	{
		/// <summary>
		/// Координаты
		/// </summary>
		public struct Coordinate
		{
			public int X;
			public int Y;

			public override string ToString()
			{
				return $"({X},{Y})";
			}
		}

		/// <summary>
		/// Стурктура масштаба оси
		/// </summary>
		public struct Axis
		{
			/// <summary>
			/// Кол-во шагов
			/// </summary>
			public int Number;

			/// <summary>
			/// Размер шага в единицах карты
			/// </summary>
			public int Step;

			/// <summary>
			/// Длина оси в единицах карты
			/// </summary>
			public int Size => Step * Number;

			public override string ToString()
			{
				return $"Num = '{Number}', Step = '{Step}', Size = '{Size}'";
			}
		}

		/// <summary>
		/// Буффер, содержащий матрицу с данными для профиля
		/// </summary>
		public byte[] Buffer;

		/// <summary>
		/// Позиция бокса: Lower Left x and Y
		/// </summary>
		public Coordinate Location;

		/// <summary>
		/// размер данных в боксе
		/// </summary>
		public byte DataSize;

		/// <summary>
		/// Опорная точка, от которой необходмио считать профели до целевых
		/// </summary>
		public Coordinate Point;

		/// <summary>
		/// Коодинаты точек по отношению к которым нужно расчитать профили 
		/// </summary>
		public Coordinate[] Points;

		/// <summary>
		/// масштаб по оис X
		/// </summary>
		public Axis AxisX;

		/// <summary>
		/// масштаб по оис Y
		/// </summary>
		public Axis AxisY;

		/// <summary>
		/// Подготовленный буфер для расчитанных профелей, память уже выделена с запасом линейно
		/// Формат: [данные профиля1][данные профиля2][данные профиля3]
		/// </summary>
		public byte[] Profiles;

		/// <summary>
		/// Размеры каждого профиля в единицах, для перевода в байты необходимо умножить DataSize
		/// </summary>
		public int[] Sizes;

		/// <summary>
		/// Позиция в масиве в первом измерения с которог необхождимо начать заполннение
		/// </summary>
		public int Offeset;
	}
}
