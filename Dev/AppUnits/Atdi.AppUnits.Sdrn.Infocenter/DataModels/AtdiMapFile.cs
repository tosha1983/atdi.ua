using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.DataModels
{
	internal class AtdiMapFile
	{
		/// <summary>
		/// Имя файла
		/// </summary>
		public string FileName;

		/// <summary>
		/// Текстовое описание карты
		/// </summary>
		public string Info;
		
		/// <summary>
		/// Тип карты
		/// </summary>
		public AtdiMapType MapType;

		/// <summary>
		/// Координаты области
		/// </summary>
		public AtdiMapCoordinates Coordinates;

		/// <summary>
		/// Точки по оси X 
		/// </summary>
		public AtdiMapAxis AxisX;
		
		/// <summary>
		/// Точки по оси Y
		/// </summary>
		public AtdiMapAxis AxisY;

		/// <summary>
		/// Тип данных содержимого карты
		/// </summary>
		public Type StepDataType;

		/// <summary>
		/// Единица измерения карты
		/// </summary>
		public string StepUnit;

		/// <summary>
		/// Используемая проекция
		/// </summary>
		public string Projection;

		//public int Max;
		//public int Min;
		//public int[] Statistics;

		/// <summary>
		/// Размер элемента данных точек в байтах, которые содержит карта.
		/// </summary>
		public byte StepDataSize => (byte)(StepDataType == typeof(byte) ? 1 : (StepDataType == typeof(ushort) ? 2 : (StepDataType == typeof(short) ? 2 : 0)));

		/// <summary>
		/// Расчетное значение размера полезных данных карты в байтах
		/// </summary>
		public int ContentCalcSize => AxisX.Number * AxisY.Number * StepDataSize;

		/// <summary>
		/// Реальное прочимтанное значение размера полезных данных карты в байтах
		/// Несовпадение с расчетным говорит о невалилности структуры карты
		/// </summary>
		public int ContentRealSize;

		/// <summary>
		/// Расчетное кол-во шагов содержащихся в карте
		/// </summary>
		public int StepCalcNumber => AxisX.Number * AxisY.Number;

		/// <summary>
		/// Реальное кол-во шагов содержащихся в карте исходя из прочитанных байт и методанных карты
		/// Несовпадение с расчетным говорит о невалилности структуры карты
		/// </summary>
		public int StepRealNumber => ContentRealSize / StepDataSize;

		

		public override string ToString()
		{
			return $"{MapType}: {Projection}; StepBox = '{AxisX.Step}{StepUnit}x{AxisY.Step}{StepUnit}' - '{AxisX.Number}x{AxisY.Number}'; {Coordinates}; File = '{FileName}'; Number = '{this.StepRealNumber}'; Size = '{this.ContentRealSize}'";
		}
	}
}
