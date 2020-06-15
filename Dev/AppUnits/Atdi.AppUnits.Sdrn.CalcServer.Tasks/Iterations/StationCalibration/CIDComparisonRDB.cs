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
    public static class GCIDComparisonRDB
    {
        private static OPSOSInfo info = new OPSOSInfo();
        static GCIDComparisonRDB()
        {
            info = SetDefault();
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
                                        // надо просматривать через настройки
                                        if (info.Operators[i].S0.ValueInDB != null && info.Operators[i].S0.ValueInRadio != null &&
                                        info.Operators[i].S0.ValueInDB.Length > 0 && info.Operators[i].S0.ValueInRadio.Length > 0)
                                        {
                                            t01 = Receive(info.Operators[i].S0.All.Length, info.Operators[i].S0.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S0.CompareWith)]));
                                            for (int ds00 = 0; ds00 < info.Operators[i].S0.ValueInDB.Length; ds00++)
                                            {
                                                t02 = Receive(info.Operators[i].S0.All.Length, info.Operators[i].S0.All, info.Operators[i].S0.ValueInDB[ds00]);
                                                if (t01 == t02)
                                                {
                                                    t03 = Receive(info.Operators[i].S0.All.Length, info.Operators[i].S0.Use, Convert.ToInt32(sr[0]));
                                                    for (int ds01 = 0; ds01 < info.Operators[i].S0.ValueInRadio.Length; ds01++)
                                                    {
                                                        t04 = Receive(info.Operators[i].S0.All.Length, info.Operators[i].S0.All, info.Operators[i].S0.ValueInRadio[ds01]);
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
                                            t01 = Receive(info.Operators[i].S0.All.Length, info.Operators[i].S0.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S0.CompareWith)]));
                                            t03 = Receive(info.Operators[i].S0.All.Length, info.Operators[i].S0.Use, Convert.ToInt32(sr[0]));
                                            if (t01 == t03)//нашли оператора по идее
                                            {
                                                b0 = true;
                                            }
                                        }

                                    }
                                    else { b0 = true; }

                                    if (b0)
                                    {
                                        if (info.Operators[i].S1.Use != null && info.Operators[i].S1.Use.Length > 0)
                                        {
                                            // надо просматривать через настройки
                                            if (info.Operators[i].S1.ValueInDB != null && info.Operators[i].S1.ValueInRadio != null &&
                                            info.Operators[i].S1.ValueInDB.Length > 0 && info.Operators[i].S1.ValueInRadio.Length > 0)
                                            {
                                                t11 = Receive(info.Operators[i].S1.All.Length, info.Operators[i].S1.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S1.CompareWith)]));
                                                for (int ds10 = 0; ds10 < info.Operators[i].S1.ValueInDB.Length; ds10++)//поищем такую страну по бд (S0)
                                                {
                                                    t12 = Receive(info.Operators[i].S1.All.Length, info.Operators[i].S1.All, info.Operators[i].S1.ValueInDB[ds10]);
                                                    if (t11 == t12)//нашли такую страну по идее в бд и настройках
                                                    {
                                                        t13 = Receive(info.Operators[i].S1.All.Length, info.Operators[i].S1.Use, Convert.ToInt32(sr[1]));
                                                        for (int ds11 = 0; ds11 < info.Operators[i].S1.ValueInRadio.Length; ds11++)//поищем такого оператора по бд (S0)
                                                        {
                                                            t14 = Receive(info.Operators[i].S1.All.Length, info.Operators[i].S1.All, info.Operators[i].S1.ValueInRadio[ds11]);
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
                                                t11 = Receive(info.Operators[i].S1.All.Length, info.Operators[i].S1.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S1.CompareWith)]));
                                                t13 = Receive(info.Operators[i].S1.All.Length, info.Operators[i].S1.Use, Convert.ToInt32(sr[1]));
                                                if (t11 == t13)//нашли оператора по идее
                                                {
                                                    b1 = true;
                                                }
                                            }
                                        }
                                        else { b1 = true; }


                                        if (b1)
                                        {
                                            if (info.Operators[i].S2.Use != null && info.Operators[i].S2.Use.Length > 0)
                                            {
                                                // надо просматривать через настройки
                                                if (info.Operators[i].S2.ValueInDB != null && info.Operators[i].S2.ValueInRadio != null &&
                                                info.Operators[i].S2.ValueInDB.Length > 0 && info.Operators[i].S2.ValueInRadio.Length > 0)
                                                {
                                                    t21 = Receive(info.Operators[i].S2.All.Length, info.Operators[i].S2.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S2.CompareWith)]));
                                                    for (int ds20 = 0; ds20 < info.Operators[i].S2.ValueInDB.Length; ds20++)//поищем такую страну по бд (S0)
                                                    {
                                                        t22 = Receive(info.Operators[i].S2.All.Length, info.Operators[i].S2.All, info.Operators[i].S2.ValueInDB[ds20]);
                                                        if (t21 == t22)//нашли такую страну по идее в бд и настройках
                                                        {
                                                            t23 = Receive(info.Operators[i].S2.All.Length, info.Operators[i].S2.Use, Convert.ToInt32(sr[2]));
                                                            for (int ds21 = 0; ds21 < info.Operators[i].S2.ValueInRadio.Length; ds21++)//поищем такого оператора по бд (S0)
                                                            {
                                                                t24 = Receive(info.Operators[i].S2.All.Length, info.Operators[i].S2.All, info.Operators[i].S2.ValueInRadio[ds21]);
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
                                                    t21 = Receive(info.Operators[i].S2.All.Length, info.Operators[i].S2.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S2.CompareWith)]));
                                                    t23 = Receive(info.Operators[i].S2.All.Length, info.Operators[i].S2.Use, Convert.ToInt32(sr[2]));
                                                    if (t21 == t23)//нашли оператора по идее
                                                    {
                                                        b2 = true;
                                                    }
                                                }
                                            }
                                            else { b2 = true; }

                                            if (b2)
                                            {
                                                if (info.Operators[i].S3.Use != null && info.Operators[i].S3.Use.Length > 0)
                                                {
                                                    if (info.Operators[i].S3.ValueInDB != null && info.Operators[i].S3.ValueInRadio != null &&
                                                   info.Operators[i].S3.ValueInDB.Length > 0 && info.Operators[i].S3.ValueInRadio.Length > 0)
                                                    {
                                                        t31 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.Use, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S3.CompareWith)]));
                                                        for (int ds30 = 0; ds30 < info.Operators[i].S3.ValueInDB.Length; ds30++)//поищем такую страну по бд (S0)
                                                        {
                                                            t32 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.All, info.Operators[i].S3.ValueInDB[ds30]);
                                                            if (t31 == t32)//нашли такую страну по идее в бд и настройках
                                                            {
                                                                t33 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.Use, Convert.ToInt32(sr[3]));
                                                                for (int ds31 = 0; ds31 < info.Operators[i].S3.ValueInRadio.Length; ds31++)//поищем такого оператора по бд (S0)
                                                                {
                                                                    t34 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.All, info.Operators[i].S3.ValueInRadio[ds31]);
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
                                                        t31 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.All, Convert.ToInt32(sdb[ReturnInt(info.Operators[i].S3.CompareWith)]));
                                                        t33 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.Use, Convert.ToInt32(sr[3]));
                                                        if (t31 == t33)//нашли оператора по идее
                                                        {
                                                            b3 = true;
                                                            t33 = Receive(info.Operators[i].S3.All.Length, info.Operators[i].S3.Use, Convert.ToInt32(sr[ReturnInt(info.Operators[i].S3.CompareWith)]));
                                                        }
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
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        static string idlength = "", str, cid;
        private static int Receive(int maxLength, int[] data, int val)
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 1 },
                            ValueInRadio = new int[]{ 1 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 3 },
                            ValueInRadio = new int[]{ 3 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 6 },
                            ValueInRadio = new int[]{ 6 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 1 },
                            ValueInRadio = new int[]{ 1 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 3 },
                            ValueInRadio = new int[]{ 3 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 1, 2, 3, 4 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 6 },
                            ValueInRadio = new int[]{ 6 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 7 },
                            ValueInRadio = new int[]{ 7 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "LAC",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ 0, 1, 2, 3 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 1, Max = 65535,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 1 },
                            ValueInRadio = new int[]{ 1 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "eNodeBId",
                            Use = new int[]{ 2, 3, 4, 5},
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4, 5 },
                            Min = 0, Max = 999999,
                            CompareWith = "S3"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2 },
                            Min = 0, Max = 255,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 3 },
                            ValueInRadio = new int[]{ 3 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "eNodeBId",
                            Use = new int[]{ 2, 3, 4, 5 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4, 5 },
                            Min = 0, Max = 999999,
                            CompareWith = "S3"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2 },
                            Min = 0, Max = 255,
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
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "MNC",
                            Use =  new int[]{ 0, 1, 2 },
                            ValueInDB = new int[]{ 6 },
                            ValueInRadio = new int[]{ 6 },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 999,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "eNodeBId",
                            Use = new int[]{ 2, 3, 4, 5 },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4, 5 },
                            Min = 0, Max = 999999,
                            CompareWith = "S3"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "CI",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2 },
                            Min = 0, Max = 255,
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
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 0, Max = 65535,
                            CompareWith = "S0"
                        },
                        S1 = new OPSOSInfo.Sn()
                        {
                            Name = "SID",
                            Use =  new int[]{ 0, 1, 2, 3, 4 },
                            ValueInDB = new int[]{ 15906, 15907, 15908, 6 },
                            ValueInRadio = new int[]{ 15906, 15907, 15908, 8929 },
                            All = new int[]{ 0, 1, 2, 3, 4 },
                            Min = 0, Max = 32767,
                            CompareWith = "S1"
                        },
                        S2 = new OPSOSInfo.Sn()
                        {
                            Name = "PN",
                            Use = new int[]{ },
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2 },
                            Min = 1, Max = 511,
                            CompareWith = "S2"
                        },
                        S3 = new OPSOSInfo.Sn()
                        {
                            Name = "BaseId",
                            Use = new int[]{ 4, 3, 2},
                            ValueInDB = new int[]{ },
                            ValueInRadio = new int[]{ },
                            All = new int[]{ 0, 1, 2, 3, 4, 5 },
                            Min = 1, Max = 999999,
                            CompareWith = "S3"
                        },
                    },

                    #endregion CDMA
                }
            };
        }

        public class OPSOSInfo
        {
            public OperatorInfo[] Operators = new OperatorInfo[] { };

            public class OperatorInfo
            {
                public string OperatorName { get; set; } = "";

                public string[] TechName { get; set; } = new string[] { };

                public Sn S0 { get; set; } = new Sn() { };

                public Sn S1 { get; set; } = new Sn() { };

                public Sn S2 { get; set; } = new Sn() { };

                public Sn S3 { get; set; } = new Sn() { };
            }
            public class Sn
            {
                public string Name { get; set; } = "";

                public int[] Use { get; set; } = new int[] { };

                public int[] ValueInDB { get; set; } = new int[] { };

                public int[] ValueInRadio { get; set; } = new int[] { };

                public int[] All { get; set; } = new int[] { };

                public string CompareWith { get; set; } = "";

                public int Min { get; set; }
                public int Max { get; set; }
            }
        }
    }
}
