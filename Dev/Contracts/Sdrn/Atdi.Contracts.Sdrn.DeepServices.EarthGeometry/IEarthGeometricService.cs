using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeepServices.EarthGeometry
{
	
	public interface IEarthGeometricService : IDeepService
	{
        /// <summary>
        /// Функция по определение центра масс
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        void CalcBarycenter(in GeometryArgs  geometryArgs, ref PointEarthGeometric pointResult);

        /// <summary>
        /// Функция по определению ближайшей точки контура
        /// </summary>
        /// <param name="geometryArgs"></param>
        /// <param name="pointResult"></param>
        void PutPointToContour(in PutPointToContourArgs geometryArgs, ref PointEarthGeometric pointResult);

        /// <summary>
        /// Функция по формированию контура от станции. 
        /// </summary>
        /// <param name="contourForStationByTriggerFieldStrengthsArgs"></param>
        /// <param name="pointResult"></param>
        /// <param name="sizeResultBuffer"></param>
        void CreateContourForStationByTriggerFieldStrengths(in ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer);

        /// <summary>
        /// Функция по формированию контура от точки
        /// </summary>
        /// <param name="contourFromPointByDistanceArgs"></param>
        /// <param name=""></param>
        /// <param name="pointResult"></param>
        /// <param name="sizeResultBuffer"></param>
        void CreateContourFromPointByDistance(in ContourFromPointByDistanceArgs contourFromPointByDistanceArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer);
    }
}
