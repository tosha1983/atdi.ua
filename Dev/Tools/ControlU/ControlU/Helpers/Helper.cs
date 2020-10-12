using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace ControlU.Helpers
{
    public class Helper
    {
        static Dictionary<char, string> translit = new Dictionary<char, string>()
#region
        {
            {'а', "a"},
            {'б', "b"},
            {'в', "v"},
            {'г', "g"},
            {'д', "d"},
{'е', "e"},
{'ё', "yo"},
{'є', "ie"},
{'ж', "zh"},
{'з', "z"},
{'и', "i"},
            {'і', "i"},
            {'ї', "i"},
{'й', "j"},
{'к', "k"},
{'л', "l"},
{'м', "m"},
{'н', "n"},
{'о', "o"},
{'п', "p"},
{'р', "r"},
{'с', "s"},
{'т', "t"},
{'у', "u"},
{'ф', "f"},
{'х', "h"},
{'ц', "c"},
{'ч', "ch"},
{'ш', "sh"},
{'щ', "sch"},
{'ъ', "j"},
{'ы', "i"},
{'ь', "j"},
{'э', "e"},
{'ю', "yu"},
{'я', "ya"},
{'А', "A"},
{'Б', "B"},
{'В', "V"},
{'Г', "G"},
{'Д', "D"},
{'Е', "E"},
{'Ё', "Yo"},
{'Є', "Ie"},
            {'Ж', "Zh"},
{'З', "Z"},
{'И', "I"},
{'І', "I"},
{'Ї', "I"},
{'Й', "J"},
{'К', "K"},
{'Л', "L"},
{'М', "M"},
{'Н', "N"},
{'О', "O"},
{'П', "P"},
{'Р', "R"},
{'С', "S"},
{'Т', "T"},
{'У', "U"},
{'Ф', "F"},
{'Х', "H"},
{'Ц', "C"},
{'Ч', "Ch"},
{'Ш', "Sh"},
{'Щ', "Sch"},
{'Ъ', "J"},
{'Ы', "I"},
{'Ь', "J"},
{'Э', "E"},
{'Ю', "Yu"},
{'Я', "Ya"}
        }; System.Timers.Timer Tmr;
        #endregion
        string ToTranslit(char c)
        {
            string result;
            if (translit.TryGetValue(c, out result))
                return result;
            else
                return c.ToString();

        }
        public void TimerStart()
        {
            Load();
            Tmr = new System.Timers.Timer(60000);
            Tmr.AutoReset = true;
            Tmr.Elapsed += Auto;
            Tmr.Enabled = true;
            Tmr.Start();
        }
        private void Auto(object sender, System.Timers.ElapsedEventArgs e)
        {
            Load();
        }
        public string ToTranslit(string src)
        {
            string outstr = string.Empty;
            foreach (char c in src)
            {
                outstr += ToTranslit(c);
            }
            return outstr;
        }

        public string LeaveOnlyLetters(string str)
        {
            return Regex.Replace(str, @"[^a-zA-Z-]", string.Empty);
        }
        public string LeaveOnlyDec(string str)
        {
            return new String(str.Where(Char.IsDigit).ToArray()); //Regex.Replace(str, @"[^a-zA-Z-]", string.Empty);
        }
        public void CheckIsInt(TextBox sender, TextCompositionEventArgs e)
        {
            int result;
            if (!int.TryParse(e.Text, out result))
            {
                e.Handled = true;
            }
        }
        public void CheckIsDouble(TextBox sender, TextCompositionEventArgs e)
        {
            double result;
            bool dot = (sender.Text.IndexOf(".") + sender.Text.IndexOf(",")) < 0 && (e.Text.Equals(".") || e.Text.Equals(",")) && sender.Text.Length > 0;
            if (!(double.TryParse(e.Text, out result) || dot))
            {
                e.Handled = true;
            }
        }
        public void CheckIsDoubleWithMinus(TextBox sender, TextCompositionEventArgs e)
        {
            double result;
            bool dot = (sender.Text.IndexOf(".") + sender.Text.IndexOf(",")) < 0 && (e.Text.Equals(".") || e.Text.Equals(",")) && sender.Text.Length > 0;
            bool minus = (e.Text.Equals("-") && sender.Text.IndexOf('-') < 0);
            if (!(double.TryParse(e.Text, out result) || dot || minus))
            {
                e.Handled = true;
            }
        }
        public void CheckIsDecimal(TextBox sender, TextCompositionEventArgs e)
        {
            decimal result;
            string str = sender.Text;
            bool dot = (str.IndexOf(".") + str.IndexOf(",")) < 0 && (e.Text.Equals(".") || e.Text.Equals(",")) && sender.Text.Length > 0;
            if (!(Decimal.TryParse(e.Text, out result) || dot))
            {
                e.Handled = true;
            }
        }
        public void CheckIsDecimalWithMinus(TextBox sender, TextCompositionEventArgs e)
        {
            decimal result;
            bool dot = (sender.Text.IndexOf(".") + sender.Text.IndexOf(",")) < 0 && (e.Text.Equals(".") || e.Text.Equals(",")) && sender.Text.Length > 0;
            bool minus = (e.Text.Equals("-") && sender.Text.IndexOf('-') < 0);
            if (!(decimal.TryParse(e.Text, out result) || dot || minus))
            {
                e.Handled = true;
            }
        }

        public int KeyDownIntWithMinus(int PreviousValue, TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            string temp = (sender as System.Windows.Controls.TextBox).Text;
            string t = string.Empty;
            foreach (char c in temp)
            {
                if (Char.IsDigit(c) || c == '-')
                {
                    t += c;
                }
            }
            //CultureInfo culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
            int d;
            if (int.TryParse(t.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                PreviousValue = int.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
            return PreviousValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PreviousValue"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>positive int</returns>
        public int KeyDownUInt(int PreviousValue, TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            string temp = (sender as System.Windows.Controls.TextBox).Text;
            string t = string.Empty;
            foreach (char c in temp)
            {
                if (Char.IsDigit(c))
                {
                    t += c;
                }
            }
            //CultureInfo culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
            int d;
            if (int.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                PreviousValue = int.Parse(t, CultureInfo.InvariantCulture);
            return PreviousValue;
        }
        public double KeyDownDouble(double PreviousValue, TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            string temp = (sender as System.Windows.Controls.TextBox).Text;
            string t = string.Empty;
            foreach (char c in temp)
            {
                if (Char.IsDigit(c) || c == '.' || c == ',' || c == '-')
                {
                    t += c;
                }
            }
            //CultureInfo culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
            double d;
            if (double.TryParse(t.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                PreviousValue = double.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
            return PreviousValue;
        }
        public decimal KeyDownDecimal(decimal PreviousValue, TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            string temp = (sender as System.Windows.Controls.TextBox).Text;
            string t = string.Empty;
            foreach (char c in temp)
            {
                if (Char.IsDigit(c) || c == '.' || c == ',' || c == '-')
                {
                    t += c;
                }
            }
            //CultureInfo culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
            decimal d;
            if (decimal.TryParse(t.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                PreviousValue = decimal.Parse(t.Replace(",", "."), CultureInfo.InvariantCulture);
            return PreviousValue;
        }
        /// <summary>
        /// Возвращает 1 или -1 при прокрутке скрола
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public int MouseWheelOutStep(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            int outstep = 0;
            if (e.Delta > 0) outstep = 1;
            else if (e.Delta < 0) outstep = -1;
            return outstep;
        }
        #region расчет координат
        public void calcDistance(double Lat1, double Lon1, double Lat2, double Lon2, out double distance, out double angledeg)
        {
            int rad = 6372795; // 6378137;// 6372795; // радиус земли

            //получение координат точек в радианах
            double lat1r = Lat1 * Math.PI / 180; //широта
            double lat2r = Lat2 * Math.PI / 180; //широта
            double lon1r = Lon1 * Math.PI / 180; //долгота
            double lon2r = Lon2 * Math.PI / 180; //долгота

            //косинусы и синусы широт и разницы долгот
            double cl1 = Math.Cos(lat1r);
            double cl2 = Math.Cos(lat2r);
            double sl1 = Math.Sin(lat1r);
            double sl2 = Math.Sin(lat2r);
            double delta = lon2r - lon1r;
            double cdelta = Math.Cos(delta);
            double sdelta = Math.Sin(delta);

            //вычисления длины большого круга
            double yd = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            double xd = sl1 * sl2 + cl1 * cl2 * cdelta;
            double ad = Math.Atan2(yd, xd);
            distance = ad * rad;

            //вычисление начального азимута
            double xa = (cl1 * sl2) - (sl1 * cl2 * cdelta);
            double ya = sdelta * cl2;
            double za = Math.Atan(-ya / xa) * 180 / Math.PI;

            if (xa < 0)
            {
                za = za + 180;
            }
            double za2 = (za + 180) % 360 - 180;
            za2 = -za2 * Math.PI / 180;
            double anglerad2 = za2 - ((2 * Math.PI) * Math.Floor((za2 / (2 * Math.PI))));
            angledeg = (anglerad2 * 180) / Math.PI;
        }
        public System.Windows.Point calculateEndPoint(System.Windows.Point x, double azi, double dist)
        {
            int rad = 6372795;
            double azi2 = (azi + 270) * Math.PI / 180;
            System.Windows.Point x2 = new System.Windows.Point(x.X * Math.PI / 180, x.Y * Math.PI / 180);
            dist = dist / rad;


            double[] pt = new double[2];
            double[] z = new double[3];
            double[] pt2 = new double[2];

            pt[0] = Math.PI / 2 - dist;
            pt[1] = Math.PI - azi2;
            ShperToCart(pt, z);
            Rotate(z, x2.X - Math.PI / 2, 1);
            Rotate(z, -1 * x2.Y, 2);
            CartToSphere(z, pt2);
            System.Windows.Point y = new System.Windows.Point(pt2[0] * 180 / Math.PI, pt2[1] * 180 / Math.PI);
            return y;
        }
        private void ShperToCart(double[] y, double[] x)
        {
            double p = Math.Cos(y[0]);
            x[2] = Math.Sin(y[0]);
            x[1] = Math.Sin(y[1]) * p;
            x[0] = Math.Cos(y[1]) * p;
        }
        private void Rotate(double[] x, double a, int i)
        {
            double c, s, xj;
            int j, k;
            j = (i + 1) % 3;
            k = (i - 1) % 3;
            c = Math.Cos(a);
            s = Math.Sin(a);
            xj = x[j] * c + x[k] * s;
            x[k] = -1 * x[j] * s + x[k] * c;
            x[j] = xj;
        }
        private double CartToSphere(double[] x, double[] y)
        {
            double p;
            p = Math.Sqrt(x[0] * x[0] + x[1] * x[1]);
            y[1] = Math.Atan2(x[1], x[0]);
            y[0] = Math.Atan2(x[2], p);
            return Math.Sqrt(p * p + x[2] * x[2]);
        }
        #endregion

        //public System.Windows.Point calculateEndPoint(System.Windows.Point x, double azi, double dist)
        //{
        //    int rad = 6372795;
        //    azi = azi * Math.PI / 180;
        //    dist = dist / rad;
        //    System.Windows.Point x2 = new System.Windows.Point(x.X * Math.PI / 180, x.Y * Math.PI / 180);

        //    double[] pt = new double[2];
        //    double[] z = new double[3];
        //    double[] pt2 = new double[2];

        //    pt[0] = Math.PI / 2 - dist;
        //    pt[1] = Math.PI - azi;
        //    ShperToCart(pt, z);
        //    Rotate(z, x2.X - Math.PI / 2, 1);
        //    Rotate(z, -1 * x2.Y, 2);
        //    CartToSphere(z, pt2);
        //    System.Windows.Point y = new System.Windows.Point(pt2[0] * 180 / Math.PI, pt2[1] * 180 / Math.PI);
        //    return y;
        //}
        //private void ShperToCart(double[] y, double[] x)
        //{
        //    double p = Math.Cos(y[0]);
        //    x[2] = Math.Sin(y[0]);
        //    x[1] = Math.Sin(y[1]) * p;
        //    x[0] = Math.Cos(y[1]) * p;
        //}
        //private void Rotate(double[] x, double a, int i)
        //{
        //    double c, s, xj;
        //    int j, k;
        //    j = (i + 1) % 3;
        //    k = (i - 1) % 3;
        //    c = Math.Cos(a);
        //    s = Math.Sin(a);
        //    xj = x[j] * c + x[k] * s;
        //    x[k] = -1 * x[j] * s + x[k] * c;
        //    x[j] = xj;
        //}
        //private double CartToSphere(double[] x, double[] y)
        //{
        //    double p;
        //    p = Math.Sqrt(x[0] * x[0] + x[1] * x[1]);
        //    y[1] = Math.Atan2(x[1], x[0]);
        //    y[0] = Math.Atan2(x[2], p);
        //    return Math.Sqrt(p * p + x[2] * x[2]);
        //}
        public double MAP(double x, double inMin, double inMax, double outMin, double outMax)
        {
            double d = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
            if (d > outMax) d = outMax;
            if (d < outMin) d = outMin;
            return d;
        }
        public decimal MAP(decimal x, decimal inMin, decimal inMax, decimal outMin, decimal outMax)
        {
            decimal d = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
            if (d > outMax) d = outMax;
            if (d < outMin) d = outMin;
            return d;
        }
        public void LatLongtoUTM(double Lat, double Long, out double UTMNorthing, out double UTMEasting, out string Zone)
        {

            double a = 6378137; //WGS84
            double eccSquared = 0.00669438; //WGS84
            double k0 = 0.9996;

            double LongOrigin;
            double eccPrimeSquared;
            double N, T, C, A, M;

            //Make sure the longitude is between -180.00 .. 179.9
            double LongTemp = (Long + 180) - ((int)((Long + 180) / 360)) * 360 - 180; // -180.00 .. 179.9;
            double deg2rad = Math.PI / 180;
            double LatRad = Lat * deg2rad;
            double LongRad = LongTemp * deg2rad;
            double LongOriginRad;
            int ZoneNumber;

            ZoneNumber = ((int)((LongTemp + 180) / 6)) + 1;

            if (Lat >= 56.0 && Lat < 64.0 && LongTemp >= 3.0 && LongTemp < 12.0)
                ZoneNumber = 32;

            // Special zones for Svalbard
            if (Lat >= 72.0 && Lat < 84.0)
            {
                if (LongTemp >= 0.0 && LongTemp < 9.0) ZoneNumber = 31;
                else if (LongTemp >= 9.0 && LongTemp < 21.0) ZoneNumber = 33;
                else if (LongTemp >= 21.0 && LongTemp < 33.0) ZoneNumber = 35;
                else if (LongTemp >= 33.0 && LongTemp < 42.0) ZoneNumber = 37;
            }
            LongOrigin = (ZoneNumber - 1) * 6 - 180 + 3; //+3 puts origin in middle of zone
            LongOriginRad = LongOrigin * deg2rad;

            //compute the UTM Zone from the latitude and longitude
            Zone = ZoneNumber.ToString() + UTMLetterDesignator(Lat);

            eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            N = a / Math.Sqrt(1 - eccSquared * Math.Sin(LatRad) * Math.Sin(LatRad));
            T = Math.Tan(LatRad) * Math.Tan(LatRad);
            C = eccPrimeSquared * Math.Cos(LatRad) * Math.Cos(LatRad);
            A = Math.Cos(LatRad) * (LongRad - LongOriginRad);

            M = a * ((1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256) * LatRad
            - (3 * eccSquared / 8 + 3 * eccSquared * eccSquared / 32 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(2 * LatRad)
            + (15 * eccSquared * eccSquared / 256 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(4 * LatRad)
            - (35 * eccSquared * eccSquared * eccSquared / 3072) * Math.Sin(6 * LatRad));

            UTMEasting = (double)(k0 * N * (A + (1 - T + C) * A * A * A / 6
            + (5 - 18 * T + T * T + 72 * C - 58 * eccPrimeSquared) * A * A * A * A * A / 120)
            + 500000.0);

            UTMNorthing = (double)(k0 * (M + N * Math.Tan(LatRad) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
            + (61 - 58 * T + T * T + 600 * C - 330 * eccPrimeSquared) * A * A * A * A * A * A / 720)));
            if (Lat < 0)
                UTMNorthing += 10000000.0; //10000000 meter offset for southern hemisphere
        }


        private char UTMLetterDesignator(double Lat)
        {
            char LetterDesignator;

            if ((84 >= Lat) && (Lat >= 72)) LetterDesignator = 'X';
            else if ((72 > Lat) && (Lat >= 64)) LetterDesignator = 'W';
            else if ((64 > Lat) && (Lat >= 56)) LetterDesignator = 'V';
            else if ((56 > Lat) && (Lat >= 48)) LetterDesignator = 'U';
            else if ((48 > Lat) && (Lat >= 40)) LetterDesignator = 'T';
            else if ((40 > Lat) && (Lat >= 32)) LetterDesignator = 'S';
            else if ((32 > Lat) && (Lat >= 24)) LetterDesignator = 'R';
            else if ((24 > Lat) && (Lat >= 16)) LetterDesignator = 'Q';
            else if ((16 > Lat) && (Lat >= 8)) LetterDesignator = 'P';
            else if ((8 > Lat) && (Lat >= 0)) LetterDesignator = 'N';
            else if ((0 > Lat) && (Lat >= -8)) LetterDesignator = 'M';
            else if ((-8 > Lat) && (Lat >= -16)) LetterDesignator = 'L';
            else if ((-16 > Lat) && (Lat >= -24)) LetterDesignator = 'K';
            else if ((-24 > Lat) && (Lat >= -32)) LetterDesignator = 'J';
            else if ((-32 > Lat) && (Lat >= -40)) LetterDesignator = 'H';
            else if ((-40 > Lat) && (Lat >= -48)) LetterDesignator = 'G';
            else if ((-48 > Lat) && (Lat >= -56)) LetterDesignator = 'F';
            else if ((-56 > Lat) && (Lat >= -64)) LetterDesignator = 'E';
            else if ((-64 > Lat) && (Lat >= -72)) LetterDesignator = 'D';
            else if ((-72 > Lat) && (Lat >= -80)) LetterDesignator = 'C';
            else LetterDesignator = 'Z'; //Latitude is outside the UTM limits
            return LetterDesignator;
        }

        class Ellipsoid
        {
            //Attributes
            public string ellipsoidName;
            public double EquatorialRadius;
            public double eccentricitySquared;

            public Ellipsoid(string name, double radius, double ecc)
            {
                ellipsoidName = name;
                EquatorialRadius = radius;
                eccentricitySquared = ecc;
            }
        };

        public string DDDtoDDMMSS(double DDD)
        {
            //string DDMMSS = "";
            int DD;
            int MM;
            float SS;

            DD = (int)DDD;
            MM = (int)((DDD - DD) * 60);
            SS = (float)Math.Round(((DDD - DD) * 60 - MM) * 60, 1);

            string t = "";
            if (DD < 10) t += "0" + DD.ToString() + "° ";
            else t += DD.ToString() + "° ";
            if (MM < 10) t += "0" + MM.ToString() + "' ";
            else t += MM.ToString() + "' ";
            if (SS < 10) t += "0" + SS.ToString() + "\" ";
            else t += SS.ToString() + "\" ";
            return t;
        }
        public double helpFreqPlus(double freq)
        {

            if (freq > 0 && freq < 1000)
            {
                freq++;
            }
            else if (freq > 999 && freq < 10000)
            {
                freq += 10;
            }
            else if (freq > 9999 && freq < 100000)
            {
                freq += 100;
            }
            else if (freq > 99999 && freq < 1000000)
            {
                freq += 1000;
            }
            else if (freq > 999999 && freq < 10000000)
            {
                freq += 10000;
            }
            else if (freq > 9999999 && freq < 100000000)
            {
                freq += 100000;
            }
            else if (freq > 99999999 && freq < 1000000000)
            {
                freq += 1000000;
            }
            else if (freq > 999999999 && freq < 10000000000)
            {
                freq += 10000000;
            }
            else if (freq > 9999999999 && freq < 100000000000)
            {
                freq += 100000000;
            }
            else if (freq > 99999999999 && freq < 1000000000000)
            {
                freq += 1000000000;
            }
            return freq;
        }
        public double helpFreqMinus(double freq)
        {
            if (freq > 0 && freq < 1000)
            {
                freq--;
            }
            else if (freq > 999 && freq < 10000)
            {
                freq -= 10;
            }
            else if (freq > 9999 && freq < 100000)
            {
                freq -= 100;
            }
            else if (freq > 99999 && freq < 1000000)
            {
                freq -= 1000;
            }
            else if (freq > 999999 && freq < 10000000)
            {
                freq -= 10000;
            }
            else if (freq > 9999999 && freq < 100000000)
            {
                freq -= 100000;
            }
            else if (freq > 99999999 && freq < 1000000000)
            {
                freq -= 1000000;
            }
            else if (freq > 999999999 && freq < 10000000000)
            {
                freq -= 10000000;
            }
            else if (freq > 9999999999 && freq < 100000000000)
            {
                freq -= 100000000;
            }
            else if (freq > 99999999999 && freq < 1000000000000)
            {
                freq -= 1000000000;
            }
            return freq;
        }
        public decimal helpFreqPlus(decimal freq)
        {

            if (freq > 0 && freq < 1000)
            {
                freq++;
            }
            else if (freq > 999 && freq < 10000)
            {
                freq += 10;
            }
            else if (freq > 9999 && freq < 100000)
            {
                freq += 100;
            }
            else if (freq > 99999 && freq < 1000000)
            {
                freq += 1000;
            }
            else if (freq > 999999 && freq < 10000000)
            {
                freq += 10000;
            }
            else if (freq > 9999999 && freq < 100000000)
            {
                freq += 100000;
            }
            else if (freq > 99999999 && freq < 1000000000)
            {
                freq += 1000000;
            }
            else if (freq > 999999999 && freq < 10000000000)
            {
                freq += 10000000;
            }
            else if (freq > 9999999999 && freq < 100000000000)
            {
                freq += 100000000;
            }
            else if (freq > 99999999999 && freq < 1000000000000)
            {
                freq += 1000000000;
            }
            return freq;
        }
        public decimal helpFreqMinus(decimal freq)
        {
            if (freq > 0 && freq < 1000)
            {
                freq--;
            }
            else if (freq > 999 && freq < 10000)
            {
                freq -= 10;
            }
            else if (freq > 9999 && freq < 100000)
            {
                freq -= 100;
            }
            else if (freq > 99999 && freq < 1000000)
            {
                freq -= 1000;
            }
            else if (freq > 999999 && freq < 10000000)
            {
                freq -= 10000;
            }
            else if (freq > 9999999 && freq < 100000000)
            {
                freq -= 100000;
            }
            else if (freq > 99999999 && freq < 1000000000)
            {
                freq -= 1000000;
            }
            else if (freq > 999999999 && freq < 10000000000)
            {
                freq -= 10000000;
            }
            else if (freq > 9999999999 && freq < 100000000000)
            {
                freq -= 100000000;
            }
            else if (freq > 99999999999 && freq < 1000000000000)
            {
                freq -= 1000000000;
            }
            return freq;
        }
        public double PlMntime(double t)
        {
            double tout = 0;
            //if (t > 1) tout = 0.1;
            if (t > 100 && t <= 1000) tout = 100;
            else if (t > 10 && t <= 100) tout = 10;
            else if (t > 1 && t <= 10) tout = 1;
            else if (t < 1 && t >= 0.1) tout = 0.1;
            else if (t < 0.1 && t >= 0.01) tout = 0.01;
            else if (t < 0.01 && t >= 0.001) tout = 0.001;
            else if (t < 0.001 && t >= 0.0001) tout = 0.0001;
            else if (t < 0.0001 && t >= 0.00001) tout = 0.00001;
            else if (t < 0.00001 && t >= 0.000001) tout = 0.000001;
            else if (t < 0.000001 && t >= 0.0000001) tout = 0.0000001;
            else if (t < 0.0000001 && t >= 0.00000001) tout = 0.00000001;
            else if (t < 0.00000001 && t >= 0.000000001) tout = 0.000000001;
            else if (t < 0.000000001 && t >= 0.0000000001) tout = 0.0000000001;
            else if (t < 0.0000000001 && t >= 0.00000000001) tout = 0.00000000001;
            return tout;
        }
        public decimal PlMntime(decimal t)
        {
            decimal tout = 0;
            //if (t > 1) tout = 0.1m;
            if (t > 100 && t <= 1000) tout = 100;
            else if (t > 10 && t <= 100) tout = 10;
            else if (t >= 1 && t <= 10) tout = 0.1m;
            else if (t < 1 && t >= 0.1m) tout = 0.01m;
            else if (t < 0.1m && t >= 0.01m) tout = 0.001m;
            else if (t < 0.01m && t >= 0.001m) tout = 0.0001m;
            else if (t < 0.001m && t >= 0.0001m) tout = 0.00001m;
            else if (t < 0.0001m && t >= 0.00001m) tout = 0.000001m;
            else if (t < 0.00001m && t >= 0.000001m) tout = 0.0000001m;
            else if (t < 0.000001m && t >= 0.0000001m) tout = 0.00000001m;
            else if (t < 0.0000001m && t >= 0.00000001m) tout = 0.000000001m;
            else if (t < 0.00000001m && t >= 0.000000001m) tout = 0.0000000001m;
            else if (t < 0.000000001m && t >= 0.0000000001m) tout = 0.00000000001m;
            else if (t < 0.0000000001m && t >= 0.00000000001m) tout = 0.000000000001m;
            return tout;
        }
        public string helpFreq(double freq)
        {
            string str = "";
            if (freq > -1000000000000 && freq < -999999999)
            {
                str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
            }
            else if (freq > -1000000000 && freq < -999999)
            {
                str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
            }
            else if (freq > -1000000 && freq < -999)
            {
                str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
            }
            else if (freq > -1000 && freq < 0)
            {
                str = String.Concat(freq, " Hz");
            }
            else if (freq >= 0 && freq < 1000)
            {
                str = String.Concat(freq, " Hz");
            }
            else if (freq > 999 && freq < 1000000)
            {
                str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
            }
            else if (freq > 999999 && freq < 1000000000)
            {
                str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
            }
            else if (freq > 999999999 && freq < 1000000000000)
            {
                str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
            }
            return str;
        }
        public string helpFreq(decimal freq)
        {
            string str = "";
            if (freq > -1000000000000 && freq < -999999999)
            {
                str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
            }
            else if (freq > -1000000000 && freq < -999999)
            {
                str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
            }
            else if (freq > -1000000 && freq < -999)
            {
                str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
            }
            else if (freq > -1000 && freq < 0)
            {
                str = String.Concat(freq, " Hz");
            }
            else if (freq >= 0 && freq < 1000)
            {
                str = String.Concat(freq, " Hz");
            }
            else if (freq > 999 && freq < 1000000)
            {
                str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
            }
            else if (freq > 999999 && freq < 1000000000)
            {
                str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
            }
            else if (freq > 999999999 && freq < 1000000000000)
            {
                str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
            }
            return str;
        }
        public double setFreqMHz(double freq)
        {
            return freq * 1000000;
        }
        public double helpFreqMHz(double freq)
        {
            return freq * 1000000;
        }
        public string helpLevel(double lev, string levelUnit)
        {
            return String.Concat(Math.Round(lev, 2), " ", levelUnit);
        }
        public async void Load()
        {
            await FactorialAsync();
        }

        public static System.Threading.Tasks.Task<int> FactorialAsync()
        {
            int result = 1;
            return System.Threading.Tasks.Task.Run(() =>
            {
                //result = Get();
                return result;
            });
        }

        public string helpTime(double t)
        {
            string tout = "";
            if (t >= 1) { tout = Math.Round(t, 2).ToString() + " s"; }
            if (t < 1 && t >= 0.001) { tout = Math.Round(t * 1000, 2).ToString() + " ms"; }
            if (t < 0.001 && t >= 0.000001) { tout = Math.Round(t * 1000000, 2).ToString() + " µs"; }
            if (t < 0.000001 && t >= 0.000000001) { tout = Math.Round(t * 1000000000, 2).ToString() + " ns"; }
            return tout;
        }
        public string helpTime(decimal t)
        {
            string tout = "";
            if (t >= 1) { tout = Math.Round(t, 2).ToString("G29") + " s"; }
            if (1 > t && t >= (decimal)0.001) { tout = Math.Round(t * 1000, 2).ToString("G29") + " ms"; }
            if ((decimal)0.001 > t && t >= (decimal)0.000001) { tout = Math.Round(t * 1000000, 2).ToString("G29") + " µs"; }
            if ((decimal)0.000001 > t && t >= (decimal)0.000000001) { tout = Math.Round(t * 1000000000, 2).ToString("G29") + " ns"; }
            return tout;
        }
        public string helpStatusFromXXToSTR(string status)
        {
            if (status.ToLower() == "zip") { status = "Архив"; }
            else if (status.ToLower() == "zz") { status = "Архив"; }
            else if (status.ToLower() == "a") { status = "Заявлений"; }
            else if (status.ToLower() == "aa") { status = "Заявлений AP"; }
            else if (status.ToLower() == "l") { status = "Ліцензія ЕМС"; }
            else if (status.ToLower() == "m") { status = "ЕМС+"; }
            else if (status.ToLower() == "n") { status = "Заморожений ЕМС"; }
            else if (status.ToLower() == "p") { status = "Задіяний"; }
            else if (status.ToLower() == "pp") { status = "Задіяний AP"; }
            else if (status.ToLower() == "r") { status = "Заморожений"; }
            else if (status.ToLower() == "rr") { status = "Заморожений AP"; }
            else if (status.ToLower() == "x") { status = "Анульований"; }
            else if (status.ToLower() == "xx") { status = "Анульований AP"; }
            return status;
        }
        public string helpOwnerNameAbbreviation(string owner)
        {
            owner = owner.Replace("Приватне акціонерне товариство", "ПрАТ");
            owner = owner.Replace("ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО", "ПрАТ");
            owner = owner.Replace("Товариство з обмеженою відповідальністю", "ТОВ");
            owner = owner.Replace("ТОВАРИСТВО З ОБМЕЖЕНОЮ ВІДПОВІДАЛЬНІСТЮ", "ТОВ");
            owner = owner.Replace("Публічне акціонерне товариство", "ПАТ");
            owner = owner.Replace("ПУБЛІЧНЕ АКЦІОНЕРНЕ ТОВАРИСТВО", "ПАТ");
            owner = owner.Replace("Державне підприємство", "ДП");
            owner = owner.Replace("ДЕРЖАВНЕ ПІДПРИЄМСТВО", "ДП");
            owner = owner.Replace("Приватне підприємство", "ПП");
            owner = owner.Replace("Приватне підприємство", "ПП");
            owner = owner.Replace("Фізична особа-підприємець", "ФОП");
            owner = owner.Replace("Фізична особа - підприємець", "ФОП");
            owner = owner.Replace("Спільне українсько-німецьке підприємство в формі товариства з обмеженою відповідальністю", "СУНПФТОФ");
            owner = owner.Replace("Український державний центр радіочастот", "УДЦР");
            return owner;
        }
        public System.Drawing.Imaging.ImageFormat FromString(string format)
        {
            Type type = typeof(System.Drawing.Imaging.ImageFormat);
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.GetProperty;
            object o = type.InvokeMember(format, flags, null, type, null);
            return (System.Drawing.Imaging.ImageFormat)o;
        }
        /*public string TraceType(int t, string instr)
        {
            List<string> TraceType = new List<string>();
            if (instr == "Rohde&Schwarz")
            {
                TraceType.Add("Clear Write");
                TraceType.Add("View");
                TraceType.Add("Avarege");
                TraceType.Add("Max Hold");
                TraceType.Add("Min Hold");
                TraceType.Add("Blank");
            }
            else if (instr == "Anritsu")
            {
                TraceType.Add("NORMal");
                TraceType.Add("");
                TraceType.Add("AVERage");
                TraceType.Add("MAXHold");
                TraceType.Add("MINHold");
                TraceType.Add("NONE");
            }
            return TraceType[t];

        }
        public string traceDetector(int t, string instr)
        {
            List<string> Detector = new List<string>();
            if (instr == "Rohde&Schwarz")
            {
                Detector.Add("Auto Select");
                Detector.Add("Auto Peak");
                Detector.Add("Average");
                Detector.Add("Positive Peak");
                Detector.Add("Negative Peak");
                Detector.Add("Sample");
                Detector.Add("RMS");
            }
            else if (instr == "Anritsu")
            {
                if (Trace1Detector == 3) NV.Write(":SENSe:DETector:FUNCtion POSitive");
                if (Trace1Detector == 4) NV.Write(":SENSe:DETector:FUNCtion NEGative");
                if (Trace1Detector == 5) NV.Write(":SENSe:DETector:FUNCtion SAMPle");
                if (Trace1Detector == 6) NV.Write(":SENSe:DETector:FUNCtion RMS");
            }
            return Detector[t];
        }*/
        public void LoadData()
        {
            //load();
        }

        public void Key()
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //byte[] publicKey, privateKey;
            const int KEY_SIZE = 16384;

            //// Генерация пары ключей
            //using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KEY_SIZE))
            //{
            //    FMeas.Settings.Set.Default.Pu = System.Convert.ToBase64String(rsa.ExportCspBlob(false));
            //    FMeas.Settings.Set.Default.Pr = System.Convert.ToBase64String(rsa.ExportCspBlob(true));
            //    FMeas.Settings.Set.Default.Save();

            //    System.IO.File.WriteAllText(@System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Settings/" + "KeyPu.txt", FMeas.Settings.Set.Default.Pu);
            //    System.IO.File.WriteAllText(@System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Settings/" + "KeyPr.txt", FMeas.Settings.Set.Default.Pr);
            //}
            //privateKey +=

            //Controls.AnLegendControl ap = new Controls.AnLegendControl();
            string s = "";

            // Генерация случайных данных для шифрования
            const int MAX_LENGTH = ((KEY_SIZE - 384) / 8) + 7 - 1;
            byte[] data = new byte[MAX_LENGTH];
            Random rnd = new Random();
            rnd.NextBytes(data);

            //string str = string.Empty;
            //str += getdata("AN:" + MainWindow.An.InstrSerialNumber) + ";";
            //str += getdata("RC:" + MainWindow.Rcvr.InstrSerialNumber) + ";";
            //System.Windows.MessageBox.Show(MAX_LENGTH.ToString());

            // Шифрование
            byte[] encrypted;
            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KEY_SIZE))
            {
                //rsa.ImportCspBlob(System.Convert.FromBase64String(FMeas.Settings.Set.Default.Pu));
                encrypted = rsa.Encrypt(data, true);
            }

            // Дешифрование
            byte[] decrypted;
            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.ImportCspBlob(System.Convert.FromBase64String(s));
                decrypted = rsa.Decrypt(encrypted, true);
            }

            // Проверка
            string t = "";
            bool t1 = true;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != decrypted[i]) { t1 = false; break; };
            }
            if (t1) { t = "Данные успешно расшифрованы"; }
            else
            {

                foreach (byte b in data)
                    t += b.ToString();
                t += "\r\n";
                foreach (byte b in decrypted)
                    t += b.ToString();
                t += "\r\n";
                t += "Ошибка при расшифровке";
            }
            //System.Windows.MessageBox.Show(t);
        }
        private byte[] getdata(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                //System.Windows.MessageBox.Show(sb.ToString() + "\r\n" + sb.Length);
                return hashBytes;
            }
        }

        /// <summary>
        /// Возвращает канал в Гц 
        ///      устанавливается в МГц надо это победить
        /// </summary>
        /// <param name="freq_Dn">в МГц надо это победить</param>
        /// <returns>Возвращает канал GSM в Гц </returns>
        public Equipment.GSM_Channel GetGSMCHfromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            Equipment.GSM_Channel temp = new Equipment.GSM_Channel();
            if (find == false && freq_Dn >= 935.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1; i <= 124; i++)
                {
                    tf.Add(935.2m + 0.2m * (i - 1));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 935.2m) * 5 + 1);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "P-GSM900";
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 925.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 975; i <= 1023; i++)
                {
                    tf.Add(925.2m + 0.2m * (i - 975));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 925.2m) * 5 + 975);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "E-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(935 + 0.2m * (i - 0));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.ARFCN = (int)((temp.FreqDn - 935) * 5 + 0);
                            temp.FreqUp = freq_Dn - 45;
                            temp.StandartSubband = "E-GSM900";
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 921.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 955; i <= 1023; i++)
                {
                    tf.Add(921.2m + 0.2m * (i - 955));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 921.2m) * 5 + 955);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "R-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(935 + 0.2m * (i - 0));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.ARFCN = (int)((temp.FreqDn - 935) * 5 + 0);
                            temp.FreqUp = freq_Dn - 45;
                            temp.StandartSubband = "R-GSM900";
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 918.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 940; i <= 1023; i++)
                {
                    tf.Add(918.2m + 0.2m * (i - 940));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 918.2m) * 5 + 940);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "R-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(935 + 0.2m * (i - 0));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.ARFCN = (int)((temp.FreqDn - 935) * 5 + 0);
                            temp.FreqUp = freq_Dn - 45;
                            temp.StandartSubband = "R-GSM900";
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1805.2m && freq_Dn <= 1879.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 512; i <= 885; i++)
                {
                    tf.Add(1805.2m + 0.2m * (i - 512));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 1805.2m) * 5 + 512);
                        temp.FreqUp = freq_Dn - 95;
                        temp.StandartSubband = "GSM1800";
                    }
                }
                #endregion
            }
            else if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        public Equipment.GSM_Channel GetGSMCHFromChannel(int Channel)
        {
            bool find = false;
            Equipment.GSM_Channel temp = new Equipment.GSM_Channel();
            if (find == false && Channel >= 1 && Channel <= 124)
            {
                #region
                List<int> tf = new List<int>() { };
                //for (decimal i = 935.2m; i <= 959.8m; i += 0.2m)
                //{
                //    tf.Add((int)(1 + (i - 935.2m) * 5));
                //}
                for (int i = 1; i <= 124; i++)
                {
                    tf.Add(i);
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 935.2m + 0.2m * (Channel - 1);
                        temp.ARFCN = Channel;
                        temp.FreqUp = 935.2m - 45 + 0.2m * (Channel - 1);
                        temp.StandartSubband = "P-GSM900";
                    }
                }
                #endregion
            }
            else if (find == false && (Channel >= 975 && Channel <= 1023 || Channel >= 0 && Channel <= 124))
            {
                #region
                List<int> tf = new List<int>() { };
                //for (decimal i = 925.2m; i <= 934.8m; i += 0.2m)
                //{
                //    tf.Add((int)(975 + (i - 925.2m) * 5));//925.2m + 0.2m * (i - 975)
                //}
                for (int i = 975; i <= 1023; i++)
                {
                    tf.Add(i);//925.2m + 0.2m * (i - 975)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 925.2m + 0.2m * (Channel - 975);
                        temp.ARFCN = Channel;
                        temp.FreqUp = 925.2m - 45 + 0.2m * (Channel - 975);
                        temp.StandartSubband = "E-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    //for (decimal i = 935; i <= 959.8m; i += 0.2m)
                    //{
                    //    tf.Add((int)(0 + (i - 935) * 5)); //935 + 0.2m * (i - 0)
                    //}
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(i); //935 + 0.2m * (i - 0)
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == Channel)
                        {
                            find = true;
                            temp.FreqDn = 935 + 0.2m * (Channel - 0);
                            temp.ARFCN = Channel;
                            temp.FreqUp = 935 - 45 + 0.2m * (Channel - 0);
                            temp.StandartSubband = "E-GSM900";
                        }
                    }
                }
                #endregion               
            }
            else if (find == false && Channel >= 512 && Channel <= 885)
            {
                #region
                List<int> tf = new List<int>() { };
                //for (decimal i = 1805.2m; i <= 1879.8m; i += 0.2m)
                //{
                //    tf.Add((int)(512 + (i - 1805.2m) * 5)); //1805.2m + 0.2m * (i - 512)
                //}
                for (int i = 512; i <= 885; i++)
                {
                    tf.Add(i); //1805.2m + 0.2m * (i - 512)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1805.2m + 0.2m * (Channel - 512);
                        temp.ARFCN = Channel;
                        temp.FreqUp = 1805.2m - 95 + 0.2m * (Channel - 512);
                        temp.StandartSubband = "GSM1800";
                    }
                }
                #endregion
            }
            else if (find == false)
                throw new Exception("Канал " + Channel + " не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="freq_Dn">in MHz</param>
        /// <returns></returns>
        public Equipment.UMTS_Channel GetUMTSCHFromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            Equipment.UMTS_Channel temp = new Equipment.UMTS_Channel();
            //Settings.WCDMAFreqs_Set temp = new Settings.WCDMAFreqs_Set();
            if (find == false && freq_Dn >= 2112.4m && freq_Dn <= 2167.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 10562; i <= 10838; i++)
                {
                    tf.Add(2112.4m + 0.2m * (i - 10562));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 2112.4m) * 5 + 10562);
                        temp.FreqUp = freq_Dn - 190;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1922.4m) * 5 + 9612);
                        temp.StandartSubband = "Band-1 2100";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1932.4m && freq_Dn <= 1987.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 9662; i <= 9938; i++)
                {
                    tf.Add(1932.4m + 0.2m * (i - 9662));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 1932.4m) * 5 + 9662);
                        temp.FreqUp = freq_Dn - 80;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1852.4m) * 5 + 9262);
                        temp.StandartSubband = "Band-2 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 412; i <= 687; i += 25)
                    {
                        tf.Add(1932.5m + 0.2m * (i - 412));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.UARFCN_DN = (int)(temp.FreqDn - 1932.5m) * 5 + 412;
                            temp.FreqUp = freq_Dn - 80;
                            temp.UARFCN_UP = (int)(temp.FreqUp - 1852.5m) * 5 + 12;
                            temp.StandartSubband = "Band-2 1900";//  Equipment.UMTSBands.Band_2_1900.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1807.4m && freq_Dn <= 1877.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1162; i <= 1513; i++)
                {
                    tf.Add(1807.4m + 0.2m * (i - 1162));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 1932.4m) * 5 + 1162);
                        temp.FreqUp = freq_Dn - 95;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1712.4m) * 5 + 937);
                        temp.StandartSubband = "Band-3 1800";// Equipment.UMTSBands.Band_3_1800.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 2112.4m && freq_Dn <= 2152.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1537; i <= 1738; i++)
                {
                    tf.Add(2112.4m + 0.2m * (i - 1537));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 2112.4m) * 5 + 1537);
                        temp.FreqUp = freq_Dn - 400;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1712.4m) * 5 + 1312);
                        temp.StandartSubband = "Band-4 1700";// Equipment.UMTSBands.Band_4_1700.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 1887; i <= 2087; i += 25)
                    {
                        tf.Add(2112.5m + 0.2m * (i - 1887));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.UARFCN_DN = (int)((temp.FreqDn - 2112.5m) * 5 + 1887);
                            temp.FreqUp = freq_Dn - 400;
                            temp.UARFCN_UP = (int)((temp.FreqUp - 1712.5m) * 5 + 1662);
                            temp.StandartSubband = "Band-4 1700";//  Equipment.UMTSBands.Band_4_1700.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        public Equipment.UMTS_Channel GetUMTSCHFromChannelDn(int Channel)
        {
            bool find = false;
            Equipment.UMTS_Channel temp = new Equipment.UMTS_Channel();
            if (find == false && Channel >= 10562 && Channel <= 10838)//2112.4m  2167.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 10562; i <= 10838; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 2112.4m + 0.2m * (Channel - 10562);
                        temp.UARFCN_DN = Channel;
                        temp.FreqUp = 2112.4m - 190 + 0.2m * (Channel - 10562 - 950); //freq_Dn - 190;
                        temp.UARFCN_UP = Channel - 950;
                        temp.StandartSubband = "Band-1 2100";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && (Channel >= 9662 && Channel <= 9938 || Channel >= 412 && Channel <= 687))//1932.4m  1987.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 9662; i <= 9938; i++)
                {
                    tf.Add(i);//1932.4m + 0.2m * (i - 9662))
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1932.4m + 0.2m * (Channel - 9662);
                        temp.UARFCN_DN = Channel;// (int)((temp.FreqDn - 1932.4m) * 5 + 9662);
                        temp.FreqUp = 1932.4m - 80 + 0.2m * (Channel - 9662);
                        temp.UARFCN_UP = Channel - 400;//(int)((temp.FreqUp - 1852.4m) * 5 + 9262);
                        temp.StandartSubband = "Band-2 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 412; i <= 687; i += 25)
                    {
                        tf.Add(i); //1932.5m + 0.2m * (i - 412)
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == Channel)
                        {
                            find = true;
                            temp.FreqDn = 1932.5m + 0.2m * (Channel - 412);
                            temp.UARFCN_DN = Channel;//(int)(temp.FreqDn - 1932.5m) * 5 + 412;
                            temp.FreqUp = 1932.5m - 80 + 0.2m * (Channel - 412);
                            temp.UARFCN_UP = Channel - 400;// (int)(temp.FreqUp - 1852.5m) * 5 + 12;
                            temp.StandartSubband = "Band-2 1900";//  Equipment.UMTSBands.Band_2_1900.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 1162 && Channel <= 1513)//1807.4m  1877.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 1162; i <= 1513; i++)
                {
                    tf.Add(i);//1807.4m + 0.2m * (i - 1162)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1807.4m + 0.2m * (Channel - 1162);
                        temp.UARFCN_DN = Channel;// (int)((temp.FreqDn - 1932.4m) * 5 + 1162);
                        temp.FreqUp = 1807.4m - 95 + 0.2m * (Channel - 1162);
                        temp.UARFCN_UP = Channel - 225;// (int)((temp.FreqUp - 1712.4m) * 5 + 937);
                        temp.StandartSubband = "Band-3 1800";// Equipment.UMTSBands.Band_3_1800.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && (Channel >= 1537 && Channel <= 1738 || Channel >= 1887 && Channel <= 2087))//2112.4m  2152.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 1537; i <= 1738; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 1537)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 2112.4m + 0.2m * (Channel - 1537);
                        temp.UARFCN_DN = Channel;//(int)((temp.FreqDn - 2112.4m) * 5 + 1537);
                        temp.FreqUp = 2112.4m - 400 + 0.2m * (Channel - 1537);
                        temp.UARFCN_UP = Channel - 225;// (int)((temp.FreqUp - 1712.4m) * 5 + 1312);
                        temp.StandartSubband = "Band-4 1700";// Equipment.UMTSBands.Band_4_1700.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 1887; i <= 2087; i += 25)
                    {
                        tf.Add(i);//2112.5m + 0.2m * (i - 1887)
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == Channel)
                        {
                            find = true;
                            temp.FreqDn = 2112.5m + 0.2m * (Channel - 1887);
                            temp.UARFCN_DN = Channel;// (int)((temp.FreqDn - 2112.5m) * 5 + 1887);
                            temp.FreqUp = 2112.5m - 400 + 0.2m * (Channel - 1887);
                            temp.UARFCN_UP = Channel - 225;//(int)((temp.FreqUp - 1712.5m) * 5 + 1662);
                            temp.StandartSubband = "Band-4 1700";//  Equipment.UMTSBands.Band_4_1700.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false)
                throw new Exception("Канал " + Channel + " не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        public Equipment.UMTS_Channel GetUMTSCHFromChannelUp(int Channel)
        {
            bool find = false;
            Equipment.UMTS_Channel temp = new Equipment.UMTS_Channel();
            if (find == false && Channel >= 9612 && Channel <= 9888)//2112.4m  2167.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 9612; i <= 9612; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 2112.4m + 0.2m * (Channel - 9612);
                        temp.UARFCN_DN = Channel + 950;
                        temp.FreqUp = 2112.4m - 190 + 0.2m * (Channel - 9612); //freq_Dn - 190;
                        temp.UARFCN_UP = Channel - 950;
                        temp.StandartSubband = "Band-1 2100";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && (Channel >= 9262 && Channel <= 9538 || Channel >= 12 && Channel <= 287))//1932.4m  1987.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 9262; i <= 9538; i++)
                {
                    tf.Add(i);//1932.4m + 0.2m * (i - 9662))
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1932.4m + 0.2m * (Channel - 9262);
                        temp.UARFCN_DN = Channel + 400;// (int)((temp.FreqDn - 1932.4m) * 5 + 9662);
                        temp.FreqUp = 1932.4m - 80 + 0.2m * (Channel - 9262);
                        temp.UARFCN_UP = Channel;//(int)((temp.FreqUp - 1852.4m) * 5 + 9262);
                        temp.StandartSubband = "Band-2 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 12; i <= 287; i += 25)
                    {
                        tf.Add(i); //1932.5m + 0.2m * (i - 412)
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == Channel)
                        {
                            find = true;
                            temp.FreqDn = 1932.5m + 0.2m * (Channel - 12);
                            temp.UARFCN_DN = Channel + 400;//(int)(temp.FreqDn - 1932.5m) * 5 + 412;
                            temp.FreqUp = 1932.5m - 80 + 0.2m * (Channel - 12);
                            temp.UARFCN_UP = Channel;// (int)(temp.FreqUp - 1852.5m) * 5 + 12;
                            temp.StandartSubband = "Band-2 1900";//  Equipment.UMTSBands.Band_2_1900.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 937 && Channel <= 1288)//1807.4m  1877.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 937; i <= 1288; i++)
                {
                    tf.Add(i);//1807.4m + 0.2m * (i - 1162)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1807.4m + 0.2m * (Channel - 937);
                        temp.UARFCN_DN = Channel + 225;// (int)((temp.FreqDn - 1932.4m) * 5 + 1162);
                        temp.FreqUp = 1807.4m - 95 + 0.2m * (Channel - 937);
                        temp.UARFCN_UP = Channel;// (int)((temp.FreqUp - 1712.4m) * 5 + 937);
                        temp.StandartSubband = "Band-3 1800";// Equipment.UMTSBands.Band_3_1800.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && (Channel >= 1312 && Channel <= 1513 || Channel >= 1662 && Channel <= 1862))//2112.4m  2152.6m
            {
                #region
                List<int> tf = new List<int>() { };
                for (int i = 1312; i <= 1513; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 1537)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 2112.4m + 0.2m * (Channel - 1312);
                        temp.UARFCN_DN = Channel + 225;//(int)((temp.FreqDn - 2112.4m) * 5 + 1537);
                        temp.FreqUp = 2112.4m - 400 + 0.2m * (Channel - 1312);
                        temp.UARFCN_UP = Channel;// (int)((temp.FreqUp - 1712.4m) * 5 + 1312);
                        temp.StandartSubband = "Band-4 1700";// Equipment.UMTSBands.Band_4_1700.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 1662; i <= 1862; i += 25)
                    {
                        tf.Add(i);//2112.5m + 0.2m * (i - 1887)
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == Channel)
                        {
                            find = true;
                            temp.FreqDn = 2112.5m + 0.2m * (Channel - 1662);
                            temp.UARFCN_DN = Channel + 225;// (int)((temp.FreqDn - 2112.5m) * 5 + 1887);
                            temp.FreqUp = 2112.5m - 400 + 0.2m * (Channel - 1662);
                            temp.UARFCN_UP = Channel;//(int)((temp.FreqUp - 1712.5m) * 5 + 1662);
                            temp.StandartSubband = "Band-4 1700";//  Equipment.UMTSBands.Band_4_1700.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false)
                throw new Exception("Канал " + Channel + " не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="freq_Dn">in MHz</param>
        /// <returns></returns>
        public Equipment.LTE_Channel GetLTECHfromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            Equipment.LTE_Channel temp = new Equipment.LTE_Channel();
            //Settings.WCDMAFreqs_Set temp = new Settings.WCDMAFreqs_Set();
            if (find == false && freq_Dn >= 2620.0m && freq_Dn <= 2689.9m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 2750; i <= 3449; i++)
                {
                    tf.Add(2620.0m + 0.1m * (i - 2750));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.EARFCN_DN = (int)((temp.FreqDn - 2620.0m) * 10 + 2750);
                        temp.FreqUp = freq_Dn - 120;
                        temp.EARFCN_UP = (int)((temp.FreqUp - 2500.0m) * 10 + 20750);
                        temp.StandartSubband = "Band-7 2600";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1805.0m && freq_Dn <= 1879.9m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1200; i <= 1949; i++)
                {
                    tf.Add(1805.0m + 0.1m * (i - 1200));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.EARFCN_DN = (int)((temp.FreqDn - 1805.0m) * 10 + 1200);
                        temp.FreqUp = freq_Dn - 95;
                        temp.EARFCN_UP = (int)((temp.FreqUp - 1710.0m) * 10 + 19200);
                        temp.StandartSubband = "Band-3 1800";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="freq_Dn">in MHz</param>
        /// <returns></returns>
        public Equipment.CDMA_Channel GetCDMACHfromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            Equipment.CDMA_Channel temp = new Equipment.CDMA_Channel();
            if (find == false && freq_Dn >= 860.04m && freq_Dn <= 869.01m)
            {
                #region 0 800
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1024; i <= 1323; i++)
                {
                    tf.Add(860.04m + 0.03m * (i - 1024));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 860.04m) / 0.03m) + 1024;
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "Band-0 800";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 869.04m && freq_Dn <= 870)
            {
                #region 0 800
                List<decimal> tf = new List<decimal>() { };
                for (int i = 991; i <= 1023; i++)
                {
                    tf.Add(869.04m + 0.03m * (i - 991));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 869.04m) / 0.03m) + 991;
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "Band-0 800";//  Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 870.03m && freq_Dn <= 893.97m)
            {
                #region 0 800
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1; i <= 799; i++)
                {
                    tf.Add(870.03m + 0.03m * (i - 1));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 870.03m) / 0.03m) + 1;
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "Band-0 800";//  Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 1930 && freq_Dn <= 1989.95m)
            {
                #region 1 1900
                List<decimal> tf = new List<decimal>() { };
                for (int i = 0; i <= 1199; i++)
                {
                    tf.Add(1930 + 0.05m * (i - 0));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 1930) / 0.05m) + 0;
                        temp.FreqUp = freq_Dn - 80;
                        temp.StandartSubband = "Band-1 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 420.0m && freq_Dn <= 429.975m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 472; i <= 871; i++)
                {
                    tf.Add(420 + 0.025m * (i - 1024));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 420) / 0.025m) + 472;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 461.31m && freq_Dn <= 469.99m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1039; i <= 1473; i++)
                {
                    tf.Add(461.31m + 0.02m * (i - 1039));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 461.31m) / 0.02m) + 1039;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 460.0m && freq_Dn <= 469.975m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1; i <= 400; i++)
                {
                    tf.Add(460.0m + 0.025m * (i - 1));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 460.0m) / 0.025m) + 1;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 489 && freq_Dn <= 493.475m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1536; i <= 1715; i++)
                {
                    tf.Add(489 + 0.025m * (i - 1536));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 489) / 0.025m) + 1536;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 489 && freq_Dn <= 493.480m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1792; i <= 2016; i++)
                {
                    tf.Add(489 + 0.02m * (i - 1792));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 489) / 0.02m) + 1792;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 1930 && freq_Dn <= 1994.95m)
            {
                #region 14 1900
                List<decimal> tf = new List<decimal>() { };
                for (int i = 0; i <= 1299; i++)
                {
                    tf.Add(1930 + 0.05m * (i - 0));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 1930) / 0.05m) + 0;
                        temp.FreqUp = freq_Dn - 80;
                        temp.StandartSubband = "Band-14 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        public Equipment.CDMA_Channel GetCDMACHFromChannel(int Channel)
        {
            bool find = false;
            Equipment.CDMA_Channel temp = new Equipment.CDMA_Channel();
            //if (find == false && Channel >= 10562 && Channel <= 10838)//2112.4m  2167.6m
            if (find == false && Channel >= 1024 && Channel <= 1323)//2112.4m  2167.6m
            {
                #region Band-0 800
                List<int> tf = new List<int>() { };
                for (int i = 10562; i <= 10838; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 860.04m + 0.03m * (Channel - 1024);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 45; //2112.4m - 190 + 0.2m * (Channel - 10562 - 950); //freq_Dn - 190;
                        temp.StandartSubband = "Band-0 800";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 991 && Channel <= 1023)//2112.4m  2167.6m
            {
                #region Band-0 800
                List<int> tf = new List<int>() { };
                for (int i = 991; i <= 1023; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 869.04m + 0.03m * (Channel - 991);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 45;
                        temp.StandartSubband = "Band-0 800";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 1 && Channel <= 799)
            {
                #region Band-0 800
                List<int> tf = new List<int>() { };
                for (int i = 1; i <= 799; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 870.03m + 0.03m * (Channel - 1);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 45;
                        temp.StandartSubband = "Band-0 800";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 0 && Channel <= 1199)
            {
                #region Band-1 1900
                List<int> tf = new List<int>() { };
                for (int i = 0; i <= 1199; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1930 + 0.05m * (Channel - 0);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 80;
                        temp.StandartSubband = "Band-1 1900";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 472 && Channel <= 871)
            {
                #region Band-5 450
                List<int> tf = new List<int>() { };
                for (int i = 472; i <= 871; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 420 + 0.025m * (Channel - 1024);// 1930 + 0.05m * (Channel - 0);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 1039 && Channel <= 1473)
            {
                #region Band-5 450
                List<int> tf = new List<int>() { };
                for (int i = 1039; i <= 1473; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 461.31m + 0.02m * (Channel - 1039);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 1 && Channel <= 400)
            {
                #region Band-5 450
                List<int> tf = new List<int>() { };
                for (int i = 1; i <= 400; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 460.0m + 0.025m * (Channel - 1);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 1536 && Channel <= 1715)
            {
                #region Band-5 450
                List<int> tf = new List<int>() { };
                for (int i = 1536; i <= 1715; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 489 + 0.025m * (Channel - 1536);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 1792 && Channel <= 2016)
            {
                #region Band-5 450
                List<int> tf = new List<int>() { };
                for (int i = 1792; i <= 2016; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 489 + 0.02m * (Channel - 1792);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && Channel >= 0 && Channel <= 1299)
            {
                #region Band-14 1900
                List<int> tf = new List<int>() { };
                for (int i = 0; i <= 1299; i++)
                {
                    tf.Add(i);//2112.4m + 0.2m * (i - 10562)
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == Channel)
                    {
                        find = true;
                        temp.FreqDn = 1930 + 0.05m * (Channel - 0);
                        temp.ChannelN = Channel;
                        temp.FreqUp = temp.FreqDn - 80;
                        temp.StandartSubband = "Band-14 1900";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }

            else if (find == false)
                throw new Exception("Канал " + Channel + " не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        public string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            return files;
        }
        public long GetObjectSize(object obj)
        {
            long size = 0;
            if (obj != null)
                using (Stream stream = new MemoryStream())
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter
                        = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    size = stream.Length;
                }
            return size;
        }
        public double CalcStrength(double dBm, double Gain, double Freq)
        {
            return dBm + 106.99 + 20 * Math.Log10(Freq / 1000000) - Gain - 29.79;
        }
    }
    public class GPSSat
    {
        public int SatNum { get; set; }
        public int SatLevel { get; set; }
    }
}
