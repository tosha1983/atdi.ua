using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public class FieldJ : Field
    {
        public int _joinIdx;
        public string Syn;
        public Join Join
        {
            get
            {
                return this.Table.Joins[this._joinIdx];
            }
        }
    }
}
