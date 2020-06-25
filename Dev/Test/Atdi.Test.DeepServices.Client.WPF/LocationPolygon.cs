using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.Test.DeepServices.Client.WPF
{
    public class LocationPolygon
    {
        private readonly List<Location> _locations;

        public LocationPolygon(string locationList)
        {
            this._locations = MakePoligonFromString(locationList);
        }

        public int Count => this._locations.Count;

        public Location this[int index] => this._locations[index];
        
        public LocationPolygonRect GetRect()
        {
            var count = this._locations.Count;

            if (count <= 0)
            {
                return null;
            }

            var result = new LocationPolygonRect();
            result.Min = this._locations[0].Copy();
            result.Max = this._locations[0].Copy();

            for (int i = 1; i < count; i++)
            {
                var location = this._locations[i];

                if (location.Lat > result.Max.Lat)
                {
                    result.Max.Lat = location.Lat;
                }
                if (location.Lon > result.Max.Lon)
                {
                    result.Max.Lon = location.Lon;
                }
                if (location.Lat < result.Min.Lat)
                {
                    result.Min.Lat = location.Lat;
                }
                if (location.Lon < result.Min.Lon)
                {
                    result.Min.Lon = location.Lon;
                }
            }

            return result;
        }

        private static List<Location> MakePoligonFromString(string locationList)
        {
            var poligon = new List<Location>();
            locationList = System.Text.RegularExpressions.Regex.Replace(locationList, "[^0-9.,]", "");

            var locations = locationList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (locations.Length > 0)
            {
                for (int i = 0; i < locations.Length; i += 2)
                {
                    if (locations.Length == i)
                    {
                        throw new InvalidOperationException($"Incorrect value of locations list: '{locationList}'");
                    }

                    var location = new Location(locations[i + 1], locations[i]);
                    poligon.Add(location);
                }
            }

            return poligon;
        }
    }
}
