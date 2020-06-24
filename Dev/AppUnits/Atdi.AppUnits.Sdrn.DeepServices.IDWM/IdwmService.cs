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
            return SdrnsControllerWcfClientIWorldMapApi.GetADMByPoint(new WCF.Point() { Longitude = point.Longitude, Latitude = point.Latitude });
        }


        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        public void GetNearestPointByADM(in PointByADM pointByADM, ref Point resultPoint)
        {
            var val = SdrnsControllerWcfClientIWorldMapApi.GetNearestPointByADM(new WCF.PointByADM() { Longitude = pointByADM.Longitude, Latitude = pointByADM.Latitude, Administration = pointByADM.Administration });
            if (val!=null)
            {
                resultPoint.Longitude = val.Longitude;
                resultPoint.Latitude = val.Latitude;
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
           var val = SdrnsControllerWcfClientIWorldMapApi.GetADMByPointAndDistance(new WCF.PointAndDistance() { Longitude = pointAndDistance.Longitude, Latitude = pointAndDistance.Latitude, Distance = pointAndDistance.Distance });
           if ((val!=null) && (val.Length>0))
           {
                sizeResultBuffer = val.Length;
                for (int i=0; i< val.Length; i++)
                {
                    administrationsResult[i].Administration = val[i].Administration;
                    administrationsResult[i].Azimuth = val[i].Azimuth;
                    administrationsResult[i].Distance = val[i].Distance;
                    administrationsResult[i].Point = new Point() { Longitude = val[i].Point.Longitude, Latitude = val[i].Point.Latitude };
                }
           }
        }

        public void Dispose()
        {
            
        }

    }
}
