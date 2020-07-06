using Atdi.DataModels.Sdrn.DeepServices.Gis.Maps;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeepServices.Gis.Maps
{
	internal static class AtdiMapDecoder
	{
		private const int AtdiMapHeaderSize = 1010;

		private static MapContentType DefineMapType(string fileName)
		{
			var ext = Path.GetExtension(fileName);
			if (".geo".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				return MapContentType.Relief;
			}
			if (".sol".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				return MapContentType.Clutter;
			}
			if (".blg".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				return MapContentType.Building;
			}
			// clutterDescFileName.sol.json
			if (".json".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				var jsonFileName = Path.GetFileNameWithoutExtension(fileName);
				ext = Path.GetExtension(jsonFileName);
				if (".sol".Equals(ext, StringComparison.OrdinalIgnoreCase))
				{
					return MapContentType.ClutterDesc;
				}
			}
			throw new InvalidOperationException($"Unsupported file extension '{ext}'");
		}

		public static T[] DecodeContentType<T>(string path)
		{
			if (!File.Exists(path))
			{
				return default(T[]);
			}

			var metadata = DecodeMetadata(path);
			using (var stream = File.OpenRead(path))
			{
				stream.Position = AtdiMapHeaderSize;

				var type = typeof(T);
				if (type == typeof(byte))
				{
					var buffer = new byte[ metadata.AxisXNumber * metadata.AxisYNumber];
					stream.Read(buffer, 0, buffer.Length);
					return (T[])(object)buffer;
				}

				if (type == typeof(short))
				{
					var byteBuffer = new byte[metadata.AxisXNumber * metadata.AxisYNumber * 2];
					stream.Read(byteBuffer, 0, byteBuffer.Length);

					var shortBuffer = new short[metadata.AxisXNumber * metadata.AxisYNumber];
					Buffer.BlockCopy(byteBuffer, 0, shortBuffer, 0, byteBuffer.Length);
					return (T[])(object)shortBuffer;
				}
				throw new InvalidOperationException($"Unsupported value type of map content: '{type}'");
			}
		}

		public static MapMetadata DecodeMetadata(string path)
		{
			using (var stream = File.OpenRead(path))
			{
				var buffer = new byte[AtdiMapHeaderSize];
				var realLength = stream.Read(buffer, 0, buffer.Length);
				if (realLength != buffer.Length)
				{
					throw new InvalidOperationException($"Invalid map file '{path}' size. Expected minimum of {buffer.Length} bytes.");
				}

				return DecodeMetadata(buffer);
			}
		}
		private static MapMetadata DecodeMetadata(byte[] body)
		{
			const uint CoordinatesUpperLeftXOffset = 160;
			const uint CoordinatesUpperLeftYOffset = 175;
			const uint CoordinatesUpperRightXOffset = 190;
			const uint CoordinatesUpperRightYOffset = 205;

			const uint CoordinatesLowerLeftXOffset = 220;
			const uint CoordinatesLowerLeftYOffset = 235;
			const uint CoordinatesLowerRightXOffset = 250;
			const uint CoordinatesLowerRightYOffset = 265;

			const uint CoordinatesSize = 15;

			var metadata = new MapMetadata();

			// required properties
			metadata.UpperLeftX = DecodeInt(body, CoordinatesUpperLeftXOffset, CoordinatesSize);
			metadata.UpperLeftY = DecodeInt(body, CoordinatesUpperLeftYOffset, CoordinatesSize);

			metadata.AxisXStep = DecodeInt(body, 280, 10);
			metadata.AxisYStep = DecodeInt(body, 290, 10);

			metadata.AxisXNumber = DecodeInt(body, 320, 10);
			metadata.AxisYNumber = DecodeInt(body, 330, 10);

			metadata.Projection = DecodeProjectionString(body, 340, 8);

			// not required properties

			// UpperRight
			var notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesUpperRightXOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				metadata.UpperRightX = notRequiredIntValue.Value;
			}
			else
			{
				metadata.UpperRightX = metadata.UpperLeftX + metadata.AxisXStep * metadata.AxisXNumber;
			}
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesUpperRightYOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				metadata.UpperRightY = notRequiredIntValue.Value;
			}
			else
			{
				metadata.UpperRightY = metadata.UpperLeftY;
			}

			// LowerLeft
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerLeftXOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				metadata.LowerLeftX = notRequiredIntValue.Value;
			}
			else
			{
				metadata.LowerLeftX = metadata.UpperLeftX;
			}
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerLeftYOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				metadata.LowerLeftY = notRequiredIntValue.Value;
			}
			else
			{
				metadata.LowerLeftY = metadata.UpperLeftY - metadata.AxisYStep * metadata.AxisYNumber;
			}

			// LowerRight
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerRightXOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				metadata.LowerRightX = notRequiredIntValue.Value;
			}
			else
			{
				metadata.LowerRightX = metadata.UpperRightX;
			}
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerRightYOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				metadata.LowerRightY = notRequiredIntValue.Value;
			}
			else
			{
				metadata.LowerRightY = metadata.LowerLeftY;
			}




			metadata.StepUnit = DecodeString(body, 300, 1);
			// без указания единици измерения карты считаем ее "в метрах"
			if (string.IsNullOrEmpty(metadata.StepUnit))
			{
				metadata.StepUnit = "M";
			}


			metadata.Note = DecodeString(body, 0, 160);
			//map.Min = DecodeInt(body, 380, 10);
			//map.Max = DecodeInt(body, 390, 10);

			// валидация
			ValidateMetadata(metadata);

			// Карту нужно сдивнуть в лево и вверх на step/2
			var xOffset = metadata.AxisXStep / 2;
			var yOffset = metadata.AxisYStep / 2;

			metadata.UpperLeftX -= xOffset;
			metadata.UpperRightX -= xOffset;
			metadata.LowerLeftX -= xOffset;
			metadata.LowerRightX -= xOffset;
			metadata.UpperLeftY += yOffset;
			metadata.UpperRightY += yOffset;
			metadata.LowerLeftY += yOffset;
			metadata.LowerRightY += yOffset;

			// повторная валидация после сдвига
			ValidateMetadata(metadata);

			return metadata;
		}

		private static void ValidateMetadata(MapMetadata metadata)
		{
			if (metadata.UpperLeftY != metadata.UpperRightY)
			{
				throw new InvalidDataException("Incorrect coordinates for the upper axis y");
			}

			if (metadata.LowerLeftY != metadata.LowerRightY)
			{
				throw new InvalidDataException("Incorrect coordinates for the lower axis y");
			}

			if (metadata.UpperLeftX != metadata.LowerLeftX)
			{
				throw new InvalidDataException("Incorrect coordinates for the left axis X");
			}

			if (metadata.UpperRightX != metadata.LowerRightX)
			{
				throw new InvalidDataException("Incorrect coordinates for the right axis X");
			}


			if (metadata.UpperLeftX > metadata.UpperRightX)
			{
				throw new InvalidDataException("Incorrect coordinates for the upper axis X");
			}

			if (metadata.LowerLeftX > metadata.LowerRightX)
			{
				throw new InvalidDataException("Incorrect coordinates for the lower axis X");
			}

			if (metadata.UpperLeftY < metadata.LowerLeftY)
			{
				throw new InvalidDataException("Incorrect coordinates for the left axis y");
			}

			if (metadata.UpperRightY < metadata.LowerRightY)
			{
				throw new InvalidDataException("Incorrect coordinates for the right axis y");
			}

			if (metadata.AxisXNumber * metadata.AxisXStep != Math.Abs(metadata.UpperRightX - metadata.UpperLeftX))
			{
				throw new InvalidDataException("Incorrect X-coordinates or X-axis properties");
			}
			if (metadata.AxisXNumber * metadata.AxisXStep != Math.Abs(metadata.LowerRightX - metadata.LowerLeftX))
			{
				throw new InvalidDataException("Incorrect X-coordinates or X-axis properties");
			}

			if(metadata.AxisYNumber * metadata.AxisYStep != Math.Abs(metadata.UpperLeftY - metadata.LowerLeftY))
			{
				throw new InvalidDataException("Incorrect Y-coordinates or Y-axis properties");
			}
			if (metadata.AxisYNumber * metadata.AxisYStep != Math.Abs(metadata.UpperRightY - metadata.LowerRightY))
			{
				throw new InvalidDataException("Incorrect Y-coordinates or Y-axis properties");
			}
		}

		private static float DecodeFloat(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			try
			{
				return float.Parse(statement);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot decode source as float: '{statement}', NumberDecimalSeparator='{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'", e);
			}

		}
		private static int DecodeInt(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			try
			{
				return Convert.ToInt32((decimal.Parse(statement)));
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot decode source as int: '{statement}', NumberDecimalSeparator='{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'", e);
			}
		}

		private static int? DecodeIntAsNotRequired(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			try
			{
				if (string.IsNullOrEmpty(statement))
				{
					return null;
				}
				return Convert.ToInt32((decimal.Parse(statement)));
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot decode source as int: '{statement}', NumberDecimalSeparator='{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'", e);
			}
		}

		private static string DecodeString(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "");
			return statement;
		}

		private static string DecodeProjectionString(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size); //.Replace("\0", "");
			var result = new StringBuilder();
			foreach (var c in statement)
			{
				if (c == '\0')
				{
					return result.ToString();
				}

				result.Append(c);
			}
			return result.ToString();
		}
	}
}
