using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.DeepServices.Gis.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis.Maps.Json;
using Newtonsoft.Json;
using System.IO;

namespace Atdi.AppUnits.Sdrn.DeepServices.Gis
{
	public class MapStorage : IMapStorage
	{
		public string DefineMapFileExtension(MapContentType contentType)
		{
			if (contentType == MapContentType.Relief)
			{
				return ".geo";
			}
			if (contentType == MapContentType.Clutter)
			{
				return ".sol";
			}
			if (contentType == MapContentType.ClutterDesc)
			{
				return ".sol.json";
			}
			if (contentType == MapContentType.Building)
			{
				return ".blg";
			}
			throw new InvalidOperationException($"Unsupported map content type '{contentType}'");
		}

		public void Dispose()
		{
			
		}

		public CluttersDesc GetClattersDesc(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(filePath));
			}

			if (!File.Exists(filePath))
			{
				return new CluttersDesc
				{
					Clutters = new CluttersDescClutter[] { },
					Frequencies = new CluttersDescFreq[] { }
				};
			}

			var cluttersDesc = JsonConvert.DeserializeObject<CluttersDesc>(File.ReadAllText(filePath));
			return cluttersDesc;
		}

		public T[] GetContent<T>(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(filePath));
			}

			var result = AtdiMapDecoder.DecodeContentType<T>(filePath);
			return result;
		}

		public MapMetadata GetMetadata(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(filePath));
			}

			var result = AtdiMapDecoder.DecodeMetadata(filePath);
			return result;
		}
	}
}
