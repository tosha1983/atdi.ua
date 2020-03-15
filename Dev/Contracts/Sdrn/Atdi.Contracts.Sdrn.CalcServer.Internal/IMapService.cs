using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer.Internal
{
    public interface IMapService
    {
	    ProjectMapData GetMapByName(long projectId, string mapName);
	    
	}
}
