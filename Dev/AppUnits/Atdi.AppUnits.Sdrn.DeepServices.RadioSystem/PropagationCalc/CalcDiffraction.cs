using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.PropagationCalc
{
    

    public class CalcDifraction
    {
        public struct DiffractionLoss
        {
            public float loss_dB;
            public float alphaToA_deg;
            public float alphaToB_deg;
        }
        

        private struct NuMaxOut
        {
            public float nuMax;
            public int nMax;
            //public float hNMax;
        }

        public static float SubDeygout91(float d_km)
        {
            return 10.0f + 0.04f * d_km;
        }

        private static float CalcElevAngle(float h1, float h2, float d, float rE)
        {
            if (d > 100)
            {
                return (float)((Math.PI / 2 - Math.Acos(( Math.Pow(rE + h1, 2) + Math.Pow(d, 2) - Math.Pow(rE + h2, 2)) / (2 * (rE + h1) * d))) * 180 / Math.PI);
            }
            else
            {
                return (float)(Math.Atan((h1 - h2) / d) * 180 / Math.PI);
            }
            //return;
        }

        //private NuMaxOut FindMaxNu(in NuMaxFinding nuMaxFinding, ref NuMaxOut nuMaxOut)
        private static NuMaxOut FindMaxNu(float hA, float hB, float wavelength, float dAB, in float[] profile, int profileStart, int profileEnd, float rE)
        {
            
            float dN = dAB / (profileEnd - profileStart);
            float dAN = dN;
            float dNB = dN * (profileEnd - profileStart - 1);
            float inv2rE = 1 / (2 * rE);
            float invDaB = 1 / dAB;
            float hNMax = profile[0] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

            // float h_n_max = -9999.0;
            // float nu_max = -9999.0;
            NuMaxOut nuMaxOut = new NuMaxOut();
            nuMaxOut.nuMax = (float)(hNMax * Math.Sqrt(2 * dAB / (wavelength * dAN * dNB)));
            nuMaxOut.nMax = 0;
            //nuMaxOut.hNMax = hNMax;
            if (profile.Length > 1)
            {
                for (int n = profileStart; n < profileEnd; n++)
                {
                    dAN = dN * n - profileStart + 1;
                    dNB = dN * (profileEnd - n);
                    float h = profile[n] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

                    float nuN = (float)(h * Math.Sqrt(2 * dAB / (wavelength * dAN * dNB)));

                    if (nuN > nuMaxOut.nuMax)
                    {
                        nuMaxOut.nuMax = nuN;
                        nuMaxOut.nMax = n;
                        //nuMaxOut.hNMax = h;
                    }
                }
            }
            return nuMaxOut;
        }

        private static float J (float nu)
        {
            if (nu <= -0.78)
            {
                return 0.0f;
            }
            else
            {
                return (float)(6.9 + 20 * Math.Log10(Math.Sqrt(Math.Pow(nu - 0.1, 2) + 1) + nu - 0.1));
            }
        }

        private static float T (float J)
        {
            return (float)(1.0 - Math.Exp(-J / 6.0f));
        }

        public static DiffractionLoss Deygout91 (float hA_m, float  hB_m, float freq_MHz, float d_km, in float[] profile_m, int profileStart, int profileEnd, float rE_km, float subDiffractionLoss)
        {
            DiffractionLoss diffractionLoss = new DiffractionLoss();

            float dAB = d_km * 1000;
            float rE = rE_km * 1000;
            float wavelength = 300 / freq_MHz;
            hA_m += profile_m[profileStart];
            hB_m += profile_m[profileEnd];

            NuMaxOut nu;
            nu = FindMaxNu(hA_m, hB_m, wavelength, dAB, profile_m, profileStart, profileEnd, rE);

            float nuP = nu.nuMax;
            //int p = nu.nMax;

            float dN = dAB / (profileEnd - profileStart);
            float dAP = dN * nu.nMax;
            
            float dPB = dN * (profileEnd - profileStart - nu.nMax);
            
            if (nuP > -0.78)
            {
                //float[] profileAP = new float[p];
                //Array.Copy(profile_m, profileAP, p);
                NuMaxOut nuT = FindMaxNu(hA_m, profile_m[nu.nMax], wavelength, dAP, profile_m, profileStart, nu.nMax, rE);

                //float[] profilePB = new float[profile_m.Length - p];
                //Array.Copy(profile_m, p, profilePB, profile_m.Length - p - 1, profile_m.Length - p - 2);
                NuMaxOut nuR = FindMaxNu(profile_m[nu.nMax], hB_m, wavelength, dPB, profile_m, nu.nMax, profileEnd, rE);
                //D = math.sqrt(2 * r_e) * (math.sqrt(h_a) + math.sqrt(h_b));
                //C = 10.0 + 0.04 * D;
                //float C = 10.0f + 0.04f * dAB;
                diffractionLoss.loss_dB = J(nuP) + T(J(nuT.nuMax) + J(nuR.nuMax) + subDiffractionLoss);
            }
            else diffractionLoss.loss_dB = 0;

            if (nuP <= 0)
            {
                diffractionLoss.alphaToB_deg = CalcElevAngle(hA_m, hB_m, dAB, rE);
                diffractionLoss.alphaToA_deg = CalcElevAngle(hB_m, hA_m, dAB, rE);
            }
            else
            {
                diffractionLoss.alphaToB_deg = CalcElevAngle(hA_m, profile_m[nu.nMax], dAP, rE);
                diffractionLoss.alphaToA_deg = CalcElevAngle(hB_m, profile_m[nu.nMax], dPB, rE);
            }
            return diffractionLoss;
        }
    }
}
