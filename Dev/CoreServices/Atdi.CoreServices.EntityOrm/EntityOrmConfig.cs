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
        private readonly XDocument _environment;
        public string Name { get; set; }
        public string Version { get; set; }
        public string RootPath { get; set; }
        public string Assembly { get; set; }
        public string Namespace { get; set; }
        public string EntitiesPath { get; set; }
        public string DataTypesPath { get; set; }
        public string UnitsPath { get; set; }
        public EntityOrmConfig(IComponentConfig config)
        {
            this._config = config;
            var dataContextsParam = config["EnvironmentFileName"];
            if (dataContextsParam != null)
            {
                var dataContextsString = Convert.ToString(dataContextsParam);
                if (!string.IsNullOrEmpty(dataContextsString))
                {
                    _environment = XDocument.Load(dataContextsString);
                    if (_environment != null)
                    {
                        XElement xElement = _environment.Element("Environment");
                        if (xElement.HasAttributes)
                        {
                            IEnumerable<XAttribute> xAttributes = xElement.Attributes();
                            if (xAttributes != null)
                            {
                                XAttribute attrName = xAttributes.ToList().Find(t => t.Name == "Name");
                                if (attrName != null)
                                {
                                    Name = attrName.Value;
                                }
                                XAttribute attrVersion = xAttributes.ToList().Find(t => t.Name == "Version");
                                if (attrVersion != null)
                                {
                                    Version = attrVersion.Value;
                                }
                            }
                        }
                        RootPath =  _environment.Element("Environment").Element("RootPath").ToString();
                        Assembly =  _environment.Element("Environment").Element("Assembly").ToString();
                        Namespace =  _environment.Element("Environment").Element("Namespace").ToString();
                        EntitiesPath =  _environment.Element("Environment").Element("EntitiesPath").ToString();
                        DataTypesPath =  _environment.Element("Environment").Element("DataTypesPath").ToString();
                        UnitsPath =  _environment.Element("Environment").Element("UnitsPath").ToString();
                    }
                }
            }
            EntityOrm entityOrm = new EntityOrm(this);
            entityOrm.GetUnitMetadata("Frequency.kHz");
        }
    
    }
}
