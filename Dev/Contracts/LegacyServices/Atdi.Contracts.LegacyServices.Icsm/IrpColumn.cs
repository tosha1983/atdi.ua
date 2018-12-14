using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.WebQuery;

namespace Atdi.Contracts.LegacyServices.Icsm
{
    public sealed class IrpColumn
    {
        public string Expr { get; set; }
        public IrpColumnEnum TypeColumn { get; set; }
        public ColumnMetadata columnMeta { get; set; }
        public ColumnProperties[] columnProperties { get; set; }
    }
}
