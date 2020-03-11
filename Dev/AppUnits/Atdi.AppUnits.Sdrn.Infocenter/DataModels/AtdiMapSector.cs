using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.DataModels
{
	internal class AtdiMapSector
	{
		/// <summary>
		/// Идентификатор карты которой принадлежит секция
		/// </summary>
		public long MapId;

		/// <summary>
		/// Кол-во шагов по X
		/// </summary>
		public int AxisXIndex;

		/// <summary>
		/// Кол-во шагов по X
		/// </summary>
		public int AxisYIndex;

		/// <summary>
		/// Кол-во шагов по X
		/// </summary>
		public int AxisXNumber;

		/// <summary>
		/// Кол-во шагов по X
		/// </summary>
		public int AxisYNumber;
		/// <summary>
		/// Координаты области
		/// </summary>
		public AtdiMapCoordinates Coordinates;

		/// <summary>
		/// Содержимое секции 
		/// </summary>
		public AtdiMapContent Content;

		public override string ToString()
		{
			return $"{AxisYIndex:D04}:{AxisXIndex:D04} ({AxisXNumber}x{AxisYNumber}) {this.Coordinates}";
		}
	}
}
