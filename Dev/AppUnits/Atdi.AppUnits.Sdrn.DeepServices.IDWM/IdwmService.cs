using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using IdwmNET;
using System.Linq;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace Atdi.AppUnits.Sdrn.DeepServices.IDWM
{
    public class IdwmService : IIdwmService
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddDllDirectory(string lpPathName);

        private static IdwmNET.Idwm _idwmVal;
        private static bool _isIdwmInit;
        public IdwmService()
        {
            try
            {
                string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
                var executingDirectory = Path.GetDirectoryName(executingAssemblyFile);
                AddDllDirectory(executingDirectory);

                // инициализация библиотеки Idwm
                _idwmVal = new IdwmNET.Idwm();
                _isIdwmInit = _idwmVal.Init(11);
            }
            catch (Exception)
            {
                _isIdwmInit = false;
            }
        }

        /// <summary>
        /// Функция по определению администрации по точке
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public string GetADMByPoint(in Point point)
        {
            if (_isIdwmInit)
            {
                return _idwmVal.GetCountry(IdwmNET.Idwm.DecToRadian((float)point.Longitude_dec), IdwmNET.Idwm.DecToRadian((float)point.Latitude_dec));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        private void GetCoordByMinDistance(in PointByADM pointByADM, ref Point resultPoint)
        {
            string administration = pointByADM.Administration;
            if (_isIdwmInit)
            {
                var nearestCountries = _idwmVal.GetNearestCountries(IdwmNET.Idwm.DecToRadian((float)pointByADM.Longitude_dec), IdwmNET.Idwm.DecToRadian((float)pointByADM.Latitude_dec), 100000, new string[] { }, 1000);
                if ((nearestCountries != null) && (nearestCountries.Length > 0))
                {
                    var findNearestCountries = nearestCountries.ToList().Find(x => x.country == administration);
                    if (findNearestCountries != null)
                    {
                        resultPoint.Longitude_dec = ((float)((findNearestCountries.rLongitude * 180) / Math.PI));
                        resultPoint.Latitude_dec = ((float)((findNearestCountries.rLatitude * 180) / Math.PI));
                    }
                }
            }
        }

        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        public void GetNearestPointByADM(in PointByADM pointByADM, ref Point resultPoint)
        {
             GetCoordByMinDistance(in pointByADM, ref resultPoint);
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
            if (_isIdwmInit)
            {
                var nearesCountries = _idwmVal.GetNearestCountries(IdwmNET.Idwm.DecToRadian((float)pointAndDistance.Longitude_dec), IdwmNET.Idwm.DecToRadian((float)pointAndDistance.Latitude_dec), pointAndDistance.Distance, new string[] { }, 1000);
                if ((nearesCountries != null) && (nearesCountries.Length > 0))
                {
                    for (int i = 0; i < nearesCountries.Length; i++)
                    {
                        administrationsResult[i].Administration = nearesCountries[i].country;
                        administrationsResult[i].Azimuth = nearesCountries[i].azimuth;
                        administrationsResult[i].Distance = nearesCountries[i].distance;
                        administrationsResult[i].Point.Longitude_dec = ((float)((nearesCountries[i].rLongitude * 180) / Math.PI));
                        administrationsResult[i].Point.Latitude_dec = ((float)((nearesCountries[i].rLatitude * 180) / Math.PI));
                    }
                    sizeResultBuffer = nearesCountries.Length;
                }
            }
        }

        public void Dispose()
        {

        }

    }
}
