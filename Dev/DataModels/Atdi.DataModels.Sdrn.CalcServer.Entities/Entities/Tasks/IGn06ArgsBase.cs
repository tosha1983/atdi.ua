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
        float? AzimuthStep_deg { get; set; }
        bool AdditionalContoursByDistances { get; set; }
        int[] Distances { get; set; }
        bool ContureByFieldStrength { get; set; }
        int[] FieldStrength { get; set; }
        int? SubscribersHeight { get; set; }
        double? PercentageTime { get; set; }
        bool UseEffectiveHeight { get; set; }
        byte CalculationTypeCode { get; set; }
        string CalculationTypeName { get; set; }
        string BroadcastingExtend { get; set; }
    }

    public enum CalculationType
    {
        ConformityCheck = 1,
        FindAffectedADM = 2,
        CreateContoursByDistance = 3,
        CreateContoursByFS = 4
    }


}
