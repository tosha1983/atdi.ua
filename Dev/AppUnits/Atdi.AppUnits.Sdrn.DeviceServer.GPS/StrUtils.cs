using System;
using System.Globalization;
using System.IO;

namespace Atdi.AppUnits.Sdrn.DeviceServer.GPS
{
    // Code by Aleksandr Dikarev, dikarev-aleksandr@yandex.ru

    public static class StrUtils
    {
        #region Methods

        public static string TrimStrToStComma(string source, int maxLen, int lenToTrim)
        {
            if (lenToTrim > maxLen)
                throw new ArgumentException("lenToTrim must be less or equal to maxLen");

            if (source.Length > maxLen)
            {
                int commaIdx = source.IndexOf(',');

                if (commaIdx > lenToTrim)
                    return source.Substring(0, lenToTrim);
                else if (commaIdx < 0)
                    return source.Substring(0, lenToTrim);
                else
                    return source.Substring(0, commaIdx);
            }
            else
                return source;
        }

        public static string BCDVersionToStr(int versionData)
        {
            return string.Format("{0}.{1}", (versionData >> 0x08).ToString(), (versionData & 0xff).ToString("X2"));
        }

        public static string GetFileNameInOwnPath(string ownPath, string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(ownPath), fileName);
        }

        public static string GetExecutableFileNameWithNewExt(string executableFileName, string newExt)
        {
            return Path.ChangeExtension(executableFileName, newExt);
        }

        public static string AngleToString(double angle)
        {
            int degree = (int)Math.Floor(angle);
            int minutes = (int)Math.Floor((angle - degree) * 60.0);
            double seconds = (angle - degree) * 3600 - minutes * 60.0;

            return string.Format(CultureInfo.InvariantCulture, "{0}°{1}\'{2:F04}\"", degree, minutes, seconds);
        }

        #endregion
    }
}
