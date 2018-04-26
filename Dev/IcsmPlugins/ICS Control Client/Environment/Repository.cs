using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;

namespace XICSM.ICSControlClient.Environment
{
    public class Repository
    {
        public static T ReadEntityById<T>(int id)
            where T : class, IRepositoryEntity, IRepositoryReadedEntity, new()
        {
            var entity = new T();

            var entityDbName = entity.GetTableName();
            var idFieldName = entity.GetIdFieldName();
            var fields = entity.GetFieldNames();

            var source = new IMRecordset(entityDbName, IMRecordset.Mode.ReadOnly);
            source.Select(fields);
            source.SetWhere(idFieldName, IMRecordset.Operation.Eq, id.ToString());
            using (source.OpenWithScope())
            {
                if (source.IsEOF())
                {
                    throw new InvalidOperationException($"Not found a record of {entityDbName} by Id #{id}");
                }
                entity.LoadFromRecordset(source);
            }

            return entity;
        }

        public static T ReadFirstEntity<T>(Action<IMRecordset> conditionHandler)
            where T : class, IRepositoryEntity, IRepositoryReadedEntity, new()
        {
            var entity = new T();

            var entityDbName = entity.GetTableName();
            var fields = entity.GetFieldNames();

            var source = new IMRecordset(entityDbName, IMRecordset.Mode.ReadOnly);
            source.Select(fields);
            conditionHandler(source);
            using (source.OpenWithScope())
            {
                if (source.IsEOF())
                {
                    return default(T);
                }

                entity.LoadFromRecordset(source);
            }

            return entity;
        }

        public static RepositoryEntityReader<T> ReadEntities<T>(Action<IMRecordset> conditionHandler)
            where T : class, IRepositoryEntity, IRepositoryReadedEntity, new()
        {
            var entity = new T();

            var entityDbName = entity.GetTableName();
            var idFieldName = entity.GetIdFieldName();
            var fields = entity.GetFieldNames();

            var source = new IMRecordset(entityDbName, IMRecordset.Mode.ReadOnly);
            conditionHandler(source);
            source.Select(fields);

            var reader = new RepositoryEntityReader<T>(source);

            return reader;
        }

        public static T[] GetEntities<T>(Action<IMRecordset> conditionHandler)
            where T : class, IRepositoryEntity, IRepositoryReadedEntity, new()
        {
            var data = new List<T>();
            using (var reader = Repository.ReadEntities<T>(conditionHandler))
            {
                while (reader.Read())
                {
                    data.Add(reader.GetEntity());
                }
            }

            return data.ToArray();
        }

        public static void UpdateEntity<T>(T entity)
            where T : class, IRepositoryEntity, IRepositoryUpdatedEntity, new()
        {
            var entityDbName = entity.GetTableName();
            var idFieldName = entity.GetIdFieldName();
            var fields = entity.GetFieldNames();
            var id = entity.GetId();

            var source = new IMRecordset(entityDbName, IMRecordset.Mode.ReadWrite);
            source.Select(fields);
            source.SetWhere(idFieldName, IMRecordset.Operation.Eq, id.ToString());
            using (source.OpenWithScope())
            {
                if (source.IsEOF())
                {
                    throw new InvalidOperationException($"Not found a record of {entityDbName} by Id #{id}");
                }

                source.Edit();
                entity.SaveToRecordset(source);
                source.Update();
            }
        }

        public static T NewEntity<T>()
            where T : class, IRepositoryEntity, IRepositoryCreatedEntity, new()
        {
            var entity = new T();

            entity.SetId(IM.AllocID(entity.GetTableName(), 1, -1));
            return entity;
        }

        public static void CreateEntity<T>(T entity)
            where T : class, IRepositoryEntity, IRepositoryCreatedEntity, new()
        {
            var entityDbName = entity.GetTableName();
            var idFieldName = entity.GetIdFieldName();
            var fields = entity.GetFieldNames();

            var source = new IMRecordset(entityDbName, IMRecordset.Mode.ReadWrite);

            source.Select(fields);
            source.SetWhere(idFieldName, IMRecordset.Operation.Eq, -1);

            using (source.OpenWithScope())
            {
                source.AddNew();
                entity.SaveToRecordset(source);
                source.Update();
            }
        }

        public static void CreateEntity<T>(Action<T> setupFields)
            where T : class, IRepositoryEntity, IRepositoryCreatedEntity, new()
        {
            var entity = NewEntity<T>();
            setupFields(entity);

            var entityDbName = entity.GetTableName();
            var idFieldName = entity.GetIdFieldName();
            var fields = entity.GetFieldNames();

            var source = new IMRecordset(entityDbName, IMRecordset.Mode.ReadWrite);

            source.Select(fields);
            source.SetWhere(idFieldName, IMRecordset.Operation.Eq, -1);

            using (source.OpenWithScope())
            {
                source.AddNew();
                entity.SaveToRecordset(source);
                source.Update();
            }
        }
    }
}
