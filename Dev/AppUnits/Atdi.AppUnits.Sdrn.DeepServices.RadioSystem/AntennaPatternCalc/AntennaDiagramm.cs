using System;
using System.Collections.Generic;
using System.Globalization;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern
{
    public class AntennaDiagramm: AntennaDiagrammArgs
    {

        //=============================================================
        /// <summary>
        /// Constructor
        /// </summary>
        public AntennaDiagramm()
        {
            Angles = new List<double>();
            Losses = new List<double>();
        }
        //=============================================================
        /// <summary>
        /// Test the corrected data
        /// </summary>
        /// <returns>true - OK; false - Error</returns>
        public bool IsValid()
        {
            if (Angles.Count == Losses.Count && Losses.Count > 0 || Type.Length > 0)
                return true;
            else
                return false;
        }
        //=============================================================
        /// <summary>
        /// Set max gain
        /// </summary>
        /// <param name="Gain">Max gain</param>
        public void SetMaximalGain(double Gain)
        {
            MaximalGain = Gain;
        }
        //=============================================================
        /// <summary>
        /// Create the directional diagram from string
        /// </summary>
        /// <param name="Points">string of the directional diagram</param>
        public TypePattern Build(String Points)
        {
            string[] split = Points.Split(new Char[] { ' ' });

            if (split[0] == "FOREIGN")
            {
                string newPoints = ReformatPoints(Points);
                BuildFromPoints(newPoints);
                return TypePattern.FOREIGN;
            }
            else if (split[0] == "POINTS")
            {
                BuildFromPoints(Points);
                return TypePattern.POINTS;
            }
            else if (split[0] == "WIEN")
            {
                SetWienType(Points);
                return TypePattern.WIEN;
            }
            else if (split[0] == "VECTOR")
            {
                BuildFromVector(Points);
                return TypePattern.VECTOR;
            }
            else
            {
                return TypePattern.UNKNOWN;
            }
        }
        //=============================================================
        /// <summary>
        /// Преобразовывает строку диаграмы направленности в нермальный вид
        /// </summary>
        /// <param name="points">Диаграма направленности</param>
        /// <returns>Возвращает нормальный вид</returns>
        public string ReformatPoints(string points)
        {
            string retVal = "";
            bool isPutSpace = false;
            foreach (char nextChar in points)
            {
                if ((nextChar == ' ') || (nextChar == '\t') || (nextChar == '\r') || (nextChar == '\n'))
                {
                    if (isPutSpace == false)
                    {
                        isPutSpace = true;
                        retVal += " ";
                    }
                }
                else
                {
                    retVal += nextChar;
                    isPutSpace = false;
                }
            }
            return retVal;
        }
        //=============================================================
        /// <summary>
        /// Return losses
        /// </summary>
        /// <param name="Degree">degree</param>
        /// <returns>losses</returns>
        public double GetLossesAmount(double Degree)
        {
            if (Type.Length > 0)
            {
                AntennaDiagrammClass CalculatedDiagram = new AntennaDiagrammClass(Alpha, Type, Aff);
                return CalculatedDiagram.GetValue(Degree, MaximalGain);
            }
            else
            {
                return Interpolate(Degree);
            }
        }

        private void SetWienType(String Points)
        {
            if (Points.Length > 0)
            {
                string[] split = Points.Split(new Char[] { ' ' });
                try
                {
                    String wienCode = split[1];
                    Alpha = Convert.ToDouble(wienCode.Substring(0, 3));
                    Type = wienCode.Substring(3, 2);
                    Aff = Convert.ToDouble(wienCode.Substring(5));
                }
                catch
                {
                    //MessageBox.Show("Cannot parse "+Points+"\r\nThis is WIEN type abbr"); 
                    Type = "";
                    Angles.Clear();
                    Losses.Clear();
                }
            }
        }

        private void BuildFromPoints(String Points)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string[] split = Points.Split(new Char[] { ' ' });

            if ((split[0] == "POINTS") || (split[0] == "FOREIGN"))
            {
                for (int k = 1; k < split.Length; k++)
                {
                    string s = string.IsNullOrEmpty(split[k]) ? "" : split[k];
                    double result = 0.0;
                    bool isConverted = true;
                    string newString = s.Replace(',', '.');
                    try
                    {
                        result = Convert.ToDouble(newString, provider);
                    }
                    catch
                    {
                        isConverted = false;
                    }

                    if (isConverted == false)
                    {
                        k++;
                        continue;
                    }

                    if (k % 2 != 0)
                        Angles.Add(result);
                    else
                        Losses.Add(MaximalGain - result);
                }
                if (Angles.Count != Losses.Count)
                {

                }
                Angles.Add(360);
                Losses.Add(Losses[0]);
            }
        }

        private void BuildFromVector(string points)
        {
            string[] split = points.Split(new Char[] { ' ' });

            if (split[0] == "VECTOR")
            {
                string OldSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

                int i = 0;
                foreach (string s in split)
                {
                    double Result;
                    string newString = s;
                    if (s.Contains("."))
                        newString = s.Replace(".", OldSeparator);

                    if (Double.TryParse(newString, out Result))
                    {
                        Losses.Add(Result);
                        Angles.Add(i);
                        i += 10;
                    }
                }
            }
        }

        private double Interpolate(double Degree)
        {
            if (Angles.Count == 1)
                return Losses[0];
            else
            {
                if (Degree < 0)
                    Degree += 360.0;
                int last = Angles.Count - 1;
                for (int i = 0; i < last; i++)
                {
                    try
                    {
                        double df = Angles[i + 1] - Angles[i];
                        double dl = Losses[i + 1] - Losses[i];
                        if (Degree >= Angles[i] && Degree <= Angles[i + 1])
                        {
                            double k = dl / df;
                            double b = Losses[i] - k * Angles[i];

                            return k * Degree + b;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return 0.0;
        }
    };
}
