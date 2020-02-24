using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.CoreServices.EntityOrm
{
	class EntityOrmComponentConfig
	{
		class OrmContextConfig
		{
			public string Context;
			public string Path;
		}

		private Dictionary<string, OrmContextConfig> _ormContexts;

		[ComponentConfigProperty("EnvironmentFileName")]
		public string DefaultEnvironmentPath{ get; set; }

		[ComponentConfigProperty("Environments")]
		public string Environments { get; set; }


		private OrmContextConfig DecodeContext(string configSource)
		{
			var value = configSource;
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
				throw new ArgumentException($"Incorrect engine config string '{configSource}'");
			}
			var contextConfig = new OrmContextConfig();
			foreach (var part in parts)
			{
				var attrs = part.Split(new string[] { "=", " " }, StringSplitOptions.RemoveEmptyEntries);
				if (attrs.Length != 2)
				{
					throw new ArgumentException($"Incorrect engine config string '{configSource}' in part '{part}'");
				}

				if ("context".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
				{
					contextConfig.Context = attrs[1];
				}
				else if ("path".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
				{
					contextConfig.Path = attrs[1];
				}
			}

			return contextConfig;
		}

		public string GetEnvironmentPathByContext(string contextName)
		{
			if (string.IsNullOrEmpty(this.Environments))
			{
				return null;
			}

			if (_ormContexts == null)
			{
				_ormContexts = new Dictionary<string, OrmContextConfig>();

				var dataContexts = this.Environments.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (dataContexts.Length > 0)
				{
					foreach (var dataContextString in dataContexts)
					{
						var contextConfig = this.DecodeContext(dataContextString);
						_ormContexts.Add(contextConfig.Context, contextConfig);
					}
				}

			}

			if (_ormContexts.TryGetValue(contextName, out OrmContextConfig configContext))
			{
				return configContext.Path;
			}

			return null;
		}
	}
}
