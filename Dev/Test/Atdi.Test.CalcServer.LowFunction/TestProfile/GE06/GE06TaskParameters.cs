using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.CalcServer.LowFunction
{
    public class GE06TaskParameters
    {
        public float AzimuthStep_deg;
        public bool AdditionalContoursByDistances;
        public int[] Distances;
        public bool CoutureByFieldStrength;
        public int[] FieldStrength;
        public int SubscribersHeight;
        public CalculationType CalculationType;
    }
    public enum CalculationType
    {
        ConformityCheck = 1,
        FindAffectedADM = 2,
        CreateContoursByDistance = 3,
        CreateContoursByFS = 4
    }

}
