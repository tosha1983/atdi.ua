using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryDataReader<TModel> : IDataReader<TModel>
    {
        private readonly IDataReader _dataReader;
        
        public QueryDataReader(IDataReader dataReader)
        {
            this._dataReader = dataReader;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetValueAsBoolean(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(bool))
            {
                return _dataReader.GetBoolean(columnIndex);
            }
            if (columnType == typeof(int))
            {
                return Convert.ToBoolean(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(string))
            {
                return Convert.ToBoolean(_dataReader.GetString(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetValueAsInt32(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(int))
            {
                return _dataReader.GetInt32(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToInt32(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToInt32(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToInt32(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToInt32(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToInt32(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToInt32(_dataReader.GetDateTime(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetValueAsFloat(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(float))
            {
                return _dataReader.GetFloat(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return (float)Convert.ToDouble(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return (float)Convert.ToDouble(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return (float)_dataReader.GetDouble(columnIndex);
            }
            if (columnType == typeof(int))
            {
                return (float)Convert.ToDouble(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return (float)Convert.ToDouble(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return (float)Convert.ToDouble(_dataReader.GetDateTime(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetValueAsDouble(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(double))
            {
                return _dataReader.GetDouble(columnIndex);
            }
            if (columnType == typeof(float))
            {
                return _dataReader.GetFloat(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToDouble(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToDouble(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToDouble(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToDouble(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToDouble(_dataReader.GetDateTime(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private decimal GetValueAsDecimal(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(decimal))
            {
                return _dataReader.GetDecimal(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToDecimal(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToDecimal(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToDecimal(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToDecimal(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToDecimal(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToDecimal(_dataReader.GetDateTime(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetValueAsString(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(string))
            {
                return _dataReader.GetString(columnIndex);
            }
            if (columnType == typeof(double))
            {
                return Convert.ToString(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToString(_dataReader.GetDecimal(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToString(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToString(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToString(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToString(_dataReader.GetDateTime(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime GetValueAsDateTime(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(DateTime))
            {
                return _dataReader.GetDateTime(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToDateTime(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToDateTime(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToDateTime(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToDateTime(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(byte))
            {
                return Convert.ToDateTime(_dataReader.GetByte(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToDateTime(_dataReader.GetDecimal(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetValueAsByte(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(byte))
            {
                return _dataReader.GetByte(columnIndex);
            }
            if (columnType == typeof(string))
            {
                return Convert.ToByte(_dataReader.GetString(columnIndex));
            }
            if (columnType == typeof(double))
            {
                return Convert.ToByte(_dataReader.GetDouble(columnIndex));
            }
            if (columnType == typeof(float))
            {
                return Convert.ToByte(_dataReader.GetFloat(columnIndex));
            }
            if (columnType == typeof(int))
            {
                return Convert.ToByte(_dataReader.GetInt32(columnIndex));
            }
            if (columnType == typeof(DateTime))
            {
                return Convert.ToByte(_dataReader.GetDateTime(columnIndex));
            }
            if (columnType == typeof(decimal))
            {
                return Convert.ToByte(_dataReader.GetDecimal(columnIndex));
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] GetValueAsBytes(int columnIndex)
        {
            var columnType = _dataReader.GetFieldType(columnIndex);
            if (columnType == typeof(byte[]))
            {
                var size = _dataReader.GetBytes(columnIndex, 0, null, 0, 0); 
                var result = new byte[size];
                const int lehght = 1024;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    readBytes += _dataReader.GetBytes(columnIndex, offset, result, offset, lehght);
                    offset += lehght;
                }

                return result;
            }
            if (columnType == typeof(byte))
            {
                return new byte[] { _dataReader.GetByte(columnIndex) };
            }
            if (columnType == typeof(string))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetString(columnIndex)) };
            }
            if (columnType == typeof(double))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDouble(columnIndex)) };
            }
            if (columnType == typeof(float))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetFloat(columnIndex)) };
            }
            if (columnType == typeof(int))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetInt32(columnIndex)) };
            }
            if (columnType == typeof(DateTime))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDateTime(columnIndex)) };
            }
            if (columnType == typeof(decimal))
            {
                return new byte[] { Convert.ToByte(_dataReader.GetDecimal(columnIndex)) };
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(columnType, _dataReader.GetName(columnIndex)));
        }

        public bool GetValue(Expression<Func<TModel, bool>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsBoolean(columnIndex);
        }

        public bool? GetValue(Expression<Func<TModel, bool?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsBoolean(columnIndex);
        }

        public int GetValue(Expression<Func<TModel, int>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsInt32(columnIndex);
        }

        public int? GetValue(Expression<Func<TModel, int?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsInt32(columnIndex);
        }

        public float GetValue(Expression<Func<TModel, float>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsFloat(columnIndex);
        }

        public float? GetValue(Expression<Func<TModel, float?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsFloat(columnIndex);
        }

        public double GetValue(Expression<Func<TModel, double>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsDouble(columnIndex);
        }

        public double? GetValue(Expression<Func<TModel, double?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsDouble(columnIndex);
        }

        public decimal GetValue(Expression<Func<TModel, decimal>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsDecimal(columnIndex);
        }

        public decimal? GetValue(Expression<Func<TModel, decimal?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsDecimal(columnIndex);
        }

        public DateTime GetValue(Expression<Func<TModel, DateTime>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsDateTime(columnIndex);
        }

        public DateTime? GetValue(Expression<Func<TModel, DateTime?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsDateTime(columnIndex);
        }

        public string GetValue(Expression<Func<TModel, string>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);

            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsString(columnIndex);
        }

        public byte GetValue(Expression<Func<TModel, byte>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            return GetValueAsByte(columnIndex);
        }

        public byte? GetValue(Expression<Func<TModel, byte?>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }

            return GetValueAsByte(columnIndex);
        }

        public bool Read()
        {
            return this._dataReader.Read();
        }

        public byte[] GetValue(Expression<Func<TModel, byte[]>> columnExpression)
        {
            if (columnExpression == null)
            {
                throw new ArgumentNullException(nameof(columnExpression));
            }

            var columnName = columnExpression.Body.GetMemberName();
            var columnIndex = _dataReader.GetOrdinal(columnName);
            if (_dataReader.IsDBNull(columnIndex))
            {
                return null;
            }
            return GetValueAsBytes(columnIndex);
        }
    }
}
