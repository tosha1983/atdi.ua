using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.EarthGeometry
{
    public struct ContourForStationByTriggerFieldStrengthsArgs
    {
        /// <summary>
        /// точка (координата в DEC)
        /// </summary>
        public PointEarthGeometric  BaryCenter;
        /// <summary>
        /// шаг (градусы)
        /// </summary>
        public double Step_deg;
        /// <summary>
        /// триггерное значение напряженности поля
        /// </summary>
        public double TriggerFieldStrength;
    }
}

