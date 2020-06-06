using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.Common
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
