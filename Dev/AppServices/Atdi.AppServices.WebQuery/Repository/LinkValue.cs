using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;

namespace Atdi.AppServices.WebQuery
{
    public class LinkValue
    {
        public string SourceColumnName { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public int Level { get; set; }
        public DataType typeValue { get; set; }
    }
}

