using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// format data "s0 s1 s2 s3" 
    /// Exemple GSM:
    /// GCID = "255 3 12345 67890"
    /// s0 = "255"
    /// s1 = "3"
    /// s2 = "12345"
    /// s3 = "67890"
    /// 
    /// GSM  = "MCC MNC LAC CID" (s0 = MCC, s1 = MNC, s2 = LAC, s3 = CI) MCC=001-999 MNC=001-999 LAC=1-65535 CI =1-65535 
    /// UMTS = "MCC MNC LAC CID" (s0 = MCC, s1 = MNC, s2 = LAC, s3 = CI) MCC=001-999 MNC=001-999 LAC=1-65535 CI =1-65535 
    /// CDMA = "NID SID PN BaseId" (s0 = NID, s1 = SID, s2 = PN, s3 = BaseId) NID=5digits SID=5digits PN=0-511 BaseId=6digits
    /// LTE  = "MCC MNC eNodeBId CID" (s0 = MCC, s1 = MNC, s2 = eNodeBId, s3 = CI) MCC=001-999 MNC=001-999 eNodeBId=6digits(20bit) CI=3digits(8bit)
    /// </summary>
    public static class GCIDComparison
    {
        private static OPSOSInfo info = new OPSOSInfo();
        private static TechInfo[] techInfos;
        private static TechInfo techSelected;
        private static TechInfo.Parametr param0, param1, param2, param3;
        //private static string FilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
        //    "/" + "OPSOSInfo.xml";
        static GCIDComparison()
        {
            SetTechInfo();
            info = SetDefault();
            //Load();
        }
        static int i = 0, k = 0, p = 0, j = 0, s = 0;
        static int t01 = -1, t11 = -1, t21 = -1, t31 = -1; // прочитанный из бд
        static int t02 = -1, t12 = -1, t22 = -1, t32 = -1; // прочитанный из параметров для бд
        static int t03 = -1, t13 = -1, t23 = -1, t33 = -1; // прочитанный из ралио
        static int t04 = -1, t14 = -1, t24 = -1, t34 = -1; // прочитанный из параметров для ралио
        static bool b0 = false, b1 = false, b2 = false, b3 = false;
        static string localTeck;
        public static bool Compare(string tech, string GCIDFromRadio, string GCIDFromDB)
        {
            try
            {
                string[] sr = GCIDFromRadio.Split(' ');
                string[] sdb = GCIDFromDB.Split(' ');
                if (sr.Length == 4)
                {
                    if (sdb.Length == 4)
                    {
                        localTeck = ReturnLetter(tech).ToLower();
                        if (localTeck != techSelected.Name.ToLower())
                        {
                            for (i = 0; i < techInfos.Length; i++)
                            {
                                if (localTeck == techInfos[i].Name.ToLower())
                                {
                                    techSelected = techInfos[i];
                                    param0 = techSelected.Parametrs[0];
                                    param1 = techSelected.Parametrs[1];
                                    param2 = techSelected.Parametrs[2];
                                    param3 = techSelected.Parametrs[3];
                                    break;
                                }
                            }
                        }

                        for (i = 0; i < info.Operators.Length; i++)
                        {
                            b0 = false;
                            b1 = false;
                            b2 = false;
                            b3 = false;
                            for (k = 0; k < info.Operators[i].TechName.Length; k++)
                            {
                                if (localTeck == info.Operators[i].TechName[k].ToLower())
                                {
                                    if (info.Operators[i].S0.Use != null && info.Operators[i].S0.Use.Length > 0)
                                    {
                                        GetParametr(info.Operators[i].S0.Name, ref param0);
                                        if (param0 != null)
                                        {
                                            // надо просматривать через настройки
                                            if (info.Operators[i].S0.ValueInDB != null && info.Operators[i].S0.ValueInRadio != null &&
                                            info.Operators[i].S0.ValueInDB.Length > 0 && info.Operators[i].S0.ValueInRadio.Length > 0)
                                            {
                                                t01 = comp(param0.Length, info.Operators[i].S0.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S0.CompareWith)]));
                                                for (int ds00 = 0; ds00 < info.Operators[i].S0.ValueInDB.Length; ds00++)
                                                {
                                                    t02 = comp(param0.Length, param0.All, info.Operators[i].S0.ValueInDB[ds00]);
                                                    if (t01 == t02)
                                                    {
                                                        t03 = comp(param0.Length, info.Operators[i].S0.Use, Convert.ToInt32(sr[0]));
                                                        for (int ds01 = 0; ds01 < info.Operators[i].S0.ValueInRadio.Length; ds01++)
                                                        {
                                                            t04 = comp(param0.Length, param0.All, info.Operators[i].S0.ValueInRadio[ds01]);
                                                            if (t03 == t04)
                                                            {
                                                                b0 = true;

                                                                break;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            else //сравниваем одно с другим лоб в лоб
                                            {
                                                t01 = comp(param0.Length, param0.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S0.CompareWith)]));
                                                t03 = comp(param0.Length, info.Operators[i].S0.Use, Convert.ToInt32(sr[0]));
                                                if (t01 == t03)//нашли оператора по идее
                                                {
                                                    b0 = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("unknown parameter " + info.Operators[i].S0.Name);
                                        }
                                    }
                                    else { b0 = true; }

                                    if (b0)
                                    {
                                        if (info.Operators[i].S1.Use != null && info.Operators[i].S1.Use.Length > 0)
                                        {
                                            GetParametr(info.Operators[i].S1.Name, ref param1);
                                            if (param1 != null)
                                            {
                                                // надо просматривать через настройки
                                                if (info.Operators[i].S1.ValueInDB != null && info.Operators[i].S1.ValueInRadio != null &&
                                                info.Operators[i].S1.ValueInDB.Length > 0 && info.Operators[i].S1.ValueInRadio.Length > 0)
                                                {
                                                    t11 = comp(param1.Length, info.Operators[i].S1.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S1.CompareWith)]));
                                                    for (int ds10 = 0; ds10 < info.Operators[i].S1.ValueInDB.Length; ds10++)//поищем такую страну по бд (S0)
                                                    {
                                                        t12 = comp(param1.Length, param1.All, info.Operators[i].S1.ValueInDB[ds10]);
                                                        if (t11 == t12)//нашли такую страну по идее в бд и настройках
                                                        {
                                                            t13 = comp(param1.Length, info.Operators[i].S1.Use, Convert.ToInt32(sr[1]));
                                                            for (int ds11 = 0; ds11 < info.Operators[i].S1.ValueInRadio.Length; ds11++)//поищем такого оператора по бд (S0)
                                                            {
                                                                t14 = comp(param1.Length, param1.All, info.Operators[i].S1.ValueInRadio[ds11]);
                                                                if (t13 == t14)//нашли оператора по идее
                                                                {
                                                                    b1 = true;
                                                                    break;
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                                else //сравниваем одно с другим лоб в лоб
                                                {
                                                    t11 = comp(param1.Length, param1.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S1.CompareWith)]));
                                                    t13 = comp(param1.Length, info.Operators[i].S1.Use, Convert.ToInt32(sr[1]));
                                                    if (t11 == t13)//нашли оператора по идее
                                                    {
                                                        b1 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("unknown parameter " + info.Operators[i].S1.Name);
                                            }
                                        }
                                        else { b1 = true; }


                                        if (b1)
                                        {
                                            if (info.Operators[i].S2.Use != null && info.Operators[i].S2.Use.Length > 0)
                                            {
                                                GetParametr(info.Operators[i].S2.Name, ref param2);
                                                if (param2 != null)
                                                {
                                                    // надо просматривать через настройки
                                                    if (info.Operators[i].S2.ValueInDB != null && info.Operators[i].S2.ValueInRadio != null &&
                                                    info.Operators[i].S2.ValueInDB.Length > 0 && info.Operators[i].S2.ValueInRadio.Length > 0)
                                                    {
                                                        t21 = comp(param2.Length, info.Operators[i].S2.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S2.CompareWith)]));
                                                        for (int ds20 = 0; ds20 < info.Operators[i].S2.ValueInDB.Length; ds20++)//поищем такую страну по бд (S0)
                                                        {
                                                            t22 = comp(param2.Length, param2.All, info.Operators[i].S2.ValueInDB[ds20]);
                                                            if (t21 == t22)//нашли такую страну по идее в бд и настройках
                                                            {
                                                                t23 = comp(param2.Length, info.Operators[i].S2.Use, Convert.ToInt32(sr[2]));
                                                                for (int ds21 = 0; ds21 < info.Operators[i].S2.ValueInRadio.Length; ds21++)//поищем такого оператора по бд (S0)
                                                                {
                                                                    t24 = comp(param2.Length, param2.All, info.Operators[i].S2.ValueInRadio[ds21]);
                                                                    if (t23 == t24)//нашли оператора по идее
                                                                    {
                                                                        b2 = true;
                                                                        break;
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else //сравниваем одно с другим лоб в лоб
                                                    {
                                                        t21 = comp(param2.Length, param2.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S2.CompareWith)]));
                                                        t23 = comp(param2.Length, info.Operators[i].S2.Use, Convert.ToInt32(sr[2]));
                                                        if (t21 == t23)//нашли оператора по идее
                                                        {
                                                            b2 = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("unknown parameter " + info.Operators[i].S2.Name);
                                                }
                                            }
                                            else { b2 = true; }

                                            if (b2)
                                            {
                                                if (info.Operators[i].S3.Use != null && info.Operators[i].S3.Use.Length > 0)
                                                {
                                                    GetParametr(info.Operators[i].S3.Name, ref param3);
                                                    if (param3 != null)
                                                    {
                                                        if (info.Operators[i].S3.ValueInDB != null && info.Operators[i].S3.ValueInRadio != null &&
                                                       info.Operators[i].S3.ValueInDB.Length > 0 && info.Operators[i].S3.ValueInRadio.Length > 0)
                                                        {
                                                            t31 = comp(param3.Length, info.Operators[i].S3.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S3.CompareWith)]));
                                                            for (int ds30 = 0; ds30 < info.Operators[i].S3.ValueInDB.Length; ds30++)//поищем такую страну по бд (S0)
                                                            {
                                                                t32 = comp(param3.Length, param3.All, info.Operators[i].S3.ValueInDB[ds30]);
                                                                if (t31 == t32)//нашли такую страну по идее в бд и настройках
                                                                {
                                                                    t33 = comp(param3.Length, info.Operators[i].S3.Use, Convert.ToInt32(sr[3]));
                                                                    for (int ds31 = 0; ds31 < info.Operators[i].S3.ValueInRadio.Length; ds31++)//поищем такого оператора по бд (S0)
                                                                    {
                                                                        t34 = comp(param3.Length, param3.All, info.Operators[i].S3.ValueInRadio[ds31]);
                                                                        if (t33 == t34)//нашли оператора по идее
                                                                        {
                                                                            b3 = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else //сравниваем одно с другим лоб в лоб
                                                        {
                                                            t31 = comp(param3.Length, param3.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S3.CompareWith)]));
                                                            t33 = comp(param3.Length, info.Operators[i].S3.Use, Convert.ToInt32(sr[3]));
                                                            if (t31 == t33)//нашли оператора по идее
                                                            {
                                                                b3 = true;
                                                                t33 = comp(param3.Length, info.Operators[i].S3.Use, Convert.ToInt32(sr[ReturnInt(info.Operators[i].S3.CompareWith)]));
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("unknown parameter " + info.Operators[i].S3.Name);
                                                    }
                                                }
                                                else { b3 = true; }
                                            }
                                        }
                                    }
                                    if (b0 && b1 && b2 && b3)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"The GCIDFromDB({GCIDFromDB}) parameter does not have four groups of digits.");
                    }
                }
                else
                {
                    throw new Exception($"The GCIDFromRadio({GCIDFromRadio}) parameter does not have four groups of digits.");
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Load()
        {
            //if (File.Exists(FilePath))
            //{
            //    XmlSerializer ser = new XmlSerializer(typeof(OPSOSInfo));
            //    TextReader reader = new StreamReader(FilePath);
            //    info = ValidateOpsosSettings(ser.Deserialize(reader) as OPSOSInfo);
            //    reader.Close();
            //}
            //else
            //{
            //    SetThisAdapterConfig(, FilePath);
            //}
        }
        static string idlength = "", str, cid;
        private static int comp(int maxLength, int[] data, int val)
        {
            idlength = "";
            for (j = 0; j < maxLength; j++)
                idlength += "0";
            str = string.Format("{0:" + idlength + "}", val);
            cid = "";

            if (data.Length > 0)
            {
                for (s = 0; s < data.Length; s++)
                {
                    if (data[s] > -1) cid += str.Substring(data[s], 1);
                }
                if (cid.Length > 0) return Convert.ToInt32(cid);
                else return -1;
            }
            else return -1;
        }
        private static int ReturnInt(string str)
        {
            string t = string.Empty;
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                {
                    t += c;
                }
            }
            return Convert.ToInt32(t);
        }
        private static string ReturnLetter(string str)
        {
            string t = string.Empty;
            foreach (char c in str)
            {
                if (Char.IsLetter(c))
                {
                    t += c;
                }
            }
            return t;
        }
        private static OPSOSInfo ValidateOpsosSettings(OPSOSInfo set)
        {
            for (int z = 0; z < set.Operators.Length; z++)
            {
                for (int n = 0; n < set.Operators[z].TechName.Length; n++)
                {
                    for (int t = 0; t < techInfos.Length; t++)
                    {
                        if (set.Operators[z].TechName[n] == techInfos[t].Name)//проверим эту технологию
                        {
                            if (techInfos[t].Parametrs[0].Necessarily)//обязателен
                            {
                                if (set.Operators[z].S0.Name == techInfos[t].Parametrs[0].Name)
                                {
                                    ValidateParametr(set.Operators[z].S0, techInfos[t].Parametrs[0]);
                                }
                                else // и ненашли его на своем месте
                                {
                                    throw new Exception($"For the operator of {set.Operators[z].OperatorName} and {set.Operators[z].TechName[n]} technology," +
                                        $" this parameter {set.Operators[z].S0.Name} is in place of the required parameter");
                                }
                            }
                            if (techInfos[t].Parametrs[1].Necessarily)//обязателен
                            {
                                if (set.Operators[z].S1.Name == techInfos[t].Parametrs[1].Name)
                                {
                                    ValidateParametr(set.Operators[z].S1, techInfos[t].Parametrs[1]);
                                }
                                else // и ненашли его на своем месте
                                {
                                    throw new Exception($"For the operator of {set.Operators[z].OperatorName} and {set.Operators[z].TechName[n]} technology," +
                                        $" this parameter {set.Operators[z].S1.Name} is in place of the required parameter");
                                }
                            }
                            if (techInfos[t].Parametrs[2].Necessarily)//обязателен
                            {
                                if (set.Operators[z].S2.Name == techInfos[t].Parametrs[2].Name)
                                {
                                    ValidateParametr(set.Operators[z].S2, techInfos[t].Parametrs[2]);
                                }
                                else // и ненашли его на своем месте
                                {
                                    throw new Exception($"For the operator of {set.Operators[z].OperatorName} and {set.Operators[z].TechName[n]} technology," +
                                        $" this parameter {set.Operators[z].S2.Name} is in place of the required parameter");
                                }
                            }
                            if (techInfos[t].Parametrs[3].Necessarily)//обязателен
                            {
                                if (set.Operators[z].S3.Name == techInfos[t].Parametrs[3].Name)
                                {
                                    ValidateParametr(set.Operators[z].S3, techInfos[t].Parametrs[3]);
                                }
                                else // и ненашли его на своем месте
                                {
                                    throw new Exception($"For the operator of {set.Operators[z].OperatorName} and {set.Operators[z].TechName[n]} technology," +
                                        $" this parameter {set.Operators[z].S3.Name} is in place of the required parameter");
                                }
                            }
                        }
                    }
                }
            }
            return set;
        }
        private static void ValidateParametr(OPSOSInfo.Sn parSet, TechInfo.Parametr parametr)
        {
            if (parSet.Use != null)
            {
                if (parSet.Use.Length > parametr.Length)
                {
                    //у параметра превышена используемая длина символов
                    throw new Exception($"Parameter {parSet.Name} exceeded the used character length");
                }
                if (parSet.Use.Max() > parametr.Length - 1)
                {
                    //у параметра привышен индекс символа
                    throw new Exception($"{parSet.Name} parameter exceeded maximum character index");
                }
                if (parSet.Use.Min() < 0)
                {
                    //у параметра индекс символа не может быть отрицательным
                    throw new Exception($"Parameter {parSet.Name} symbol index cannot be negative");
                }
            }

            if (parSet.CompareWith == string.Empty || parSet.CompareWith == "")
            {
                //у параметра должен быть указан параметр сравнения
                throw new Exception($"Parameter {parSet.Name} must be specified comparison parameter");
            }
            else
            {
                if (parSet.CompareWith != "S0" ||
                    parSet.CompareWith != "S1" ||
                    parSet.CompareWith != "S2" ||
                    parSet.CompareWith != "S3")
                {
                    //Параметр сравнения имеет недоступное значение
                }
            }
        }
        private static OPSOSInfo SetDefault()
        {
            return new OPSOSInfo()
            {
                Operators = new OPSOSInfo.OperatorInfo[]
                {
                    #region GSM
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "VF",
                        TechName = new string[]{ "GSM" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use = new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 1 },
                            ValueInRadio = new int[]{ 1 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "KS",
                        TechName = new string[]{ "GSM" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 3 },
                            ValueInRadio = new int[]{ 3 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 3, 4 },//0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "Life",
                        TechName = new string[]{ "GSM" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 6 },
                            ValueInRadio = new int[]{ 6 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    #endregion GSM
                    #region UMTS
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "VF",
                        TechName = new string[]{ "UMTS", "WCDMA" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 1 },
                            ValueInRadio = new int[]{ 1 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "KS",
                        TechName = new string[]{ "UMTS", "WCDMA" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 3 },
                            ValueInRadio = new int[]{ 3 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 1, 2, 3, 4 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "Life",
                        TechName = new string[]{ "UMTS", "WCDMA" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 6 },
                            ValueInRadio = new int[]{ 6 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "3mob",
                        TechName = new string[]{ "UMTS", "WCDMA" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 7 },
                            ValueInRadio = new int[]{ 7 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },
                    #endregion UMTS
                    #region LTE
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "VF",
                        TechName = new string[]{ "LTE" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 1 },
                            ValueInRadio = new int[]{ 1 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "eNodeBId",
                            Use = new int[]{ 2, 3, 4, 5},
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "KS",
                        TechName = new string[]{ "LTE" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 3 },
                            ValueInRadio = new int[]{ 3 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "eNodeBId",
                            Use = new int[]{ 2, 3, 4, 5 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                    },
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "Life",
                        TechName = new string[]{ "LTE" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "MCC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 255 },
                            ValueInRadio = new int[]{ 255 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 6 },
                            ValueInRadio = new int[]{ 6 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "eNodeBId",
                            Use = new int[]{ 2, 3, 4, 5 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                    },
                    #endregion LTE
                    #region CDMA
                    new OPSOSInfo.OperatorInfo()
                    {
                        OperatorName = "IT",
                        TechName = new string[]{ "CDMA", "EVDO" },
                        S0 = new OPSOSInfo.Sn()
                        {
                            Name = "NID",
                            Use =  new int[]{ 0, 1, 2, 3, 4 },
                            ValueInDB = new int[]{ 0, 255 },
                            ValueInRadio = new int[]{ 0 },
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "SID",
                            Use =  new int[]{ 0, 1, 2, 3, 4 },
                            ValueInDB = new int[]{ 15906, 15907, 15908, 6 },
                            ValueInRadio = new int[]{ 15906, 15907, 15908, 8929 },
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "PN",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "BaseId",
                            Use = new int[]{ 4, 3, 2},
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            CompareWith = "S3"
                        },
                    },

                    #endregion CDMA
                }
            };
        }

        private static void SetTechInfo()
        {
            techInfos = new TechInfo[]
            {
                new TechInfo()
                {
                    Name = "GSM",
                    Parametrs = new TechInfo.Parametr[]
                    {
                        new TechInfo.Parametr(){Name = "MCC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 0 },
                        new TechInfo.Parametr(){Name = "MNC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 1 },
                        new TechInfo.Parametr(){Name = "LAC", Necessarily = false, Length = 5, Min = 1, Max = 65535, Position = 2 },
                        new TechInfo.Parametr(){Name = "CI", Necessarily = true, Length = 5, Min = 1, Max = 65535, Position = 3 },
                    }
                },
                new TechInfo()
                {
                    Name = "UMTS",
                    Parametrs = new TechInfo.Parametr[]
                    {
                        new TechInfo.Parametr(){Name = "MCC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 0 },
                        new TechInfo.Parametr(){Name = "MNC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 1 },
                        new TechInfo.Parametr(){Name = "LAC", Necessarily = false, Length = 5, Min = 1, Max = 65535, Position = 2 },
                        new TechInfo.Parametr(){Name = "CI", Necessarily = true, Length = 5, Min = 1, Max = 65535, Position = 3 },
                    }
                },
                new TechInfo()
                {
                    Name = "WCDMA",
                    Parametrs = new TechInfo.Parametr[]
                    {
                        new TechInfo.Parametr(){Name = "MCC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 0 },
                        new TechInfo.Parametr(){Name = "MNC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 1 },
                        new TechInfo.Parametr(){Name = "LAC", Necessarily = false, Length = 5, Min = 1, Max = 65535, Position = 2 },
                        new TechInfo.Parametr(){Name = "CI", Necessarily = true, Length = 5, Min = 1, Max = 65535, Position = 3 },
                    }
                },
                new TechInfo()
                {
                    Name = "CDMA",
                    Parametrs = new TechInfo.Parametr[]
                    {
                        new TechInfo.Parametr(){Name = "NID", Necessarily = false, Length = 5, Min = 0, Max = 65535, Position = 0 },
                        new TechInfo.Parametr(){Name = "SID", Necessarily = true, Length = 5, Min = 0, Max = 32767, Position = 1 },
                        new TechInfo.Parametr(){Name = "PN", Necessarily = false, Length = 3, Min = 1, Max = 511, Position = 2 },
                        new TechInfo.Parametr(){Name = "BaseId", Necessarily = true, Length = 6, Min = 1, Max = 999999, Position = 3 },
                    }
                },
                new TechInfo()
                {
                    Name = "EVDO",
                    Parametrs = new TechInfo.Parametr[]
                    {
                        new TechInfo.Parametr(){Name = "NID", Necessarily = false, Length = 5, Min = 0, Max = 65535, Position = 0 },
                        new TechInfo.Parametr(){Name = "SID", Necessarily = true, Length = 5, Min = 0, Max = 32767, Position = 1 },
                        new TechInfo.Parametr(){Name = "PN", Necessarily = false, Length = 3, Min = 1, Max = 511, Position = 2 },
                        new TechInfo.Parametr(){Name = "BaseId", Necessarily = true, Length = 6, Min = 1, Max = 999999, Position = 3 },
                    }
                },
                new TechInfo()
                {
                    Name = "LTE",
                    Parametrs = new TechInfo.Parametr[]
                    {
                        new TechInfo.Parametr(){Name = "MCC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 0 },
                        new TechInfo.Parametr(){Name = "MNC", Necessarily = true, Length = 3, Min = 1, Max = 999, Position = 1},
                        new TechInfo.Parametr(){Name = "eNodeBId", Necessarily = true, Length = 6, Min = 0, Max = 999999, Position = 2 },
                        new TechInfo.Parametr(){Name = "CI", Necessarily = false, Length = 3, Min = 0, Max = 255, Position = 3 },
                        //new TechInfo.Parametr(){Name = "TAC", Necessarily = false, Length = 5, Min = 0, Max = 65535, Position = -1 },
                    }
                },
            };
            techSelected = techInfos[0];
            param0 = techSelected.Parametrs[0];
            param1 = techSelected.Parametrs[1];
            param2 = techSelected.Parametrs[2];
            param3 = techSelected.Parametrs[3];
        }
        private static void GetParametr(string name, ref TechInfo.Parametr parametr)
        {
            for (p = 0; p < techSelected.Parametrs.Length; p++)
            {
                if (name.ToLower().Contains(techSelected.Parametrs[p].Name.ToLower()))
                {
                    parametr = techSelected.Parametrs[p];
                    break;
                }
            }
        }

        private static void SetThisAdapterConfig(OPSOSInfo config, string filePath)
        {
            info = config;
            config.Serialize(filePath);
        }


        [Serializable]
        public class OPSOSInfo
        {
            [XmlArray]
            public OperatorInfo[] Operators = new OperatorInfo[] { };

            [Serializable]
            public class OperatorInfo
            {
                [XmlElement]
                public string OperatorName { get; set; } = "";

                [XmlElement]
                public string[] TechName { get; set; } = new string[] { };

                [XmlElement]
                public Sn S0 { get; set; } = new Sn() { };

                [XmlElement]
                public Sn S1 { get; set; } = new Sn() { };

                [XmlElement]
                public Sn S2 { get; set; } = new Sn() { };

                [XmlElement]
                public Sn S3 { get; set; } = new Sn() { };
            }
            [Serializable]
            public class Sn
            {
                [XmlElement]
                public string Name { get; set; } = "";

                [XmlElement]
                public int[] Use { get; set; } = new int[] { };

                [XmlElement]
                public int[] ValueInDB { get; set; } = new int[] { };

                [XmlElement]
                public int[] ValueInRadio { get; set; } = new int[] { };

                [XmlElement]
                public string CompareWith { get; set; } = "";
            }


            public void Serialize(string FilePath)
            {
                XmlSerializer ser = new XmlSerializer(this.GetType());
                TextWriter writer = new StreamWriter(FilePath, false);
                ser.Serialize(writer, this);
                writer.Close();
            }
        }

        public class TechInfo
        {
            public string Name;
            public Parametr[] Parametrs;

            public class Parametr
            {
                public string Name;
                public int Length;
                public int Min;
                public int Max;
                public int Position;
                public bool Necessarily;
                public int[] All
                {
                    get
                    {
                        int[] res = new int[Length];
                        for (int i = 0; i < Length; i++)
                        {
                            res[i] = i;
                        }
                        return res;
                    }
                    private set { }
                }
            }
        }
    }
}
