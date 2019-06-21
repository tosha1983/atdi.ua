using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    

    public interface IEngineDataReader
    {
        bool IsDBNull(int i);

        int Depth { get; }

        bool IsClosed { get; }

        int RecordsAffected { get; }

        bool NextResult();

        bool Read();

        int FieldCount { get; }

        string GetName(int i);

        int GetOrdinal(string name);

        Type GetFieldType(int i);

        object GetValue(int i, Type type);

        object GetValue(int i);

        bool GetBool(int i);

        byte GetByte(int i);

        sbyte GetSByte(int i);

        byte[] GetBytes(int i);

        char GetChar(int i);

        char[] GetChars(int i);

        int GetInt32(int i);

        uint GetUInt32(int i);

        short GetInt16(int i);

        ushort GetUInt16(int i);

        long GetInt64(int i);

        ulong GetUInt64(int i);

        string GetString(int i);

        DateTime GetDateTime(int i);

        DateTimeOffset GetDateTimeOffset(int i);

        TimeSpan GetTimeSpan(int i);

        float GetFloat(int i);

        double GetDouble(int i);

        decimal GetDecimal(int i);

        Guid GetGuid(int i);

        TextReader GetTextReader(int i);

        XmlReader GetXmlReader(int i);

        Stream GetStream(int i);

        long GetPartBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length);

        long GetPartChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);
    }
}
