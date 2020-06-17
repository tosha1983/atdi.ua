using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Test.Api.Sdrn.CalcServer.Client.DataModels;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client
{
	class MapMaker
	{
		private const int AtdiMapHeaderSize = 1010;

		const uint CoordinatesUpperLeftXOffset = 160;
		const uint CoordinatesUpperLeftYOffset = 175;
		const uint CoordinatesUpperRightXOffset = 190;
		const uint CoordinatesUpperRightYOffset = 205;

		const uint CoordinatesLowerLeftXOffset = 220;
		const uint CoordinatesLowerLeftYOffset = 235;
		const uint CoordinatesLowerRightXOffset = 250;
		const uint CoordinatesLowerRightYOffset = 265;

		const uint CoordinatesSize = 15;


		public static void Make(MapFile mapFile, string folderPath)
		{
			var bufferSize = AtdiMapHeaderSize + (mapFile.AxisX.Number * mapFile.AxisY.Number) * mapFile.StepDataSize;
			var buffer = new byte[bufferSize];
			Fill(mapFile, buffer);
			File.WriteAllBytes(Path.Combine(folderPath, mapFile.FileName), buffer);
		}

		private static void Fill(MapFile mapFile, byte[] buffer)
		{
			var data = EncodeInt(mapFile.Coordinates.UpperLeft.X, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesUpperLeftXOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.UpperLeft.Y, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesUpperLeftYOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.UpperRight.X, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesUpperRightXOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.UpperRight.Y, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesUpperRightYOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.LowerLeft.X, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesLowerLeftXOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.LowerLeft.Y, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesLowerLeftYOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.LowerRight.X, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesLowerRightXOffset, data.Length);

			data = EncodeInt(mapFile.Coordinates.LowerRight.Y, CoordinatesSize);
			Array.Copy(data, 0, buffer, CoordinatesLowerRightYOffset, data.Length);

			data = EncodeInt(mapFile.AxisX.Step, 10);
			Array.Copy(data, 0, buffer, 280, data.Length);

			data = EncodeInt(mapFile.AxisY.Step, 10);
			Array.Copy(data, 0, buffer, 290, data.Length);

			//map.StepUnit = DecodeString(body, 300, 1);
			buffer[300] = (byte) mapFile.StepUnit[0];

			//map.AxisX.Number = DecodeInt(body, 320, 10);
			//map.AxisY.Number = DecodeInt(body, 330, 10);

			data = EncodeInt(mapFile.AxisX.Number, 10, withoutZeros: true);
			Array.Copy(data, 0, buffer, 320, data.Length);

			data = EncodeInt(mapFile.AxisY.Number, 10, withoutZeros: true);
			Array.Copy(data, 0, buffer, 330, data.Length);

			//map.Projection = DecodeString(body, 340, 6);
			data = Encode(mapFile.Projection);
			Array.Copy(data, 0, buffer, 340, data.Length);

			//  map.Info = DecodeString(body, 0, 160);
			data = Encode(mapFile.MapNote);
			Array.Copy(data, 0, buffer, 0, data.Length);

			data = mapFile.DecodeContent();
			Array.Copy(data, 0, buffer, AtdiMapHeaderSize, data.Length);

			buffer[1009] = (byte) 26;
		}

		private static byte[] EncodeInt(int value, uint size, bool withoutZeros = false)
		{
			if (withoutZeros)
			{
				return value.ToString().Select(c => (byte)c).ToArray();
			}

			return  (value.ToString() + ".000000").Select(c =>  (byte)c).ToArray();
		}

		private static byte[] Encode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new byte[]{ };
			}
			return value.Select(c => (byte)c).ToArray();
		}
	}
}
