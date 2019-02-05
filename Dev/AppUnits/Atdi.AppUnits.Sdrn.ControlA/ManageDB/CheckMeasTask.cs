using System;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppUnits.Sdrn.ControlA;

namespace Atdi.AppUnits.Sdrn.ControlA.ManageDB
{
    /// <summary>
    /// Класс для валидации тасков.
    /// </summary>
    public class CheckMeasTask
    {
        // Проверка на координаты (пока что заглушка)
        public static bool CheckCoordinate(NH_MeasTaskSDR M, double LAT, double LON, double ASL)
        {
            bool isCheck = true;

            return isCheck;
        }

        /// <summary>
        /// Проверка на совместимость параметров оборудования (пока что заглушка)
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static bool CheckTechnicalParams(NH_MeasTaskSDR M)
        {
            bool isCheck = true;

            return isCheck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static bool CheckValidate(MeasSdrTask M)
        {
            bool Check = true;
            //if ((M.status == "A") || (M.status == "O") || (M.status == "P") || (M.status == "E_L"))
            {
                if (M.Time_stop < DateTime.Now)
                {
                    Check = false;
                    //M.status = "E_T";
                }
                else
                {
                    Check = true;
                }
            }
            //else Check = false;
            return Check;
        }
    }

}
