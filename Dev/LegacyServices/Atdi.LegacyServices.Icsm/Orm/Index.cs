using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class Index
    {
        public IndexType Type;
        public Module Module;
        public string Name;
        public Field[] Fields;
        public bool HasField(Field fd)
        {
            Field[] fields = this.Fields;
            for (int i = 0; i < fields.Length; i++)
            {
                Field ormField = fields[i];
                if (ormField == fd || ormField.Name == fd.Name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
