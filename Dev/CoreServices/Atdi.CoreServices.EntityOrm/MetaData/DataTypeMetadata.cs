﻿using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.Metadata
{


    public class DataTypeMetadata : IDataTypeMetadata
    {
        public string Name { get; set; }

        public DataSourceType DataSourceType { get; set; }

        public DataType CodeVarType { get; set; }

        public string SourceVarType { get; set; }

        public int? Length { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public IAutonumMetadata Autonum { get; set; }
    }

}
