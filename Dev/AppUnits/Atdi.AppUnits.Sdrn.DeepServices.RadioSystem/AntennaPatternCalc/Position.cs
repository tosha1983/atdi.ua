using System;
using System.Collections.Generic;
using System.Text;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern
{
    public struct RecordPos
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
    }

    //=====================================================
    /// <summary>
    /// Структура координаты
    /// </summary>
    public class RecordXY
    {
        private double _x;
        private double _y;
        //
        public RecordXY(double Lon, double Lat)
        {
            Longitude = Lon;
            Latitude = Lat;
        }
        //
        public RecordXY() : this(0.0, 0.0) { }
        //
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Longitude { get { return _x; } set { _x = value; } }
        public double Latitude { get { return _y; } set { _y = value; } }
    }

    public class Position
    {
        private const double _midleRadiusEarth = 6371.032; // средний радиус земли // midle radius earth               

        #region METHODS
        //===========================================================
        /// <summary>
        /// Возвращает SQL выражение для ограничения радиуса поиска нужных точек
        /// Returns the SQL statement for limit the search radius needed coordinates
        /// </summary>
        /// <param name="X">координата по Х (град)</param>
        /// <param name="Y">координата по У (град)</param>
        /// <param name="radius">радиус поиска(град)</param>
        /// <param name="alias_X">псевдоним поля таблицы по оси Х(передавать с именем таблицы "имя таблицы.имя поля")</param>
        /// <param name="alias_Y">псевдоним поля таблицы по оси Y(передавать с именем таблицы "имя таблицы.имя поля")</param>
        /// <returns>SQL выражение</returns>
        public static RecordPos GetRangeCoordinates(double X, double Y, double radius,
            string alias_X, string alias_Y)
        {
            //double countEarths = _midleRadiusEarth / radius; // если заданный радиус больше радиуса земли тогда делаем вычитание
            //if (countEarths > 1)
            //{
            //    int counts = (int)countEarths;
            //    radius = countEarths - counts;
            //}

            radius = radius / 111.11111; // переводим км в градусы // convert km to degree

            // определяем точки для поиска
            // define points for search


            double radiusX = radius / Math.Cos(Y * Math.PI / 180);

            RecordPos points = new RecordPos();

            double X2 = X + radiusX;
            if (X2 < 180 && X2 > -180)
                points.X2 = X2;
            else
            {
                points.X2 = 180;
            }

            double X1 = X - radiusX;
            if (X1 < 180 && X1 > -180)
                points.X1 = X1;
            else
            {
                points.X1 = -180;
            }

            double Y1 = Y + radius;
            if (Y1 < 90 && Y1 > -90)
                points.Y1 = Y1;
            else
            {
                points.Y1 = 90;
            }

            double Y2 = Y - radius;
            if (Y2 < 90 && Y2 > -90)
                points.Y2 = Y2;
            else
            {
                points.Y2 = -90;
            }

            return points;

            //string resultSQL = GetSQLRequest(points, alias_X, alias_Y);
            //return resultSQL;
        }

        //===========================================================
        /// <summary>
        /// Возвращает SQL выражение для ограничения радиуса поиска нужных точек
        /// Returns the SQL statement for limit the search radius needed coordinates
        /// </summary>
        /// <param name="Position">точки для поиска</param>
        /// <param name="alias_X">псевдоним поля таблицы по оси Х(передавать с именем таблицы "имя таблицы.имя поля")</param>
        /// <param name="alias_Y">псевдоним поля таблицы по оси Y(передавать с именем таблицы "имя таблицы.имя поля")</param>
        /// <returns>SQL выражение</returns>
        //public static string GetSQLRequest(RecordPos Position, string alias_X, string alias_Y)
        //{
        //    StringBuilder builder = new StringBuilder();

        //    builder.Append(alias_X + " > " + Position.X1.ToString() + " AND ");
        //    builder.Append(alias_X + " < " + Position.X2.ToString() + " AND ");
        //    builder.Append(alias_Y + " < " + Position.Y1.ToString() + " AND ");
        //    builder.Append(alias_Y + " > " + Position.Y2.ToString());

        //    return builder.ToString();
        //}

        static public double GetEarthRadius()
        {
            return 6371.0;
        }

        public struct GeoPosition
        {
            public double Longitude;
            public double Latitude;
        };

        public struct SphereCoordinate
        {
            public double R;
            public double Phita;
            public double Phi;
        };

        public struct DecartCoordinate
        {
            public double X;
            public double Y;
            public double Z;
        };


        //===================================================
        /// <summary>
        /// Перевод градусов из DEC в DMS
        /// </summary>
        /// <param name="Degree">градусы в формате DEC</param>
        /// <returns>градусы в формата DMS</returns>
        static public double DecToDms(double Degree)
        {
            // Градусы
            double Dms = (double)((int)(Degree));
            Degree -= Dms;
            // Минуты
            Degree *= 60.0;
            double tmp = (double)((int)(Degree));
            Degree -= tmp;
            Dms += tmp / 100.0;
            //Секунды
            Degree *= 60.0;
            tmp = (double)(((int)(Degree * 10.0)) / 10.0);
            Dms += tmp / 10000.0;
            return Dms;
        }
        //===================================================
        /// <summary>
        /// Перевод градусов из DMS в DEC
        /// </summary>
        /// <param name="Degree">градусы в формате DMS</param>
        /// <returns>градусы в формата DEC</returns>
        static public double DmsToDec(double Degree)
        {
            // Градусы
            double dgr = (double)((int)(Degree));
            Degree -= dgr;
            // Минуты
            Degree *= 100.0;
            double min = (double)((int)(Degree));
            Degree -= min;
            //Секунды
            Degree *= 100.0;
            double sec = Degree;
            double retVal = (sec / 60.0 + min) / 60.0 + dgr;
            return retVal;
        }


        static public double GeoPositionToAngleDegree(double GeoCoordinate)
        {
            return DmsToDec(GeoCoordinate);
        }

        static public double AngleDegreeToGeoPosition(double Degree)
        {
            return DecToDms(Degree);
        }

        static public double DegreeToRadian(double Angle)
        {
            return System.Math.PI * Angle / 180.0;
        }

        static public double RadianToDegree(double Angle)
        {
            return Angle * (180.0 / System.Math.PI);
        }

        static public SphereCoordinate SphereCoordinateFromGeoPosition(double Longitude, double Lattitude)
        {
            SphereCoordinate SphereCoord = new SphereCoordinate();
            SphereCoord.R = 6371.0; //Average Radius;
            SphereCoord.Phi = GeoPositionToAngleDegree(Longitude);
            SphereCoord.Phita = GeoPositionToAngleDegree(Lattitude);
            return SphereCoord;
        }

        static private SphereCoordinate SphereCoordinateFromGeoPosition(GeoPosition GeoPosition)
        {
            SphereCoordinate SphereCoord = new SphereCoordinate();
            SphereCoord.R = 6371.0; //Average Radius;
            SphereCoord.Phi = GeoPositionToAngleDegree(GeoPosition.Longitude);
            SphereCoord.Phita = GeoPositionToAngleDegree(GeoPosition.Latitude);
            return SphereCoord;
        }

        static private GeoPosition GeoPositionFromSphereCoordinates(SphereCoordinate SphereCoord)
        {
            GeoPosition GeoPosition = new GeoPosition();
            GeoPosition.Latitude = AngleDegreeToGeoPosition(SphereCoord.Phita);
            GeoPosition.Longitude = AngleDegreeToGeoPosition(SphereCoord.Phi);
            return GeoPosition;
        }

        static public DecartCoordinate DecartCoordinateFromSphereCoordinate(SphereCoordinate SphereCoord)
        {
            DecartCoordinate DecartCoord = new DecartCoordinate();
            DecartCoord.X = SphereCoord.R * System.Math.Sin(DegreeToRadian(SphereCoord.Phita)) * System.Math.Cos(DegreeToRadian(SphereCoord.Phi));
            DecartCoord.Y = SphereCoord.R * System.Math.Sin(DegreeToRadian(SphereCoord.Phita)) * System.Math.Sin(DegreeToRadian(SphereCoord.Phi));
            DecartCoord.Z = SphereCoord.R * System.Math.Cos(DegreeToRadian(SphereCoord.Phita));
            return DecartCoord;
        }

        static public SphereCoordinate SphereCoordinateFromDecartCoordinate(DecartCoordinate DecardCoord)
        {
            SphereCoordinate SphereCoord = new SphereCoordinate();
            SphereCoord.R = System.Math.Sqrt(DecardCoord.X * DecardCoord.X + DecardCoord.Y * DecardCoord.Y + DecardCoord.Z * DecardCoord.Z);
            SphereCoord.Phita = System.Math.Acos(DecardCoord.Z / SphereCoord.R);

            if (DecardCoord.X > 0 && DecardCoord.Y > 0)
                SphereCoord.Phi = System.Math.Atan(DecardCoord.Y / DecardCoord.X);
            if (DecardCoord.X < 0 && DecardCoord.Y > 0)
                SphereCoord.Phi = (System.Math.PI / 2.0) + System.Math.Atan(DecardCoord.Y / DecardCoord.X);
            if (DecardCoord.X < 0 && DecardCoord.Y > 0)
                SphereCoord.Phi = -(System.Math.PI / 2.0) - System.Math.Atan(DecardCoord.Y / DecardCoord.X);
            if (DecardCoord.X > 0 && DecardCoord.Y < 0)
                SphereCoord.Phi = -System.Math.Atan(DecardCoord.Y / DecardCoord.X);

            return SphereCoord;
        }

        static public double OneDegreeLatitudeRange()
        {
            return GetEarthRadius() * DegreeToRadian(1);
        }

        static public double OneDegreeLongitudeRange(double Latitude)
        {
            return System.Math.PI * GetEarthRadius() / 180.0 * System.Math.Cos(DegreeToRadian(Latitude));
        }

        static public double GetDeltaLatitudeForRange(double Range)
        {
            return Range / OneDegreeLatitudeRange();
        }

        static public double GetDeltaLongitudeForRange(double Range, double Latitude)
        {
            return Range / OneDegreeLongitudeRange(Latitude);
        }

        static public double RangeOnSphereCoorinates(SphereCoordinate Sphere1, SphereCoordinate Sphere2)
        {
            //Math.SphereCoordinate Sphere1 = Math.SphereCoordinateFromGeoPosition( PositionX, PositionY);
            //Math.SphereCoordinate Sphere2 = Math.SphereCoordinateFromGeoPosition(station.PositionX, station.PositionY);

            double DegreeRange = Sphere1.R * DegreeToRadian(1);

            double Delta1 = (Sphere1.Phita - Sphere2.Phita) * DegreeRange;
            double Delta2 = (Sphere1.Phi - Sphere2.Phi) * DegreeRange * System.Math.Cos(DegreeToRadian(Sphere1.Phita + Sphere2.Phita) / 2.0);

            /*Math.SphereCoordinate Sphere1 = Math.SphereCoordinateFromGeoPosition(PositionX, PositionY);
			Math.DecartCoordinate thisCoord = Math.DecartCoordinateFromSphereCoordinate(Sphere1);
			Math.SphereCoordinate Sphere2 = Math.SphereCoordinateFromGeoPosition(station.PositionX, station.PositionY);
			Math.DecartCoordinate itsCoord = Math.DecartCoordinateFromSphereCoordinate(Sphere2);         
			
			double Range = System.Math.Sqrt(
				(thisCoord.X - itsCoord.X) * (thisCoord.X - itsCoord.X)+
				(thisCoord.Y - itsCoord.Y) * (thisCoord.Y - itsCoord.Y)+
				(thisCoord.Z - itsCoord.Z) * (thisCoord.Z - itsCoord.Z));*/

            return System.Math.Sqrt(Delta1 * Delta1 + Delta2 * Delta2);
        }

        static public double GetAzimuth(SphereCoordinate Sphere1, SphereCoordinate Sphere2)
        {
            //Math.SphereCoordinate Sphere1 = Math.SphereCoordinateFromGeoPosition(PositionX, PositionY);
            //Math.SphereCoordinate Sphere2 = Math.SphereCoordinateFromGeoPosition(station.PositionX, station.PositionY);

            double Delta1 = System.Math.PI * (Sphere1.Phita - Sphere2.Phita) / 180.0 * Sphere1.R;
            double Delta2 = System.Math.PI * (Sphere1.Phi - Sphere2.Phi) / 180.0 * Sphere2.R * System.Math.Cos((Sphere1.Phita + Sphere2.Phita) / 2);

            double TanAngle = System.Math.PI / 2;
            if (Delta2 != 0)
                TanAngle = System.Math.Abs(Delta1 / Delta2);
            else
            {
                if (Delta1 > 0)
                    return 0;
                else
                    return System.Math.PI;
            }

            if (Delta1 >= 0 && Delta2 >= 0)
                return System.Math.PI / 2.0 - System.Math.Atan(TanAngle);

            if (Delta1 >= 0 && Delta2 <= 0)
                return 3.0 * System.Math.PI / 2.0 + System.Math.Atan(TanAngle);

            if (Delta1 <= 0 && Delta2 >= 0)
                return System.Math.PI / 2.0 + System.Math.Atan(TanAngle);

            if (Delta1 <= 0 && Delta2 <= 0)//!!
                return 3.0 * System.Math.PI / 2.0 - System.Math.Atan(TanAngle);

            return 0.0;
        }

        static public SphereCoordinate SphereCoordinateByCoorinateSystem(string cSys, double Longitude, double Latitude)
        {
            if (cSys.Contains("DMS"))
                return SphereCoordinateFromGeoPosition(Longitude, Latitude);

            if (cSys.Contains("DEC"))
            {
                SphereCoordinate SphereCoord = new SphereCoordinate();
                SphereCoord.R = 6371.0; //Average Radius;
                SphereCoord.Phi = Longitude;
                SphereCoord.Phita = Latitude;
                return SphereCoord;
            }

            if (cSys.Contains("SEC"))
            {
                SphereCoordinate SphereCoord = new SphereCoordinate();
                SphereCoord.R = 6371.0; //Average Radius;
                SphereCoord.Phi = Longitude / 3600.0;
                SphereCoord.Phita = Latitude / 3600.0;
                return SphereCoord;
            }

            if (cSys.Contains("DMD"))
            {
                SphereCoordinate SphereCoord = new SphereCoordinate();
                SphereCoord.R = 6371.0; //Average Radius;

                double retVal = (int)Longitude;
                Longitude -= retVal;
                retVal += 100 * Longitude / 60;
                SphereCoord.Phi = retVal;

                retVal = (int)Latitude;
                Longitude -= retVal;
                retVal += 100 * Latitude / 60;
                SphereCoord.Phita = retVal;

                //SphereCoord.Phi = Longitude;
                //SphereCoord.Phita = Latitude;
                return SphereCoord;
            }

            SphereCoordinate DummyCoord = new SphereCoordinate();
            return DummyCoord;

            /*	if (sysCoord.IndexOf("DEC") != -1)
				{// DEC
					return pos;
				}
				else if (sysCoord.IndexOf("DMD") != -1)
				{// DMD
					double retVal = (int)pos;
					pos -= retVal;
					retVal += 100 * pos / 60;
					return retVal;
				}
				else if (sysCoord.IndexOf("DMS") != -1)
				{// DMS
					//Convert to DMD
					double retval = ((int)(pos * 100.0)) / 100.0;
					double tmpVal = (pos - retval) / 0.6;
					pos = retval + tmpVal;
					retval = (int)pos;
					pos -= retval;
					retval += 100 * pos / 60;
					return retval;
				}
				else if (sysCoord.IndexOf("SEC") != -1)
				{// SEC
					return pos / 3600;
				}
				return pos; */
        }

        //===================================================
        /// <summary>
        /// Переводит координаты из одной системы в другую
        /// </summary>
        /// <param name="pos">координата</param>
        /// <param name="fromCSYS">из системы</param>
        /// <param name="toCSYS">в систему</param>
        /// <returns>переведенная система координат</returns>
        public static RecordXY convertPosition(RecordXY pos, string fromCSYS, string toCSYS)
        {
            RecordXY retXY = new RecordXY(pos.Longitude, pos.Latitude);
            if (fromCSYS.Contains("DMS"))
            {// Из DMS
                if (toCSYS.Contains("DEC"))
                {// в DEC
                    retXY.Longitude = DmsToDec(pos.Longitude);
                    retXY.Latitude = DmsToDec(pos.Latitude);
                }
            }
            else if (fromCSYS.Contains("DEC"))
            {// Из DEC
                if (toCSYS.Contains("DMS"))
                {// в DMS
                    retXY.Longitude = DecToDms(pos.Longitude);
                    retXY.Latitude = DecToDms(pos.Latitude);
                }
            }
            return retXY;
        }
        //===================================================
        /// <summary>
        /// Перевозит координаты из одной системы в другую
        /// </summary>
        /// <param name="Lat">Широта</param>
        /// <param name="Lon">Долгота</param>
        /// <param name="fromCSYS">из системы</param>
        /// <param name="toCSYS">в систему</param>
        /// <returns>переведенная система координат</returns>
        public static RecordXY convertPosition(double Lon, double Lat, string fromCSYS, string toCSYS)
        {
            return convertPosition(new RecordXY(Lon, Lat), fromCSYS, toCSYS);
        }


        public static GeoPosition GeoPositionFromCoordinates(double position_X, double position_Y,
            string cSys)
        {
            SphereCoordinate spheraCoord = SphereCoordinateByCoorinateSystem(cSys, position_X, position_Y);
            GeoPosition geoPos = GeoPositionFromSphereCoordinates(spheraCoord);
            return geoPos;
        }

        #endregion
    }
}
