using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Brific.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface Ifmtv_fdg_PK
	{
        int terrakey { get; set; }
    }

    [Entity]
    public interface Ifmtv_fdg : Ifmtv_fdg_PK
	{
		string fdg_type { get; set; }

        string fdg_status { get; set; }

		string is_favorbl { get; set; }

		string src_fdg { get; set; }

		DateTime d_updated { get; set; }
	}

    
}
