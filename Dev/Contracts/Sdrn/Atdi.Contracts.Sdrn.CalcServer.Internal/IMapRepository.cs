using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer.Internal
{
    public interface IMapRepository
    {
	    

		ProjectMapData GetMapByName(IDataLayerScope dbScope, long projectId, string mapName);

		CluttersDesc GetCluttersDesc(IDataLayerScope dbScope, long mapId);
    }

    public static class MapSpecification
    {
	    public static readonly int CluttersMaxCount = 20;

		public static readonly short DefaultForRelief = (short)-9999;
	    public static readonly byte DefaultForClutter = (byte)0;
	    public static readonly byte DefaultForBuilding = (byte)0;
	}
}
