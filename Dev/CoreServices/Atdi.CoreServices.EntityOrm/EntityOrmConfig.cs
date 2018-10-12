﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.AppComponent;
using System.Xml.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;


namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmConfig : IEntityOrmConfig
    {
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
            var entityOrm = new EntityOrm(this);
            entityOrm.GetDataTypeMetadata("DateTime", Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database);
            entityOrm.GetUnitMetadata("Frequency.kHz");
            entityOrm.GetEntityMetadata("AntennaExten1");
            //entityOrm.GetEntityMetadata("SensorSensitivites");


        }

    }




    
}
