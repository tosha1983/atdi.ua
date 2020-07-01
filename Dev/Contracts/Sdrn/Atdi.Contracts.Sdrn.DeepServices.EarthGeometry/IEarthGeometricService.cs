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
        void CalcBarycenter(in GeometryArgs geometryArgs, ref PointEarthGeometric pointResult);

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
        void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometric[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer);


        /// <summary>
        /// Рассчитать точку по заданной дистанции, азимуту и начальной точке
        /// </summary>
        /// <param name="PointStart"></param>
        /// <param name="distance_km"></param>
        /// <param name="azimuth"></param>
        /// <param name="LargeCircleArc"></param>
        /// <returns></returns>
        PointEarthGeometric CalculationCoordinateByLengthAndAzimuth(in PointEarthGeometric PointStart, double distance_km, double azimuth, bool LargeCircleArc = true);


        /// <summary>
        /// Проверка попадания точки point в контур poligon
        /// </summary>
        /// <param name="poligon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        bool CheckHitting(in CheckHittingArgs checkHittingArgs);
    }
        
}
