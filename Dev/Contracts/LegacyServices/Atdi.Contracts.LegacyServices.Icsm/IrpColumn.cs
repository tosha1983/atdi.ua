using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.LegacyServices.Icsm
{
    public sealed class IrpColumn
    {
        public string Expr { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public IrpColumnEnum TypeColumn { get; set; }
    }
}
