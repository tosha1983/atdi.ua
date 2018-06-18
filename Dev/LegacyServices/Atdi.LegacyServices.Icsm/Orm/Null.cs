using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public static class Null
    {
        public const int I = 2147483647;
        public const float F = 9.999999E-39f;
        public const double D = 1E-99;
        public static DateTime T = new DateTime(0L);
        //public static char Lower(this char c)
        //{
        //    if (c >= 'a' && c <= 'z')
        //    {
        //        return c;
        //    }
        //    if (c >= 'A' && c <= 'Z')
        //    {
        //        return c + 'a' - 'A';
        //    }
        //    return char.ToLower(c);
        //}
        //public static char Upper(this char c)
        //{
        //    if (c >= 'A' && c <= 'Z')
        //    {
        //        return c;
        //    }
        //    if (c >= 'a' && c <= 'z')
        //    {
        //        return c + 'A' - 'a';
        //    }
        //    return char.ToUpper(c);
        //}
        public static double Double(this int d)
        {
            if (d != 2147483647)
            {
                return (double)d;
            }
            return 1E-99;
        }
        public static int Int(this double d)
        {
            if (d == 1E-99)
            {
                return 2147483647;
            }
            return (int)Math.Round(d, MidpointRounding.AwayFromZero);
        }
        public static string String(this double d)
        {
            if (d == 1E-99)
            {
                return "";
            }
            return d.ToString(IcsmOrmExtensitions.CultureEnUs);
        }
        public static string ToSql(this double d)
        {
            if (d == 1E-99)
            {
                return "NULL";
            }
            return d.ToString(IcsmOrmExtensitions.CultureEnUs);
        }
        public static string String(this int d)
        {
            if (d == 2147483647)
            {
                return "";
            }
            return d.ToString();
        }
        public static string ToSql(this int d)
        {
            if (d == 2147483647)
            {
                return "NULL";
            }
            return d.ToString();
        }
        public static string ToSql(this string s)
        {
            if (s == null)
            {
                return "NULL";
            }
            return "'" + s.Replace("'", "''") + "'";
        }
        //public static int ParseInt(this string s)
        //{
        //    int result;
        //    if (s == null || !int.TryParse(s, out result))
        //    {
        //        result = 2147483647;
        //    }
        //    return result;
        //}
        public static int Default(this int v, int def)
        {
            if (v != 2147483647)
            {
                return v;
            }
            return def;
        }
        public static double Default(this double v, double def)
        {
            if (v != 1E-99)
            {
                return v;
            }
            return def;
        }
        public static string Default(this string v, string def)
        {
            if (!string.IsNullOrEmpty(v))
            {
                return v;
            }
            return def;
        }
        public static double AddToMinMax(this double v, ref double min, ref double max)
        {
            if (v != 1E-99)
            {
                if (min == 1E-99 || min > v)
                {
                    min = v;
                }
                if (max == 1E-99 || max < v)
                {
                    max = v;
                }
            }
            return v;
        }
        //public static double ParseDouble(this string s)
        //{
        //    double result;
        //    if (s == null || !double.TryParse(s, NumberStyles.Float, IcsmOrmExtensitions.CultureEnUs, out result))
        //    {
        //        result = 1E-99;
        //    }
        //    return result;
        //}
        //public static bool IsNull(this DateTime dat)
        //{
        //    return dat.Ticks == 0L;
        //}
        public static bool IsNull(this double dat)
        {
            return dat == 1E-99;
        }
        public static bool IsNull(this int dat)
        {
            return dat == 2147483647;
        }
        public static bool IsNull(this string dat)
        {
            return string.IsNullOrEmpty(dat);
        }
        public static string ToStringNull(this DateTime dat)
        {
            if (dat.IsNull())
            {
                return "";
            }
            return dat.ToString();
        }
        public static string ToStringNull(this double dat)
        {
            if (dat.IsNull())
            {
                return "";
            }
            return dat.ToString();
        }
        public static string ToStringNull(this int dat)
        {
            if (dat.IsNull())
            {
                return "";
            }
            return dat.ToString();
        }
        public static string ToStringNull(this string dat)
        {
            if (dat.IsNull())
            {
                return "";
            }
            return dat.ToString();
        }
        public static bool IsNotNull(this DateTime dat)
        {
            return dat.Ticks != 0L;
        }
        public static bool IsNotNull(this double dat)
        {
            return dat != 1E-99;
        }
        public static bool IsNotNull(this int dat)
        {
            return dat != 2147483647;
        }
        public static bool IsNotNull(this string dat)
        {
            return !string.IsNullOrEmpty(dat);
        }
        public static List<string> NE(this List<string> lst)
        {
            if (lst != null)
            {
                return lst;
            }
            return new List<string>(0);
        }
        public static double DmsToDec(this double val)
        {
            if (val == 1E-99)
            {
                return val;
            }
            double num = val;
            bool flag = num < 0.0;
            if (flag)
            {
                num = -num;
            }
            int num2 = (int)num;
            num = (num - (double)num2) * 100.0;
            if (num >= 99.999)
            {
                num2++;
                num = 0.0;
            }
            int num3 = (int)num;
            num = (num - (double)num3) * 100.0;
            if (num >= 99.9)
            {
                num3++;
                num = 0.0;
            }
            if (num3 == 60)
            {
                num3 = 0;
                num2++;
            }
            int num4 = (int)num;
            num -= (double)num4;
            if (num >= 0.99)
            {
                num4++;
                num = 0.0;
            }
            if (num4 == 60)
            {
                num3++;
                num4 = 0;
                if (num < 0.01)
                {
                    num = 0.0;
                }
            }
            if (num3 == 60)
            {
                num3 = 0;
                num2++;
            }
            if (num3 > 60 || num4 > 60)
            {
                return 1E-99;
            }
            num = (double)num2 + (double)num3 / 60.0 + ((double)num4 + num) / 3600.0;
            if (flag)
            {
                num = -num;
            }
            return num;
        }
        public static bool CalcAzimuthDistance(out double distKm, out double aDeg, double ax, double ay, double bx, double by)
        {
            if (ax == 1E-99 || ay == 1E-99 || bx == 1E-99 || by == 1E-99)
            {
                distKm = 1E-99;
                aDeg = 1E-99;
                return false;
            }
            double num = ay * 3.1415926535897931 / 180.0;
            double num2 = ax * 3.1415926535897931 / 180.0;
            double num3 = by * 3.1415926535897931 / 180.0;
            double num4 = bx * 3.1415926535897931 / 180.0;
            double num5 = Math.Sin(num) * Math.Sin(num3) + Math.Cos(num) * Math.Cos(num3) * Math.Cos(num4 - num2);
            double num6;
            if (num5 >= 1.0)
            {
                num6 = 0.0;
            }
            else if (num5 <= -1.0)
            {
                num6 = 3.1415926535897931;
            }
            else
            {
                num6 = Math.Acos(num5);
            }
            distKm = num6 * 6373.0;
            double num7;
            if (Math.Abs(num4 - num2) < 1E-08)
            {
                num7 = ((num >= num3) ? 3.1415926535897931 : 0.0);
            }
            else
            {
                double num8 = (Math.Sin(num3) - Math.Sin(num) * Math.Cos(num6)) / (Math.Cos(num) * Math.Sin(num6));
                if (num8 >= 1.0)
                {
                    num7 = 0.0;
                }
                else if (num8 <= -1.0)
                {
                    num7 = 3.1415926535897931;
                }
                else
                {
                    num7 = Math.Acos(num8);
                }
            }
            if (Math.Abs(num2 - num4) < 3.1415926535897931)
            {
                if (num4 < num2)
                {
                    num7 = 6.2831853071795862 - num7;
                }
            }
            else if (num4 >= num2)
            {
                num7 = 6.2831853071795862 - num7;
            }
            aDeg = num7 * 180.0 / 3.1415926535897931;
            return true;
        }
    }
}
