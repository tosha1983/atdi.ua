using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.Test.DeepServices.Client.WPF
{
    public class Location
    {

        public Location(string lon, string lat)
        {
            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            this.Lon = Convert.ToDouble(lon, invariantCulture);
            this.Lat = Convert.ToDouble(lat, invariantCulture);
        }

        public Location(double lon, double lat)
        {
            this.Lon = lon;
            this.Lat = lat;
        }

        public Location(double lon, double lat, double lon_m, double lat_m, double height, double fs)
        {
            this.Lon = lon;
            this.Lat = lat;
            this.Lon_m = lon_m;
            this.Lat_m = lat_m;
            this.FS = fs;
            this.Height = height;
        }

        public double Lon;
        public double Lat;
        public double Height;
        public double FS;
        public double Lon_m;
        public double Lat_m;

        public Location Copy()
        {
            return new Location(this.Lon, this.Lat);
        }

        public bool CheckHitting(LocationPolygon poligon)
        {
            if (poligon == null || poligon.Count == 0)
            {
                return false;
            }

            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < poligon.Count - 1; i++)
            {
                if (((poligon[i].Lat <= this.Lat) && ((poligon[i + 1].Lat > this.Lat))) || ((poligon[i].Lat > this.Lat) && ((poligon[i + 1].Lat <= this.Lat))))
                {
                    if ((poligon[i].Lon > this.Lon) && (poligon[i + 1].Lon > this.Lon))
                    {
                        hit = !hit;
                    }
                    else if (!((poligon[i].Lon < this.Lon) && (poligon[i + 1].Lon < this.Lon)))
                    {
                        if (this.Lon < poligon[i + 1].Lon - (this.Lat - poligon[i + 1].Lat) * (poligon[i + 1].Lon - poligon[i].Lon) / (poligon[i].Lat - poligon[i + 1].Lat))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = poligon.Count - 1;
            if (((poligon[i_].Lat <= this.Lat) && ((poligon[0].Lat > this.Lat))) || ((poligon[i_].Lat > this.Lat) && ((poligon[0].Lat <= this.Lat))))
            {
                if ((poligon[i_].Lon > this.Lon) && (poligon[0].Lon > this.Lon))
                {
                    hit = !hit;
                }
                else if (!((poligon[i_].Lon < this.Lon) && (poligon[0].Lon < this.Lon)))
                {
                    if (this.Lon < poligon[0].Lon - (this.Lat - poligon[0].Lat) * (poligon[0].Lon - poligon[i_].Lon) / (poligon[i_].Lat - poligon[0].Lat))
                    {
                        hit = !hit;
                    }
                }
            }

            return hit;

        }

        
    }
}
