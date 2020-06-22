using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;


namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{

	[Entity]
	public interface IGn06ArgsBase
	{
        float AzimuthStep_deg { get; set; }
        bool AdditionalContoursByDistances { get; set; }
        int[] Distances { get; set; }
        bool ContureByFieldStrength { get; set; }
        int[] FieldStrength { get; set; }
        int SubscribersHeight { get; set; }
        double PercentageTime { get; set; }
        bool UseEffectiveHeight { get; set; }
        string CalculationType { get; set; }
        string SourceBroadcasting { get; set; }
        byte[] BroadcastingAssignments { get; set; }
        byte[] BroadcastingAllotments { get; set; }
    }
	
}
