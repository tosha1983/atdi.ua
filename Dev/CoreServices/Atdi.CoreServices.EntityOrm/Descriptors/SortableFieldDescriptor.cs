using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class SortableFieldDescriptor
    {
        public FieldDescriptor Field { get; set; }

        public SortDirection Direction { get; set; }

        public override string ToString()
        {
            return $"{Field}: Direction = '{Direction}'";
        }
    }
}
