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
        private readonly string XsdSchema = "{http://schemas.atdi.com/orm/entity.xsd}";
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
                string directory = System.IO.Path.GetDirectoryName(Convert.ToString(dataContextsParam));
                var dataContextsString = Convert.ToString(dataContextsParam);
                if (!string.IsNullOrEmpty(dataContextsString))
                {
                    _environment = XDocument.Load(dataContextsString);
                    if (_environment != null)
                    {
                        XElement xElement = _environment.Element(XsdSchema+"Environment");
                        if (xElement != null)
                        {
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
                           
                            RootPath = xElement.Element(XsdSchema + "RootPath").Value;
                            Assembly = xElement.Element(XsdSchema + "Assembly").Value;
                            Namespace = xElement.Element(XsdSchema + "Namespace").Value;
                            EntitiesPath = directory + @"\"+ xElement.Element(XsdSchema + "EntitiesPath").Value;
                            DataTypesPath = directory + @"\" + xElement.Element(XsdSchema + "DataTypesPath").Value;
                            UnitsPath = directory + @"\" + xElement.Element(XsdSchema + "UnitsPath").Value;
                        }
                    }
                }
            }
            EntityOrm entityOrm = new EntityOrm(this);
            entityOrm.GetUnitMetadata("Frequency.kHz");
            entityOrm.GetDataTypeMetadata("Counter.xml", Atdi.Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database);
        }
    
    }
}
