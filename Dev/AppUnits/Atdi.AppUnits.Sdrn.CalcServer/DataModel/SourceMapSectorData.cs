using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.DataModel
{
	class SourceMapSectorData
	{
		public long SectorId;

		public SourceMapData Map;

		public int AxisXIndex;

		public int AxisYIndex;

		public int AxisXNumber;

		public int AxisYNumber;

		public int UpperLeftX;

		public int UpperLeftY;

		public int LowerRightX;

		public int LowerRightY;

		public byte[] Content;

		public T GetValue<T>(int xIndex, int yIndex)
			where T : struct
		{
			var type = typeof(T);

			// переводим в координаты - левый нижний угол
			var lowerLeft = this.Map.IndexToLowerLeftCoordinate(xIndex, yIndex);

			var sectorXIndex = (int)Math.Ceiling((lowerLeft.X - this.UpperLeftX + 1) / (double) this.Map.AxisXStep) - 1;
			var lowerLeftY = this.UpperLeftY - (this.AxisYNumber * this.Map.AxisYStep);
			var sectorYIndex = this.AxisXNumber - ((int)Math.Ceiling((lowerLeft.Y - lowerLeftY + 1) / (double)this.Map.AxisXStep));

			//var xIndex = (int)Math.Ceiling((x - upperLeftX + 1) / (double)xStep) - 1;
			//var yIndex = yNumber - ((int)Math.Ceiling((y - (upperLeftY - (yNumber * yStep)) + 1) / (double)yStep));

			if (type == typeof(byte))
			{
				return (T)(object)this.Content[sectorYIndex * AxisXNumber + sectorXIndex];
			}
			if (type == typeof(short))
			{
				return (T)(object)BitConverter.ToInt16(this.Content, sectorYIndex * AxisXNumber * 2 + sectorXIndex * 2);
			}

			throw new InvalidOperationException($"Unsupported value type '{type}'");
		}
	}
}
