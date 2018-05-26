using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.CoreServices.DataLayer
{
    class DataEngineConfig : IDataEngineConfig
    {
        public DataEngineConfig(string configString)
        {
            if (string.IsNullOrEmpty(configString))
            {
                throw new ArgumentNullException(nameof(configString));
            }
            var value = configString;
            if (value.StartsWith("{"))
            {
                value = value.Substring(1);
            }
            if (value.EndsWith("}"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            var parts = value.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new ArgumentException($"Incorrect engine config string '{configString}'");
            }

            foreach (var part in parts)
            {
                var attrs = part.Split(new string[] { "=", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (attrs.Length != 2)
                {
                    throw new ArgumentException($"Incorrect engine config string '{configString}' in part '{part}'");
                }

                if ("name".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.ContextName = attrs[1];
                }
                else if ("dataEngine".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.Type = (DataEngineType)Enum.Parse(typeof(DataEngineType),  attrs[1], true);
                }
                else if ("connectionStringConfig".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    var connectionConfig = System.Configuration.ConfigurationManager.ConnectionStrings[attrs[1]];
                    if (connectionConfig == null)
                    {
                        throw new ArgumentException($"Incorrect engine config string '{configString}' in part '{part}': not found connection string with config name '{attrs[1]}'");
                    }
                    this.ConnectionString = connectionConfig.ConnectionString;
                    this.ProviderName = connectionConfig.ProviderName;
                }
            }
        }

        public string ContextName {get; private set;}

        public DataEngineType Type { get; private set; }

        public string ConnectionString { get; private set; }

        public string ProviderName { get; private set; }


    }
}
