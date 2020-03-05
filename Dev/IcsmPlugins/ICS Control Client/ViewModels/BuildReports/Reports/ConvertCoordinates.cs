using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using XICSM.ICSControlClient.ViewModels.Coordinates;

namespace XICSM.ICSControlClient.ViewModels.Reports
{
    public class ConvertCoordinates
    {
        public static string DecToDmsToString(double coordDec, EnumCoordLine line)
        {
            double coord = IMPosition.Dec2Dms(coordDec);

            if (coord == IM.NullD)
                return "";

            Int64 coordInt = (Int64)((coord + 0.000005) * 100000.0);
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
                seconds = tmp.ToString(" 00");
            }
            seconds += Convert.ToString(symbol[1]) + Convert.ToString(symbol[1]);
            // Минуты
            coordInt = coordInt / 1000;
            string minutes = (coordInt % 100).ToString(" 00") + Convert.ToString(symbol[1]);
            // Градусы
            coordInt = coordInt / 100;
            string degree = (coordInt % 100).ToString("00") + Convert.ToString(symbol[0]);
            return degree + minutes + seconds;
        }
    }
}
