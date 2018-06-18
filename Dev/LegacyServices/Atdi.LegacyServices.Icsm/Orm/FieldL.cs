using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public class FieldL : Field
    {
        public Table ComponentTable;
        internal object _componentJoin;
        public FieldJ ComponentJoin
        {
            get
            {
                object componentJoin = this._componentJoin;
                if (componentJoin is string)
                {
                    this._componentJoin = (FieldJ)this.ComponentTable.Field((string)componentJoin);
                }
                return (FieldJ)this._componentJoin;
            }
        }
    }
}
