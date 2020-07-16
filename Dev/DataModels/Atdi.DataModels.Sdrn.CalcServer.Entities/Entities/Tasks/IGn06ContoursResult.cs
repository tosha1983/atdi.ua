using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
    [EntityPrimaryKey]
    public interface IGn06ContoursResult_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IGn06ContoursResult : IGn06ContoursResult_PK
    {
        long Gn06ResultId { get; set; }
        byte ContourType { get; set; }
        int Distance { get; set; }
        double FS { get; set; }
        string AffectedADM { get; set; }
        int PointsCount { get; set; }
		// string == CountoursPoint[]
        string CountoursPoints { get; set; }
    }
}

