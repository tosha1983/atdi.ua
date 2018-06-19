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
        TResult Fetch<TResult>(IQuerySelectStatement statement, Func<System.Data.IDataReader, TResult> handler);

        TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler);

        DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure);
    }

    public interface IDataReader<TModel>
    {
        bool Read();

        int GetValue(Expression<Func<TModel, int>> columnExpression);
        int? GetValue(Expression<Func<TModel, int?>> columnExpression);

        float GetValue(Expression<Func<TModel, float>> columnExpression);
        float? GetValue(Expression<Func<TModel, float?>> columnExpression);

        double GetValue(Expression<Func<TModel, double>> columnExpression);
        double? GetValue(Expression<Func<TModel, double?>> columnExpression);

        decimal GetValue(Expression<Func<TModel, decimal>> columnExpression);
        decimal? GetValue(Expression<Func<TModel, decimal?>> columnExpression);

        bool GetValue(Expression<Func<TModel, bool>> columnExpression);
        bool? GetValue(Expression<Func<TModel, bool?>> columnExpression);

        string GetValue(Expression<Func<TModel, string>> columnExpression);

        DateTime GetValue(Expression<Func<TModel, DateTime>> columnExpression);
        DateTime? GetValue(Expression<Func<TModel, DateTime?>> columnExpression);

        byte GetValue(Expression<Func<TModel, byte>> columnExpression);
        byte? GetValue(Expression<Func<TModel, byte?>> columnExpression);

        byte[] GetValue(Expression<Func<TModel, byte[]>> columnExpression);

        Guid GetValue(Expression<Func<TModel, Guid>> columnExpression);

        Guid? GetValue(Expression<Func<TModel, Guid?>> columnExpression);

    }
}
