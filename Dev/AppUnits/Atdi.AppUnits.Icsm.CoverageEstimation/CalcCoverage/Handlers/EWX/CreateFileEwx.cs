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
using Atdi.AppUnits.Icsm.CoverageEstimation.Utilities;
using Atdi.AppUnits.Icsm.CoverageEstimation.Localization;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class CreateFileEwx
    {
        private ILogger _logger { get; set; }
        private DataConfig _dataConfig { get; set; }
        public CreateFileEwx(DataConfig dataConfig, ILogger logger)
        {
            this._logger = logger;
            this._dataConfig = dataConfig;
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

                bool isFindStationData = false;

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
                            this._logger.Error(Contexts.CalcCoverages, (EventText)$"{CLocaliz.TxT("Reject station Id")} = '{id}'");
                            Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Reject station Id")} = '{id}'");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.Exception(Contexts.CalcCoverages, (EventCategory)$"{CLocaliz.TxT("Reject station Id")} = '{id}'", ex);
                        Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Reject station Id")} = '{id}'");
                        continue;
                    }

                    isFindStationData = true;

                    writer.WriteStartElement("STATION");
                    writer.WriteStartElement("RECORD");
                    writer.WriteElementString("TYPE_COORD", bts.TypeCoord == null ? "162DEC" : bts.TypeCoord);
                    writer.WriteElementString("Category", bts.Category == null ? "2" : bts.Category); // 2 is generic signal type
                    //writer.WriteElementString("CALL_SIGN", bts.CallSign);
                    writer.WriteElementString("CALL_SIGN", Guid.NewGuid().ToString().SubString(13).Replace("-", "_"));
                    writer.WriteElementString("ADDRESS", bts.Address);
                    writer.WriteElementString("ALTITUDE", Convert.ToString(bts.Altitude).Replace(",","."));
                    writer.WriteElementString("NOMINAL_POWER", Convert.ToString(bts.NominalPower).Replace(",", "."));
                    writer.WriteElementString("FREQUENCY", Convert.ToString(bts.Frequency).Replace(",", "."));
                    writer.WriteElementString("BANDWIDTH", Convert.ToString(bts.Bandwidth).Replace(",", "."));
                    writer.WriteElementString("BANDWIDTHRX", Convert.ToString(bts.BandwidthRx).Replace(",", "."));
                    writer.WriteElementString("H_ANTENNA", Convert.ToString(bts.HAntenna).Replace(",", "."));
                    writer.WriteElementString("AZIMUTH", Convert.ToString(bts.Azimuth).Replace(",", "."));
                    writer.WriteElementString("TILT", Convert.ToString(bts.Tilt).Replace(",", "."));
                    //writer.WriteElementString("TILT", "-9");
                    writer.WriteElementString("GAIN", Convert.ToString(bts.Gain).Replace(",", "."));
                    writer.WriteElementString("GAINRX", Convert.ToString(bts.GainRx).Replace(",", "."));
                    writer.WriteElementString("LOSSES", Convert.ToString(bts.Losses).Replace(",", "."));
                    writer.WriteElementString("LOSSESRX", Convert.ToString(bts.LossesRx).Replace(",", "."));
                    writer.WriteElementString("COORD_X", Convert.ToString(bts.CoordX).Replace(",", "."));
                    writer.WriteElementString("COORD_Y", Convert.ToString(bts.CoordY).Replace(",", "."));
                    writer.WriteElementString("INFO1", bts.Info1);
                    writer.WriteElementString("NETID", Convert.ToString(bts.NetId));
                    writer.WriteElementString("POLAR", bts.Polar);
                    writer.WriteElementString("POLARRX", bts.PolarRx);
                    writer.WriteElementString(diagHNameTag, diagH);
                    writer.WriteElementString(diagVNameTag, diagV);
                    writer.WriteElementString("D_cx1", Convert.ToString(bts.D_cx1).Replace(",", "."));
                    writer.WriteElementString("U_cx1", Convert.ToString(bts.U_cx1).Replace(",", "."));
                    writer.WriteElementString("Downlink_cx", "1");
                    writer.WriteElementString("Uplink_cx", "1");
                    double KTBF = -174 + 7 + 10 * Math.Log10(bts.Bandwidth);
                    writer.WriteElementString("FKTB", Convert.ToString(KTBF).Replace(",", "."));
                    writer.WriteEndElement();
                    writer.WriteEndElement();


                }
                writer.WriteEndElement();
                writer.Close();
                if (isFindStationData)
                {
                    isSuccessCreateEwxFile = true;
                    this._logger.Info(Contexts.CalcCoverages, string.Format(CLocaliz.TxT(Events.OperationSaveEWXFileCompleted.ToString()), Path));
                }
                else
                {
                    File.Delete(Path);
                }
            }
            catch (Exception e)
            {
               this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCreateEwxFile;
        }



        private  bool CheckDiagH(ref string diagH, out string NameTag)
        {
            if (string.IsNullOrEmpty(diagH))
            {
                NameTag = null;
                return false;
            }
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
                diagH = GetFormatArrayHorizontal(arrayPolarH);
                NameTag = "DIAG_H";
                return true;
            }
            else if (diagH.Contains("VECTOR 5"))
            {
                diagH = diagH.Replace("VECTOR 5", "").Replace(",", ".");
                var array = ParseStringToArray(diagH);
                if (array.Length==72)
                {
                    diagH = GetFormatArrayHorizontal(array);
                }
                NameTag = "DIAG_H";
                return true;
            }
            else if (diagH.Contains("VECTOR 1"))
            {
                diagH = diagH.Replace("VECTOR 1", "").Replace(",", ".");
                var array = ParseStringToArray(diagH);
                var arrayPolarH = InterpolationForICSTelecomHorizontalOneDeg(array);
                diagH = GetFormatArrayHorizontal(arrayPolarH);
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

                        for (int i=0; i< array.Length; i++)
                        {
                            if (array[i].Azimuth<0)
                            {
                                array[i].Azimuth = 360 + array[i].Azimuth;
                            }
                        }
                        var sortedarray = from u in array
                                          orderby u.Azimuth ascending
                                          select u;

                        var arrOrdered = sortedarray.ToArray();
                        var arrayPolarH = InterpolationPointForICSTelecomHorizontal(arrOrdered);
                        diagH = GetFormatArrayHorizontal(arrayPolarH);
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
            if (string.IsNullOrEmpty(diagV))
            {
                NameTag = null;
                return false;
            }
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
                diagV = GetFormatArrayVertical(arrayPolarV);
                NameTag = "DIAG_V";
                return true;
            }
            if (diagV.Contains("VECTOR 1"))
            {
                diagV = diagV.Replace("VECTOR 1", "").Replace(",", ".");
                var array = ParseStringToArray(diagV);
                diagV = GetFormatArrayVertical(array);
                NameTag = "DIAG_V";
                return true;
            }
            else if (diagV.Contains("VECTOR 5"))
            {
                NameTag = "DIAG_V";
                Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("DiagV value =")} '{diagV}' {CLocaliz.TxT("not support")}");
                throw new InvalidOperationException($"{CLocaliz.TxT("DiagV value =")} '{diagV}' {CLocaliz.TxT("not support")}");
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
                        diagV = GetFormatArrayVertical(arrayPolarV);
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

        private string GetFormatArrayVertical(double[] inArr)
        {
            string outString = "";
            if (inArr != null)
            {
                for (int i = 0; i < inArr.Length; i++)
                {
                    var s = Math.Round(inArr[i].ToString().ConvertStringToDouble().Value, 1).ToString().Replace(",", ".");
                    if (s.Length > 4)
                    {
                        var delta = s.Length - 4;
                        s = s.Remove(s.Length - delta, delta);
                    }

                    if (s.Length == 1)
                    {
                        s = "0" + s;
                    }
                    s = s.PadLeft(4, ' ');
                    outString += s;
                }
            }
            return outString;
        }

        private string GetFormatArrayHorizontal(double[] inArr)
        {
            string outString = "";
            if (inArr != null)
            {
                bool isFirst = true;

                for (int i = 0; i < inArr.Length; i++)
                {
                    var s = Math.Round(inArr[i].ToString().ConvertStringToDouble().Value, 1).ToString().Replace(",", ".");
                    if (s.Length > 4)
                    {
                        var delta = s.Length - 4;
                        s = s.Remove(s.Length - delta, delta);
                    }
                    if (isFirst)
                    {
                        s = "0000";
                        isFirst = false;
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
                        pointObjects[idx] = pointObject;
                        idx++;
                    }
                }
                else
                {
                    Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("Dimension array must be an even number"));
                    throw new InvalidOperationException(CLocaliz.TxT("Dimension array must be an even number"));
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
                        if (pointObject.Value > 0)
                        {
                            isChecked = true;
                            break;
                        }
                    }
                }
                else
                {
                    Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("Dimension array must be an even number"));
                    throw new InvalidOperationException(CLocaliz.TxT("Dimension array must be an even number"));
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
                    Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Incorrect count element in inArray")} ({inArray.Length} < {MinCount})  {CLocaliz.TxT("in the InterpolationForICSTelecomHorizontal method")}");
                    throw new InvalidOperationException($"{CLocaliz.TxT("Incorrect count element in inArray")} ({inArray.Length} < {MinCount})  {CLocaliz.TxT("in the InterpolationForICSTelecomHorizontal method")}");
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
                Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!"));
                throw new InvalidOperationException(CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!"));
            }
            return result;
        }

        private double[] InterpolationForICSTelecomHorizontalOneDeg(double[] inArray)
        {
            const int MinCount = 360;
            var result = new double[72];
            if (inArray != null)
            {
                if (inArray.Length < MinCount)
                {
                    Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Incorrect count element in inArray")} ({inArray.Length} < {MinCount})  {CLocaliz.TxT("in the InterpolationForICSTelecomHorizontal method")}");
                    throw new InvalidOperationException($"{CLocaliz.TxT("Incorrect count element in inArray")} ({inArray.Length} < {MinCount})  {CLocaliz.TxT("in the InterpolationForICSTelecomHorizontal method")}");
                }
                for (int i = 0; i < 72; i++)
                {
                    result[i] = inArray[i*5];
                }
            }
            else
            {
                Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!"));
                throw new InvalidOperationException(CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!"));
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
                Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!"));
                throw new InvalidOperationException(CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomHorizontal method is null!"));
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
                Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomVertical method is null!"));
                throw new InvalidOperationException(CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomVertical method is null!"));
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
                    Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Incorrect count element in inArray")} ({inArray.Length} < {MinCount})  {CLocaliz.TxT("in the InterpolationForICSTelecomVertical method")}");
                    throw new InvalidOperationException($"{CLocaliz.TxT("Incorrect count element in inArray")} ({inArray.Length} < {MinCount})  {CLocaliz.TxT("in the InterpolationForICSTelecomVertical method")}");
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
                Utils.LogInfo(this._dataConfig, Contexts.CalcCoverages, CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomVertical method is null!"));
                throw new InvalidOperationException(CLocaliz.TxT("The 'inArray' input parameter in the InterpolationForICSTelecomVertical method is null!"));
            }
            return result;
        }

     
    }
}
