using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using System.Drawing;

namespace XICSM.ICSControlClient.ViewModels.Reports
{
    public class InsertSpectrogram
    {
        public static string GetDirTemplates(string Name)
        {
            string DirTemplate = "";
            IMRecordset r = new IMRecordset("SYS_CONFIG", IMRecordset.Mode.ReadOnly);
            r.Select("ITEM,WHAT");
            r.SetWhere("ITEM", IMRecordset.Operation.Like, Name);
            try
            {
                r.Open();
                if (!r.IsEOF())
                {
                    DirTemplate = r.GetS("WHAT");
                }
            }
            finally
            {
                r.Close();
                r.Destroy();
            }
            return DirTemplate;
        }

        private static string ImageToHex(System.Drawing.Bitmap bmp)
        {
            var converter = new ImageConverter();
            var xByte = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
            string hex = BitConverter.ToString(xByte);
            return hex.Replace("-", "");
        }


        public static void InsertImageToRtf(string SourceFile, System.Drawing.Bitmap bmp, int Width, int Height)
        {
            var strRtf = @"{\pict\wmetafile8\picw" + Width.ToString() + @"\pich" + Height.ToString() + @"\picwgoal" + ((int)(Width / 1.76)).ToString() + @"\pichgoal" + ((int)(Height / 1.76)).ToString() + @"\jpegblip\bliptag " + ImageToHex(bmp) + "}";
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
