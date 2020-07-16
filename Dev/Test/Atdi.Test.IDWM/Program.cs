using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Idwm;

namespace Atdi.Test.IDWM
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize IDWM using WDBUNIT
            int unr = 111;
            Idwm.Idwm.WDBUNIT(ref unr);


            // call GEOPRMS
            int N = 4;
            int IER = -1;
            Idwm.Idwm.GEOPRMS(ref N, out IER);




            // call GEOPRCP

            float longitude1 = DecToRadian(30);
            float latitude1 = DecToRadian(50);

            float longitude2 = DecToRadian(50.50f);
            float latitude2 = DecToRadian(50.90f);

            float rdist = 100; // distance;
            int izones = 0; // propagation zones;
            StringBuilder PRCODVEC = new StringBuilder();
            float[] PRDVEK = new float[1000];
            float[] RATIOVEK = new float[1000];
            Idwm.Idwm.GEOPRCP(ref longitude1, ref latitude1, ref longitude2, ref latitude2, out rdist, out izones, PRCODVEC, PRDVEK, RATIOVEK, out IER);
            System.Console.WriteLine(PRCODVEC.ToString());
            System.Console.ReadKey();
        }

        public static float DecToRadian(float decCoord)
        {
            return (float)(decCoord * Math.PI / 180.0);
        }
    }
}
