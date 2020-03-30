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
            var epsgCoordinate = new EpsgCoordinate();
            var coordinatesString = GetArrCoordinatesFromWktString(wktString);
            if ((coordinatesString != null) && (coordinatesString.Length == 2))
            {
                epsgCoordinate.X = GetDouble(coordinatesString[0]);
                epsgCoordinate.Y = GetDouble(coordinatesString[1]);
            }
            else
            {
                throw new NotImplementedException(Events.MethodGetArrCoordinatesFromWktStringReturnNullOrCountElementsNotEqual2.Text);
            }
            return epsgCoordinate;
        }

        public static Wgs84Coordinate GetWgs84Coordinate(string wktString)
        {
            var wgs84Coordinate = new Wgs84Coordinate();
            var coordinatesString = GetArrCoordinatesFromWktString(wktString);
            if ((coordinatesString != null) && (coordinatesString.Length == 2))
            {
                wgs84Coordinate.Longitude = GetDouble(coordinatesString[0]);
                wgs84Coordinate.Latitude = GetDouble(coordinatesString[1]);
            }
            else
            {
                throw new NotImplementedException(Events.MethodGetArrCoordinatesFromWktStringReturnNullOrCountElementsNotEqual2.Text);
            }
            return wgs84Coordinate;
        }

        public static string GenerateWktString(double lon, double lat)
        {
            return $"POINT ({lon.ToString().Replace(",", ".")} {lat.ToString().Replace(",", ".")})";
        }
        public static void PrepareConverting(out SpatialReference source, out SpatialReference destination, uint epsgSourceCode, uint epsgDestinationCode)
        {
            source = new SpatialReference(null);
            destination = new SpatialReference(null);
            source.ImportFromEPSG((int)epsgSourceCode);
            destination.ImportFromEPSG((int)epsgDestinationCode);
        }
    }
}
