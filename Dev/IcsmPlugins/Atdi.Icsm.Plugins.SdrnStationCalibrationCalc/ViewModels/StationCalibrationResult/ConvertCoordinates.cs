using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using MP = Atdi.WpfControls.EntityOrm.Maps;
using System.Data;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Adapters;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using System.ComponentModel;



namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    public static class ConvertCoordinates
    {
        public static string DecToDmsToString(double coordDms, EnumCoordLine line)
        {
            Int64 coordInt = (Int64)((coordDms + 0.000005) * 100000.0);
            Char[] symbol = new Char[] { '\xB0', '\x27' };
            bool isNegative = false;
            if (coordInt < 0)
            {// Меняем знак
                isNegative = true;
                coordInt = -coordInt;
            }
            // Секунды
            Int64 sec = coordInt % 1000;
            string seconds = "";
            if ((sec % 10) != 0)
            {
                double tmp = (double)sec / 10.0;
                seconds = tmp.ToString(" 00.0");
            }
            else
            {
                double tmp = (double)sec / 10.0;
                seconds = tmp.ToString(" 00.0");
            }
            seconds += Convert.ToString(symbol[1]) + Convert.ToString(symbol[1]);
            // Минуты
            coordInt = coordInt / 1000;
            string minutes = (coordInt % 100).ToString(" 00") + Convert.ToString(symbol[1]);
            // Градусы
            coordInt = coordInt / 100;
            string degree = (coordInt % 100).ToString("00") + Convert.ToString(symbol[0]);

            var outStr = degree + minutes + seconds + " ";
            if (line == EnumCoordLine.Lon)
            {
                outStr += (isNegative) ? "W" : "E";
            }
            else
            {
                outStr += (isNegative) ? "S" : "N";
            }
            return outStr;
        }
    }

    public enum EnumCoordLine
    {
        Lon = 1,   //долгота
        Lat = 2    //широта
    };
}
