using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class SchemasMetadataConfig
    {
        public string Edition { get; private set; }

        public string SchemasPath { get; private set; }

        public string SchemaPrefix { get; private set; }

        public string[] Schemas { get; private set; }

        public string[] Modules { get; private set; }

        public SchemasMetadataConfig(IComponentConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.SchemasPath = Convert.ToString(config[Parameters.SchemasPath]);
            if (string.IsNullOrEmpty(this.SchemasPath))
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.SchemasPath));
            }

            this.SchemaPrefix = Convert.ToString(config[Parameters.SchemaPrefix]);
            if (string.IsNullOrEmpty(this.SchemaPrefix))
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.SchemaPrefix));
            }

            this.Edition = Convert.ToString(config[Parameters.Edition]);
            if (string.IsNullOrEmpty(this.Edition))
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Edition));
            }

            var schemasDesc = Convert.ToString(config[Parameters.Schemas]);
            if (string.IsNullOrEmpty(schemasDesc))
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Schemas));
            }

            var modulesDesc = Convert.ToString(config[Parameters.Modules]);
            if (string.IsNullOrEmpty(modulesDesc))
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Modules));
            }

            this.Schemas = schemasDesc.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (this.Schemas.Length == 0)
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Schemas));
            }

            this.Modules = modulesDesc.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (this.Modules.Length == 0)
            {
                throw new ArgumentException(Exceptions.UndefinedParameter.With(Parameters.Modules));
            }
        }
    }
}
