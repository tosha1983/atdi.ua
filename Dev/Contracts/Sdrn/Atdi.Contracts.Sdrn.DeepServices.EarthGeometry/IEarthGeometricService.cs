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
        void CreateContourForStationByTriggerFieldStrengths(Func<PointEarthGeometric, PointEarthGeometric, double> calcFieldStrengths, in ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer);

        /// <summary>
        /// Функция по формированию контура от точки
        /// </summary>
        /// <param name="contourFromPointByDistanceArgs"></param>
        /// <param name=""></param>
        /// <param name="pointResult"></param>
        /// <param name="sizeResultBuffer"></param>
        void CreateContourFromPointByDistance(in ContourFromPointByDistanceArgs contourFromPointByDistanceArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer);

        /// <summary>
        /// Функция определения расстояния между двумя заданными точками
        /// </summary>
        /// <param name="sourcePointAgs"></param>
        /// <param name="targetPointArgs"></param>
        /// <param name="coordinateUnits"></param>
        /// <returns></returns>
        double GetDistance_km(in PointEarthGeometric sourcePointAgs, in PointEarthGeometric targetPointArgs);

        /// <summary>
        /// Определение азимута
        /// </summary>
        /// <param name="sourcePointAgs"></param>
        /// <param name="targetPointArgs"></param>
        /// <param name="coordinateUnits"></param>
        /// <returns></returns>
        double GetAzimut(in PointEarthGeometric sourcePointAgs, in PointEarthGeometric targetPointArgs);

        /// <summary>
        /// Функция по формированию контура от контура
        /// </summary>
        /// <param name="contourFromContureByDistanceArgs"></param>
        /// <param name="pointEarthGeometricWithAzimuth"></param>
        /// <param name="sizeResultBuffer"></param>
        void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer);


    }
}
