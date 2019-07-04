using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
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




        char? GetNullableValueAsChar(Type fieldDbType, int ordinal);

        char GetValueAsChar(Type fieldDbType, int ordinal);

        short? GetNullableValueAsShort(Type fieldDbType, int ordinal);

        short GetValueAsShort(Type fieldDbType, int ordinal);


        UInt16? GetNullableValueAsUnsignedShort(Type fieldDbType, int ordinal);
        UInt16 GetValueAsUnsignedShort(Type fieldDbType, int ordinal);


        UInt32? GetNullableValueAsUnsignedInteger(Type fieldDbType, int ordinal);
        UInt32 GetValueAsUnsignedInteger(Type fieldDbType, int ordinal);


        long? GetNullableValueAsLong(Type fieldDbType, int ordinal);
        long GetValueAsLong(Type fieldDbType, int ordinal);

        UInt64? GetNullableValueAsUnsignedLong(Type fieldDbType, int ordinal);
        UInt64 GetValueAsUnsignedLong(Type fieldDbType, int ordinal);

        sbyte? GetNullableValueAsSignedByte(Type fieldDbType, int ordinal);
        sbyte GetValueAsSignedByte(Type fieldDbType, int ordinal);

        TimeSpan? GetNullableValueAsTime(Type fieldDbType, int ordinal);
        TimeSpan GetValueAsTime(Type fieldDbType, int ordinal);

        DateTime? GetNullableValueAsDate(Type fieldDbType, int ordinal);
        DateTime GetValueAsDate(Type fieldDbType, int ordinal);

        DateTimeOffset? GetNullableValueAsDateTimeOffset(Type fieldDbType, int ordinal);
        DateTimeOffset GetValueAsDateTimeOffset(Type fieldDbType, int ordinal);

        string GetNullableValueAsXml(Type fieldDbType, int ordinal);
        string GetValueAsXml(Type fieldDbType, int ordinal);

        string GetNullableValueAsJson(Type fieldDbType, int ordinal);
        string GetValueAsJson(Type fieldDbType, int ordinal);

        Object GetNullableValueAsClrType(Type fieldDbType, int ordinal);
        Object GetValueAsClrType(Type fieldDbType, int ordinal);

        Enum GetNullableValueAsClrEnum(Type fieldDbType, int ordinal);
        Enum GetValueAsClrEnum(Type fieldDbType, int ordinal);

        Type GetFieldType(int ordinal);
        bool IsDBNull(int ordinal);
    }

    public interface IDataReader<TModel>
    {
        bool Read();

        int GetValue(Expression<Func<TModel, int>> columnExpression);
        int? GetValue(Expression<Func<TModel, int?>> columnExpression);
        int[] GetValue(Expression<Func<TModel, int[]>> columnExpression);

        float GetValue(Expression<Func<TModel, float>> columnExpression);
        float? GetValue(Expression<Func<TModel, float?>> columnExpression);
        float[] GetValue(Expression<Func<TModel, float[]>> columnExpression);

        double GetValue(Expression<Func<TModel, double>> columnExpression);
        double? GetValue(Expression<Func<TModel, double?>> columnExpression);
        double[] GetValue(Expression<Func<TModel, double[]>> columnExpression);

        decimal GetValue(Expression<Func<TModel, decimal>> columnExpression);
        decimal? GetValue(Expression<Func<TModel, decimal?>> columnExpression);
        decimal[] GetValue(Expression<Func<TModel, decimal[]>> columnExpression);

        bool GetValue(Expression<Func<TModel, bool>> columnExpression);
        bool? GetValue(Expression<Func<TModel, bool?>> columnExpression);
        bool[] GetValue(Expression<Func<TModel, bool[]>> columnExpression);

        string GetValue(Expression<Func<TModel, string>> columnExpression);
        string[] GetValue(Expression<Func<TModel, string[]>> columnExpression);

        DateTime GetValue(Expression<Func<TModel, DateTime>> columnExpression);
        DateTime? GetValue(Expression<Func<TModel, DateTime?>> columnExpression);
        DateTime[] GetValue(Expression<Func<TModel, DateTime[]>> columnExpression);

        byte GetValue(Expression<Func<TModel, byte>> columnExpression);
        byte? GetValue(Expression<Func<TModel, byte?>> columnExpression);
        byte[] GetValue(Expression<Func<TModel, byte[]>> columnExpression);

        Guid GetValue(Expression<Func<TModel, Guid>> columnExpression);
        Guid? GetValue(Expression<Func<TModel, Guid?>> columnExpression);
        Guid[] GetValue(Expression<Func<TModel, Guid[]>> columnExpression);

        char GetValue(Expression<Func<TModel, char>> columnExpression);
        char? GetValue(Expression<Func<TModel, char?>> columnExpression);
        char[] GetValue(Expression<Func<TModel, char[]>> columnExpression);

        short GetValue(Expression<Func<TModel, short>> columnExpression);
        short? GetValue(Expression<Func<TModel, short?>> columnExpression);
        short[] GetValue(Expression<Func<TModel, short[]>> columnExpression);

        UInt16 GetValue(Expression<Func<TModel, UInt16>> columnExpression);
        UInt16? GetValue(Expression<Func<TModel, UInt16?>> columnExpression);
        UInt16[] GetValue(Expression<Func<TModel, UInt16[]>> columnExpression);

        UInt32 GetValue(Expression<Func<TModel, UInt32>> columnExpression);
        UInt32? GetValue(Expression<Func<TModel, UInt32?>> columnExpression);
        UInt32[] GetValue(Expression<Func<TModel, UInt32[]>> columnExpression);

        Int64 GetValue(Expression<Func<TModel, Int64>> columnExpression);
        Int64? GetValue(Expression<Func<TModel, Int64?>> columnExpression);
        Int64[] GetValue(Expression<Func<TModel, Int64[]>> columnExpression);

        UInt64 GetValue(Expression<Func<TModel, UInt64>> columnExpression);
        UInt64? GetValue(Expression<Func<TModel, UInt64?>> columnExpression);
        UInt64[] GetValue(Expression<Func<TModel, UInt64[]>> columnExpression);

        sbyte GetValue(Expression<Func<TModel, sbyte>> columnExpression);
        sbyte? GetValue(Expression<Func<TModel, sbyte?>> columnExpression);
        sbyte[] GetValue(Expression<Func<TModel, sbyte[]>> columnExpression);

        TimeSpan GetValue(Expression<Func<TModel, TimeSpan>> columnExpression);
        TimeSpan? GetValue(Expression<Func<TModel, TimeSpan?>> columnExpression);
        TimeSpan[] GetValue(Expression<Func<TModel, TimeSpan[]>> columnExpression);

        DateTimeOffset GetValue(Expression<Func<TModel, DateTimeOffset>> columnExpression);
        DateTimeOffset? GetValue(Expression<Func<TModel, DateTimeOffset?>> columnExpression);
        DateTimeOffset[] GetValue(Expression<Func<TModel, DateTimeOffset[]>> columnExpression);

        Enum GetValue(Expression<Func<TModel, Enum>> columnExpression);
    }
}
