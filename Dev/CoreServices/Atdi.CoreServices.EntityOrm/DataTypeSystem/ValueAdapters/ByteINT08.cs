using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    class ByteINT08 : ValueAdapter<byte?, sbyte?>
    {
        public ByteINT08(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override byte? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (byte?)null;
            }
            return Convert.ToByte(storeValue);
        }

        public override byte? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return (byte?)ValueReader.ReadAsINT08(dataReader, ordinal);
        }

        public override sbyte? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as ByteColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return (sbyte?)source.Value;
        }
    }
    
}
