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

        public EntityOrm(IEntityOrmConfig config)
        {
            this._config = config;
        }

        public IEntityMetadata GetEntityMetadata(string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
