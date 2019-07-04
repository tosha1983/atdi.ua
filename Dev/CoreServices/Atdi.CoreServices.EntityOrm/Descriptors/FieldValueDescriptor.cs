using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class FieldValueDescriptor
    {
        public FieldDescriptor Descriptor { get; set; }

        public ColumnValue Store { get; set; }

        public override string ToString()
        {
            return $"Descriptor = [{this.Descriptor}], Value = [{Store}]";
        }
    }
}
