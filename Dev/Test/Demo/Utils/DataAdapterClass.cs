using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using DAL;
using System.Data;
using System.Web.UI.WebControls;
using OnlinePortal;
using System.Text;
using OnlinePortal.Utils;
using System.Web.Services;
using System.Web.WebSockets;
using System.Web.UI;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.ClientServices.Providers;
using System.Web.Caching;
using System.Web.ClientServices;
using System.Web.Globalization;
using System.Web.Handlers;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows;

namespace LitvaPortal.Utils
{

    public enum EnumCoordLine
    {
        Lon = 1,   //долгота
        Lat = 2    //широта
    };
    public class DataAdapterClass
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string DmsToString(double coord, EnumCoordLine line)
        {
            if (coord == ConnectDB.NullD)
                return "";

            Int64 coordInt = (Int64)((coord + 0.000005) * 100000.0);
            //Char[] symbol = new Char[] { '\xB0', '\x27' };
            bool isNegative = false;
            if (coordInt < 0)
            {// Меняем знак
                isNegative = true;
                coordInt = -coordInt;
            }
            // Секунды
            Int64 sec = coordInt % 1000;
            string seconds = "";
            if ((sec % 10) != 0)
            {
                double tmp = (double)sec / 10.0;
                seconds = tmp.ToString("00");
            }
            else
            {
                double tmp = (double)sec / 10.0;
                seconds = tmp.ToString("00");
            }
            //seconds += Convert.ToString(symbol[1]) + Convert.ToString(symbol[1]);
            // Минуты
            coordInt = coordInt / 1000;
            string minutes = (coordInt % 100).ToString("00");// +Convert.ToString(symbol[1]);
            // Градусы
            coordInt = coordInt / 100;
            string degree = (coordInt % 100).ToString("00");// +Convert.ToString(symbol[0]);
            if (line == EnumCoordLine.Lon)
            {
                degree += (isNegative) ? "W" : "E";
            }
            else
            {
                degree += (isNegative) ? "S" : "N";
            }
            return degree + minutes + seconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Degree"></param>
        /// <returns></returns>
        static public double DecToDms(double Degree)
        {
            double Dms = ConnectDB.NullD;
            if (Degree != ConnectDB.NullD)
            {
                // Градусы
                Dms = (double)((int)(Degree));
                Degree -= Dms;
                // Минуты
                Degree *= 60.0;
                double tmp = (double)((int)(Degree));
                Degree -= tmp;
                Dms += tmp / 100.0;
                //Секунды
                Degree *= 60.0;
                tmp = (double)(((int)(Degree * 10.0)) / 10.0);
                Dms += tmp / 10000.0;
            }
            return Dms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Degree"></param>
        /// <returns></returns>
        public static double DmsToDec(double Degree)
        {
            double retVal = ConnectDB.NullD;
            if (Degree != ConnectDB.NullD)
            {
                // Градусы
                double dgr = (double)((int)(Degree));
                Degree -= dgr;
                // Минуты
                Degree *= 100.0;
                double min = (double)((int)(Degree));
                Degree -= min;
                //Секунды
                Degree *= 100.0;
                double sec = Degree;
                //double retVal = (sec / 60.0 + min) / 60.0 + dgr;
                retVal = sec / 3600.0 + min / 60 + dgr;
            }
            return retVal;
        }



        /// <summary>
        /// Преобразование строк в табличный формат вывода
        /// </summary>
        /// <param name="types">Список типов</param>
        /// <param name="NameField">Список полей</param>
        /// <param name="values">Список строк, содержащих ячейки с данными</param>
        /// <returns>Сформированный объект DataTable</returns>
        public static DataTable GetData(Type[] types, string[] NameField, List<object[]> values)
        {
            DataTable dt = new DataTable();
            try
            {
                ConnectDB conn = new ConnectDB();
                for (int i = 0; i < NameField.Count(); i++)
                {
                    dt.Columns.Add(new DataColumn(NameField[i], types[i]));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < values[i].Count(); j++)
                    {
                        dr[NameField[j]] = values[i][j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="type_r"></param>
        /// <returns></returns>
        public static bool CheckEqualObjects(object P1, object P2, TypeRadioButton type_r)
        {
            bool CompareRes = false;
            if (!string.IsNullOrEmpty(P2.ToString()))
            {
                if ((P1 is string) && (P2 is string))
                {
                    string Vl_D1; string Vl_D2;
                    Vl_D1 = ((string)P1).ToLower();
                    Vl_D2 = ((string)P2).ToLower();
                    if (Vl_D1.Contains(Vl_D2))
                    {
                        CompareRes = true;
                    }
                }

                if ((P1 is bool) && (P2 is bool))
                {
                    bool Vl_D1; bool Vl_D2;
                    Vl_D1 = (bool)P1;
                    Vl_D2 = (bool)P2;
                    if (Vl_D1 == Vl_D2)
                    {
                        CompareRes = true;
                    }
                }
                ///

                if ((P1 is double) && (P2 is double))
                {
                    double Vl_D1; double Vl_D2;
                    Vl_D1 = (double)P1;
                    Vl_D2 = (double)P2;
                    if (type_r == TypeRadioButton.Less)
                    {
                        if (Vl_D1 < Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                if ((P1 is int) && (P2 is int))
                {
                    int Vl_D1; int Vl_D2;
                    Vl_D1 = (int)P1;
                    Vl_D2 = (int)P2;
                    if (type_r == TypeRadioButton.Less)
                    {
                        if (Vl_D1 < Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                if ((P1 is DateTime) && (P2 is DateTime))
                {
                    DateTime Vl_D1; DateTime Vl_D2;
                    Vl_D1 = (DateTime)P1;
                    Vl_D2 = (DateTime)P2;
                    if (type_r == TypeRadioButton.Less)
                    {
                        if (Vl_D1 < Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                ////////////

                if ((P1 is double) && (P2 is double))
                {
                    double Vl_D1; double Vl_D2;
                    Vl_D1 = (double)P1;
                    Vl_D2 = (double)P2;
                    if (type_r == TypeRadioButton.More)
                    {
                        if (Vl_D1 > Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                if ((P1 is int) && (P2 is int))
                {
                    int Vl_D1; int Vl_D2;
                    Vl_D1 = (int)P1;
                    Vl_D2 = (int)P2;
                    if (type_r == TypeRadioButton.More)
                    {
                        if (Vl_D1 > Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                if ((P1 is DateTime) && (P2 is DateTime))
                {
                    DateTime Vl_D1; DateTime Vl_D2;
                    Vl_D1 = (DateTime)P1;
                    Vl_D2 = (DateTime)P2;
                    if (type_r == TypeRadioButton.More)
                    {
                        if (Vl_D1 > Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                ////////////

                if ((P1 is double) && (P2 is double))
                {
                    double Vl_D1; double Vl_D2;
                    Vl_D1 = (double)P1;
                    Vl_D2 = (double)P2;
                    if (type_r == TypeRadioButton.Equally)
                    {
                        if (Vl_D1 == Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                if ((P1 is int) && (P2 is int))
                {
                    int Vl_D1; int Vl_D2;
                    Vl_D1 = (int)P1;
                    Vl_D2 = (int)P2;
                    if (type_r == TypeRadioButton.Equally)
                    {
                        if (Vl_D1 == Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }

                if ((P1 is DateTime) && (P2 is DateTime))
                {
                    DateTime Vl_D1; DateTime Vl_D2;
                    Vl_D1 = (DateTime)P1;
                    Vl_D2 = (DateTime)P2;
                    if (type_r == TypeRadioButton.Equally)
                    {
                        if (Vl_D1 == Vl_D2)
                        {
                            CompareRes = true;
                        }
                    }
                }
            }
            else
            {
                CompareRes = true;
            }
            return CompareRes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gvrc"></param>
        /// <param name="startIndex"></param>
        /// <param name="total"></param>
        public static void GroupGridView(GridViewRowCollection gvrc, int startIndex, int total)
        {
            try
            {
                if (total == 0) return;
                int i, count = 1;
                ArrayList lst = new ArrayList();
                lst.Add(gvrc[0]);

                var ctrl = gvrc[0].Cells[startIndex];
                for (i = 1; i < gvrc.Count; i++)
                {
                    TableCell nextCell = gvrc[i].Cells[startIndex];
                    if (ctrl.Text == nextCell.Text)
                    {
                        count++;
                        nextCell.Visible = false;
                        lst.Add(gvrc[i]);
                    }
                    else
                    {
                        if (count > 1)
                        {
                            ctrl.RowSpan = count;
                            GroupGridView(new GridViewRowCollection(lst), startIndex + 1, total - 1);
                        }
                        count = 1;
                        lst.Clear();
                        ctrl = gvrc[i].Cells[startIndex];
                        lst.Add(gvrc[i]);
                    }
                }
                if (count > 1)
                {
                    ctrl.RowSpan = count;
                    GroupGridView(new GridViewRowCollection(lst), startIndex + 1, total - 1);
                }
                count = 1;
                lst.Clear();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Преобразование строк в табличный формат вывода
        /// </summary>
        /// <param name="types"></param>
        /// <param name="NameField"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DataTable GetData(ClassIRPObject values, List<BlockDataFind> Find_Params)
        {
            DataTable dt = new DataTable();
            try
            {
                //if (Find_Params != null)
                {
                    List<Type> tp_lst = new List<Type>();
                    List<string> LF = new List<string>();
                    int idxKey = -1;
                    for (int i = 0; i < values.Val_Arr.Count(); i++)
                    {
                        for (int j = 0; j < values.Val_Arr[i].Count(); j++)
                        {
                            if (values.CAPTION_FLD[j].Contains("ID"))
                            {
                                idxKey = j;
                            }

                            if ((values.Setting_param.MAX_COLUMNS != ConnectDB.NullI) && (values.Setting_param.MAX_COLUMNS > 0))
                            {
                                if (j < values.Setting_param.MAX_COLUMNS)
                                {
                                    tp_lst.Add(values.Val_Arr[i][j]!=null ? values.Val_Arr[i][j].GetType() : typeof(string));


                                    if (!LF.Contains(values.CAPTION_FLD[j]))
                                        LF.Add(values.CAPTION_FLD[j]);
                                }
                            }
                            else
                            {
                                tp_lst.Add(values.Val_Arr[i][j] != null ? values.Val_Arr[i][j].GetType() : typeof(string));
                            }
                        }
                        break;
                    }

                    if ((idxKey > -1) && (!LF.Contains("ID")))
                    {
                        if (values.Val_Arr.Count > 0)
                        {
                            tp_lst.Insert(0, values.Val_Arr[0][idxKey]!=null ? values.Val_Arr[0][idxKey].GetType() : typeof(string));
                        };
                    }


                    tp_lst = values.FLD_TYPE;


                    if ((idxKey > -1) && (!LF.Contains("ID")))
                    {
                        dt.Columns.Add(new DataColumn(values.CAPTION_FLD[idxKey], tp_lst[0] == typeof(DateTime) ? typeof(System.String) : tp_lst[0]));
                    }

                    for (int i = 0; i < values.CAPTION_FLD.Count(); i++)
                    {

                        if ((values.Setting_param.MAX_COLUMNS != ConnectDB.NullI) && (values.Setting_param.MAX_COLUMNS > 0))
                        {
                            if (i < values.Setting_param.MAX_COLUMNS)
                            {
                                if ((idxKey > -1) && (!LF.Contains("ID")))
                                {
                                    bool isFindColumn = false;
                                    foreach (DataColumn collect in dt.Columns) { if (collect.ColumnName == values.CAPTION_FLD[i]) { isFindColumn = true; break; } }
                                    if (isFindColumn == false) dt.Columns.Add(new DataColumn(values.CAPTION_FLD[i], tp_lst[i + 1] == typeof(DateTime) ? typeof(System.String) : tp_lst[i + 1]));
                                }
                                else
                                {
                                    bool isFindColumn = false;
                                    foreach (DataColumn collect in dt.Columns) { if (collect.ColumnName == values.CAPTION_FLD[i]) { isFindColumn = true; break; } }
                                    if (isFindColumn == false) dt.Columns.Add(new DataColumn(values.CAPTION_FLD[i], tp_lst[i] == typeof(DateTime) ? typeof(System.String) : tp_lst[i]));
                                }

                            }
                        }
                        else {
                            bool isFindColumn = false;
                            foreach (DataColumn collect in dt.Columns) { if (collect.ColumnName == values.CAPTION_FLD[i]) { isFindColumn = true; break; } }
                            if (isFindColumn == false) dt.Columns.Add(new DataColumn(values.CAPTION_FLD[i], tp_lst[i] == typeof(DateTime) ? typeof(System.String) : tp_lst[i]));
                        }
                    }


                    ///Session["NavigationPages"] 

                    for (int i = 0; i < values.Val_Arr.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();


                        if ((idxKey > -1) && (!LF.Contains("ID")))
                        {
                            dr["ID"] = values.Val_Arr[i][idxKey];
                        }

                        for (int j = 0; j < values.Val_Arr[i].Count(); j++)
                        {

                            if ((values.Setting_param.MAX_COLUMNS != ConnectDB.NullI) && (values.Setting_param.MAX_COLUMNS > 0))
                            {
                                if (j < values.Setting_param.MAX_COLUMNS)
                                {
                                    if (values.Val_Arr[i][j] == null) values.Val_Arr[i][j] = "";
                                    if ((values.Val_Arr[i][j].ToString() != ConnectDB.NullD.ToString()) && (values.Val_Arr[i][j].ToString() != ConnectDB.NullI.ToString()) && values.Val_Arr[i][j].ToString() != new DateTime(01, 01, 01, 0, 0, 0).ToString())
                                    {
                                        if (values.CAPTION_FLD[j] == "PATH")
                                        {
                                            string fl = System.IO.Path.GetFileName(values.Val_Arr[i][j] == null ? "" : values.Val_Arr[i][j].ToString());
                                            dr[values.CAPTION_FLD[j]] = fl;
                                        }

                                        else
                                        {
                                            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                                            string Val_ = ""; object bl_f = "";
                                            switch (tp_lst[j].ToString())
                                            {
                                                case "System.Double":
                                                    double Vl_D;
                                                    Val_ = values.Val_Arr[i][j].ToString().Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                    if (double.TryParse(Val_, out Vl_D)) { bl_f = Vl_D; } else { bl_f = ConnectDB.NullD; }
                                                    break;
                                                case "System.Int32":
                                                    Val_ = values.Val_Arr[i][j].ToString().Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                    int Vl_I;
                                                    if (int.TryParse(Val_, out Vl_I)) { bl_f = Vl_I; } else { bl_f = ConnectDB.NullI; }
                                                    break;
                                                case "System.String":
                                                    bl_f = values.Val_Arr[i][j].ToString();
                                                    break;
                                                case "System.DateTime":
                                                    DateTime Vl_T;
                                                    if (DateTime.TryParse(values.Val_Arr[i][j].ToString(), out Vl_T)) { bl_f = Vl_T.ToString("yyyy-MM-dd"); } else { bl_f = ConnectDB.NullT; }
                                                    break;
                                                default:
                                                    bl_f = values.Val_Arr[i][j];
                                                    break;
                                            }


                                            dr[values.CAPTION_FLD[j]] = bl_f;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if ((values.Val_Arr[i][j].ToString() != ConnectDB.NullD.ToString()) && (values.Val_Arr[i][j].ToString() != ConnectDB.NullI.ToString()) && values.Val_Arr[i][j].ToString() != new DateTime(01, 01, 01, 0, 0, 0).ToString())
                                {
                                    if (values.CAPTION_FLD[j] == "PATH")
                                    {
                                        string fl = System.IO.Path.GetFileName(values.Val_Arr[i][j] == null ? "" : values.Val_Arr[i][j].ToString());
                                        dr[values.CAPTION_FLD[j]] = fl;
                                    }
                                    else
                                    {
                                        string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                                        string Val_ = ""; object bl_f = "";
                                        switch (tp_lst[j].ToString())
                                        {
                                            case "System.Double":
                                                double Vl_D;
                                                Val_ = values.Val_Arr[i][j].ToString().Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                if (double.TryParse(Val_, out Vl_D)) { bl_f = Vl_D; } else { bl_f = ""; }
                                                break;
                                            case "System.Int32":
                                                Val_ = values.Val_Arr[i][j].ToString().Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                int Vl_I;
                                                if (int.TryParse(Val_, out Vl_I)) { bl_f = Vl_I; } else { bl_f = ""; }
                                                break;
                                            case "System.String":
                                                bl_f = values.Val_Arr[i][j].ToString();
                                                break;
                                            case "System.DateTime":
                                                DateTime Vl_T;
                                                if (DateTime.TryParse(values.Val_Arr[i][j].ToString(), out Vl_T)) { bl_f = Vl_T.ToString("yyyy-MM-dd"); } else { bl_f = ""; }
                                                break;
                                            default:
                                                bl_f = values.Val_Arr[i][j];
                                                break;
                                        }


                                        dr[values.CAPTION_FLD[j]] = bl_f;
                                    }

                                }
                            }
                        }



                        bool isAdditionalCompare1 = false;
                        bool isAdditionalCompare2 = false;
                        bool isCorrect = true;
                        List<bool> CorrectISV = new List<bool>();
                        string StrRecViewUser = "";
                        if (Find_Params != null)
                        {

                            double LON_point = ConnectDB.NullD; double LON = ConnectDB.NullD;
                            double LAT_point = ConnectDB.NullD; double LAT = ConnectDB.NullD;
                            double RADIUS_point = ConnectDB.NullD; double RADIUS = ConnectDB.NullD;

                            for (int x = 0; x < Find_Params.Count(); x++) { CorrectISV.Add(true); }
                            for (int k = 0; k < values.CAPTION_FLD.Count(); k++)
                            {
                                if (values.CAPTION_FLD[k].Trim() == "ID")
                                    continue;


                                BlockDataFind block_f = Find_Params.Find(r => r.CaptionField == values.CAPTION_FLD[k]);
                                if (block_f == null) continue;

                                string dr_ = values.Val_Arr[i][k].ToString();
                                string in_ = block_f.Value.ToString();

                                //string in_ = Find_Params[k].Value.ToString();
                                ////////////////
                                if (values.Setting_param.IS_SQL_REQUEST)
                                {
                                    string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                                    string rds_ = dr_.ToString();
                                    if ((dr_.ToString().EndsWith("_")) && ((dr_.ToString().Contains(",")) || (dr_.ToString().Contains("."))))
                                    {
                                        rds_ = dr_.ToString().Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                        dr_ = rds_;

                                    }
                                    else {
                                        if (dr_.ToString().EndsWith("_"))
                                        {
                                            rds_ = dr_.ToString().Replace("_", "");
                                            char[] chs = rds_.ToArray();
                                            bool isSuccess = true;
                                            foreach (char c in chs)
                                            {
                                                if ((!char.IsDigit(c)) && (c != '-'))
                                                {
                                                    isSuccess = false;
                                                    break;
                                                }
                                            }
                                            if (!isSuccess)
                                            {
                                                rds_ = dr_.ToString();
                                                dr_ = rds_;
                                            }
                                        }
                                        else if (((dr_.ToString().Contains(",")) || (dr_.ToString().Contains("."))) && (!dr_.ToString().EndsWith("_")))
                                        {
                                            rds_ = dr_.ToString().Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                            dr_ = rds_;
                                        }
                                    }


                                    rds_ = in_.ToString();
                                    if ((in_.ToString().EndsWith("_")) && ((in_.ToString().Contains(",")) || (in_.ToString().Contains("."))))
                                    {
                                        rds_ = in_.ToString().Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                        in_ = rds_;

                                    }
                                    else
                                    {
                                        if (in_.ToString().EndsWith("_"))
                                        {
                                            rds_ = in_.ToString().Replace("_", "");
                                            char[] chs = rds_.ToArray();
                                            bool isSuccess = true;
                                            foreach (char c in chs)
                                            {
                                                if ((!char.IsDigit(c)) && (c != '-'))
                                                {
                                                    isSuccess = false;
                                                    break;
                                                }
                                            }
                                            if (!isSuccess)
                                            {
                                                rds_ = in_.ToString();
                                                in_ = rds_;
                                            }
                                        }
                                        else if (((in_.ToString().Contains(",")) || (in_.ToString().Contains("."))) && (!in_.ToString().EndsWith("_")))
                                        {
                                            rds_ = in_.ToString().Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                            in_ = rds_;
                                        }
                                    }
                                }
                                ///////////////



                                if (values.Setting_param.SYS_COORD == "DMS")
                                {
                                    if (values.CAPTION_FLD[k].Contains("Ilguma"))
                                    {
                                        string L_DMS_dr_ = dr_.Replace("S", ",").Replace("E", ",").Replace("W", ",").Replace("N", ",");
                                        double L_DMS_dr_double = ConnectDB.NullD;
                                        if (double.TryParse(L_DMS_dr_, out L_DMS_dr_double))
                                            dr_ = DataAdapterClass.DmsToDec(L_DMS_dr_double).ToString();
                                        else L_DMS_dr_double = ConnectDB.NullD;

                                        string L_DMS_in_ = in_.Replace("S", ",").Replace("E", ",").Replace("W", ",").Replace("N", ",");
                                        double L_DMS_in_double = ConnectDB.NullD;
                                        if (double.TryParse(L_DMS_in_, out L_DMS_in_double))
                                            in_ = DataAdapterClass.DmsToDec(L_DMS_in_double).ToString();
                                        else L_DMS_in_double = ConnectDB.NullD;

                                    }

                                    if (values.CAPTION_FLD[k].Contains("Platuma"))
                                    {
                                        string L_DMS_dr_ = dr_.Replace("S", ",").Replace("E", ",").Replace("W", ",").Replace("N", ",");
                                        double L_DMS_dr_double = ConnectDB.NullD;
                                        if (double.TryParse(L_DMS_dr_, out L_DMS_dr_double))
                                            dr_ = DataAdapterClass.DmsToDec(L_DMS_dr_double).ToString();
                                        else L_DMS_dr_double = ConnectDB.NullD;

                                        string L_DMS_in_ = in_.Replace("S", ",").Replace("E", ",").Replace("W", ",").Replace("N", ",");
                                        double L_DMS_in_double = ConnectDB.NullD;
                                        if (double.TryParse(L_DMS_in_, out L_DMS_in_double))
                                            in_ = DataAdapterClass.DmsToDec(L_DMS_in_double).ToString();
                                        else L_DMS_in_double = ConnectDB.NullD;
                                    }
                                }

                                {
                                    if (!string.IsNullOrEmpty(dr_)) StrRecViewUser += string.Format(" Field {0} = {1} ", values.CAPTION_FLD[k], dr_);
                                    if (values.CAPTION_FLD[k].Trim().Contains("Ilguma"))
                                    {
                                        if (dr_.ToString() != "") double.TryParse(dr_.ToString(), out LON);
                                        if (in_.Trim() != "") double.TryParse(in_, out LON_point);
                                    }
                                    if (values.CAPTION_FLD[k].Trim().Contains("Platuma"))
                                    {

                                        if (dr_.ToString() != "") double.TryParse(dr_.ToString(), out LAT);
                                        if (in_.Trim() != "") double.TryParse(in_, out LAT_point);
                                    }
                                    if (values.CAPTION_FLD[k].Trim().Contains("Spindulys"))
                                    {
                                        if (dr_.ToString() != "") double.TryParse(dr_.ToString(), out RADIUS);
                                        if (in_.Trim() != "") double.TryParse(in_, out RADIUS_point);
                                    }

                                    if ((!((values.CAPTION_FLD[k].Contains("Ilguma")) || (values.CAPTION_FLD[k].Contains("Platuma")) || (values.CAPTION_FLD[k].Contains("Spindulys")))))
                                    {
                                        object Vl_h = new object();



                                        switch (block_f.type_.ToString())
                                        {
                                            case "System.Double":
                                                double Vl_D;
                                                if (double.TryParse(dr_, out Vl_D)) { Vl_h = Vl_D; } else { Vl_h = ConnectDB.NullD; }
                                                break;
                                            case "System.Int32":
                                                int Vl_I;
                                                if (int.TryParse(dr_, out Vl_I)) { Vl_h = Vl_I; } else { Vl_h = ConnectDB.NullI; }
                                                break;
                                            case "System.String":
                                                Vl_h = dr_;
                                                break;
                                            case "System.DateTime":
                                                DateTime Vl_T;
                                                if (DateTime.TryParse(dr_, out Vl_T)) { Vl_h = Vl_T; } else { Vl_h = ConnectDB.NullT; }
                                                break;
                                            default:
                                                Vl_h = dr_;
                                                break;
                                        }



                                        if (!CheckEqualObjects(Vl_h, block_f.Value, block_f.typeRadio_))
                                        {
                                            isAdditionalCompare1 = true;
                                            isCorrect = false;
                                        }

                                    }
                                    else
                                    {
                                        if (((values.CAPTION_FLD[k].Contains("Ilguma")) && (LON_point != ConnectDB.NullD)) || ((values.CAPTION_FLD[k].Contains("Platuma")) && (LAT_point != ConnectDB.NullD)) || (((values.CAPTION_FLD[k].Contains("Spindulys")) && ((RADIUS_point != ConnectDB.NullD) || (RADIUS_point != -1)))))
                                        {


                                            object Vl_h = new object();

                                            switch (block_f.type_.ToString())
                                            {
                                                case "System.Double":
                                                    double Vl_D;
                                                    if (double.TryParse(dr_, out Vl_D)) { Vl_h = Vl_D; } else { Vl_h = ""; }
                                                    break;
                                                case "System.Int32":
                                                    int Vl_I;
                                                    if (int.TryParse(dr_, out Vl_I)) { Vl_h = Vl_I; } else { Vl_h = ""; }
                                                    break;
                                                case "System.String":
                                                    Vl_h = dr_;
                                                    break;
                                                case "System.DateTime":
                                                    DateTime Vl_T;
                                                    if (DateTime.TryParse(dr_, out Vl_T)) { Vl_h = Vl_T; } else { Vl_h = ""; }
                                                    break;
                                                default:
                                                    Vl_h = dr_;
                                                    break;
                                            }



                                            if (!CheckEqualObjects(Vl_h, block_f.Value, block_f.typeRadio_))
                                            {
                                                isAdditionalCompare2 = true;
                                                //isCorrect = false;
                                            }
                                        }
                                    }



                                    /*
                                    if (!dr_.Contains(in_))
                                    {
                                        isCorrect = false;
                                    }
                                     */
                                }
                            }


                            //if ((values.StatusObject == TypeStatus.HCC) || (values.StatusObject == TypeStatus.ISV)) 
                            //if (!isAdditionalCompare)
                            bool isNullCoordinate = true;
                            if ((LAT != ConnectDB.NullD) && (LON != ConnectDB.NullD) && (LON_point != ConnectDB.NullD) && (LAT_point != ConnectDB.NullD) && (RADIUS_point != ConnectDB.NullD) && (RADIUS_point != -1))
                            {
                                isNullCoordinate = false;
                                double R_f = Math.Pow((Math.Pow(((111.35 * (LON_point - LON)) * Math.Cos(3.1415 * (LAT_point + LAT) / 360)), 2) + Math.Pow(111.35 * (LAT_point - LAT), 2)), 0.5);

                                //double R_f = Math.Pow(Math.Pow(((LON_point - LON) * Math.Cos(3.1415 * (LAT_point + LAT) / 360)), 2) + Math.Pow((LAT_point - LAT), 2), 0.5);
                                if (!(R_f <= RADIUS_point))
                                {
                                    isCorrect = false;
                                }
                                else
                                {

                                    if ((isAdditionalCompare1) && (isAdditionalCompare2)) isCorrect = false;
                                   
                                }
                            }
                            else
                            {
                                if (isAdditionalCompare2) isCorrect = false;
                                if ((isNullCoordinate) && (LON_point != ConnectDB.NullD) && (LAT_point != ConnectDB.NullD)) { isCorrect = false; }
                            }

                        }
                        {
                            if (isCorrect) dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetData: " + ex.Message);
            }
            return dt;

        }
    }



  

}