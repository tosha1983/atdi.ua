using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public class EntityPathDescriptor
    {
        // квалифицированное имя сущности,  состоящее из
        public string QualifiedName { get; set; }

        public string Name { get; set; }

        // путь относительно
        public string FolderPath { get; set; }

        public override string ToString()
        {
            return $"{Name}: QualifiedName = '{this.QualifiedName}', FolderPath = {this.FolderPath}";
        }

        public string GetFilePath(IEntityOrmConfig ormConfig)
        {
            if (string.IsNullOrEmpty(ormConfig.RootPath))
            {
                throw new InvalidOperationException(Exceptions.UndefinedRootPath);
            }
            if (string.IsNullOrEmpty(ormConfig.EntitiesPath))
            {
                throw new InvalidOperationException(Exceptions.UndefinedEntitiesPath);
            }
            var filePath = ormConfig.EntitiesPath + "\\" + this.FolderPath.Replace('.', '\\') + ((!string.IsNullOrEmpty(this.FolderPath)) ? "\\" : "") + this.Name + ".xml";

            if (!System.IO.File.Exists(filePath))
            {
                throw new InvalidOperationException(Exceptions.FileNotFound.With(filePath));
            }

            return filePath;
        }

        public static string GetFoldersFromQualifiedName(string ns, string qualifiedName)
        {
            var path = qualifiedName.Substring(ns.Length + 1, qualifiedName.Length - ns.Length - 1);
            var pathParts = path.Split('.');
            if (pathParts.Length == 1)
            {
                return string.Empty;
            }
            return path.Substring(0, path.Length - 1 - pathParts[pathParts.Length - 1].Length);
        }

        // "[root_namespace].[folder_namespace1],.[folder_namespace2]....[entityName]"
        public static EntityPathDescriptor EnsureEntityPath(IEntityOrmConfig ormConfig, string path, IEntityMetadata relatedEntity)
        {
            // проверим, содержит ли путь часть корневого пространства имен
            if (path.StartsWith(ormConfig.Namespace, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(ormConfig.Namespace.Length + 1, path.Length - ormConfig.Namespace.Length - 1);
            }
            var pathParts = path.Split('.');
            var entityName = pathParts[pathParts.Length - 1];
            var folders = string.Empty;

            if (relatedEntity != null)
            {
                folders = EntityPathDescriptor.GetFoldersFromQualifiedName(ormConfig.Namespace, relatedEntity.QualifiedName);
            }
            if (pathParts.Length > 1)
            {
                folders = ((string.IsNullOrEmpty(folders)) ? "." : "") + path.Substring(0, path.Length - 1 - entityName.Length);
            }

            return new EntityPathDescriptor
            {
                QualifiedName = ormConfig.Namespace + ((!string.IsNullOrEmpty(folders)) ? $".{folders}" : "") + "." + entityName,
                Name = entityName,
                FolderPath = folders
            };
        }
    }
}
