using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.AppComponent;
using System.Xml.Linq;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmConfig : IEntityOrmConfig
    {
        private readonly IComponentConfig _config;
        public IEntityOrmConfig _ormConfig;
        public EntityOrmConfig(IComponentConfig config)
        {
            this._config = config;
            var dataContextsParam = config["EnvironmentFileName"];
            if (dataContextsParam != null)
            {
                var dataContextsString = Convert.ToString(dataContextsParam);
                if (!string.IsNullOrEmpty(dataContextsString))
                {
                    XDocument xdoc = XDocument.Load(dataContextsString);
                    XElement RootPath = xdoc.Element("Environment").Element("RootPath");
                    XElement Assembly = xdoc.Element("Environment").Element("Assembly");
                    XElement Namespace = xdoc.Element("Environment").Element("Namespace");
                    XElement EntitiesPath = xdoc.Element("Environment").Element("EntitiesPath");
                    XElement DataTypesPath = xdoc.Element("Environment").Element("DataTypesPath");
                    if (Assembly != null)
                    {
                        _ormConfig.Assembly = 
                    }
                }
            }
        }

        public string Name => throw new NotImplementedException();

        public string Version => throw new NotImplementedException();

        public string RootPath => throw new NotImplementedException();

        public string Assembly => throw new NotImplementedException();

        public string Namespace => throw new NotImplementedException();

        public string EntitiesPath => throw new NotImplementedException();

        public string DataTypesPath => throw new NotImplementedException();

        public string UnitsPath => throw new NotImplementedException();
    }
}
