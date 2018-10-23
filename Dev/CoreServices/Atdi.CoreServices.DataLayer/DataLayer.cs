using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.DataLayer
{
    public sealed class DataLayer : LoggedObject, IDataLayer
    {
        private readonly IDataLayerConfig _config;

        public DataLayer(IDataLayerConfig config, ILogger logger) : base(logger)
        {
            this._config = config;
        }


        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            var engineConfig = this._config.GetEngineConfig<TContext>();

            switch (engineConfig.Type)
            {
                case DataEngineType.SqlServer:
                    return new SqlServerDataEngine(engineConfig, this.Logger);
                case DataEngineType.Oracle:
                    return new OracleDataEngine(engineConfig, this.Logger);
                default:
                    throw new InvalidOperationException(Exceptions.EngineTypeNotSupported.With(engineConfig.Type));
            }

        }

    }
}
