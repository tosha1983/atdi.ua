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
using Atdi.Common;


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
                    var id = ewx.Stations[i].Id;
                    var bts = ewx.Stations[i];
                    string diagHNameTag = null;
                    string diagVNameTag = null;
                    var diagH = bts.DiagH;
                    var diagV = bts.DiagV;

                    try
                    {
                        if ((CheckDiagH(ref diagH, out diagHNameTag) == false) || (CheckDiagV(ref diagV, out diagVNameTag) == false))
                        {
                            this._logger.Info(Contexts.CalcCoverages, (EventText)$"Reject station Id = '{id}'");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.Exception(Contexts.CalcCoverages, (EventCategory)$"Reject station Id = '{id}'", ex);
                        continue;
                    }

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
                    writer.WriteElementString(diagHNameTag, diagH);
                    writer.WriteElementString(diagVNameTag, diagV);
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
            else if (diagH.Contains("VECTOR 10"))
            {
                diagH = diagH.Replace("VECTOR 10", "").Replace(",",".");
                var array = ParseStringToArray(diagH);
                var arrayPolarH = InterpolationForICSTelecomHorizontal(array);
                diagH = GetFormatArray(arrayPolarH);
                NameTag = "DIAG_H";
                return true;
            }
            else if (diagH.Contains("VECTOR 5"))
            {
                diagH = diagH.Replace("VECTOR 5", "").Replace(",", ".");
                var array = ParseStringToArray(diagH);
                if (array.Length==72)
                {
                    diagH = GetFormatArray(array);
                }
                //var arrayPolarH = InterpolationForICSTelecomHorizontal(array);
                //diagH = GetFormatArray(arrayPolarH);
                NameTag = "DIAG_H";
                return true;
            }
            else if (diagH.Contains("POINTS"))
            {
                diagH = diagH.Replace("POINTS ", "").Replace(",", ".");
                if (!string.IsNullOrEmpty(diagH))
                {
                    var isNotZero = isNotZeroValuesOnPointObjects(diagH);
                    if (isNotZero)
                    {
                        var array = ParseStringToPointObjects(diagH);
                        var arrayPolarH = InterpolationPointForICSTelecomHorizontal(array);
                        diagH = GetFormatArray(arrayPolarH);
                    }
                    else
                    {
                        diagH = "OMNI";
                    }
                }
                else
                {
                    diagH = "OMNI";
                }
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
            if (diagV.Contains("VECTOR 10"))
            {
                diagV = diagV.Replace("VECTOR 10", "").Replace(",", ".");
                var array = ParseStringToArray(diagV);
                var arrayPolarV = InterpolationForICSTelecomVertical(array);
                diagV = GetFormatArray(arrayPolarV);
                NameTag = "DIAG_V";
                return true;
            }
            else if (diagV.Contains("VECTOR 5"))
            {
                //diagV = diagV.Replace("VECTOR 5", "").Replace(",", ".");
                //var array = ParseStringToArray(diagV);
                NameTag = "DIAG_V";
                throw new InvalidOperationException($"DiagV value = '{diagV}' not support");
                //var arrayPolarV = InterpolationForICSTelecomVertical(array);
                //diagV = GetFormatArray(arrayPolarV);
                //return true;
            }
            else if (diagV.Contains("POINTS"))
            {
                diagV = diagV.Replace("POINTS ", "").Replace(",", ".");
                if (!string.IsNullOrEmpty(diagV))
                {
                    var isNotZero = isNotZeroValuesOnPointObjects(diagV);
                    if (isNotZero)
                    {
                        var array = ParseStringToPointObjects(diagV);
                        var arrayPolarV = InterpolationPointForICSTelecomVertical(array);
                        diagV = GetFormatArray(arrayPolarV);
                    }
                    else
                    {
                        diagV = "OMNI";
                    }
                }
                else
                {
                    diagV = "OMNI";
                }
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

        private PointObject[] ParseStringToPointObjects(string diag)
        {
            PointObject[] pointObjects = null;
            string[] inArr = diag.Split(new char[] { '\t', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if ((inArr != null) && (inArr.Length > 1))
            {
                int size = (int)(inArr.Length / 2);
                if (inArr.Length % 2 == 0)
                {
                    int idx = 0;
                    pointObjects = new PointObject[size];
                    for (int i = 0, j = 1; (i < inArr.Length - 1 && j < inArr.Length); i = i + 2, j = j + 2)
                    {
                        var pointObject = new PointObject();
                        pointObject.Azimuth = inArr[i].ConvertStringToDouble().Value;
                        pointObject.Value = inArr[j].ConvertStringToDouble().Value;
                        //pointObject.Azimuth = Convert.ToDouble(inArr[i]);
                        //pointObject.Value = Convert.ToDouble(inArr[j]);
                        pointObjects[idx] = pointObject;
                        idx++;
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Dimension array must be an even number");
                }
            }
            return pointObjects;
        }

        private bool isNotZeroValuesOnPointObjects(string diag)
        {
            var isChecked = false;

            var inArr = diag.Split(new char[] { '\t', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if ((inArr != null) && (inArr.Length > 1))
            {
                int size = (int)(inArr.Length / 2);
                if (inArr.Length % 2 == 0)
                {
                    for (int i = 0, j = 1; (i < inArr.Length - 1 && j < inArr.Length); i = i + 2, j = j + 2)
                    {
                        var pointObject = new PointObject();
                        pointObject.Azimuth = inArr[i].ConvertStringToDouble().Value;
                        pointObject.Value = inArr[j].ConvertStringToDouble().Value;
                        //pointObject.Azimuth = Convert.ToDouble(inArr[i]);
                        //pointObject.Value = Convert.ToDouble(inArr[j]);
                        if (pointObject.Value > 0)
                        {
                            isChecked = true;
                            break;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Dimension array must be an even number");
                }
            }
            return isChecked;
        }


        private double[] ParseStringToArray(string diag)
        {
            string[] inArr = diag.Split(new char[] { '\t', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double[] inArrDouble = null;
            if (inArr != null)
            {
                inArrDouble = new double[inArr.Length];
                for (int i = 0; i < inArr.Length; i++)
                {
                    //inArrDouble[i] = Convert.ToDouble(inArr[i]);
                    inArrDouble[i] = inArr[i].ConvertStringToDouble().Value;
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

        private double[] InterpolationPointForICSTelecomHorizontal(PointObject[] inArray)
        {
            var result = new double[72];
            if (inArray != null)
            {
                int j = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    double degre_out = i * 5.0;
                    while ((inArray.Length - 1 > j) && (inArray[j].Azimuth < degre_out))
                    { j++; }
                    if (j == 0)
                    {
                        result[i] = inArray[0].Value;
                    }
                    else if (j == inArray.Length - 1)
                    {
                        if (inArray[0].Azimuth == 0)
                        {
                            result[i] = inArray[j].Value + (degre_out - inArray[j].Azimuth) * (inArray[0].Value - inArray[j].Value) / (360 - inArray[j].Azimuth);
                        }
                        else
                        {
                            result[i] = inArray[inArray.Length - 1].Value;
                        }
                    }
                    else
                    {
                        result[i] = inArray[j - 1].Value + (degre_out - inArray[j - 1].Azimuth) * (inArray[j].Value - inArray[j - 1].Value) / (inArray[j].Azimuth - inArray[j - 1].Azimuth);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!");
            }
            return result;
        }


        private double[] InterpolationPointForICSTelecomVertical(PointObject[] inArray)
        {
            var result = new double[179];
            if (inArray != null)
            {
                int j = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    double degre_out = i - 89.0;
                    while ((inArray.Length - 1 > j) && (inArray[j].Azimuth < degre_out))
                    {
                        j++;
                    }
                    if (j == 0)
                    {
                        result[i] = inArray[0].Value;
                    }
                    else if (j == inArray.Length - 1)
                    {
                        result[i] = inArray[inArray.Length - 1].Value;
                    }
                    else
                    {
                        result[i] = inArray[j - 1].Value + (degre_out - inArray[j - 1].Azimuth) * (inArray[j].Value - inArray[j - 1].Value) / (inArray[j].Azimuth - inArray[j - 1].Azimuth);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("The 'inArray' input parameter in the InterpolationForICSTelecomVertical method is null!");
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
