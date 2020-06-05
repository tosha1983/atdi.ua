using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WpfControls.EntityOrm.Controls
{
    public class OrmEnumBoxData
    {
        public int Id;

        public string Name;

        public string ViewName;

        public override string ToString()
        {
            return ViewName;
        }
    }
}
