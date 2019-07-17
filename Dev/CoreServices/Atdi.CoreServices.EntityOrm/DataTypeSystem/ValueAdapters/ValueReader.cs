using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    static class ValueReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadAsBOOL(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBOOL(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadAsBOOL(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(bool))
            {
                return dataReader.GetBool(ordinal);
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
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToBoolean(blob[0]);
                }
                return default(bool);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToBoolean(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToBoolean(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToBoolean(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToBoolean(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToBoolean(dataReader.GetUInt64(ordinal));
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
                return Convert.ToBoolean(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToBoolean(blob[0]);
                }
                return false;
            }


            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBIT(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBIT(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBIT(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            var valueAsByte = ValueReader.ReadAsBYTE(dataReader, ordinal, fieldDbType);
            if (valueAsByte != (byte)0)
            {
                return (byte)1;
            }
            return (byte)0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBYTE(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBYTE(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsBYTE(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToByte(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return dataReader.GetByte(ordinal);
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToByte(dataReader.GetSByte(ordinal));
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
                return Convert.ToByte(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToByte(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToByte(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToByte(blob[0]);
                }
                return default(byte);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToByte(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToByte(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToByte(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToByte(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return blob[0];
                }

                return default(byte);
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return (byte)valAsEnum;
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadAsINT08(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsINT08(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadAsINT08(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToSByte(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToSByte(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToSByte(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToSByte(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToSByte(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToSByte(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToSByte(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToSByte(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToSByte(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToSByte(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToSByte(blob[0]);
                }
                return default(sbyte);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToSByte(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToSByte(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToSByte(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToSByte(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToSByte(blob[0]);
                }

                return default(sbyte);
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return (sbyte)valAsEnum;
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadAsINT16(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsINT16(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadAsINT16(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt16(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt16(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt16(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt16(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt16(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt16(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToInt16(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToInt16(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt16(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt16(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToByte(blob[0]);
                }
                return default(byte);
            }
            if (fieldDbType == typeof(short))
            {
                return dataReader.GetInt16(ordinal);
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToInt16(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt16(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToSByte(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToInt16(blob[0]);
                }

                return default(sbyte);
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return (short)valAsEnum;
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAsINT32(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsINT32(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAsINT32(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt32(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt32(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt32(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt32(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt32(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt32(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return dataReader.GetInt32(ordinal);
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToInt32(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt32(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt32(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToByte(blob[0]);
                }
                return default(byte);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt32(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToInt32(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToInt32(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToInt32(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToInt32(blob[0]);
                }

                return default(int);
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return (int)valAsEnum;
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadAsDECIMAL(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsDECIMAL(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadAsDECIMAL(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDecimal(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToDecimal(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDecimal(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToDecimal(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToDecimal(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToDecimal(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDecimal(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToDecimal(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return dataReader.GetDecimal(ordinal);
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDecimal(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToDecimal(blob[0]);
                }
                return default(decimal);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDecimal(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToDecimal(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDecimal(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToDecimal(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToDecimal(blob[0]);
                }
                return default(decimal);
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadAsFLOAT(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsFLOAT(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadAsFLOAT(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToSingle(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToSingle(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToSingle(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToSingle(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToSingle(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return dataReader.GetFloat(ordinal);
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToSingle(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToSingle(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToSingle(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToSingle(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToSingle(blob[0]);
                }
                return default(float);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToSingle(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToSingle(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToSingle(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToSingle(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToSingle(blob[0]);
                }
                return default(float);
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadAsDOUBLE(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsDOUBLE(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadAsDOUBLE(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDouble(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToDouble(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToDouble(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToDouble(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return dataReader.GetDouble(ordinal);
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToDouble(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDouble(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToDouble(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDouble(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToDouble(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToDouble(blob[0]);
                }
                return default(double);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToDouble(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToDouble(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDouble(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToDouble(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToDouble(blob[0]);
                }
                return default(double);
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadAsINT64(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsINT64(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadAsINT64(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return Convert.ToInt64(dataReader.GetString(ordinal));
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToInt64(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToInt64(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToInt64(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToInt64(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToInt64(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return dataReader.GetInt32(ordinal);
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToInt64(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToInt64(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToInt64(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToByte(blob[0]);
                }
                return default(byte);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToInt64(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToInt64(dataReader.GetUInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return dataReader.GetInt64(ordinal);
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToInt64(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType == typeof(byte[]))
            {
                var blob = dataReader.GetBytes(ordinal);
                if (blob.Length > 0)
                {
                    return Convert.ToInt64(blob[0]);
                }

                return default(long);
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return (long)valAsEnum;
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ReadAsTIME(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsTIME(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ReadAsTIME(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(TimeSpan) || fieldDbType == typeof(TimeSpan?))
            {
                return dataReader.GetTimeSpan(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return TimeSpan.Parse(dataReader.GetString(ordinal));
            }
            //if (fieldDbType == typeof(bool))
            //{
            //    return Convert.ToInt64(dataReader.GetBoolean(ordinal));
            //}
            //if (fieldDbType == typeof(byte))
            //{
            //    return Convert.ToInt64(dataReader.GetByte(ordinal));
            //}
            //if (fieldDbType == typeof(sbyte))
            //{
            //    return Convert.ToInt64(dataReader.GetByte(ordinal));
            //}

            //if (fieldDbType == typeof(double))
            //{
            //    return Convert.ToDateTime(dataReader.GetDouble(ordinal));
            //}
            //if (fieldDbType == typeof(float))
            //{
            //    return Convert.ToDateTime(dataReader.GetFloat(ordinal));
            //}
            //if (fieldDbType == typeof(int))
            //{
            //    return Convert.ToDateTime(dataReader.GetInt32(ordinal));
            //}
            //if (fieldDbType == typeof(uint))
            //{
            //    return Convert.ToDateTime(dataReader.GetUInt32(ordinal));
            //}
            //if (fieldDbType == typeof(decimal))
            //{
            //    return Convert.ToDateTime(dataReader.GetDecimal(ordinal));
            //}
            //if (fieldDbType == typeof(char))
            //{
            //    return Convert.ToInt64(dataReader.GetChar(ordinal));
            //}
            //if (fieldDbType == typeof(char[]))
            //{
            //    var blob = dataReader.GetChars(ordinal);
            //    if (blob.Length > 0)
            //    {
            //        return Convert.ToByte(blob[0]);
            //    }
            //    return default(byte);
            //}
            //if (fieldDbType == typeof(short))
            //{
            //    return Convert.ToInt64(dataReader.GetInt16(ordinal));
            //}
            //if (fieldDbType == typeof(ushort))
            //{
            //    return Convert.ToInt64(dataReader.GetInt16(ordinal));
            //}
            //if (fieldDbType == typeof(long))
            //{
            //    return Convert.ToDateTime(dataReader.GetInt64(ordinal));
            //}
            //if (fieldDbType == typeof(ulong))
            //{
            //    return Convert.ToDateTime(dataReader.GetUInt64(ordinal));
            //}
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadAsDATETIME(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsDATETIME(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadAsDATETIME(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(DateTime))
            {
                return dataReader.GetDateTime(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDateTime(dataReader.GetString(ordinal));
            }
            //if (fieldDbType == typeof(bool))
            //{
            //    return Convert.ToInt64(dataReader.GetBoolean(ordinal));
            //}
            //if (fieldDbType == typeof(byte))
            //{
            //    return Convert.ToInt64(dataReader.GetByte(ordinal));
            //}
            //if (fieldDbType == typeof(sbyte))
            //{
            //    return Convert.ToInt64(dataReader.GetByte(ordinal));
            //}

            //if (fieldDbType == typeof(double))
            //{
            //    return Convert.ToDateTime(dataReader.GetDouble(ordinal));
            //}
            //if (fieldDbType == typeof(float))
            //{
            //    return Convert.ToDateTime(dataReader.GetFloat(ordinal));
            //}
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDateTime(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToDateTime(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDateTime(dataReader.GetDecimal(ordinal));
            }
            //if (fieldDbType == typeof(char))
            //{
            //    return Convert.ToInt64(dataReader.GetChar(ordinal));
            //}
            //if (fieldDbType == typeof(char[]))
            //{
            //    var blob = dataReader.GetChars(ordinal);
            //    if (blob.Length > 0)
            //    {
            //        return Convert.ToByte(blob[0]);
            //    }
            //    return default(byte);
            //}
            //if (fieldDbType == typeof(short))
            //{
            //    return Convert.ToInt64(dataReader.GetInt16(ordinal));
            //}
            //if (fieldDbType == typeof(ushort))
            //{
            //    return Convert.ToInt64(dataReader.GetInt16(ordinal));
            //}
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDateTime(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToDateTime(dataReader.GetUInt64(ordinal));
            }
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset ReadAsDATETIMEOFFSET(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsDATETIMEOFFSET(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset ReadAsDATETIMEOFFSET(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(DateTimeOffset))
            {
                return dataReader.GetDateTimeOffset(ordinal);
            }
            if (fieldDbType == typeof(DateTime))
            {
                return dataReader.GetDateTime(ordinal);
            }
            if (fieldDbType == typeof(string))
            {
                return Convert.ToDateTime(dataReader.GetString(ordinal));
            }
            //if (fieldDbType == typeof(bool))
            //{
            //    return Convert.ToInt64(dataReader.GetBoolean(ordinal));
            //}
            //if (fieldDbType == typeof(byte))
            //{
            //    return Convert.ToInt64(dataReader.GetByte(ordinal));
            //}
            //if (fieldDbType == typeof(sbyte))
            //{
            //    return Convert.ToInt64(dataReader.GetByte(ordinal));
            //}

            //if (fieldDbType == typeof(double))
            //{
            //    return Convert.ToDateTime(dataReader.GetDouble(ordinal));
            //}
            //if (fieldDbType == typeof(float))
            //{
            //    return Convert.ToDateTime(dataReader.GetFloat(ordinal));
            //}
            if (fieldDbType == typeof(int))
            {
                return Convert.ToDateTime(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToDateTime(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToDateTime(dataReader.GetDecimal(ordinal));
            }
            //if (fieldDbType == typeof(char))
            //{
            //    return Convert.ToInt64(dataReader.GetChar(ordinal));
            //}
            //if (fieldDbType == typeof(char[]))
            //{
            //    var blob = dataReader.GetChars(ordinal);
            //    if (blob.Length > 0)
            //    {
            //        return Convert.ToByte(blob[0]);
            //    }
            //    return default(byte);
            //}
            //if (fieldDbType == typeof(short))
            //{
            //    return Convert.ToInt64(dataReader.GetInt16(ordinal));
            //}
            //if (fieldDbType == typeof(ushort))
            //{
            //    return Convert.ToInt64(dataReader.GetInt16(ordinal));
            //}
            if (fieldDbType == typeof(long))
            {
                return Convert.ToDateTime(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToDateTime(dataReader.GetUInt64(ordinal));
            }
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ReadAsGUID(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsGUID(dataReader, ordinal, fieldDbType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ReadAsGUID(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(Guid))
            {
                return dataReader.GetGuid(ordinal);
            }
            if (fieldDbType == typeof(byte[]))
            {
                var store = dataReader.GetBytes(ordinal);
                if (store.Length != 16)
                {
                    throw new InvalidCastException($"Incorrect value for GUID");
                }
                return new Guid(store);
            }
            if (fieldDbType == typeof(string))
            {
                var store = dataReader.GetString(ordinal);
                
                return new Guid(store);
            }
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsNVARCHAR(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsNVARCHAR(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsNVARCHAR(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return dataReader.GetString(ordinal);
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToString(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToString(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToString(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToString(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToString(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToString(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToString(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToString(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return string.Join("", blob);
                }
                return default(string);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToString(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return valAsEnum.ToString();
            }
            if (fieldDbType.IsValueType)
            {
                var valAsValueType = dataReader.GetValue(ordinal);
                return valAsValueType.ToString();
            }
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsVARCHAR(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsVARCHAR(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsVARCHAR(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return dataReader.GetString(ordinal);
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToString(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToString(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToString(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToString(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToString(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToString(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToString(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToString(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return string.Join("", blob);
                }
                return default(string);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToString(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return valAsEnum.ToString();
            }
            if (fieldDbType.IsValueType)
            {
                var valAsValueType = dataReader.GetValue(ordinal);
                return valAsValueType.ToString();
            }
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsNTEXT(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsNTEXT(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsNTEXT(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return dataReader.GetString(ordinal);
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToString(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToString(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToString(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToString(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToString(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToString(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToString(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToString(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return string.Join("", blob);
                }
                return default(string);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToString(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return valAsEnum.ToString();
            }
            if (fieldDbType.IsValueType)
            {
                var valAsValueType = dataReader.GetValue(ordinal);
                return valAsValueType.ToString();
            }
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsTEXT(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsTEXT(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsTEXT(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(string))
            {
                return dataReader.GetString(ordinal);
            }
            if (fieldDbType == typeof(bool))
            {
                return Convert.ToString(dataReader.GetBool(ordinal));
            }
            if (fieldDbType == typeof(byte))
            {
                return Convert.ToString(dataReader.GetByte(ordinal));
            }
            if (fieldDbType == typeof(sbyte))
            {
                return Convert.ToString(dataReader.GetSByte(ordinal));
            }
            if (fieldDbType == typeof(double))
            {
                return Convert.ToString(dataReader.GetDouble(ordinal));
            }
            if (fieldDbType == typeof(float))
            {
                return Convert.ToString(dataReader.GetFloat(ordinal));
            }
            if (fieldDbType == typeof(int))
            {
                return Convert.ToString(dataReader.GetInt32(ordinal));
            }
            if (fieldDbType == typeof(uint))
            {
                return Convert.ToString(dataReader.GetUInt32(ordinal));
            }
            if (fieldDbType == typeof(decimal))
            {
                return Convert.ToString(dataReader.GetDecimal(ordinal));
            }
            if (fieldDbType == typeof(char))
            {
                return Convert.ToString(dataReader.GetChar(ordinal));
            }
            if (fieldDbType == typeof(char[]))
            {
                var blob = dataReader.GetChars(ordinal);
                if (blob.Length > 0)
                {
                    return string.Join("", blob);
                }
                return default(string);
            }
            if (fieldDbType == typeof(short))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(ushort))
            {
                return Convert.ToString(dataReader.GetInt16(ordinal));
            }
            if (fieldDbType == typeof(long))
            {
                return Convert.ToString(dataReader.GetInt64(ordinal));
            }
            if (fieldDbType == typeof(ulong))
            {
                return Convert.ToString(dataReader.GetUInt64(ordinal));
            }
            if (fieldDbType.IsEnum)
            {
                var valAsEnum = dataReader.GetValue(ordinal);
                return valAsEnum.ToString();
            }
            if (fieldDbType.IsValueType)
            {
                var valAsValueType = dataReader.GetValue(ordinal);
                return valAsValueType.ToString();
            }
            //if (fieldDbType == typeof(byte[]))
            //{
            //    return Encoding.UTF8.GetString(ReadAsBLOB(dataReader, ordinal, fieldDbType));
            //}
            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadAsBYTES(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBLOB(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadAsBLOB(IEngineDataReader dataReader, int ordinal)
        {
            var fieldDbType = dataReader.GetFieldType(ordinal);
            return ReadAsBLOB(dataReader, ordinal, fieldDbType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadAsBLOB(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        {
            if (fieldDbType == typeof(byte[]))
            {
                return dataReader.GetBytes(ordinal);
            }

            throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadAsCLRTYPE(IEngineDataReader dataReader, int ordinal)
        {
            return dataReader.GetValue(ordinal);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static object ReadAsCLRTYPE(IEngineDataReader dataReader, int ordinal, Type fieldDbType)
        //{
        //    if (fieldDbType == typeof(byte[]))
        //    {
        //        return dataReader.GetBytes(ordinal);
        //    }

        //    throw new InvalidOperationException(Exceptions.ColumnValueTypeNotSupported.With(fieldDbType, dataReader.GetPath(ordinal)));
        //}
    }
}
