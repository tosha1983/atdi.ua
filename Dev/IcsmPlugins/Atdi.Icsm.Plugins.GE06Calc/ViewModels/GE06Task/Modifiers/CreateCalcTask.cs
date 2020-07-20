using Atdi.DataModels.Sdrn.DeepServices.GN06;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Modifiers
{
    public class CreateCalcTask
    {
        public string MapName;
        public long ContextId;
        public Guid OwnerId;
        public float AzimuthStep_deg;
        public bool AdditionalContoursByDistances;
        public int[] Distances;
        public string DistancesString;
        public bool ContureByFieldStrength;
        public int[] FieldStrength;
        public string FieldStrengthString;
        public int SubscribersHeight;
        public double PercentageTime;
        public bool UseEffectiveHeight;
        public byte CalculationTypeCode;
        public string CalculationTypeName;
        public int? StepBetweenBoundaryPoints;
        public BroadcastingContext BroadcastingExtend;

        public bool Success;
    }
}
