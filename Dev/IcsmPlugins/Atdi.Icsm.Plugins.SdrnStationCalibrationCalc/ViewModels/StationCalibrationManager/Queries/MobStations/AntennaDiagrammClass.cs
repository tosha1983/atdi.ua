using System;
using System.Collections.Generic;
using System.Text;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class AntennaDiagrammClass
    {
        //=============================================================
        private double Alpha;
        private string DiagramType;
        private double Aff;
        //=============================================================
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AlphaParam"></param>
        /// <param name="DiagramTypeParam"></param>
        /// <param name="AffParam"></param>
        public AntennaDiagrammClass(double AlphaParam, string DiagramTypeParam, double AffParam)
        {
            Alpha = AlphaParam;
            DiagramType = DiagramTypeParam;
            Aff = AffParam / 100.0;
        }

        public double GetValue(double Phi, double MaximalGain)
        {
            double AntenneFactor = 1.0;

            if (DiagramType == "CA")
                AntenneFactor = GetCA(Phi);
            else if (DiagramType == "CB")
                AntenneFactor = GetCB(Phi);
            else if (DiagramType == "CC")
                AntenneFactor = GetCC(Phi);
            else if (DiagramType == "EA")
                AntenneFactor = GetEA(Phi);
            else if (DiagramType == "EB")
                AntenneFactor = GetEB(Phi);
            else if (DiagramType == "EC")
                AntenneFactor = GetEC(Phi);
            else if (DiagramType == "DE")
                AntenneFactor = GetDE(Phi);
            else if (DiagramType == "KA")
                AntenneFactor = GetKA(Phi);
            else if (DiagramType == "LA")
                AntenneFactor = GetLA(Phi);
            else if (DiagramType == "TA")
                AntenneFactor = GetTA(Phi);
            else if (DiagramType == "ND")
                AntenneFactor = 1.0;

            double R = System.Math.Abs(20.0 * System.Math.Log10(AntenneFactor));
            if (R > 40.0)
                R = 40.0;

            return MaximalGain - R;
        }

        //+++
        private double GetEA(double Phi)
        {
            Phi = Phi % 360.0;
            if (Phi >= 90 && Phi <= 270)
                return Aff;
            double CosAlpha = System.Math.Cos(Position.DegreeToRadian(Alpha));
            double Pwa = CosAlpha * CosAlpha;
            double B2 = 0.5 * (1 - Pwa) /
               (1 - System.Math.Pow(System.Math.Sqrt(2) * System.Math.Cos(Position.DegreeToRadian(Alpha)) - 1, 2));

            double CosPhi = System.Math.Cos(Position.DegreeToRadian(Phi));
            double Pwp = CosPhi * CosPhi;
            double R = (4 * B2 * Pwp) / ((4 * B2 - 1.0) * Pwp + 1);

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetLA(double Phi)
        {
            double PhiLow = -3.0 * Alpha / 2.0;
            double PhiUp = 3.0 * Alpha / 2.0;

            Phi = Phi % 360;
            if (Phi > 180)
                Phi -= 360;

            if (Phi >= PhiLow && Phi <= PhiUp)
            {
                double W = System.Math.Cos(Position.DegreeToRadian(60.0 / Alpha * Phi));
                double R = System.Math.Cos(Position.DegreeToRadian((1.0 - W) * 90.0));

                return R;
            }
            else
            {
                return Aff;
            }
        }

        //+++
        private double GetDE(double Phi)
        {
            Phi = Phi % 360.0;
            double CosAlpha = System.Math.Cos(Position.DegreeToRadian(Alpha));
            double Pwa = CosAlpha * CosAlpha;
            double B2 = (1 - Pwa) / (2.0 - System.Math.Pow((2.0 * System.Math.Cos(Position.DegreeToRadian(Alpha)) - System.Math.Sqrt(2.0)), 2.0));

            double Pwp = System.Math.Pow(System.Math.Cos(Position.DegreeToRadian(Phi)), 2.0);
            double R = System.Math.Abs((4.0 * B2 * Pwp) / ((4.0 * B2 - 1.0) * Pwp + 1.0));

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetEB(double Phi)
        {
            Phi = Phi % 360.0;
            double Pwa = System.Math.Pow(System.Math.Cos(Position.DegreeToRadian(Alpha)), 2.0);
            double B2 = 0.77 * (1.0 - Pwa) / (1.44 - System.Math.Pow(System.Math.Sqrt(2.0) * System.Math.Cos(Position.DegreeToRadian(Alpha)) - 0.8, 2.0));

            // Algorithme	
            double Pwp = System.Math.Pow(System.Math.Cos(Position.DegreeToRadian(Phi)), 2.0);
            double R = (1.6 * B2 * System.Math.Cos(Position.DegreeToRadian(Phi)) + 2.4 * System.Math.Sqrt(B2 * (B2 - 0.2) * Pwp + 0.2 * B2)) / ((4 * B2 - 1.44) * Pwp + 1.44);

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetEC(double Phi)
        {
            Phi = Phi % 360.0;
            double Pwa = System.Math.Pow(System.Math.Cos(Position.DegreeToRadian(Alpha)), 2.0);
            double B2 = 0.98 * (1.0 - Pwa) / (1.96 - System.Math.Pow(System.Math.Sqrt(2.0) * System.Math.Cos(Position.DegreeToRadian(Alpha)) - 0.6, 2.0));

            // Algorithme
            double Pwp = System.Math.Pow(System.Math.Cos(Position.DegreeToRadian(Phi)), 2.0);
            double R = (1.2 * B2 * System.Math.Cos(Position.DegreeToRadian(Phi)) + 2.8 * System.Math.Sqrt(B2 * (B2 - 0.4) * Pwp + 0.4 * B2)) / ((4.0 * B2 - 1.96) * Pwp + 1.96);

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetCA(double Phi)
        {
            Phi = Phi % 360.0;
            double tmpAlpha = Alpha / 100.0;
            double U = 1.0 - tmpAlpha * tmpAlpha;

            double V = System.Math.Cos(Position.DegreeToRadian(2.0 * Phi));
            double Temp = U * V + System.Math.Sqrt(U * U * V * V + 4 * tmpAlpha * tmpAlpha);

            double R = 0.0;
            if (Temp > 0.0)
                R = System.Math.Sqrt(Temp / 2.0);

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetCB(double Phi)
        {
            Phi = Phi % 360.0;
            double tmpAlpha = Alpha / 100.0;
            double U = 1.0 - tmpAlpha * tmpAlpha;

            double V = System.Math.Cos(Position.DegreeToRadian(3.0 * Phi));
            double Temp = U * V + System.Math.Sqrt(U * U * V * V + 4 * tmpAlpha * tmpAlpha);

            double R = 0.0;
            if (Temp > 0.0)
                R = System.Math.Sqrt(Temp / 2.0);

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetCC(double Phi)
        {
            Phi = Phi % 360.0;
            double tmpAlpha = Alpha / 100.0;
            double U = 1.0 - tmpAlpha * tmpAlpha;

            double V = System.Math.Cos(Position.DegreeToRadian(4.0 * Phi));

            double Temp = U * V + System.Math.Sqrt(U * U * V * V + 4 * tmpAlpha * tmpAlpha);

            double R = 0.0;
            if (Temp > 0.0)
                R = System.Math.Sqrt(Temp / 2.0);

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetKA(double Phi)
        {
            Phi = Phi % 360.0;
            double tmpAlpha = Alpha / 100.0;
            double U = 1.0 - tmpAlpha;

            double V = System.Math.Cos(Position.DegreeToRadian(Phi));
            double R = (U * V + System.Math.Sqrt(U * U * V * V + 4 * tmpAlpha)) / 2.0;

            if (R < Aff)
                R = Aff;

            return R;
        }

        //+++
        private double GetTA(double Phi)
        {
            Phi = Phi % 360.0;
            double tmpAlpha = Alpha * 0.1;
            double tmpAff = Aff * 100.0 * 0.01;

            double N = -0.1505 / System.Math.Log10(System.Math.Cos(Position.DegreeToRadian(tmpAlpha)));
            double V = System.Math.Cos(Position.DegreeToRadian(Phi));

            double R = 0;
            if (V > 0)
                R = System.Math.Pow(V, N);

            if (R < tmpAff)
                R = tmpAff;

            return R;
        }

        private double GetV(double Phi, string AntennaType)
        {
            int IntAlpha = Convert.ToInt32(Alpha);
            double M = Convert.ToDouble(IntAlpha / 100);
            double Nn = Convert.ToDouble(IntAlpha % 100);
            double Rr = Aff;

            double Alpha1 = M * 5.0 + 15.0;    // Algorithmes
            double Beta = Nn;
            double R0 = Rr / 100.0;
            double E = 0;

            if (AntennaType == "VB")
                E = 0.05;

            if (AntennaType == "VC")
                E = 0.1;

            if (AntennaType == "VD")
                E = 0.15;

            if (AntennaType == "VE")
                E = 0.2;

            if (AntennaType == "VF")
                E = 0.25;

            if (AntennaType == "VG")
                E = 0.3;

            if (AntennaType == "VH")
                E = 0.35;

            if (AntennaType == "VI")
                E = 0.40;

            double K5 = (1.0 + E) * (1.0 + E) / 4.0;

            //double B2 = K5/2.0*(1.0-System.Math.Cos(XICSM.Math.DegreeToRadian(Alpha))*System.Math.Cos(XICSM.Math.DegreeToRadian(Alpha)))/
            //(K5 - (System.Math.Cos(XICSM.Math.DegreeToRadian(Alpha))/System.Math.Sqrt(2.0)-(1-e)/2)*(cos(alphac)/sqrt(2.0)-(1-e)/2) );

            return 1;
        }
    }
}
