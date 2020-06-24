using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM
{
	[ServiceContract(Namespace = "http://schemas.atdi.com/appserver/services/sdrn/deepservices/idwn")]
	public interface IWorldMapApi
	{
       /// <summary>
        /// Функция по определению администрации по точке.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [OperationContract]
        string GetADMByPoint(Point point);

        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        [OperationContract]
        Point GetNearestPointByADM(PointByADM pointByADM);


        /// <summary>
        /// Определяем все администрации, которые попали в соответствующий радиус от точки. 
        /// </summary>
        /// <param name="administrations"></param>
        /// <param name="pointAndDistance"></param>
        [OperationContract]
        AdministrationsResult[] GetADMByPointAndDistance(PointAndDistance pointAndDistance);
    }
}
