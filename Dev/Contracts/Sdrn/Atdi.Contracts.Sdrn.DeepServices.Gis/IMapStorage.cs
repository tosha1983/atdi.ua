using Atdi.DataModels.Sdrn.DeepServices.Gis.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis.Maps.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeepServices.Gis
{
	public interface IMapStorage : IDeepService
	{
		string DefineMapFileExtension(MapContentType contentType);

		MapMetadata GetMetadata(string filePath);

		T[] GetContent<T>(string filePath);

		CluttersDesc GetClattersDesc(string filePath);

	}
}
