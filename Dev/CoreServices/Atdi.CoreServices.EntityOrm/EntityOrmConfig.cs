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

        /// <summary>
        /// НЕ ЗАБУДЬ УДАЛИТЬ
        /// </summary>
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
                            RootPath = environment.RootPath.Value.Replace(".", @"\");
                            Assembly = environment.Assembly.Value;
                            Namespace = environment.Namespace.Value;
                            if (RootPath != @"\")
                            {
                                EntitiesPath = string.Format(@"{0}\{1}\{2}", directory, RootPath, environment.EntitiesPath.Value);
                                DataTypesPath = string.Format(@"{0}\{1}\{2}", directory, RootPath, environment.DataTypesPath.Value);
                                UnitsPath = string.Format(@"{0}\{1}\{2}", directory, RootPath, environment.UnitsPath.Value);
                            }
                            else
                            {
                                EntitiesPath = string.Format(@"{0}\{1}", directory, environment.EntitiesPath.Value);
                                DataTypesPath = string.Format(@"{0}\{1}", directory, environment.DataTypesPath.Value);
                                UnitsPath = string.Format(@"{0}\{1}", directory, environment.UnitsPath.Value);
                            }
                        }
                    }
                }
            }
            /// НЕ ЗАБУДЬ УДАЛИТЬ
            var entityOrm = new EntityOrm(this);
            //entityOrm.GetEntityMetadata("IAntennaType");
            //entityOrm.GetEntityMetadata("IAntennaExten1");
            //entityOrm.GetEntityMetadata("ISensorSensitivites");

            this._dataLayer = new FakeDataLayer<EntityDataOrm>();
            this._logger = new FakeLogger();
            



            //entityOrm.GetDataTypeMetadata("DateTime", Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database);
            //entityOrm.GetUnitMetadata("Frequency.kHz");
            EnitityOrmDataLayer enitityOrmDataLayer = new EnitityOrmDataLayer(this._dataLayer, entityOrm, this._logger);
            /*
            var query = enitityOrmDataLayer.GetBuilder<MD.ISensor>()
                     .From()
                     .Select(c => c.Name)
                     //.Delete()
                     //.SetValue(c => c.Name, "Value");
                     .Where(c => c.Name, ConditionOperator.Equal, "1")
                     .Where(c => c.TechId, ConditionOperator.Equal, "2")
                     .OnTop(1);
           
            var sensorExistsInDb = enitityOrmDataLayer.Executor<SdrnServerDataContext>()
                .Execute<MD.ISensor>(query) == 0;
           */

            /*
              var query = enitityOrmDataLayer.GetBuilder<MD.IAntenna>()
                .From()
                .Select( c=> c.FrequencyMHz,
                         c => c.POS.Id,
                         c => c.Id,
                         c => c.Name,
                         c => c.TYPE,
                         c => c.TYPE.TYPE2.Name2,
                       //c=> c.POS.PosType,
                       //c => c.EXT1.FullName,
                       //c => c.EXT1.ShortName,
                       //c => c.EXT1.EXTENDED.EXT1.EXTENDED.EXT1,
                       c => c.PROP1.NamePropertyBase,
                       c => c.PROP1.PropName,
                       //c => c.EXT1.FullName,
                       c => c.PROP2.PropName,
                       c => c.EXT1.EXTENDED.PROP2.NamePropertyBase,
                       c => c.PROP1,
                       c => c.EXT1.FullName,
                       c => c.PROP3
                       //c => c.Name
                       )
                .OrderByDesc(x=>x.FrequencyMHz)
                .Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35)
                .OnTop(1);
            */

         
            var query = enitityOrmDataLayer.GetBuilder<MD.IAntenna>()
              .From()
              .Select(c => c.FrequencyMHz,
                       //c => c.POS.Id,
                       c => c.Name,
                       c => c.EXT1.FullName,
                       c => c.EXT1.EXT2.FullName2,
                       c => c.TYPE.TYPE2.TYPE3.Name3,
                       //c => c.EXT1.EXTENDED.EXT1.FullName,
                       c => c.PROP1.TableName,
                       c => c.POS.PosX,
                       c => c.POS.PSS2.PosType
                     )
              .OrderByDesc(x => x.FrequencyMHz)
              //.Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35)
              //.Where(c => c.PROP1.PropName, ConditionOperator.Equal, "i")
              .Where(c => c.Id, ConditionOperator.Equal, 2)
              .OnTop(1);

        
           /*
            var query = enitityOrmDataLayer.GetBuilder<MD.IAntenna>()
          .Delete()
          .Where(c => c.Id, ConditionOperator.Equal, 2);
            //.Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35);
            */

           /*
             var query = enitityOrmDataLayer.GetBuilder<MD.IAntenna>()
          .Insert()
          .SetValue(c => c.FrequencyMHz, 4535.346);
             //.Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35);
           */

            /*
            var query = enitityOrmDataLayer.GetBuilder<MD.IAntenna>()
         .Update()
         .Where(c => c.Id, ConditionOperator.Equal, 2)
         .SetValue(c => c.FrequencyMHz, 89.67);
            //.Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35);
            */
            var sensorExistsInDb = enitityOrmDataLayer.Executor<SdrnServerDataContext>()
            .Execute<MD.IAntenna>(query) == 0;
            
        }

    }




    
}
