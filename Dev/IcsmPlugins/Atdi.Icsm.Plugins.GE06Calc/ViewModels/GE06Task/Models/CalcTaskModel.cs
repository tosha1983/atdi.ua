using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task
{
    public class CalcTaskModel
    {
        public string MapName { get; set; }
        public float AzimuthStep_deg { get; set; }
        public bool AdditionalContoursByDistances { get; set; }
        public int[] Distances { get; set; }
        public string DistancesString { get; set; }
        public bool ContureByFieldStrength { get; set; }
        public int[] FieldStrength { get; set; }
        public string FieldStrengthString { get; set; }
        public int SubscribersHeight { get; set; }
        public double PercentageTime { get; set; }
        public bool UseEffectiveHeight { get; set; }
        public int? StepBetweenBoundaryPoints { get; set; }
        public bool StepBetweenBoundaryPointsDefault { get; set; }
    }
}
