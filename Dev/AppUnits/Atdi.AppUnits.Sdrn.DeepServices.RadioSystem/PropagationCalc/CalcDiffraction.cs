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

        //public static float SubDeygout91(float d_km)
        //{
        //    return 10.0f + 0.04f * d_km;
        //}

        private static float CalcElevAngle(float H1, float H2, float D, float RE)
        {
            if (D > 100)
            {
                return (float)((Math.PI / 2 - Math.Acos(( Math.Pow(RE + H1, 2) + Math.Pow(D, 2) - Math.Pow(RE + H2, 2)) / (2 * (RE + H1) * D))) * 180 / Math.PI);
            }
            else
            {
                return (float)(Math.Atan((H1 - H2) / D) * 180 / Math.PI);
            }
            //return;
        }

        //private NuMaxOut FindMaxNu(in NuMaxFinding nuMaxFinding, ref NuMaxOut nuMaxOut)
        private static NuMaxOut FindMaxNu(float HA, float HB, float Wavelength, float DAB, in float[] Profile, int ProfileStart, int ProfileEnd, float RE)
        {
            int profileArrayLength = (ProfileEnd - ProfileStart);
            float dN = DAB / profileArrayLength;
            float dAN = dN;
            float dNB = dN * (profileArrayLength - 1);
            float inv2rE = 1 / (2 * RE);
            float invDaB = 1 / DAB;

            float hNMax = Profile[ProfileStart] + dAN * dNB * inv2rE - (HA * dNB + HB * dAN) * invDaB;

            // float h_n_max = -9999.0;
            // float nu_max = -9999.0;
            NuMaxOut nuMaxOut = new NuMaxOut
            {
                nuMax = (float)(hNMax * Math.Sqrt(2 * DAB / (Wavelength * dAN * dNB))),
                nMax = ProfileStart
            };
            //nuMaxOut.hNMax = hNMax;
            if (profileArrayLength > 1)
            {
                for (int n = ProfileStart + 1; n < ProfileEnd; n++)
                {
                    dAN = dN * (n - ProfileStart);
                    dNB = dN * (ProfileEnd - n);
                    float h = Profile[n] + dAN * dNB * inv2rE - (HA * dNB + HB * dAN) * invDaB;

                    float nuN = (float)(h * Math.Sqrt(2 * DAB / (Wavelength * dAN * dNB)));

                    if (nuN > nuMaxOut.nuMax)
                    {
                        nuMaxOut.nuMax = nuN;
                        nuMaxOut.nMax = n;
                    }
                }
            }
            return nuMaxOut;
        }

        private static float J (float Nu)
        {
            if (Nu <= -0.78)
            {
                return 0.0f;
            }
            else
            {
                return (float)(6.9 + 20 * Math.Log10(Math.Sqrt(Math.Pow(Nu - 0.1, 2) + 1) + Nu - 0.1));
            }
        }

        //private static float T (float J)
        //{
        //    return (float)(1.0 - Math.Exp(-J / 6.0f));
        //}

        public static DiffractionLoss Deygout91 (float HA_m, float  HB_m, float Freq_MHz, float D_km, in float[] Profile_m, int ProfileStart, int ProfileEnd, float RE_km)
        {
            DiffractionLoss diffractionLoss = new DiffractionLoss();

            float dAB = D_km * 1000;
            float rE = RE_km * 1000;
            float wavelength = 300 / Freq_MHz;
            HA_m += Profile_m[ProfileStart];
            HB_m += Profile_m[ProfileEnd];

            NuMaxOut nu;
            nu = FindMaxNu(HA_m, HB_m, wavelength, dAB, Profile_m, ProfileStart, ProfileEnd, rE);

            float nuP = nu.nuMax;
            //int p = nu.nMax;

            float dN = dAB / (ProfileEnd - ProfileStart);
            float dAP = dN * nu.nMax;
            
            float dPB = dN * (ProfileEnd - ProfileStart - nu.nMax);
            
            if (nuP > -0.78)
            {
                //float[] profileAP = new float[p];
                //Array.Copy(profile_m, profileAP, p);
                NuMaxOut nuT = FindMaxNu(HA_m, Profile_m[nu.nMax], wavelength, dAP, in Profile_m, ProfileStart, nu.nMax, rE);

                //float[] profilePB = new float[profile_m.Length - p];
                //Array.Copy(profile_m, p, profilePB, profile_m.Length - p - 1, profile_m.Length - p - 2);
                NuMaxOut nuR = FindMaxNu(Profile_m[nu.nMax], HB_m, wavelength, dPB, in Profile_m, nu.nMax, ProfileEnd, rE);
                //D = math.sqrt(2 * r_e) * (math.sqrt(h_a) + math.sqrt(h_b));
                //C = 10.0 + 0.04 * D;
                float C = 10.0f + 0.04f * D_km;
                diffractionLoss.loss_dB = (float)(J(nuP) + (1.0 - Math.Exp(- J(nuP) / 6.0f)) * (J(nuT.nuMax) + J(nuR.nuMax) + C));
            }
            else diffractionLoss.loss_dB = 0;

            if (nuP <= 0)
            {
                diffractionLoss.alphaToB_deg = CalcElevAngle(HA_m, HB_m, dAB, rE);
                diffractionLoss.alphaToA_deg = CalcElevAngle(HB_m, HA_m, dAB, rE);
            }
            else
            {
                diffractionLoss.alphaToB_deg = CalcElevAngle(HA_m, Profile_m[nu.nMax], dAP, rE);
                diffractionLoss.alphaToA_deg = CalcElevAngle(HB_m, Profile_m[nu.nMax], dPB, rE);
            }
            return diffractionLoss;
        }
    }
}
