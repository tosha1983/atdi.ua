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
    class SignedByteINT08 : ValueAdapter<sbyte?, sbyte?>
    {
        public SignedByteINT08(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override sbyte? DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (sbyte?)null;
            }
            return Convert.ToSByte(storeValue);
        }

        public override sbyte? DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsINT08(dataReader, ordinal);
        }

        public override sbyte? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as SignedByteColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
