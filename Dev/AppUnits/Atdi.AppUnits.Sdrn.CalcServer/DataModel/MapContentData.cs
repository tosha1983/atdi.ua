using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.DataModel
{
	internal class MapContentData
	{
		protected MapContentData(ProjectMapData data, ProjectMapType mapType)
		{
			this.ProjectMap = data;
			this.MapType = mapType;
			this.StepDataSize = DefineMapStepDataSize(mapType);
		}

		public ProjectMapType MapType;

		public ProjectMapData ProjectMap;

		public byte StepDataSize;

		public decimal SourceCoverage;

		public int SourceCount;

		private static byte DefineMapStepDataSize(ProjectMapType mapType)
		{
			if (mapType == ProjectMapType.Clutter || mapType == ProjectMapType.Building)
			{
				return 1;
			}
			else if (mapType == ProjectMapType.Relief)
			{
				return 2;
			}
			throw new InvalidOperationException($"Unsupported the map type '{mapType}'");
		}
	}

	internal class MapContentData<T> : MapContentData
		where T : struct
	{
		public T[] Content;

		public MapContentData(ProjectMapData data, ProjectMapType mapType) 
			: base(data, mapType)
		{
		}
	}
}
