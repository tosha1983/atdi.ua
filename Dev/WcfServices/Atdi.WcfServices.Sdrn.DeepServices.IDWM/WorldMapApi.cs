using Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;


namespace Atdi.WcfServices.Sdrn.DeepServices.IDWM
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WorldMapApi : WcfServiceBase<IWorldMapApi>, IWorldMapApi
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddDllDirectory(string lpPathName);

        private static IdwmNET.Idwm _idwmVal;
        private static bool _isIdwmInit;
        public WorldMapApi()
        {
            try
            {
                if (!_isIdwmInit)
                {
                    string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
                    var executingDirectory = Path.GetDirectoryName(executingAssemblyFile);
                    AddDllDirectory(executingDirectory);

                    // инициализация библиотеки Idwm
                    _idwmVal = new IdwmNET.Idwm();
                    _isIdwmInit = _idwmVal.Init(11);
                }
            }
            catch (Exception)
            {
                _isIdwmInit = false;
            }
        }



        /// <summary>
        /// Функция по определению ближайшей точки искомой администрации от заданной точки
        /// </summary>
        /// <param name="pointByADM"></param>
        /// <returns></returns>
        public Point GetNearestPointByADM(PointByADM pointByADM)
        {
            var resultPoint = new Point();
            string administration = pointByADM.Administration;
            if (_isIdwmInit)
            {
                var nearestCountries = _idwmVal.GetNearestCountries(IdwmNET.Idwm.DecToRadian((float)pointByADM.Longitude), IdwmNET.Idwm.DecToRadian((float)pointByADM.Latitude), 100000, new string[] { }, 1000);
                if ((nearestCountries != null) && (nearestCountries.Length > 0))
                {
                    var findNearestCountries = nearestCountries.ToList().Find(x => x.country == administration);
                    if (findNearestCountries != null)
                    {
                        resultPoint.Longitude = ((float)((findNearestCountries.rLongitude * 180) / Math.PI));
                        resultPoint.Latitude = ((float)((findNearestCountries.rLatitude * 180) / Math.PI));
                    }
                }
            }
            return resultPoint;
        }


        /// <summary>
        /// Функция по определению администрации по точке
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public string GetADMByPoint(Point point)
        {
            if (_isIdwmInit)
            {
                return _idwmVal.GetCountry(IdwmNET.Idwm.DecToRadian((float)point.Longitude), IdwmNET.Idwm.DecToRadian((float)point.Latitude));
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
        private Point GetCoordByMinDistance(PointByADM pointByADM)
        {
            var resultPoint = new Point();
            string administration = pointByADM.Administration;
            if (_isIdwmInit)
            {
                var nearestCountries = _idwmVal.GetNearestCountries(IdwmNET.Idwm.DecToRadian((float)pointByADM.Longitude), IdwmNET.Idwm.DecToRadian((float)pointByADM.Latitude), 100000, new string[] { }, 1000);
                if ((nearestCountries != null) && (nearestCountries.Length > 0))
                {
                    var findNearestCountries = nearestCountries.ToList().Find(x => x.country == administration);
                    if (findNearestCountries != null)
                    {
                        resultPoint.Longitude = ((float)((findNearestCountries.rLongitude * 180) / Math.PI));
                        resultPoint.Latitude = ((float)((findNearestCountries.rLatitude * 180) / Math.PI));
                    }
                }
            }
            return resultPoint;
        }

        /// <summary>
        /// Определяем все администрации, которые попали в соответствующий радиус от точки. 
        /// </summary>
        /// <param name="pointAndDistance"></param>
        /// <param name="administrationsResult"></param>
        /// <param name="SizeBuffer"></param>
        public AdministrationsResult[] GetADMByPointAndDistance( PointAndDistance pointAndDistance)
        {
            var administrationsResult = new List<AdministrationsResult>();
            if (_isIdwmInit)
            {
                var nearesCountries = _idwmVal.GetNearestCountries(IdwmNET.Idwm.DecToRadian((float)pointAndDistance.Longitude), IdwmNET.Idwm.DecToRadian((float)pointAndDistance.Latitude), pointAndDistance.Distance, new string[] { }, 1000);
                if ((nearesCountries != null) && (nearesCountries.Length > 0))
                {
                    for (int i = 0; i < nearesCountries.Length; i++)
                    {
                        administrationsResult.Add(new AdministrationsResult()
                        {
                            Point = new Point()
                            {
                                Longitude = ((float)((nearesCountries[i].rLongitude * 180) / Math.PI)),
                                Latitude = ((float)((nearesCountries[i].rLatitude * 180) / Math.PI))
                            },
                            Administration = nearesCountries[i].country,
                            Azimuth = nearesCountries[i].azimuth,
                            Distance = nearesCountries[i].distance

                        });
                    }

                }
            }
            return administrationsResult.ToArray();
        }
    }
}
