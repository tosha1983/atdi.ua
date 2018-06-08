using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Atdi.LegacyServices.Icsm
{
    public enum type { tNull = 0, tSym = 1, tStr = 2, tFra = 3, tInt = 4, tDou = 5, tChr = 6, tTim = 7 };
    public enum OrmDataCoding { tvalNULL = 0, tvalNUMBER = 1, tvalSTRING = 2, tvalDATETIME = 3, tvalBINARY = 4, tvalGUID = 5 }
    public enum OrmOp { opNop = 0, opGt = 1, opGe = 2, opLt = 3, opLe = 4, opEq = 5, opNeq = 6, opLike = 7, opIn = 8, opNotIn = 9, opNotLike = 10, opNull = 11, opNotNull = 12 };
    public enum typSemant
    {
        tNorm = 0, tFreq = 1, tWatts = 2, tLongi = 3, tLati = 4, tNumber = 5, tDate = 6,
        tHour = 7, tDist = 8, tdBWatts = 9, tCurrency = 10, tBw = 11, tStri = 12, tdBm = 13,
        tM = 14, tdB = 15, tDeg = 16, tCombo = 17, tBool = 18, tKelvins = 19, tdBWpHz = 20,
        tPattern = 21, tAsl = 22, tAgl = 23, tEDeg = 24, tmVpm = 25, tComboNum = 26, tdBmuVm = 27,
        tComboUser = 28, tMinpDay = 29, tDesigEm = 30, tCsys = 31, tForeignId = 32,
        tInteger = 33, tMbitps = 34, tM2 = 35, tComboWrkf = 36, tmSm = 37, tdBkW = 38, tSecond = 39, tHour2 = 40,
        tPerc = 41, tDatMsec = 42, tListCombo = 43, tListComboUser = 44, tPolygonItu = 45,
        tTelsys = 46, tItuSrvc = 47, tTariff = 48, tKm2 = 49, tGuid = 50, tTons = 51, tComboUserNum = 52, tBinary = 53,
        tFolder = 54,
    };
    public enum Ordering { oNone = 0, oAsc = 1, oDesc = 2 };
    public enum QueryQsho { QshoNONE = 0, QshoSEL = 1, QshoCOL = 3 };

    public static class Utils
    {
        public static int NullI = 2147483647;
        public static double NullD = 1E-99;
        public static DateTime NullT = new DateTime(1, 1, 1, 0, 0, 0);


        public static bool IsNull(this DateTime dat)
        {
            return dat.Ticks == 0;
        }
        public static bool IsNull(this double dat)
        {
            return dat == 1e-99;
        }
        public static bool IsNull(this int dat)
        {
            return dat == 0x7FFFFFFF;
        }
        public static bool IsNull(this string dat)
        {
            return string.IsNullOrEmpty(dat);
        }
        public static bool IsNull(object x)
        {
            if (x == null) return false;
            if (x.ToString() == NullI.ToString()
                || x.ToString() == NullT.ToString()
                || x.ToString() == NullD.ToString()
                || x.ToString() == "" || x == null) { return true; }
            else { return false; }
        }
        public static string ToStringNull(this DateTime dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static string ToStringNull(this double dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static string ToStringNull(this int dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static string ToStringNull(this string dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static bool IsNotNull(this DateTime dat)
        {
            return dat.Ticks != 0;
        }
        public static bool IsNotNull(object x)
        {
            if (x.ToString() != NullI.ToString()
                && x.ToString() != NullT.ToString()
                && x.ToString() != NullD.ToString()
                && x.ToString() != "" && x != null) { return true; }
            else { return false; }
        }
        public static bool IsNotNull(this double dat)
        {
            return dat != 1e-99;
        }
        public static bool IsNotNull(this int dat)
        {
            return dat != 0x7FFFFFFF;
        }
        public static bool IsNotNull(this string dat)
        {
            return !string.IsNullOrEmpty(dat);
        }

        public static double ParseDouble(this string s)
        {
            double v;
            if (s == null || !double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out v)) v = NullD;
            return v;
        }

        public static int IndexFirstNonWhite(this string buff, int ifrm = 0)
        {
            if (string.IsNullOrEmpty(buff)) return 0;
            int ip = ifrm;
            while (ip < buff.Length && char.IsWhiteSpace(buff[ip])) ++ip;
            return ip;
        }


        static public bool ParseDouble(this string buf, ref int idx, out double val, bool allowThousandSeparator = true)
        {
            val = 1e-99;
            int ip = buf.IndexFirstNonWhite(idx);
            string cand = "";
            if (ip == buf.Length) return true;
            if (ip < buf.Length && buf[ip] == '-') { cand += '-'; ip = buf.IndexFirstNonWhite(ip + 1); }
            else if (ip < buf.Length && buf[ip] == '+') ip = buf.IndexFirstNonWhite(ip + 1);
            bool lastDig = false;
            num1:
            while (ip < buf.Length && char.IsDigit(buf[ip])) { cand += buf[ip]; lastDig = true; ip++; }
            if (allowThousandSeparator && lastDig && ip + 3 < buf.Length && buf[ip] == ' '
                && char.IsDigit(buf[ip + 1]) && char.IsDigit(buf[ip + 2]) && char.IsDigit(buf[ip + 3])
                && (ip + 4 == buf.Length || !char.IsDigit(buf[ip + 4]))) { ++ip; goto num1; }  //skip white space separator of thousands
            if (ip < buf.Length && (buf[ip] == ',' || buf[ip] == '.'))
            {
                cand += '.'; ip++;
                while (ip < buf.Length && char.IsDigit(buf[ip])) { cand += buf[ip]; lastDig = true; ip++; }
            }
            if (!lastDig) return false;
            ip = buf.IndexFirstNonWhite(ip);
            if (ip < buf.Length && (buf[ip] == 'e' || buf[ip] == 'E'))
            {
                int ip2 = buf.IndexFirstNonWhite(ip + 1);
                if (ip2 < buf.Length && (buf[ip2] == '-' || buf[ip2] == '+' || char.IsDigit(buf[ip2])))
                {
                    cand += 'e'; ip = ip2;
                    if (ip < buf.Length && buf[ip] == '-') { cand += '-'; ip = buf.IndexFirstNonWhite(ip + 1); }
                    else if (ip < buf.Length && buf[ip] == '+') ip = buf.IndexFirstNonWhite(ip + 1);
                    if (ip == buf.Length || !char.IsDigit(buf[ip])) return false;
                    while (ip < buf.Length && char.IsDigit(buf[ip])) { cand += buf[ip]; ip++; }
                }
            }
            try { val = Convert.ToDouble(cand, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS")); idx = ip; return true; }
            catch { return false; }
        }


        public static Encoding GetFileEncoding(string srcFile)
        {
            Encoding enc = Encoding.Default; //Ansi codepage!
            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            try
            {
                FileStream file = new FileStream(srcFile, FileMode.Open);
                file.Read(buffer, 0, 5);
                file.Close();
                if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                    enc = Encoding.UTF8;
                else if (buffer[0] == 0xff && buffer[1] == 0xfe)
                    enc = Encoding.Unicode;
                else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                    enc = Encoding.UTF32;
                else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                    enc = Encoding.UTF7;
            }
            catch (Exception) { }
            return enc;
        }
    }
}
