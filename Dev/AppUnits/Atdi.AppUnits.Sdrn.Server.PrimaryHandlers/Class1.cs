using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Platform.Logging;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers
{
    public class TestHandler
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IQueryExecutor _executor;

        public TestHandler(ILogger logger, IDataLayer<EntityDataOrm> dataLayer)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._executor = dataLayer.Executor<SdrnServerDataContext>();
        }

        void OnMessage()
        {
            var query = this._dataLayer.Builder
                .From<IAntenna>()
                .Select(
                    c => c.EXT1.FullName,
                    c => c.POS.PosY
                );

            var result = this._executor.Fetch(query, reader =>
            {
                string fullName = reader.GetValue(c => c.EXT1.FullName);
                return string.Empty;
            });
        }
    }
}
