using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	
	public class Utils 
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Заказываем у контейнера нужные сервисы
		/// </summary>
		public Utils(ILogger logger)
		{
			_logger = logger;
		}

        public bool CompareGSID(bool GSID1, bool GSID2, string Standard)
        {
            return false;
        }
    }
}
