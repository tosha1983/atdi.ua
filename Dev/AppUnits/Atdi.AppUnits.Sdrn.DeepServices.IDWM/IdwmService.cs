using System;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using System.Linq;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using WCF = Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM;


namespace Atdi.AppUnits.Sdrn.DeepServices.IDWM
{
    public class IdwmService : IIdwmService
    {

        /// <summary>
        /// Функция по определению администрации по точке
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public string GetADMByPoint(in Point point)
        {
            return SdrnsControllerWcfClientIWorldMapApi.GetADMByPoint(new WCF.Point() { Longitude = point.Longitude_dec, Latitude = point.Latitude_dec });
        }


        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        public void GetNearestPointByADM(in PointByADM pointByADM, ref Point resultPoint)
        {
            var val = SdrnsControllerWcfClientIWorldMapApi.GetNearestPointByADM(new WCF.PointByADM() { Point = new WCF.Point() { Longitude = pointByADM.Point.Longitude_dec.Value, Latitude = pointByADM.Point.Latitude_dec.Value }, Administration = pointByADM.Administration });
            if (val!=null)
            {
                resultPoint.Longitude_dec = val.Longitude;
                resultPoint.Latitude_dec = val.Latitude;
            }
        }

        /// <summary>
        /// Определяем все администрации, которые попали в соответствующий радиус от точки. 
        /// </summary>
        /// <param name="pointAndDistance"></param>
        /// <param name="administrationsResult"></param>
        /// <param name="SizeBuffer"></param>
        public void GetADMByPointAndDistance(in PointAndDistance pointAndDistance, ref AdministrationsResult[] administrationsResult, out int sizeResultBuffer)
        {
           sizeResultBuffer = 0;
           var val = SdrnsControllerWcfClientIWorldMapApi.GetADMByPointAndDistance(new WCF.PointAndDistance() { Point = new WCF.Point() { Longitude = pointAndDistance.Point.Longitude_dec.Value, Latitude = pointAndDistance.Point.Latitude_dec.Value }, Distance = pointAndDistance.Distance });
           if ((val!=null) && (val.Length>0))
           {
                sizeResultBuffer = val.Length;
                for (int i=0; i< val.Length; i++)
                {
                    administrationsResult[i].Administration = val[i].Administration;
                    administrationsResult[i].Azimuth = val[i].Azimuth;
                    administrationsResult[i].Distance = val[i].Distance;
                    administrationsResult[i].Point = new Point() { Longitude_dec = val[i].Point.Longitude, Latitude_dec = val[i].Point.Latitude };
                }
           }
        }

        public void Dispose()
        {
            
        }

    }
}
