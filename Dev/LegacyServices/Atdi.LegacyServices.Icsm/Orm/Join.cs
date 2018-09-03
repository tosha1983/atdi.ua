using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class Join
    {
        public Table Table;
        public FieldJ Repr;
        public Module Module;
        public JoinType Type;
        public Table JoinedTable;
        public int Nequ;
        public Field[] From;
        private FieldF[] _To;
        public string[] ToNames;
        public string JoinedListName;
        public string[] BadConstr;
        public FieldF[] To
        {
            get
            {
                if (this._To == null)
                {
                    FieldF[] array = new FieldF[this.Nequ];
                    for (int i = 0; i < this.Nequ; i++)
                    {
                        array[i] = (FieldF)this.JoinedTable.Field(this.ToNames[i]);
                    }
                    this._To = array;
                }
                return this._To;
            }
        }
    }
}
