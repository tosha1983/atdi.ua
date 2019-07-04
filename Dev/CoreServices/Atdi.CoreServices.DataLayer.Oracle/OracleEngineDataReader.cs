using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;


namespace Atdi.CoreServices.DataLayer.Oracle
{
    class OracleEngineDataReader : IEngineDataReader
    {
        private readonly OracleDataReader _reader;
        private readonly Mapper _mapper;

        public OracleEngineDataReader(OracleDataReader reader, Mapper mapper)
        {
            this._reader = reader;
            this._mapper = mapper;
        }

        public int Depth => _reader.Depth;

        public bool IsClosed => _reader.IsClosed;

        public int RecordsAffected => _reader.RecordsAffected;

        public int FieldCount => _reader.FieldCount;

        public Type GetFieldType(int i)
        {
            var providerSpecificFieldType = _reader.GetProviderSpecificFieldType(i);

            if (providerSpecificFieldType == typeof(OracleTimeStampTZ))
            {
                return typeof(DateTimeOffset);
            }
            else if (providerSpecificFieldType == typeof(OracleIntervalDS))
            {
                return typeof(TimeSpan);
            }
            else if ((providerSpecificFieldType == typeof(OracleDate)) || (providerSpecificFieldType == typeof(OracleTimeStampLTZ)) ||
                (providerSpecificFieldType == typeof(OracleTimeStamp)))
            {
                return typeof(DateTime);
            }
            else
            {
                return _reader.GetFieldType(i);
            }
        }

        public string GetName(int i)
        {
            return _reader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public object GetValue(int i, Type type)
        {
            if (type == typeof(int))
            {
                return _reader.GetInt32(i);
            }
            if (type == typeof(long))
            {
                return _reader.GetInt64(i);
            }
            if (type == typeof(string))
            {
                return _reader.GetString(i);
            }
            if (type == typeof(float))
            {
                return _reader.GetFloat(i);
            }
            if (type == typeof(double))
            {
                return _reader.GetDouble(i);
            }
            if (type == typeof(decimal))
            {
                return _reader.GetDecimal(i);
            }
            if (type == typeof(bool))
            {
                return _reader.GetBoolean(i);
            }
            if (type == typeof(byte))
            {
                return _reader.GetByte(i);
            }
            if (type == typeof(byte[]))
            {
                return ReadBytes(_reader, i);
            }
            if (type == typeof(short))
            {
                return _reader.GetInt16(i);
            }
            if (type == typeof(Guid))
            {
                return GetGuid(i);
            }
            if (type == typeof(DateTime))
            {
                return _reader.GetDateTime(i);
            }
            if (type == typeof(DateTimeOffset))
            {
                return GetDateTimeOffset(i);
            }
            if (type == typeof(TimeSpan))
            {
                return  GetTimeSpan(i);
            }
            if (type == typeof(object))
            {
                return _reader.GetValue(i);
            }
            throw new InvalidOperationException(Exceptions.NotSupportedFieldType.With(type.FullName));
        }

        public object GetValue(int i)
        {
            return _reader.GetValue(i);
        }

        public bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i);
        }

