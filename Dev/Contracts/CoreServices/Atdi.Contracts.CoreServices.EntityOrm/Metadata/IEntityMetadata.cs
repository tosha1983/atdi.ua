using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IEntityMetadata
    {
        /// <summary>
        /// Имя сущности
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Имя сущности
        /// </summary>
        string QualifiedName { get; }

        /// <summary>
        /// Отображаемое название сущности
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Описание сущности и ее назначение
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// Тип сущности
        /// Вид наследования
        /// Планируются следующие видынаследования
        /// 1. Прототипное наследование (по умолчанию) - Базовая таблица - Таблица наследуемой сущности - Общий первичный ключ - отношение один к одному
        /// 2. Ролевое наследование - Базовая таблица - таблицы ролевых сущностей - Синхронизация значений для пересикающиеся по именам поля среди всех таблиц Общий первичный ключ - отношение один к одному
        /// 3. Простое наследование - Базовой таблицы нет или она не используется - Таблица наследуемой сущности получает все поля базовой сущности, включая первичный ключ - нет физических отношений так как одна таблица.
        /// </summary>
        EntityType Type { get; }

        /// <summary>
        /// Наследуемая или расширяемая сущность
        /// </summary>
        IEntityMetadata BaseEntity { get; }

        /// <summary>
        /// Описание природы источника данных
        /// </summary>
        IDataSourceMetadata DataSource { get; }

        /// <summary>
        ///  Описание первичного ключа сущности
        /// </summary>
        IPrimaryKeyMetadata PrimaryKey { get; }

        /// <summary>
        /// Набор полей сущности
        /// </summary>
        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

    }

    public static class EntityMetadataExtension
    {
        public static bool UsesInheritance(this IEntityMetadata entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extention
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Simple;
        }

        public static bool UsesBaseEntityPrimaryKey(this IEntityMetadata entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extention
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role;
            // при простом наследовании просто копируется вся структура и объект выглядит как Normal
             //   || entityMetadata.Type == EntityType.Simple;
        }
        public static bool UsesBaseEntity(this IEntityMetadata entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extention
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Simple;
        }

        // метод рекурсивного поиска локальногополя во всей цепочки наследования или расширения
        public static bool TryGetLocalField(this IEntityMetadata entityMetadata, string name, out IFieldMetadata fieldMetadata)
        {
            if (entityMetadata.Fields.TryGetValue(name, out fieldMetadata))
            {
                return true;
            }
            if (entityMetadata.UsesBaseEntity() && entityMetadata.BaseEntity != null)
            {
                return entityMetadata.BaseEntity.TryGetLocalField(name, out fieldMetadata);
            }
            return false;
        }
        // метод рекурсивного поиска локального поля во всей цепочки наследования или расширения
        //  path - это не имя локального поля а путь, в котором имя локального поля в начале
        public static bool TryGetLocalFieldByPath(this IEntityMetadata entityMetadata, string path, out IFieldMetadata fieldMetadata)
        {
            var pathParts = path.Split('.');
            var name = pathParts[0];
            if (entityMetadata.Fields.TryGetValue(name, out fieldMetadata))
            {
                return true;
            }
            if (entityMetadata.UsesBaseEntity() && entityMetadata.BaseEntity != null)
            {
                return entityMetadata.BaseEntity.TryGetLocalField(name, out fieldMetadata);
            }
            return false;
        }

        // метод рекурсивного поиска конечного поля в указаном пути с учетом во всей цепочки наследования или расширения
        //  path - это полный путь к полб относительно текущей сущности,
        public static bool TryGetEndFieldByPath(this IEntityMetadata entityMetadata, string path, out IFieldMetadata fieldMetadata)
        {
            var pathParts = path.Split('.');
            var name = pathParts[0];
            if (!entityMetadata.TryGetLocalField(name, out IFieldMetadata localField))
            {
                fieldMetadata = null;
                return false;
            }
            
            if (pathParts.Length == 1)
            {
                fieldMetadata = localField;
                return true;
            }

            var refEntity = localField.GetRefEntity();
            if (refEntity == null)
            {
                throw new InvalidOperationException($"Invalid field path '{path}'");
            }

            var nextPath = path.Substring(name.Length + 1, path.Length - name.Length - 1);
            return refEntity.TryGetEndFieldByPath(nextPath, out fieldMetadata);
        }

        // метод рекурсивного поиска поля первичного ключа во всей цепочки наследования или расширения
        public static bool TryGetPrimaryKeyField(this IEntityMetadata entityMetadata, string path, out IFieldMetadata fieldMetadata)
        {
            if (entityMetadata.PrimaryKey != null && entityMetadata.PrimaryKey.FieldRefs.TryGetValue(path, out IPrimaryKeyFieldRefMetadata fieldRef))
            {
                fieldMetadata = fieldRef.Field;
                return true;
            }
            if (entityMetadata.UsesBaseEntityPrimaryKey() && entityMetadata.BaseEntity != null)
            {
                return entityMetadata.BaseEntity.TryGetPrimaryKeyField(path, out fieldMetadata);
            }
            fieldMetadata = null;
            return false;
        }

        public static IPrimaryKeyMetadata DefinePrimaryKey(this IEntityMetadata entityMetadata)
        {
            if (entityMetadata.PrimaryKey != null)
            {
                return entityMetadata.PrimaryKey;
            }

            if (entityMetadata.UsesBaseEntityPrimaryKey())
            {
                return entityMetadata.DefinePrimaryKey();
            }

            return null;
        }

        /// <summary>
        /// Метод пытается вернуть первичный ключ если он определен на уровне даннйо сущности
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <param name="path"></param>
        /// <param name="fieldMetadata"></param>
        /// <returns></returns>
        public static bool TryGetPrimaryKey(this IEntityMetadata entityMetadata, out IPrimaryKeyMetadata primaryKey)
        {
            if (entityMetadata.PrimaryKey != null)
            {
                primaryKey = entityMetadata.PrimaryKey;
                return true;
            }
            primaryKey = null;
            return false;
        }
        public static bool TryGetPrimaryKeyRefFields(this IEntityMetadata entityMetadata, out IPrimaryKeyFieldRefMetadata[] primaryKeyFields)
        {
            if (entityMetadata.PrimaryKey != null)
            {
                primaryKeyFields = entityMetadata.PrimaryKey.FieldRefs.Values.ToArray();
                return true;
            }
            primaryKeyFields = null;
            return false;
        }
    }
}
