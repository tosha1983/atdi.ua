using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using Atdi.Platform.AppComponent;
using System.Xml.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake;
using Atdi.CoreServices.EntityOrm;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.Server;


namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmConfig : IEntityOrmConfig
    {

        
        private IDataLayer<EntityDataOrm> _dataLayer;
        private ILogger _logger;
        private IEntityOrm _entityOrm;

     
        public void InitEnvironment()
        {
            this._dataLayer = new FakeDataLayer<EntityDataOrm>();
            this._logger = new FakeLogger();
        }

        private readonly IComponentConfig _config;
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
                    var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.EnvironmentDef));
                    var reader = new StreamReader(dataContextsString);
                    object resenvironment = serializer.Deserialize(reader);
                    if (resenvironment is Atdi.CoreServices.EntityOrm.Metadata.EnvironmentDef)
                    {
                        var environment = resenvironment as Atdi.CoreServices.EntityOrm.Metadata.EnvironmentDef;
                        if (environment != null)
                        {
                            Name = environment.Name;
                            Version = environment.Version;
                            RootPath = environment.RootPath.Value;
                            Assembly = environment.Assembly.Value;
                            Namespace = environment.Namespace.Value;
                            EntitiesPath = string.Format(@"{0}\{1}", directory, environment.EntitiesPath.Value);
                            DataTypesPath = string.Format(@"{0}\{1}", directory, environment.DataTypesPath.Value);
                            UnitsPath = string.Format(@"{0}\{1}", directory, environment.UnitsPath.Value);
                        }
                    }
                }
            }

            this._dataLayer = new FakeDataLayer<EntityDataOrm>();
            this._logger = new FakeLogger();
            var entityOrm = new EntityOrm(this);
            entityOrm.GetDataTypeMetadata("DateTime", Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database);
            entityOrm.GetUnitMetadata("Frequency.kHz");
            EnitityOrmDataLayer enitityOrmDataLayer = new EnitityOrmDataLayer(this._dataLayer, entityOrm, this._logger);
            var query = enitityOrmDataLayer.GetBuilder<MD.ISensor>()
                     .From()
                     .Where(c => c.Name, ConditionOperator.Equal, "1")
                     .Where(c => c.TechId, ConditionOperator.Equal, "2")
                     .OnTop(1);
           
            var sensorExistsInDb = enitityOrmDataLayer.Executor<SdrnServerDataContext>()
                .Execute(query) == 0;

            // entityOrm.GetEntityMetadata("AntennaExten1");
            // entityOrm.GetEntityMetadata("SensorSensitivites");
        }

    }




    
}
