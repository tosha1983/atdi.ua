using Atdi.Contracts.CoreServices.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class EntityOrmConfig : API.IEntityOrmConfig
    {
        public EntityOrmConfig(IEntityOrmConfig ormConfig)
        {
            this.Name = ormConfig.Name;
            this.Namespace = ormConfig.Namespace;
            this.Version = ormConfig.Version;
        }

        public string Name { get; }

        public string Version { get; }

        public string Namespace { get; }
    }
}
