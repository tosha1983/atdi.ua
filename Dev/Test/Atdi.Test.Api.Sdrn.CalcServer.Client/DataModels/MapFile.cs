using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common.Extensions;
using Atdi.Test.Api.Sdrn.CalcServer.Client.Helpers;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.DataModels
{
	internal class MapFile
	{
		/// <summary>
		/// Имя файла
		/// </summary>
		public string MapName;

		/// <summary>
		/// 
		/// </summary>
		public string MapNote;

		public string FileName =>
			MapName + (MapType == MapType.Relief ? ".geo" : (MapType == MapType.Building) ? ".blg" : ".sol");

		/// <summary>
		/// Тип карты
		/// </summary>
		public MapType MapType;

		/// <summary>
		/// Тип карты
		/// </summary>
		public string MapTypeName;

		/// <summary>
		/// Координаты области
		/// </summary>
		public MapCoordinates Coordinates;

		/// <summary>
		/// Точки по оси X 
		/// </summary>
		public MapAxis AxisX;
		
		/// <summary>
		/// Точки по оси Y
		/// </summary>
		public MapAxis AxisY;

		/// <summary>
		/// Тип данных содержимого карты
		/// </summary>
		public string StepDataType;

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
		public byte StepDataSize;



		public string ContentType;
		public string ContentEncoding;
		public byte[] Content;

		public byte[] DecodeContent(bool isSector)
		{

			var raw = this.Content; 
			if (!string.IsNullOrEmpty(ContentEncoding))
			{
				if (ContentEncoding.Contains("compressed"))
				{
					raw = Compressor.Decompress(raw);
				}
				else
				{
					throw new InvalidOperationException($"Unsupported encoding '{ContentEncoding}'");
				}
			}

			if (isSector)
			{
				return raw;
			}

			if (typeof(byte[]).AssemblyQualifiedName == ContentType)
			{
				return raw.Deserialize<byte[]>();
			}
			else if (typeof(short[]).AssemblyQualifiedName == ContentType)
			{
				var shortArray = raw.Deserialize<short[]>();
				var result = new byte[shortArray.Length * sizeof(short)];
				Buffer.BlockCopy(shortArray, 0, result, 0, result.Length);
				return result;
			}
			else
			{
				throw new InvalidOperationException($"Unsupported content type '{ContentType}'");
			}

			
		}
		public override string ToString()
		{
			return $"{MapType}: {Projection}; StepBox = '{AxisX.Step}{StepUnit}x{AxisY.Step}{StepUnit}' - '{AxisX.Number}x{AxisY.Number}'; {Coordinates}; Name = '{MapName}'; Number = '{AxisX.Number * AxisY.Number}'; Size = '{AxisX.Number * AxisY.Number * StepDataSize}'";
		}
	}
}
