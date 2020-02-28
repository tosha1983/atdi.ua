using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.Reports
{
    public class Utitlity
    {
        public struct FrequencyRange
        {
            public float MaxValue;
            public float MinValue;
            public float Step;
        }

        public struct LevelRange
        {
            public float MaxValue;
            public float MinValue;
        }

        static public LevelRange CalcLevelRange(double minValue, double maxValue)
        {
            var result = new LevelRange
            {
               MaxValue  = maxValue >= 0 ? Convert.ToSingle(Math.Truncate((decimal)maxValue / 10) * 10 + 10) : Convert.ToSingle(Math.Truncate((decimal)maxValue / 10) * 10),
               MinValue =  minValue >= 0 ? Convert.ToSingle(Math.Truncate((decimal)minValue / 10) * 10) : Convert.ToSingle(Math.Truncate((decimal)minValue / 10) * 10 - 10)
            };

            return result;
        }

        static public FrequencyRange CalcFrequencyRange(double minValue, double maxValue, int maxNumberLine)
        {
            List<decimal> Steps = new List<decimal>();
            long number = 0;
            int Razrad = -6;
            int LastPoint = 10;
            decimal step, fstart, fstop;
            do
            {
                if (LastPoint == 10) { LastPoint = 5; Razrad = Razrad + 1; } else { LastPoint = 10; }
                step = (decimal)(LastPoint * Math.Pow(10, Razrad));
                fstart = Math.Floor((decimal)minValue / step) * step;
                fstop = Math.Ceiling((decimal)maxValue / step) * step;
                number = (long)((fstop - fstart) / step);
            }
            while (number > maxNumberLine);

            Steps.Add(fstart);
            for (int i = 1; number >= i; i++)
            {
                decimal f = fstart + step * i;
                Steps.Add(f);
            }
            return new FrequencyRange
            {
                MinValue = Convert.ToSingle(Steps[0]),
                MaxValue = Convert.ToSingle(Steps[Steps.Count - 1]),
                Step = Convert.ToSingle(step)
            };
        }

        private string ImageToHex(System.Drawing.Image img)
        {
            var ms = new System.IO.MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = ms.ToArray();
            string hex = BitConverter.ToString(bytes);
            return hex.Replace("-", "");
        }


        public void InsertImageToRtf(string SourceFile, System.Drawing.Image img, int Width, int Height)
        {
            var strRtf = @"{\pict\wmetafile8\picw" + Width.ToString() + @"\pich" + Height.ToString() + @"\picwgoal" + ((int)(Width / 1.76)).ToString() + @"\pichgoal" + ((int)(Height / 1.76)).ToString() + @"\jpegblip\bliptag " + ImageToHex(img) + "}";
            using (System.IO.FileStream fstream = new System.IO.FileStream(SourceFile, System.IO.FileMode.OpenOrCreate))
            {
                var array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                string s = textFromFile.Replace(@":PIC1", strRtf);
                var arrayPaste = System.Text.Encoding.Default.GetBytes(s);
                fstream.Position = 0;
                fstream.Write(arrayPaste, 0, arrayPaste.Length);
                fstream.Close();
            }
        }
    }
}
