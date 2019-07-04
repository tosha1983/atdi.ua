﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.DataLayer
{
    public sealed class DataLayer : LoggedObject, IDataLayer
    {
        private readonly IDataLayerConfig _config;
        private readonly IServicesResolver _servicesResolver;

        public DataLayer(IDataLayerConfig config, IServicesResolver servicesResolver, ILogger logger) : base(logger)
        {
            this._config = config;
            this._servicesResolver = servicesResolver;
        }


        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            var engineConfig = this._config.GetEngineConfig<TContext>();

            switch (engineConfig.Type)
            {
                case DataEngineType.SqlServer:
                    var sqlEngine =  this._servicesResolver.Resolve<ISqlServerDataEngine>();
                    sqlEngine.SetConfig(engineConfig);
                    return sqlEngine;
                case DataEngineType.Oracle:
                    var oracleEngine = this._servicesResolver.Resolve<IOracleDataEngine>();
                    oracleEngine.SetConfig(engineConfig);
                    return oracleEngine;
                default:
                    throw new InvalidOperationException(Exceptions.EngineTypeNotSupported.With(engineConfig.Type));
            }

        }

    }
}
