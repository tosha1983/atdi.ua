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
using Atdi.Common.Extensions;
namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    class ClrTypeCLRTYPE : ValueAdapter<object, object>
    {
        public ClrTypeCLRTYPE(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override object DecodeAs(object storeValue)
        {
            return storeValue;
        }

        public override object DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            var value = ValueReader.ReadAsCLRTYPE(dataReader, ordinal);
            return value;
        }

        public override object EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as ClrTypeColumnValue).Value;
            return source;
        }
    }
    
}
