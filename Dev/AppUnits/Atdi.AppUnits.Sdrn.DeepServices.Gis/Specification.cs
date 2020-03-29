using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeepServices.Gis
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "DeepServices Gis Component";
      
    }

    static class Categories
    {
        public static readonly EventCategory Convert = "Convert";
    }

    static class Events
    {
        public static readonly EventText ForAtdiNameProjectionNoAlgorithmConvertingToEPSG = $"For 'Atdi Name Projection' - '{0}' no algorithm for converting to EPSG code has been defined!";
        public static readonly EventText ForEPSGNoAlgorithmConvertingToAtdiNameProjection = $"For EPSG code - '{0}' no algorithm for converting to 'Atdi Name Projection' code has been defined!";
        public static readonly EventText MethodGetArrCoordinatesFromWktStringReturnNullOrCountElementsNotEqual2 = "Method 'GetArrCoordinatesFromWktString' return null, empty or count elements not equal 2!";
    }
    static class TraceScopeNames
    {

    }

    static class Exceptions
    {

    }
}
