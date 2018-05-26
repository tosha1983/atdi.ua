using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public class EngineCommandParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataType DataType { get; set; }
    }
}
