using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.Platform.Logging;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class CreateFileEwx
    {
        private ILogger _logger { get; set; }
        public CreateFileEwx(ILogger logger)
        {
            this._logger = logger;
        }

        public bool CreateFile(string Path, EwxData ewx)
        {
            bool isSuccessCreateEwxFile = false;
            try
            {
                TextWriter text = File.CreateText(Path);
                XmlTextWriter writer = new XmlTextWriter(text);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("ATDI_EWF");
                writer.WriteStartElement("Header");
                writer.WriteElementString("VERSION", ewx.Header.Version == null ? "1" : ewx.Header.Version);
                writer.WriteElementString("UNICODE", ewx.Header.Unicode == null ? "0" : ewx.Header.Unicode);
                writer.WriteElementString("TYPE", ewx.Header.Type == null ? "1" : ewx.Header.Type);
                writer.WriteElementString("COUNT_STATIONS", XmlConvert.ToString(ewx.Header.CountStation));
                writer.WriteElementString("COUNT_MWS", XmlConvert.ToString(ewx.Header.CountMWS));
                writer.WriteEndElement();


                for (int i = 0; i < ewx.Stations.Length; i++)
                {
                    var bts = ewx.Stations[i];
                    writer.WriteStartElement("STATION");
                    writer.WriteStartElement("RECORD");
                    writer.WriteElementString("TYPE_COORD", bts.TypeCoord == null ? "162DEC" : bts.TypeCoord);
                    writer.WriteElementString("Category", bts.Category == null ? "2" : bts.Category); // 2 is generic signal type
                    writer.WriteElementString("CALL_SIGN", bts.CallSign);
                    writer.WriteElementString("ADDRESS", bts.Address);
                    writer.WriteElementString("ALTITUDE", Convert.ToString(bts.Altitude));
                    writer.WriteElementString("NOMINAL_POWER", Convert.ToString(bts.NominalPower));
                    writer.WriteElementString("FREQUENCY", Convert.ToString(bts.Frequency));
                    writer.WriteElementString("BANDWIDTH", Convert.ToString(bts.Bandwidth));
                    writer.WriteElementString("BANDWIDTHRX", Convert.ToString(bts.BandwidthRx));
                    writer.WriteElementString("H_ANTENNA", Convert.ToString(bts.HAntenna));
                    writer.WriteElementString("AZIMUTH", Convert.ToString(bts.Azimuth));
                    writer.WriteElementString("TILT", Convert.ToString(bts.Tilt));
                    writer.WriteElementString("GAIN", Convert.ToString(bts.Gain));
                    writer.WriteElementString("GAINRX", Convert.ToString(bts.GainRx));
                    writer.WriteElementString("LOSSES", Convert.ToString(bts.Losses));
                    writer.WriteElementString("LOSSESRX", Convert.ToString(bts.LossesRx));
                    writer.WriteElementString("COORD_X", Convert.ToString(bts.CoordX));
                    writer.WriteElementString("COORD_Y", Convert.ToString(bts.CoordY));
                    writer.WriteElementString("INFO1", bts.Info1);
                    writer.WriteElementString("NETID", Convert.ToString(bts.NetId));
                    writer.WriteElementString("POLAR", bts.Polar);
                    writer.WriteElementString("POLARRX", bts.PolarRx);
                    var diagH = bts.DiagH;
                    var diagV = bts.DiagV;
                    string diagHNameTag = null;
                    string diagVNameTag = null;
                    if (CheckDiagH(ref diagH, out diagHNameTag))
                    {
                        writer.WriteElementString(diagHNameTag, diagH);
                    }
                    if (CheckDiagV(ref diagV, out diagVNameTag))
                    {
                        writer.WriteElementString(diagVNameTag, diagV);
                    }

                    writer.WriteElementString("D_cx1", Convert.ToString(bts.D_cx1));
                    writer.WriteElementString("U_cx1", Convert.ToString(bts.U_cx1));
                    writer.WriteElementString("Downlink_cx", "1");
                    writer.WriteElementString("Uplink_cx", "1");
                    double KTBF = -174 + 7 + 10 * Math.Log10(bts.Bandwidth);
                    writer.WriteElementString("FKTB", Convert.ToString(KTBF));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Close();
                isSuccessCreateEwxFile = true;

                this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveEWXFileCompleted.ToString(), Path));
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCreateEwxFile;
        }



        private  bool CheckDiagH(ref string diagH, out string NameTag)
        {
            if (diagH.Contains("WIEN "))
            {
                diagH = diagH.Replace("WIEN ", "");
                NameTag = "WiencodeH";
                return true;
            }
            else if (diagH.Contains("VECTOR"))
            {
                diagH = diagH.Replace("VECTOR 10", "");
                var array = ParseStringToArray(diagH);
                var arrayPolarH = InterpolationForICSTelecomHorizontal(array);
                diagH = GetFormatArray(arrayPolarH);
                NameTag = "DIAG_H";
                return true;
            }
            else
            {
                NameTag = null;
                return false;
            }
        }

        private  bool CheckDiagV(ref string diagV, out string NameTag)
        {
            if (diagV.Contains("WIEN "))
            {
                diagV = diagV.Replace("WIEN ","");
                NameTag = "WiencodeV";
                return true;
            }
            if (diagV.Contains("VECTOR"))
            {
                diagV = diagV.Replace("VECTOR 10", "");
                var array = ParseStringToArray(diagV);
                var arrayPolarV = InterpolationForICSTelecomVertical(array);
                diagV = GetFormatArray(arrayPolarV);
                NameTag = "DIAG_V";
                return true;
            }
            else
            {
                NameTag = null;
                return false;
            }
        }


        private string GetFormatArray(double[] inArr)
        {
            string outString = "";
            if (inArr != null)
            {
                for (int i = 0; i < inArr.Length; i++)
                {
                    var s = Math.Round(inArr[i]).ToString().Replace(",", ".");
                    if (s.Length==1)
                    {
                        s = "0" + s;
                    }
                    s = s.PadLeft(4, ' ');
                    outString += s;
                }
            }
            return outString;
        }

        private  double[] ParseStringToArray(string diag)
        {
            string[] inArr = diag.Split(new char[] { '\t', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double[] inArrDouble = null;
            if (inArr != null)
            {
                inArrDouble = new double[inArr.Length];
                for (int i = 0; i < inArr.Length; i++)
                {
                    inArrDouble[i] = Convert.ToDouble(inArr[i]);
                }
            }
            return inArrDouble;
        }


        private double[] InterpolationForICSTelecomHorizontal(double[] inArray)
        {
            const int MinCount = 36;
            var result = new double[72];
            if (inArray != null)
            {
                if (inArray.Length < MinCount)
                {
                    throw new InvalidOperationException($"Incorrect count element in inArray ({inArray.Length} < {MinCount})  in the InterpolationForICSTelecomHorizontal method");
                }
                for (int i = 0; i < 36; i++)
                {
                    result[2 * i] = inArray[i];
                    if (i == 35) { result[2 * i + 1] = (inArray[i] + inArray[0]) / 2.0; }
                    else { result[2 * i + 1] = (inArray[i] + inArray[i + 1]) / 2.0; }
                }
            }
            else
            {
                throw new InvalidOperationException("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!");
            }
            return result;
        }


        private double[] InterpolationForICSTelecomVertical(double[] inArray)
        {
            var result = new double[179];
            const int MinCount = 19;
            if (inArray != null)
            {
                if (inArray.Length < MinCount)
                {
                    throw new InvalidOperationException($"Incorrect count element in inArray ({inArray.Length} < {MinCount})  in the InterpolationForICSTelecomVertical method");
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if ((i != 0) || (j != 0))
                        {
                            result[10 * i + j - 1] =  inArray[i] + j * (inArray[i + 1] - inArray[i]) / (10);
                        }
                    }
                }
                for (int i = 9; i < 18; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        result[10 * i + j - 1] = inArray[i] + j * (inArray[i + 1] - inArray[i]) / (10);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("The 'inArray' input parameter in the InterpolationForICSTelecomVertical method is null!");
            }
            return result;
        }
    }
}
