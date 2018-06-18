using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class DataDesc
    {
        public string Name;
        public DataCoding Coding;
        public VarType ClassType;
        public object ClassNull;
        public int Precision;
        public int Scale;

        public static Dictionary<string, DataDesc> map;
        public string ParamType
        {
            get
            {
                switch (this.ClassType)
                {
                    case VarType.var_String:
                        return string.Format("char({0})", this.Precision);
                    case VarType.var_Dou:
                        return "double";
                    case VarType.var_Flo:
                        return "float";
                    case VarType.var_Int:
                        return "int";
                    case VarType.var_Tim:
                        return "datetime";
                    case VarType.var_Bytes:
                        return "binary";
                    case VarType.var_Guid:
                        return "guid";
                }
                return null;
            }
        }
        
        public object StrToData(string str)
        {
            if (this.Coding == DataCoding.tvalNUMBER)
            {
                return str.ParseDouble();
            }
            if (this.Coding == DataCoding.tvalDATETIME)
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                if (str.Length >= 8)
                {
                    if (char.IsDigit(str[0]))
                    {
                        num3 = str.Substring(0, 4).ParseInt();
                        num = str.Substring(4, 2).ParseInt();
                        num2 = str.Substring(6, 2).ParseInt();
                    }
                }
                else if (str[0] == 'T')
                {
                    int num4 = 0;
                    while (num4 < str.Length && !char.IsDigit(str[num4]))
                    {
                        num4++;
                    }
                    int num5 = str.IndexOf('\'', num4);
                    if (num5 > num4)
                    {
                        string[] array = str.Substring(num4, num5 - num4).Split(new char[]
                        {
                            '/'
                        });
                        if (array.Length == 3)
                        {
                            int.TryParse(array[0], out num2);
                            int.TryParse(array[1], out num);
                            int.TryParse(array[2], out num3);
                        }
                    }
                }
                else if (str[0] == '#')
                {
                    int num6 = 1;
                    int num7 = str.IndexOf('#', num6);
                    if (num7 > num6)
                    {
                        string[] array2 = str.Substring(num6, num7 - num6).Split(new char[]
                        {
                            '/'
                        });
                        if (array2.Length == 3)
                        {
                            int.TryParse(array2[0], out num);
                            int.TryParse(array2[1], out num2);
                            int.TryParse(array2[2], out num3);
                        }
                    }
                }
                if (num2 != 0 && num != 0 && num3 != 0)
                {
                    return new DateTime(num3, num, num2);
                }
                return Null.T;
            }
            else
            {
                if (this.Coding == DataCoding.tvalSTRING)
                {
                    return str;
                }
                return null;
            }
        }
        public string DataToStr(object data)
        {
            string result = null;
            if (data is string)
            {
                if (this.Coding != DataCoding.tvalSTRING)
                {
                    return null;
                }
                result = (string)data;
            }
            else if (data is double)
            {
                if (this.Coding != DataCoding.tvalNUMBER)
                {
                    return null;
                }
                double num = (double)data;
                if (num == 1E-99)
                {
                    result = "";
                }
                else
                {
                    result = num.ToString("G15", IcsmOrmExtensitions.CultureEnUs);
                }
            }
            else if (data is int)
            {
                if (this.Coding != DataCoding.tvalNUMBER)
                {
                    return null;
                }
                int num2 = (int)data;
                if (num2 == 2147483647)
                {
                    result = "";
                }
                else
                {
                    result = num2.ToString();
                }
            }
            else if (data is DateTime)
            {
                if (this.Coding != DataCoding.tvalDATETIME)
                {
                    return null;
                }
                DateTime dat = (DateTime)data;
                if (dat.IsNull())
                {
                    result = "00000000";
                }
                else
                {
                    result = string.Format("{0:0000}{1:00}{2:00}", dat.Year, dat.Month, dat.Day);
                }
            }
            return result;
        }
    }
}
