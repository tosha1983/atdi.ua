using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    static class ValueReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadAsBOOL(IDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBOOL(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadAsBOOL(IDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(bool))
            {
                return dataReader.GetBoolean(ordinal);
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToBoolean(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToBoolean(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToBoolean(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToBoolean(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToBoolean(dataReader.GetInt64(ordinal));
            }

            if (fieldDbType == typeof(UInt16))
            {
                return Convert.ToBoolean(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(UInt32))
            {
                return Convert.ToBoolean(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(UInt64))
            {
                return Convert.ToBoolean(dataReader.GetInt64(ordinal));
            }

            if (fieldDbType == typeof(double))
            {
                return Convert.ToBoolean(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToBoolean(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToBoolean(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToBoolean(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToBoolean(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = ValueReader.ReadAsBLOB(dataReader, ordinal, fieldDbType);
                if (blob.Length > 0)
                {
                    return Convert.ToBoolean(blob[0]);
                }
                return false;
            }


            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBIT(IDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBIT(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBIT(IDataReader dataReader, int ordinal, Type fieldDbType)
        {
            var valueAsByte = ValueReader.ReadAsBYTE(dataReader, ordinal, fieldDbType);
            if (valueAsByte != (byte)0)
            {
                return (byte)1;
            }
            return (byte)0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBYTE(IDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToByte(dataReader.GetBoolean(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(sbyte))
            {
                return dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToByte(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToByte(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToByte(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToByte(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToByte(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToByte(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToByte(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToByte(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToByte(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToByte(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToByte(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = ValueReader.ReadAsBLOB(dataReader, ordinal, fieldDbType);
                if (blob.Length > 0)
                {
                    return blob[0];
                }

                return default(byte);
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadAsBLOB(IDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(byte[]))
            {
                var size = dataReader.GetBytes(ordinal, 0, null, 0, 0);
                var result = new byte[size];
                const int lehght = 1;
                long readBytes = 0;
                int offset = 0;
                while (readBytes < size)
                {
                    if (lehght <= size - readBytes)
                        readBytes += dataReader.GetBytes(ordinal, offset, result, offset, lehght);
                    else readBytes += dataReader.GetBytes(ordinal, offset, result, offset, (int)(size - readBytes));

                    offset += lehght;
                }
                return result;
            }


            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetName(ordinal)));
        }
    }
}
