using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public enum FieldNature
    {
        Column = 70,
        Join = 74,
        Calc = 67,
        Expr = 69,
        Update = 85,
        List = 76,
        MultipleValues
    }
}
