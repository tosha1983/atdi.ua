using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeepServices.IDWM
{
	
	public interface IIdwmService : IDeepService
	{
        /// <summary>
        /// Функция по определению администрации по точке.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        string GetADMByPoint(in Point point);

        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        void GetNearestPointByADM(in PointByADM pointByADM, ref Point resultPoint);

        /// <summary>
        /// Определяем все администрации, которые попали в соответствующий радиус от точки. 
        /// </summary>
        /// <param name="administrations"></param>
        /// <param name="pointAndDistance"></param>
        void GetADMByPointAndDistance(in PointAndDistance pointAndDistance, ref AdministrationsResult[] administrationsResult, out int sizeResultBuffer);

    }
}
