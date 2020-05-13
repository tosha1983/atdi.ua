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
        /// Простарнство имен в котором расположена сущность
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Квалифицированное имя сущности
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
            return entityMetadata.Type == EntityType.Extension
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Simple;
        }

        public static bool UsesBaseEntityPrimaryKey(this IEntityMetadata entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extension
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Abstract
                || entityMetadata.Type == EntityType.Role;
            // при простом наследовании просто копируется вся структура и объект выглядит как Normal
            //   || entityMetadata.Type == EntityType.Simple;
        }
        public static bool UsesBaseEntity(this IEntityMetadata entityMetadata)
        {
            return entityMetadata.Type == EntityType.Extension
                || entityMetadata.Type == EntityType.Prototype
                || entityMetadata.Type == EntityType.Role
                || entityMetadata.Type == EntityType.Abstract
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
                return entityMetadata.BaseEntity?.DefinePrimaryKey();
            }

            return null;
        }

        /// <summary>
        /// Метод определения всех полей сущности включая с наследуемыми
        /// Важно: в цепочку исследования входят базовые абстрактные сущности
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <returns></returns>
        public static IFieldMetadata[] DefineFieldsWithInherited(this IEntityMetadata entityMetadata)
        {
            var fields = new List<IFieldMetadata>();

            // копируем локальные если есть
            var local = entityMetadata.Fields.Values.ToArray();
            if (local.Length > 0)
            {
                fields.AddRange(local);
            }


            // если это простая сущность или роль, не продолжаем - они и так все копируют к себе
            if (entityMetadata.Type == EntityType.Simple
                || entityMetadata.Type == EntityType.Role)
            {
                return fields.ToArray();
            }

            if (entityMetadata.UsesInheritance())
            {
                var inheritedField = entityMetadata.BaseEntity.DefineFieldsWithInherited();
                if (inheritedField.Length > 0)
                {
                    fields.AddRange(inheritedField);
                }
            }

            return fields.ToArray();
        }

        /// <summary>
        /// Метод определения цепочки наследования сущностей
        /// Важно: в цепочку не входят базовые абстрактные сущности
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <returns></returns>
        public static IEntityMetadata[] DefineInheritChain(this IEntityMetadata entityMetadata)
        {
            var chain = new List<IEntityMetadata>();

            if (!entityMetadata.UsesInheritance())
            {
                return new IEntityMetadata[] { };
            }
            var nextBase = entityMetadata.BaseEntity;
            while (nextBase != null && nextBase.Type != EntityType.Abstract)
            {
                if (nextBase.QualifiedName == entityMetadata.QualifiedName)
                {
                    throw new InvalidOperationException($"Detected looping inheritance chain by entity '{nextBase.QualifiedName}'");
                }

                chain.Add(nextBase);
                if (nextBase.UsesInheritance())
                {
                    nextBase = nextBase.BaseEntity;
                }
                else
                {
                    nextBase = null;
                }
            }
            chain.Reverse();
            return chain.ToArray();
        }

        public static IEntityMetadata[] DefineInheritChainWithMe(this IEntityMetadata entityMetadata)
        {
            var chain = entityMetadata.DefineInheritChain();
            var result = new IEntityMetadata[chain.Length + 1];
            for (int i = 0; i < chain.Length; i++)
            {
                result[i] = chain[i];
            }
            result[chain.Length] = entityMetadata;
            return result;
        }
        /// <summary>
        /// Метод проверки всей цепочки наследования сущностей от замыкания
        /// Важно: в цепочке проверяются все сущности
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <returns></returns>
        public static IEntityMetadata[] CheckFullInheritChain(this IEntityMetadata entityMetadata)
        {
            var chain = new Dictionary<string, IEntityMetadata>();

            if (!entityMetadata.UsesInheritance())
            {
                return new IEntityMetadata[] { };
            }
            var nextBase = entityMetadata.BaseEntity;
            while (nextBase != null )
            {
                if (nextBase.QualifiedName == entityMetadata.QualifiedName)
                {
                    throw new InvalidOperationException($"Detected looping inheritance chain by entity '{nextBase.QualifiedName}'");
                }
                if (chain.ContainsKey(nextBase.QualifiedName))
                {
                    throw new InvalidOperationException($"Detected looping inheritance chain by entity '{nextBase.QualifiedName}'");
                }

                chain.Add(nextBase.QualifiedName, nextBase);
                if (nextBase.UsesInheritance())
                {
                    nextBase = nextBase.BaseEntity;
                }
                else
                {
                    nextBase = null;
                }
            }
            return chain.Values.Reverse().ToArray();
        }

        /// <summary>
        /// Метод определения полной (включая абстрактные сущности) цепочки наследования сущностей
        /// Важно: в цепочку входитя базовые абстрактные сущности
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <returns></returns>
        public static IEntityMetadata[] DefineFullInheritChain(this IEntityMetadata entityMetadata)
        {
            var chain = new List<IEntityMetadata>();

            if (!entityMetadata.UsesInheritance())
            {
                return new IEntityMetadata[] { };
            }
            var nextBase = entityMetadata.BaseEntity;
            while (nextBase != null)
            {
                if (nextBase.QualifiedName == entityMetadata.QualifiedName)
                {
                    throw new InvalidOperationException($"Detected looping inheritance chain by entity '{nextBase.QualifiedName}'");
                }

                chain.Add(nextBase);
                if (nextBase.UsesInheritance())
                {
                    nextBase = nextBase.BaseEntity;
                }
                else
                {
                    nextBase = null;
                }
            }
            chain.Reverse();
            return chain.ToArray();
        }
        /// <summary>
        /// Метод пытается вернуть первичный ключ если он определен на уровне данной сущности
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
