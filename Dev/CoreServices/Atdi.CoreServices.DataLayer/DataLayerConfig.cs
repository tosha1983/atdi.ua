using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.AppComponent;

namespace Atdi.CoreServices.DataLayer
{
    internal sealed class DataLayerConfig : IDataLayerConfig
    {
        // <parameter name="DataContexts" value="{name=ICSM_DB, dataEngine=SqlServer, connectionStringConfig=DB_ICSM_ConnectionString}" />
        private readonly IComponentConfig _config;
        private readonly Dictionary<string, IDataEngineConfig> _dataEngineConfigs;

        public DataLayerConfig(IComponentConfig config)
        {
            this._config = config;
            this._dataEngineConfigs = new Dictionary<string, IDataEngineConfig>();

            var dataContextsParam = config[ConfigParameters.DataContexts];
            if (dataContextsParam != null)
            {
                var dataContextsString = Convert.ToString(dataContextsParam);
                if (!string.IsNullOrEmpty(dataContextsString))
                {
                    var dataContexts = dataContextsString.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (dataContexts.Length > 0)
                    {
                        foreach (var dataContextString in dataContexts)
                        {
                            var dataEngineConfig = new DataEngineConfig(dataContextString);
                            this._dataEngineConfigs[dataEngineConfig.ContextName] = dataEngineConfig;
                        }
                    }
                }
            }

        }

        public IDataEngineConfig GetEngineConfig<TContext>() where TContext : IDataContext, new()
        {
            var dataContext = new TContext();
            return this.GetEngineConfig(dataContext);
        }

        public IDataEngineConfig GetEngineConfig(IDataContext dataContext)
        {
            if (dataContext == null)
            {
                throw new ArgumentNullException(nameof(dataContext));
            }

            if (!_dataEngineConfigs.TryGetValue(dataContext.Name, out IDataEngineConfig config))
            {
                throw new InvalidOperationException($"Not found an engine config by context with name '{dataContext.Name}'");
            }
            return config;
        }
    }
}
