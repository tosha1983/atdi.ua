using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    class BooleanBOOL : ValueAdapter<bool?, bool?>
    {
        public BooleanBOOL(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

        public override bool? DecodeAs(object storeValue)
        {
            return Convert.ToBoolean(storeValue);
        }

        public override bool? DecodeAs(IDataReader dataReader, int ordinal)
        {
            return ValueReader.ReadAsBOOL(dataReader, ordinal);
        }

        public override bool? EncodeAs(ColumnValue columnValue)
        {
            var source = (columnValue as BooleanColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return source.Value;
        }
    }
    
}
