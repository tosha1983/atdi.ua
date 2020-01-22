using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IQFilters
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        string strPath = @System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\IQ.txt";
        public static float[] IQArr = null;
        App()
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(strPath))
            {
                string line = sr.ReadLine();
                int length = int.Parse(line.Replace(";", ""));
                float[] temp = new float[length * 2];
                int index = 0;
                string[] sss = new string[2];
                while (!sr.EndOfStream)
                {

                    sss = sr.ReadLine().Split(';');
                    temp[index*2] = float.Parse(sss[0].Replace(',','.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    temp[index*2 + 1] = float.Parse(sss[1].Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                    index++;
                    //бла-бла-бла
                }
                sr.Close();
                IQArr = temp;
            }
        }
    }
}
