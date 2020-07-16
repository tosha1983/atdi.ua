using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class Ge06TaskParameters
    {
        public float? AzimuthStep_deg;
        public bool AdditionalContoursByDistances;
        public int[] Distances;
        public bool ContureByFieldStrength;
        public int[] FieldStrength;
        public int? SubscribersHeight;
        public double? PercentageTime;
        public bool UseEffectiveHeight;
        public byte CalculationTypeCode;
        public string CalculationTypeName;
        public string Projection;
        public string MapName;
        public int? StepBetweenBoundaryPoints;
        public BroadcastingContext BroadcastingContext;
    }
}
