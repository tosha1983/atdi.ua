using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQueryExecutor
    {

        DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure);

        //TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler);

        //TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler);

        int Execute(IQueryStatement statement);

        object Execute(IQueryStatement statement, Type resultType);

        TResult Execute<TResult>(IQueryStatement statement);

        TResult ExecuteAndFetch<TResult>(IQueryStatement statement, Func<IDataReader, TResult> handler);

        TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler);

        //TResult ExecuteAndFetch<TResult>(IQueryStatement[] statements, Func<IDataReader, TResult> handler);

        //TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel>[] statements, Func<IDataReader<TModel>, TResult> handler);

        //int InsertSelect<TModelInsert,TModelSelect>(IQueryStatement<TModelInsert> statement, IQuerySelectStatement<TModelSelect> selectStatement);

        //TPKResult Insert<TPKResult>(IQueryInsertStatement statements);

        

    }

    public static class QueryExecutorExtensions
    {
        public static TResult ExecuteAndFetch<TResult>(this IQueryExecutor queryExecutor, IQueryStatement[] statements, Func<IDataReader, TResult> handler)
        {
            if (statements == null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            for (int i = 0; i < statements.Length; i++)
            {
                queryExecutor.Execute(statements[i]);
            }
            return default(TResult);
        }

        public static TResult ExecuteAndFetch<TModel, TResult>(this IQueryExecutor queryExecutor, IQueryStatement<TModel>[] statements, Func<IDataReader<TModel>, TResult> handler)
        {
            if (statements == null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            for (int i = 0; i < statements.Length; i++)
            {
                queryExecutor.Execute(statements[i]);
            }
            return default(TResult);
        }

        public static TResult Fetch<TResult>(this IQueryExecutor queryExecutor, IQuerySelectStatement statement, Func<IDataReader, TResult> handler)
        {
            return queryExecutor.ExecuteAndFetch(statement, handler);
        }

        public static TResult Fetch<TModel, TResult>(this IQueryExecutor queryExecutor, IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        {
            return queryExecutor.ExecuteAndFetch(statement, handler);
        }

        public static void BeginTransaction(this IQueryExecutor queryExecutor)
        {
            throw new NotImplementedException("Нужно использовать другую модель транзакций");
        }

        public static void CommitTransaction(this IQueryExecutor queryExecutor)
        {
            throw new NotImplementedException("Нужно использовать другую модель транзакций");
        }

        public static void RollbackTransaction(this IQueryExecutor queryExecutor)
        {
            throw new NotImplementedException("Нужно использовать другую модель транзакций");
        }
    }
}
