using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using OSGeo.OGR;
using OSGeo.OSR;
using System.Globalization;

namespace Atdi.AppUnits.Sdrn.DeepServices.Gis
{
    public static class Utils
    {
        public static double GetDouble(string value)
        {
            var ci = CultureInfo.CurrentCulture;
            var _pos = value.Replace(".", ci.NumberFormat.NumberDecimalSeparator).Replace(",", ci.NumberFormat.NumberDecimalSeparator);
            return double.Parse(_pos);
        }

        public static string[] GetArrCoordinatesFromWktString(string wktString)
        {
            var wktStringCoord = wktString.Replace("POINT", "").Replace(")", "").Replace("(", "");
            return wktStringCoord.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static EpsgCoordinate GetEpsgCoordinate(string wktString)
        {
            var coordinatesString = GetArrCoordinatesFromWktString(wktString);
            if ((coordinatesString != null) && (coordinatesString.Length == 2))
            {
                return new EpsgCoordinate()
                {
                    X = GetDouble(coordinatesString[0]),
                    Y = GetDouble(coordinatesString[1])
                };
            }
            else
            {
                throw new NotImplementedException(Events.MethodGetArrCoordinatesFromWktStringReturnNullOrCountElementsNotEqual2.Text);
            }
        }

        public static Wgs84Coordinate GetWgs84Coordinate(string wktString)
        {
            var coordinatesString = GetArrCoordinatesFromWktString(wktString);
            if ((coordinatesString != null) && (coordinatesString.Length == 2))
            {
                return new Wgs84Coordinate()
                {
                    Longitude = GetDouble(coordinatesString[0]),
                    Latitude = GetDouble(coordinatesString[1])
                };
            }
            else
            {
                throw new NotImplementedException(Events.MethodGetArrCoordinatesFromWktStringReturnNullOrCountElementsNotEqual2.Text);
            }
        }

        public static string GenerateWktString(double lon, double lat)
        {
            return $"POINT ({lon.ToString().Replace(",", ".")} {lat.ToString().Replace(",", ".")})";
        }
    }
}
