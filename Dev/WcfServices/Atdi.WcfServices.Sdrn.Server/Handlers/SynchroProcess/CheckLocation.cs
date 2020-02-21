using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Xml;
using System.Linq;


namespace Atdi.WcfServices.Sdrn.Server
{

    public class CheckLocation
    {

        public CheckLocation(string lon, string lat)
        {
            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            this.Lon = Convert.ToDouble(lon, invariantCulture);
            this.Lat = Convert.ToDouble(lat, invariantCulture);
        }

        public CheckLocation(double lon, double lat)
        {
            this.Lon = lon;
            this.Lat = lat;
        }

        public double Lon;
        public double Lat;

        public CheckLocation Copy()
        {
            return new CheckLocation(this.Lon, this.Lat);
        }

        public bool CheckHitting(List<DataLocation> poligon)
        {
            if (poligon == null || poligon.Count == 0)
            {
                return false;
            }

            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < poligon.Count - 1; i++)
            {
                if (((poligon[i].Latitude <= this.Lat) && ((poligon[i + 1].Latitude > this.Lat))) || ((poligon[i].Latitude > this.Lat) && ((poligon[i + 1].Latitude <= this.Lat))))
                {
                    if ((poligon[i].Longitude > this.Lon) && (poligon[i + 1].Longitude > this.Lon))
                    {
                        hit = !hit;
                    }
                    else if (!((poligon[i].Longitude < this.Lon) && (poligon[i + 1].Longitude < this.Lon)))
                    {
                        if (this.Lon < poligon[i + 1].Longitude - (this.Lat - poligon[i + 1].Latitude) * (poligon[i + 1].Longitude - poligon[i].Longitude) / (poligon[i].Latitude - poligon[i + 1].Latitude))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = poligon.Count - 1;
            if (((poligon[i_].Latitude <= this.Lat) && ((poligon[0].Latitude > this.Lat))) || ((poligon[i_].Latitude > this.Lat) && ((poligon[0].Latitude <= this.Lat))))
            {
                if ((poligon[i_].Longitude > this.Lon) && (poligon[0].Longitude > this.Lon))
                {
                    hit = !hit;
                }
                else if (!((poligon[i_].Longitude < this.Lon) && (poligon[0].Longitude < this.Lon)))
                {
                    if (this.Lon < poligon[0].Longitude - (this.Lat - poligon[0].Latitude) * (poligon[0].Longitude - poligon[i_].Longitude) / (poligon[i_].Latitude - poligon[0].Latitude))
                    {
                        hit = !hit;
                    }
                }
            }
            return hit;
        }
    }
}




