using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Platform.AppComponent;

namespace Atdi.CoreServices.EntityOrm
{
    public class EntityOrm : IEntityOrm
    {
        private readonly IEntityOrmConfig _config;
        private readonly Dictionary<string, IEntityMetadata> _cashe;
        public EntityOrm(IEntityOrmConfig config)
        {
            this._config = config;
        }

        public IDataTypeMetadata GetDataTypeMetadata(string dataTypeName)
        {
            throw new NotImplementedException();
        }

        public IEntityMetadata GetEntityMetadata(string entityName)
        {
            if (_cashe.ContainsKey(entityName))
            {
                return _cashe[entityName];
            }
            // ...load
            throw new NotImplementedException();
        }

        public IUnitMetadata GetUnitMetadata(string unitName)
        {
            throw new NotImplementedException();
        }
    }
}