        public bool NextResult()
        {
            return _reader.NextResult();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        private static byte[] ReadBytes(OracleDataReader dataReader, int ordinal)
        {
            var result = new Byte[Convert.ToInt32(dataReader.GetBytes(ordinal, 0, null, 0, Int32.MaxValue))];
            dataReader.GetBytes(ordinal, 0, result, 0, result.Length);
            return result;
        }

        public bool GetBool(int i)
        {
            return _reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _reader.GetByte(i);
        }

        public sbyte GetSByte(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetSByte)));
        }

        public byte[] GetBytes(int i)
        {
            return (byte[])_reader.GetValue(i);
        }


        public char[] GetChars(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetChars)));
        }

        public int GetInt32(int i)
        {
            return _reader.GetInt32(i);
        }

        public uint GetUInt32(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetUInt32)));
        }

        public short GetInt16(int i)
        {
            return _reader.GetInt16(i);
        }

        public ushort GetUInt16(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetUInt16)));
        }

        public long GetInt64(int i)
        {
            return _reader.GetInt64(i);
        }

        public ulong GetUInt64(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetUInt64)));
        }

        public string GetString(int i)
        {
            return _reader.GetString(i);
        }

        public DateTime GetDateTime(int i)
        {
            var providerSpecificFieldType = _reader.GetProviderSpecificFieldType(i);

            if (providerSpecificFieldType == typeof(OracleTimeStampLTZ))
            {
                return GetOracleTimeStampLTZ(i);
            }
            else if (providerSpecificFieldType == typeof(OracleTimeStamp))
            {
                return  GetOracleTimeStamp(i);
            }
            else
            {
                return _reader.GetDateTime(i);
            }
        }

        public DateTimeOffset GetDateTimeOffset(int i)
        {
            var dateTimeOffset = new DateTimeOffset();

            var providerSpecificFieldType = _reader.GetProviderSpecificFieldType(i);

            if (providerSpecificFieldType == typeof(OracleTimeStampTZ))
            {
                dateTimeOffset = GetOracleTimeStampTZ(i);
            }
            else
            {
                throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(providerSpecificFieldType.Name));
            }
            return dateTimeOffset;
        }

        private DateTimeOffset GetOracleTimeStampTZ(int i)
        {
            var val = _reader.GetOracleTimeStampTZ(i);
            var oracleTimeStamp = val.ToOracleTimeStamp();
            return new DateTimeOffset(oracleTimeStamp.Value, val.GetTimeZoneOffset());
        }

        private DateTime GetOracleTimeStampLTZ(int i)
        {
            var val = _reader.GetOracleTimeStampLTZ(i);
            var oracleTimeStamp = val.ToOracleTimeStamp();
            return oracleTimeStamp.Value;
        }

        private DateTime GetOracleTimeStamp(int i)
        {
            var oracleTimeStamp = _reader.GetOracleTimeStamp(i);
            return oracleTimeStamp.Value;
        }


        public TimeSpan GetTimeSpan(int i)
        {
            var oracleIntervalDS = _reader.GetOracleIntervalDS(i);
            return oracleIntervalDS.Value;
        }

        public Guid GetGuid(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetGuid)));
        }

        public float GetFloat(int i)
        {
            return _reader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _reader.GetDouble(i);
        }

        public decimal GetDecimal(int i)
        {
            return _reader.GetDecimal(i);
        }


        public TextReader GetTextReader(int i)
        {
            return _reader.GetTextReader(i);
        }
        public XmlReader GetXmlReader(int i)
        {
            var text = GetString(i);
            var stringReader = new StringReader(text);
            var xmlReader = XmlReader.Create(stringReader);
            return xmlReader;
        }

        public Stream GetStream(int i)
        {
            return _reader.GetStream(i);
        }

        public override string ToString()
        {
            return $"Fields = {_reader.FieldCount}, RowsAffacted = {_reader.RecordsAffected}, HasRows = {_reader.HasRows}";
        }

        public long GetPartBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
        {
            return _reader.GetBytes(i, dataIndex, buffer, bufferIndex, length);
        }
        public long GetPartChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
        {
            return _reader.GetChars(i, dataIndex, buffer, bufferIndex, length);
        }

        public char GetChar(int i)
        {
            throw new InvalidOperationException(Exceptions.NotSupportedMethod.With(nameof(GetChar)));
        }

        public int GetOrdinalByAlias(string alias)
        {
            return this._reader.GetOrdinal(alias);
        }

        public int GetOrdinalByPath(string path)
        {
            var alias = this._mapper.GetAlias(path);
            return this._reader.GetOrdinal(alias);
        }

        public string GetAlias(int i)
        {
            return _reader.GetName(i);
        }

        public string GetPath(int i)
        {
            var alias = _reader.GetName(i);
            return _mapper.GetPath(alias);
        }
    }
}
