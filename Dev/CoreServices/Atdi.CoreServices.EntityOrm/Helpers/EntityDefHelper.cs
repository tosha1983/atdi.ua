using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm.Metadata
{
    public static class EntityDefHelper
    {
        public static bool UsesInheritance(EntityDef entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extention
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Simple;
        }

        public static bool UsesBaseEntityPrimaryKey(EntityDef entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extention
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Simple;
        }
        public static bool UsesBaseEntity(EntityDef entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extention
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Simple;
        }
    }
}
