using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.AppComponent;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmConfig : IEntityOrmConfig
    {
        private readonly IComponentConfig _config;

        public EntityOrmConfig(IComponentConfig config)
        {
            this._config = config;
            // из конфиг аберем им яфала и читаем файл конфигурации окрежения ОРМ 

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
