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
using Atdi.CoreServices.EntityOrm;
using Atdi.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels.DataConstraint;



namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmConfig : IEntityOrmConfig
    {

        //private readonly IComponentConfig _config;

        public string Name { get; set; }
        public string Version { get; set; }
        public string RootPath { get; set; }
        public string Assembly { get; set; }
        public string Namespace { get; set; }
        public string EntitiesPath { get; set; }
        public string DataTypesPath { get; set; }
        public string UnitsPath { get; set; }


        public EntityOrmConfig(string path)
        {
            try
            {
	            if (string.IsNullOrEmpty(path))
	            {
					return;
	            }

	            var directory = System.IO.Path.GetDirectoryName(path);
	            var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.EnvironmentDef));
                using (var reader = new StreamReader(path))
                {
	                var source = serializer.Deserialize(reader);
	                if (!(source is EnvironmentDef environment))
	                {
		                return;
	                }
	                Name = environment.Name;
	                Version = environment.Version;
	                RootPath = environment.RootPath.Value.Replace(".", @"\");
	                Assembly = environment.Assembly.Value;
	                Namespace = environment.Namespace.Value;
	                if (RootPath != @"\")
	                {
		                EntitiesPath = $@"{directory}\{RootPath}\{environment.EntitiesPath.Value}";
		                DataTypesPath = $@"{directory}\{RootPath}\{environment.DataTypesPath.Value}";
		                UnitsPath = $@"{directory}\{RootPath}\{environment.UnitsPath.Value}";
	                }
	                else
	                {
		                EntitiesPath = $@"{directory}\{environment.EntitiesPath.Value}";
		                DataTypesPath = $@"{directory}\{environment.DataTypesPath.Value}";
		                UnitsPath = $@"{directory}\{environment.UnitsPath.Value}";
	                }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(string.Format(Exceptions.ErrorLoadEnvironment, e.Message));
            }
        }
    }
}
