using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
    [Serializable]
    public struct AbsorptionCalcBlock
    {
        public AbsorptionCalcBlockModelType ModelType;
        public bool Hybrid; // Case when instead of relief 
        public bool Available;
    }

    [Serializable]
    public enum AbsorptionCalcBlockModelType
    {
        /// <summary>
        /// Unknown Model
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Flat Model
        /// </summary>
        Flat = 1,

        /// <summary>
        /// Linear Model
        /// </summary>
        Linear = 2,

        /// <summary>
        /// ITU 2109(2) Model
        /// </summary>
        ITU2109_2 = 3,

        /// <summary>
        /// Flat + Linear Model
        /// </summary>
        FlatAndLinear = 4,

        /// <summary>
        /// ITU 2109 + Linear Model
        /// </summary>
        ITU2109AndLinear = 5
    }
}
