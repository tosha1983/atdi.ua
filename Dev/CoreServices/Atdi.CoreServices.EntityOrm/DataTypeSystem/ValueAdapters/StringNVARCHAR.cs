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
    class StringNVARCHAR : ValueAdapter<string, string>
    {
        public StringNVARCHAR(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override string DecodeAs(object storeValue)
        {
            if (storeValue == null)
            {
                return (string)null;
            }
            return Convert.ToString(storeValue);
        }

        public override string DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsNVARCHAR(dataReader, ordinal);
        }

        public override string EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as StringColumnValue).Value;
            //if (!source)
            //{
            //    return null;
            //}
            return source;
        }
    }
    
}
