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
        TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler);

        TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler);

        DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure);

        int Execute(IQueryStatement statement);

        int Execute<TModel>(IQueryStatement statement);
    }

    public interface IDataReader
    {
        bool Read();

        int GetOrdinal(string name);

        object GetValueAsObject(DataType columnType, Type fieldDbType, int ordinal);

        string GetValueAsString(DataType columnType, Type fieldDbType, int ordinal);

        bool? GetNullableValueAsBoolean(Type fieldDbType, int ordinal);

        bool GetValueAsBoolean(Type fieldDbType, int ordinal);

        int? GetNullableValueAsInt32(Type fieldDbType, int ordinal);

        int GetValueAsInt32(Type fieldDbType, int ordinal);

        float? GetNullableValueAsFloat(Type fieldDbType, int ordinal);

        float GetValueAsFloat(Type fieldDbType, int ordinal);

        double? GetNullableValueAsDouble(Type fieldDbType, int ordinal);

        double GetValueAsDouble(Type fieldDbType, int ordinal);

        decimal? GetNullableValueAsDecimal(Type fieldDbType, int ordinal);

        decimal GetValueAsDecimal(Type fieldDbType, int ordinal);

        string GetNullableValueAsString(Type fieldDbType, int ordinal);

        string GetValueAsString(Type fieldDbType, int ordinal);

        DateTime? GetNullableValueAsDateTime(Type fieldDbType, int ordinal);

        DateTime GetValueAsDateTime(Type fieldDbType, int ordinal);

        byte? GetNullableValueAsByte(Type fieldDbType, int ordinal);

        byte GetValueAsByte(Type fieldDbType, int ordinal);

        Guid? GetNullableValueAsGuid(Type fieldDbType, int ordinal);

        Guid GetValueAsGuid(Type fieldDbType, int ordinal);

        byte[] GetNullableValueAsBytes(Type fieldDbType, int ordinal);

        byte[] GetValueAsBytes(Type fieldDbType, int ordinal);

        Type GetFieldType(int ordinal);
        bool IsDBNull(int ordinal);
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
