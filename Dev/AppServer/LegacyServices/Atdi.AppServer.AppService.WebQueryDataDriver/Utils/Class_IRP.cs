using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OrmCs;
using FormsCs;
using System.Text;


namespace Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities
{
    public enum RolesUsers
    {
        Admin,
        Provider,
        Government,
        HealthCare,
        Guest,
        Provider_Government,
        Provider_Healthcare,
        Government_Healthcare
    }
    public class Class_ManageSettingInstance
    {

        /// <summary>
        /// Настройки с таблицы WEB_QUERY
        /// </summary>
        /// <param name="SettingTableName"></param>
        /// <returns></returns>
        public static List<SettingIRPClass> GetSettingWebQuery(string SettingTableName)
        {
            List<SettingIRPClass> IRP = new List<SettingIRPClass>();
            try
            {
                ConnectDB conn = new ConnectDB();
                bool IsSelectedData = false;
                List<string> ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 0);
                List<string> Name = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 1);
                List<string> Status = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 2);
                List<string> Val_Query = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 3);
                List<string> Description = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 4);
                List<string> MaxRec = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 5);
                List<string> IsVisible = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 6);
                List<string> MaxCol = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 7);
                List<string> IdentUser = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 8);
                List<string> MaxRecPage = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 9);
                List<string> ExtendedControlRight = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 10);
                List<string> FieldGroup = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 11);
                List<string> FieldIsSqlRequest = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 12);
                List<string> Comments = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 13);
                List<string> FieldIsFillColor = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 14);
                List<string> FieldSysCoord = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 15);
                List<string> FieldIsSendNotify = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 16);
                List<string> FieldIsTestingRequest = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 17);
                List<string> FieldIsVisibleAllRec = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 18);
                List<string> FieldEmailQuest = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 19);
                List<string> FieldEmailAnswer = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 20);
                List<string> FieldIsEnableInterval = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 21);
                List<string> FieldMaxValInterval = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 22);
                List<string> FieldMinValInterval = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 23);
                List<string> FieldMaxVal = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 24);
                List<string> FieldValidLicense = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 25);
                List<string> FieldTotalCountGroup = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 26);
                List<string> HideCol = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 27);
                List<string> LicType2 = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 28);
                List<string> LicType3 = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 29);
                List<string> LicAdmName = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 30);
                List<string> LicType = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 31);



                for (int i = 0; i < Name.Count(); i++)
                {
                    SettingIRPClass ir = new SettingIRPClass();
                    int Num = -1;
                    if (int.TryParse(ID[i], out Num))
                    { ir.ID = Num; }
                    else { ir.ID = ConnectDB.NullI; }


                    ir.NAME = Name[i];
                    ir.DESCRIPTION = Description[i];
                    ir.Query = Val_Query[i];
                    ir.Ident_User = IdentUser[i];
                    ir.ExtendedControlRight = ExtendedControlRight[i];
                    ir.GROUP_FIELD = FieldGroup[i];
                    ir.COMMENTS = Comments[i];
                    ir.TYPE2 = LicType2[i];
                    ir.TYPE3 = LicType3[i];
                    ir.ADM_NAME = LicAdmName[i];
                    ir.SYS_COORD = FieldSysCoord[i];
                    ir.EMAIL_QUESTION = FieldEmailQuest[i];
                    ir.EMAIL_ANSWER = FieldEmailAnswer[i];
                    ir.TYPE = LicType[i];

                    switch (Status[i])
                    {
                        case "PRI":
                            ir.STATUS_ = TypeStatus.PRI;
                            break;
                        case "PUB":
                            ir.STATUS_ = TypeStatus.PUB;
                            break;
                        case "CUS":
                            ir.STATUS_ = TypeStatus.CUS;
                            break;
                        case "DSL":
                            ir.STATUS_ = TypeStatus.DSL;
                            break;
                        case "LT1":
                            ir.STATUS_ = TypeStatus.LT1;
                            break;
                        case "LT2":
                            ir.STATUS_ = TypeStatus.LT2;
                            break;
                        case "ISV":
                            ir.STATUS_ = TypeStatus.ISV;
                            break;
                        case "HCC":
                            ir.STATUS_ = TypeStatus.HCC;
                            break;
                        case "CON":
                            ir.STATUS_ = TypeStatus.CON;
                            break;
                    }

                    Num = -1;
                    if (int.TryParse(MaxRec[i], out Num))
                    { ir.MAX_REC = Num; }
                    else { ir.MAX_REC = 50; }

                    Num = -1;
                    if (int.TryParse(MaxRecPage[i], out Num))
                    { ir.MAX_REC_PAGE = Num; }
                    else { ir.MAX_REC_PAGE = 50; }


                    Num = -1;
                    if (int.TryParse(MaxCol[i], out Num))
                    { ir.MAX_COLUMNS = Num; }
                    else { ir.MAX_COLUMNS = ConnectDB.NullI; }

                    Num = 0;
                    int.TryParse(IsVisible[i], out Num);
                    if (Num == 0) { ir.IS_VISIBLE = false; } else { ir.IS_VISIBLE = true; }

                    Num = 0;
                    int.TryParse(FieldIsVisibleAllRec[i], out Num);
                    if (Num == 0) { ir.IS_VISIBLE_ALL_REC = false; } else { ir.IS_VISIBLE_ALL_REC = true; }

                    Num = 0;
                    int.TryParse(FieldIsFillColor[i], out Num);
                    if (Num == 0) { ir.IS_FILL_COLOR = false; } else { ir.IS_FILL_COLOR = true; }


                    Num = 0;
                    int.TryParse(FieldIsSqlRequest[i], out Num);
                    if (Num == 0) { ir.IS_SQL_REQUEST = false; } else { ir.IS_SQL_REQUEST = true; }


                    Num = 0;
                    int.TryParse(FieldIsSendNotify[i], out Num);
                    if (Num == 0) { ir.IS_SEND_NOTIFY_MESS = false; } else { ir.IS_SEND_NOTIFY_MESS = true; }

                    Num = 0;
                    int.TryParse(FieldIsTestingRequest[i], out Num);
                    if (Num == 0) { ir.IS_TESTING_REQUEST = false; } else { ir.IS_TESTING_REQUEST = true; }

                    Num = 0;
                    int.TryParse(FieldIsEnableInterval[i], out Num);
                    if (Num == 0) { ir.IS_ENABLE_INTERVAL = false; } else { ir.IS_ENABLE_INTERVAL = true; }


                    Num = -1;
                    int.TryParse(FieldMaxValInterval[i], out Num);
                    if (Num > 0) { ir.MAX_VAL_INTERVAL = Num; } else { ir.MAX_VAL_INTERVAL = 1000; }

                    Num = -1;
                    int.TryParse(FieldMinValInterval[i], out Num);
                    if (Num > 0) { ir.MIN_VAL_INTERVAL = Num; } else { ir.MIN_VAL_INTERVAL = 1000; }

                    Num = -1;
                    int.TryParse(FieldMaxVal[i], out Num);
                    if (Num > 0) { ir.MAX_VAL = Num; } else { ir.MAX_VAL = 0; }


                    Num = 0;
                    int.TryParse(FieldValidLicense[i], out Num);
                    if (Num == 0) { ir.IS_VALID_LICENSE = false; } else { ir.IS_VALID_LICENSE = true; }

                    Num = 1;
                    int.TryParse(FieldTotalCountGroup[i], out Num);
                    if (Num >= 1) { ir.TOTAL_COUNT_GROUP = Num; }


                    Num = -1;
                    if (int.TryParse(HideCol[i], out Num))
                    { ir.HIDDEN_COLUMNS = Num; }
                    else { ir.HIDDEN_COLUMNS = ConnectDB.NullI; }


                    IRP.Add(ir);
                }
            }
            catch (Exception)
            {

            }

            return IRP;
        }


        /// <summary>
        /// Получить все записи таблицы WEBDELEG_STAT_LST
        /// </summary>
        /// <param name="SettingTableName"></param>
        /// <returns></returns>
        public static List<WebDelegatedStations> GetSettingWebDelegatedStations(string SettingTableName)
        {
            List<WebDelegatedStations> IRP = new List<WebDelegatedStations>();
            try
            {
                ConnectDB conn = new ConnectDB();
                bool IsSelectedData = false;
                List<string> ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 0);
                List<string> TABLE_NAME = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 1);
                List<string> RECORD_ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 2);
                List<string> STATUS = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 3);
                List<string> USER_ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 4);
                List<string> QUERY_ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 5);
                for (int i = 0; i < TABLE_NAME.Count(); i++)
                {
                    WebDelegatedStations ir = new WebDelegatedStations();

                    int Num = -1;
                    if (int.TryParse(ID[i], out Num))
                    { ir.ID = Num; }
                    else { ir.ID = ConnectDB.NullI; }

                    ir.TABLE_NAME = TABLE_NAME[i];

                    Num = -1;
                    if (int.TryParse(RECORD_ID[i], out Num))
                    { ir.RECORD_ID = Num; }
                    else { ir.RECORD_ID = ConnectDB.NullI; }

                    Num = -1;
                    if (int.TryParse(USER_ID[i], out Num))
                    { ir.USER_ID = Num; }
                    else { ir.USER_ID = ConnectDB.NullI; }

                    Num = -1;
                    if (int.TryParse(QUERY_ID[i], out Num))
                    { ir.QUERY_ID = Num; }
                    else { ir.QUERY_ID = ConnectDB.NullI; }

                    Num = -1;
                    if (int.TryParse(STATUS[i], out Num))
                    { ir.IS_ACTIVATE = Num; }
                    else { ir.IS_ACTIVATE = ConnectDB.NullI; }

                    IRP.Add(ir);
                }
            }
            catch (Exception)
            {
            }
            return IRP;
        }

        /// <summary>
        /// Получить все записи таблицы XWEB_DEL
        /// </summary>
        /// <param name="SettingTableName"></param>
        /// <returns></returns>
        public static List<XWeb_Del> GetSettingWebDeltations(string SettingTableName)
        {
            List<XWeb_Del> IRP = new List<XWeb_Del>();
            try
            {
                ConnectDB conn = new ConnectDB();
                bool IsSelectedData = false;
                List<string> ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 0);
                List<string> TABLE_NAME = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 1);
                List<string> RECORD_ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 2);
                for (int i = 0; i < TABLE_NAME.Count(); i++)
                {
                    XWeb_Del ir = new XWeb_Del();
                    int Num = -1;
                    if (int.TryParse(ID[i], out Num))
                    { ir.ID = Num; }
                    else { ir.ID = ConnectDB.NullI; }
                    ir.TABLE_NAME = TABLE_NAME[i];
                    Num = -1;
                    if (int.TryParse(RECORD_ID[i], out Num))
                    { ir.RECORD_ID = Num; }
                    else { ir.RECORD_ID = ConnectDB.NullI; }
                    IRP.Add(ir);
                }
            }
            catch (Exception)
            {
            }
            return IRP;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SettingTableName"></param>
        /// <returns></returns>
        public static List<WebAddParams> GetSettingAddParams(string SettingTableName)
        {
            List<WebAddParams> IRP = new List<WebAddParams>();
            try
            {
                ConnectDB conn = new ConnectDB();
                bool IsSelectedData = false;
                List<string> ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 0);
                List<string> WEB_QUERY_ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 1);
                List<string> NAME = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 2);
                List<string> TYPE_COMP = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 3);
                List<string> DOMAIN = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 4);
                List<string> ERI_FILE_NAME = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 5);
                List<string> ERI_CONTENT = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 6);
                for (int i = 0; i < NAME.Count(); i++)
                {
                    WebAddParams ir = new WebAddParams();
                    int Num = -1;
                    if (int.TryParse(ID[i], out Num))
                    { ir.ID = Num; }
                    else { ir.ID = ConnectDB.NullI; }

                    Num = -1;
                    if (int.TryParse(TYPE_COMP[i], out Num))
                    { ir.TYPE_COMP = Num; }
                    else { ir.TYPE_COMP = ConnectDB.NullI; }

                    Num = -1;
                    if (int.TryParse(WEB_QUERY_ID[i], out Num))
                    { ir.WEB_QUERY_ID = Num; }
                    else { ir.WEB_QUERY_ID = ConnectDB.NullI; }

                    ir.NAME = NAME[i];
                    ir.DOMAIN = DOMAIN[i];
                    ir.ERI_FILE_NAME = ERI_FILE_NAME[i];
                    ir.ERI_CONTENT = ERI_CONTENT[i];

                    IRP.Add(ir);
                }
            }
            catch (Exception)
            {
            }
            return IRP;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SettingTableName"></param>
        /// <returns></returns>
        public static List<WebConstraint> GetSettingWebConstraint(string SettingTableName)
        {
            List<WebConstraint> IRP = new List<WebConstraint>();
            try
            {
                ConnectDB conn = new ConnectDB();
                bool IsSelectedData = false;
                List<string> ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 0);
                List<string> WEB_QUERY_ID = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 1);
                List<string> NAME = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 2);
                List<string> PATH = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 3);
                List<string> MIN = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 4);
                List<string> MAX = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 5);
                List<string> STR_VALUE = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 6);
                List<string> DATE_VALUE = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 7);
                List<string> INCLUDE = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 8);
                List<string> DATE_VALUE_MAX = conn.SelectAllRec_From_WebQuery(out IsSelectedData, SettingTableName, 9);
                for (int i = 0; i < PATH.Count(); i++)
                {
                    WebConstraint ir = new WebConstraint();
                    int Num = -1;
                    if (int.TryParse(ID[i], out Num))
                    { ir.ID = Num; }
                    else { ir.ID = ConnectDB.NullI; }

                    Num = -1;
                    if (int.TryParse(WEB_QUERY_ID[i], out Num))
                    { ir.QUERY_ID = Num; }
                    else { ir.QUERY_ID = ConnectDB.NullI; }

                    ir.NAME = NAME[i];
                    ir.PATH = PATH[i];

                    double Num_D = -1;
                    if (double.TryParse(MIN[i], out Num_D))
                    { ir.MIN = Num_D; }
                    else { ir.MIN = ConnectDB.NullD; }

                    Num_D = -1;
                    if (double.TryParse(MAX[i], out Num_D))
                    { ir.MAX = Num_D; }
                    else { ir.MAX = ConnectDB.NullD; }

                    ir.STR_VALUE = STR_VALUE[i];

                    DateTime DT_MIN;
                    if (DateTime.TryParse(DATE_VALUE[i], out DT_MIN))
                    { ir.DATE_VALUE = DT_MIN; }

                    Num = -1;
                    int.TryParse(INCLUDE[i], out Num);
                    if (Num == 1) { ir.INCLUDE = true; } else { ir.INCLUDE = false; }

                    DateTime DT_MAX;
                    if (DateTime.TryParse(DATE_VALUE_MAX[i], out DT_MAX))
                    { ir.DATE_VALUE_MAX = DT_MAX; }

                    IRP.Add(ir);
                }
            }
            catch (Exception)
            {
            }
            return IRP;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRoleUser(int ID)
        {
            ConnectDB conn = new ConnectDB();
            string NameRole = conn.GetRoleUserName(ID).TrimStart().TrimEnd();
            if (NameRole == "Pro_Govern") NameRole = RolesUsers.Provider_Government.ToString();
            else if (NameRole == "Pro_Health") NameRole = RolesUsers.Provider_Healthcare.ToString();
            else if (NameRole == "Gov_Health") NameRole = RolesUsers.Government_Healthcare.ToString();
            else if (NameRole == "") NameRole = RolesUsers.Guest.ToString();
            return NameRole;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID_USER"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool isAccessUser(string ID_USER, TypeStatus status)
        {
            bool isAccess = false;
            try {
                int USER_ID = -1;
                if (!string.IsNullOrEmpty(ID_USER)) {
                    USER_ID = int.Parse(ID_USER);
                    if (USER_ID > -1) {
                        string Role = GetRoleUser(USER_ID);
                        if (!string.IsNullOrEmpty(Role)) {
                            switch (status) {
                                case TypeStatus.CON:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Guest.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.CUR:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role)){
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.CUS:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role))  {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.DSL:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.EXP:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.HCC:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.ISV:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.LT1:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.LT2:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.PRI:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.PUB:
                                    if ((RolesUsers.Admin.ToString() == Role) || (RolesUsers.Provider.ToString() == Role) || (RolesUsers.Government.ToString() == Role) || (RolesUsers.HealthCare.ToString() == Role) || (RolesUsers.Guest.ToString() == Role) || (RolesUsers.Provider_Government.ToString() == Role) || (RolesUsers.Provider_Healthcare.ToString() == Role) || (RolesUsers.Government_Healthcare.ToString() == Role)) {
                                        isAccess = true;
                                    }
                                    break;
                            }
                        }
                    }
                    else {
                        string Role = RolesUsers.Guest.ToString();
                        if (!string.IsNullOrEmpty(Role)) {
                            switch (status) {
                                case TypeStatus.CON:
                                    if (RolesUsers.Guest.ToString() == Role){
                                        isAccess = true;
                                    }
                                    break;
                                case TypeStatus.PUB:
                                    if (RolesUsers.Guest.ToString() == Role)                                    {
                                        isAccess = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return isAccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FLD"></param>
        /// <param name="startIndex"></param>
        /// <param name="total"></param>
        /// <param name="GroupField"></param>
        /// <returns></returns>
        public static string GetOrderByforGrouping(List<string> FLD, int total, string GroupField)
        {
            int startIndex = 0;
            int cntr = 0;
            foreach (string v in FLD)
            {
                if (v == GroupField)
                {
                    startIndex = cntr;
                    break;
                }
                cntr++;
            }

            string Out = string.Format("[{0}] DESC", GroupField);
            if (total > 1)
            {
                Out = "";
                cntr = 0;
                for (int i = startIndex; i <= FLD.Count - 1; i++)
                {
                    if (cntr <= total)
                    {
                        Out += string.Format("[{0}] DESC,", FLD[i]);
                    }
                    cntr++;
                }
            }
            if (Out.Length > 0) Out = Out.Remove(Out.Length - 1, 1);
            return Out;
        }


        public QueryMetaD GetQueryMetaData(List<SettingIRPClass> IRP, int QueryID, int User_ID)
        {
            List<ColumnMetaD> colMeta = new List<ColumnMetaD>();
            QueryMetaD metaData = new QueryMetaD();
            bool isCorrectQuery = true;
            SettingIRPClass idf = IRP.Find(t => t.ID == QueryID);
            if (idf != null) {
                SettingIRPClass Its = IRP.Find(r => r.NAME == idf.NAME && r.STATUS_ == idf.STATUS_);
                if (Its != null) {
                    if (Its.IS_SQL_REQUEST == false) {
                        string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                        string gd_name = Guid.NewGuid().ToString();
                        gd_name = appFolder + @"\Temp\TmpIrp" + gd_name + ".IRP";
                        if (!System.IO.Directory.Exists(appFolder + @"\Temp"))
                            System.IO.Directory.CreateDirectory(appFolder + @"\Temp");

                        StreamWriter write = new StreamWriter(gd_name, false, Encoding.UTF8);
                        string cipherText = CryptorEngine.Decrypt(Its.Query, true);
                        write.Write(cipherText);
                        write.Close();
                        if (System.IO.File.Exists(gd_name)) {
                            List<OrmField> fld_orm = new List<OrmField>();
                            string str_value = "";
                            IcsmReport ics = new IcsmReport();
                            OrmRs orm = new OrmRs();
                            orm.Init(ConnectDB.Connect_);
                            orm.m_allFetched = true;
                            orm.AllFetched();
                            ics.m_records.LinkTo(orm);
                            try  {
                                ics.Load(gd_name);
                                isCorrectQuery = true;
                            }
                            catch (Exception) {
                                isCorrectQuery = false;
                            }
                            if (ics != null) {
                                //hg_.TABLE_NAME = ics.m_dat.m_tab;
                                metaData.Description = ics.m_desc;
                                metaData.Techno = ics.m_techno;
                                metaData.Title = ""; //???????????
                                metaData.Name = ics.m_desc;  //???????????
                                metaData.TableName = ics.m_dat.m_tab;
                                if (ics.m_dat.m_list.Count() > 0) {
                                    for (int i = 0; i < ics.m_dat.m_list[0].m_query.lq.Count(); i++) {
                                        if (!ics.m_dat.m_list[0].m_query.lq[i].m_isCustExpr) {
                                            string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                                            t = t.Replace(ics.m_dat.m_tab + ".", "");
                                            ColumnMetaD metaCol = new ColumnMetaD();
                                            metaCol.Description = t;
                                            metaCol.Format = ics.m_dat.m_list[0].m_query.lq[i].format;
                                            if (ics.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oNone) metaCol.Order = 0;
                                            else if (ics.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oAsc) metaCol.Order = 1;
                                            else if (ics.m_dat.m_list[0].m_query.lq[i].ord == Ordering.oDesc) metaCol.Order = 2;
                                            metaCol.Position = 0;
                                            metaCol.Rank = (uint)ics.m_dat.m_list[0].m_query.lq[i].ord_rank;
                                            metaCol.Show = (uint)ics.m_dat.m_list[0].m_query.lq[i].show;
                                            metaCol.Title = ics.m_dat.m_list[0].m_query.lq[i].title;
                                            metaCol.Type = (typeof(int));
                                            metaCol.Width = (uint)ics.m_dat.m_list[0].m_query.lq[i].colWidth;
                                            OrmField ty_p = ConnectDB.GetOrmDataDesc(t, ics.m_dat.m_tab);
                                            if (ty_p == null) {
                                                string FLD_STATE_FORMAT = ""; string FLD_STATE_value = "";
                                                if (t.Contains(".")){
                                                    FLD_STATE_value = t;
                                                    var count = t.Count(chr => chr == '.');
                                                    FLD_STATE_FORMAT = t.Replace(".", "(") + "".PadRight(count, ')');
                                                }
                                                if (FLD_STATE_FORMAT != "") {
                                                    ty_p = ConnectDB.GetFieldFromOrm(ics.m_dat.m_tab, FLD_STATE_value);
                                                }
                                            }
                                            metaCol.ormFld = ty_p;
                                            switch (ty_p.DDesc.ClassType) {
                                                case OrmVarType.var_Bytes:
                                                    metaCol.Type = typeof(byte);
                                                    break;
                                                case OrmVarType.var_Flo:
                                                    metaCol.Type = typeof(float);
                                                    break;
                                                case OrmVarType.var_Int:
                                                    metaCol.Type = typeof(int);
                                                    break;
                                                case OrmVarType.var_Dou:
                                                    metaCol.Type = typeof(double);
                                                    break;
                                                case OrmVarType.var_String:
                                                    metaCol.Type = typeof(string);
                                                    break;
                                                case OrmVarType.var_Tim:
                                                    metaCol.Type = typeof(DateTime);
                                                    break;
                                                default:
                                                    metaCol.Type = typeof(string);
                                                    break;
                                            }
                                            colMeta.Add(metaCol);
                                        }
                                    }
                                    metaData.Columns = colMeta.ToArray();
                                }
                            }
                        }
                        System.IO.File.Delete(gd_name);
                    }
                    else if (Its.IS_SQL_REQUEST == true) {
                        string Sql_ = CryptorEngine.Decrypt(Its.Query, true);
                        if (!string.IsNullOrEmpty(Sql_)) {
                            metaData.Description = Its.NAME;
                            metaData.Techno = "";
                            metaData.Title = "";
                            metaData.Name = Its.NAME;
                            string TB_Name = "";
                            Class_IRP_Object hg_ = new Class_IRP_Object();
                            List<string> FLD_X = new List<string>(); 
                            List<KeyValuePair<string, Type>> L_Field = new List<KeyValuePair<string, Type>>();
                            string AdditFilter = "";
                            if (!string.IsNullOrEmpty(Its.Ident_User)) {
                                if (Its.Ident_User.Length > 0) AdditFilter = string.Format(" AND ([{0}] = {1})", Its.Ident_User, User_ID);
                            }
                            hg_.Val_Arr = ConnectDB.ExecuteSQLCommand(ref FLD_X, Sql_, out TB_Name, Its.MAX_REC, AdditFilter, out L_Field, User_ID.ToString());
                            hg_.FLD = FLD_X;
                            hg_.TABLE_NAME = TB_Name;
                            hg_.CAPTION_FLD = FLD_X;
                            hg_.FLD_TYPE = new List<Type>(); foreach (string it in hg_.FLD)
                                foreach (KeyValuePair<string, Type> item in L_Field) {
                                    if (it == item.Key) {
                                        hg_.FLD_TYPE.Add(item.Value);
                                    }
                                }
                            hg_.Setting_param.isCorrectQuery = true;
                            metaData.Name = TB_Name;
                            foreach (string it in hg_.FLD) {
                                ColumnMetaD metaCol = new ColumnMetaD();
                                metaCol.Description = it;
                                metaCol.Format = "";
                                metaCol.Order = 0;
                                metaCol.Position = 0;
                                metaCol.Rank = 0;
                                metaCol.Show = 1;
                                metaCol.Title = it;
                                metaCol.Width = 150;
                                foreach (KeyValuePair<string, Type> item in L_Field) {
                                    if (it == item.Key) { metaCol.Type = item.Value; break; }
                                }
                                colMeta.Add(metaCol);
                            }
                            metaData.Columns = colMeta.ToArray();
                        }
                    }
                }
            }
           return metaData;
        }

        /// <summary>
        /// Здесь требуется переделка под классы DataConstraint!!!
        /// </summary>
        /// <param name="QueryParams"></param>
        /// <param name="metaD"></param>
        /// <returns></returns>
        public string GenerateQueryFromParams(Dictionary<string, object> QueryParams, QueryMetaD metaD)
        {
            //string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            string decimal_sep = ".";
            string SQL = "";
            string SQL_string = " AND ([{0}]='{1}') ";
            string SQL_digit = " AND ([{0}]={1}) ";
            if ((metaD != null) && (QueryParams!=null)) {
                foreach (KeyValuePair<string, object> Z in QueryParams) {
                    ColumnMetaD Cv = metaD.Columns.ToList().Find(t => t.Description == Z.Key);
                    if (Cv != null) {
                        if ((Cv.Type == typeof(int)) && (Z.Value != null)) SQL += string.Format(SQL_digit, Z.Key, Convert.ToInt32(Z.Value));
                        else if ((Cv.Type == typeof(double)) && (Z.Value != null)) SQL += string.Format(SQL_digit, Z.Key, Convert.ToDouble(Z.Value).ToString().Replace(".", decimal_sep).Replace(",", decimal_sep));
                        else if ((Cv.Type == typeof(float)) && (Z.Value != null)) SQL += string.Format(SQL_digit, Z.Key, Convert.ToDouble(Z.Value).ToString().Replace(".", decimal_sep).Replace(",", decimal_sep));
                        else if ((Cv.Type == typeof(bool)) && (Z.Value != null)) SQL += string.Format(SQL_string, Z.Key, Convert.ToBoolean(Z.Value));
                        else if ((Cv.Type == typeof(string)) && (Z.Value != null)) SQL += string.Format(SQL_string, Z.Key, Convert.ToString(Z.Value));
                        else if ((Cv.Type == typeof(DateTime)) && (Z.Value != null)) { DateTime DT = Convert.ToDateTime(Z.Value); SQL += string.Format(SQL_string, Z.Key, string.Format("{0}.{1}.{2}", DT.Day.ToString().PadLeft(2,'0'), DT.Month.ToString().PadLeft(2, '0'), DT.Year.ToString())); }
                    }
                }
            }
            return SQL;
        }

        public Class_IRP_Object ExecuteSQL(string filter, List<string> ID_USER, string SettingTableName, List<SettingIRPClass> IRP, int QueryID, string ActiveUser)
        {
            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            List<SettingIRPClass[]> LIr_Pages = new List<SettingIRPClass[]>();
            Class_IRP_Object hg_ = new Class_IRP_Object();
            try
            {
                List<WebDelegatedStations> LstDeleg = GetSettingWebDelegatedStations("XWEBDELEG_STAT_LST");
                List<WebConstraint> LstWebConstraint = GetSettingWebConstraint("XWEB_CONSTRAINT");
                List<XWeb_Del> LstWebDelStat = GetSettingWebDeltations("XWEB_DEL");
                List<WebAddParams> LstWebAppParams = GetSettingAddParams("XWEB_ADD_PARAMS");
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string gd_name = Guid.NewGuid().ToString();
                gd_name = appFolder + @"\Temp\TmpIrp" + gd_name + ".IRP";
                if (!System.IO.Directory.Exists(appFolder + @"\Temp"))
                    System.IO.Directory.CreateDirectory(appFolder + @"\Temp");
                try
                {
                    int Curr_UD_User = -1;
                    List<int> Ids_V = new List<int>();
                    foreach (string v in ID_USER)
                    {
                        int us_id = -1;
                        if (int.TryParse(v, out us_id))
                            Ids_V.Add(us_id);
                    }
                    SettingIRPClass idf = IRP.Find(t => t.ID == QueryID);
                    //foreach (SettingIRPClass idf in IRP)
                    if (idf !=null)
                    {
                        List<SettingIRPClass> Temp_Pages_Indexes = new List<SettingIRPClass>();
                        SettingIRPClass Its = IRP.Find(r => r.NAME == idf.NAME && r.STATUS_ == idf.STATUS_);
                        if (Its != null)
                        {
                            List<WebAddParams> LAppParams = LstWebAppParams.FindAll(r => r.WEB_QUERY_ID == idf.ID);
                            if (LAppParams != null)
                            {
                                hg_.SettingAddParams = LAppParams;
                            }
                            List<WebConstraint> Lconstraint = LstWebConstraint.FindAll(r => r.QUERY_ID == idf.ID);
                            if (Lconstraint != null)
                            {
                                hg_.SettingConstraint = Lconstraint;
                                if (Lconstraint.Count > 0)
                                {
                                    string AllConstraints = "(!) Apribojimai įvedamiems parametrams <br />";
                                    foreach (WebConstraint cntr in Lconstraint)
                                    {
                                        if ((cntr.MIN != ConnectDB.NullD) || (cntr.MAX != ConnectDB.NullD))
                                        {
                                            if (cntr.STR_VALUE != "")
                                                hg_.FormatConstraint += string.Format("Parametras: '{0}', Galimos vertės: {1} - {2} ", cntr.NAME != null ? cntr.NAME : "Tuščia", cntr.MIN, cntr.MAX) + "<br />";
                                            else
                                                hg_.FormatConstraint += string.Format("Parametras: '{0}', Negalimos vertės: {1} - {2} ", cntr.NAME != null ? cntr.NAME : "Tuščia", cntr.MIN, cntr.MAX) + "<br />";
                                        }
                                        else if (cntr.STR_VALUE != null)
                                        {
                                            if (cntr.STR_VALUE != "")
                                            {
                                                if (cntr.INCLUDE)
                                                    hg_.FormatConstraint += string.Format("Parametras: '{0}', Galimos vertės: {1} ", cntr.NAME != null ? cntr.NAME : "Tuščia", cntr.STR_VALUE) + "<br />";
                                                else
                                                    hg_.FormatConstraint += string.Format("Parametras: '{0}', Negalimos vertės: {1} ", cntr.NAME != null ? cntr.NAME : "Tuščia", cntr.STR_VALUE) + "<br />";
                                            }
                                        }
                                        if ((cntr.DATE_VALUE != ConnectDB.NullT) || (cntr.DATE_VALUE_MAX != ConnectDB.NullT))
                                        {
                                            if (cntr.INCLUDE)
                                                hg_.FormatConstraint += string.Format("Parametras: '{0}',  Galimos vertės: {1} - {2}", cntr.NAME != null ? cntr.NAME : "Tuščia", cntr.DATE_VALUE, cntr.DATE_VALUE_MAX) + "<br />";
                                            else
                                                hg_.FormatConstraint += string.Format("Parametras: '{0}',  Negalimos vertės: {1} - {2}", cntr.NAME != null ? cntr.NAME : "Tuščia", cntr.DATE_VALUE, cntr.DATE_VALUE_MAX) + "<br />";
                                        }
                                    }
                                    AllConstraints += hg_.FormatConstraint;
                                    hg_.FormatConstraint = AllConstraints;
                                }
                            }
                            hg_.Setting_param = Its;
                            if (Its.IS_VISIBLE == true)
                            {
                                string Ident_User_S = "";
                                string AdditFilter = "";
                                if (!string.IsNullOrEmpty(Its.Ident_User))
                                {
                                    if ((Its.STATUS_ == TypeStatus.DSL) || (Its.STATUS_ == TypeStatus.CUS) || (Its.STATUS_ == TypeStatus.LT1) || (Its.STATUS_ == TypeStatus.LT2) || (Its.STATUS_ == TypeStatus.PRI))
                                    {
                                        if (!Its.Ident_User.Contains("."))
                                        {
                                            if (((Its.STATUS_ == TypeStatus.CUS) || (Its.STATUS_ == TypeStatus.LT1) || (Its.STATUS_ == TypeStatus.LT2)) && (Its.IS_VISIBLE_ALL_REC == false))
                                            {
                                                if (ID_USER.Count > 0)
                                                {
                                                    AdditFilter += " AND (";
                                                    foreach (string rtz in ID_USER)
                                                    {
                                                        AdditFilter += string.Format(" ([{0}] = {1}) OR", Its.Ident_User, rtz);
                                                        Ident_User_S = rtz;
                                                    }
                                                    if (AdditFilter.EndsWith("OR")) AdditFilter = AdditFilter.Remove(AdditFilter.Length - 2, 2) + ")";
                                                }
                                            }
                                            if ((Its.STATUS_ == TypeStatus.DSL) || (Its.STATUS_ == TypeStatus.PRI))
                                            {
                                                if (ID_USER.Count > 0)
                                                {
                                                    AdditFilter += " AND (";
                                                    foreach (string rtz in ID_USER)
                                                    {
                                                        AdditFilter += string.Format(" ([{0}] = {1}) OR", Its.Ident_User, rtz);
                                                        Ident_User_S = rtz;
                                                    }
                                                    if (AdditFilter.EndsWith("OR")) AdditFilter = AdditFilter.Remove(AdditFilter.Length - 2, 2) + ")";
                                                }
                                            }
                                        }
                                        else {
                                            if (((Its.STATUS_ == TypeStatus.CUS) || (Its.STATUS_ == TypeStatus.LT1) || (Its.STATUS_ == TypeStatus.LT2)) && (Its.IS_VISIBLE_ALL_REC == false))
                                            {
                                                if (ID_USER.Count > 0)
                                                {
                                                    AdditFilter += " AND (";
                                                    foreach (string rtz in ID_USER)
                                                    {
                                                        AdditFilter += string.Format(" ([{0}] = {1}) OR", Its.Ident_User, rtz);
                                                        Ident_User_S = rtz;
                                                    }
                                                    if (AdditFilter.EndsWith("OR")) AdditFilter = AdditFilter.Remove(AdditFilter.Length - 2, 2) + ")";
                                                }
                                            }
                                            if ((Its.STATUS_ == TypeStatus.DSL) || (Its.STATUS_ == TypeStatus.PRI))
                                            {
                                                if (ID_USER.Count > 0)
                                                {
                                                    AdditFilter += " AND (";
                                                    foreach (string rtz in ID_USER)
                                                    {
                                                        AdditFilter += string.Format(" ([{0}] = {1}) OR", Its.Ident_User, rtz);
                                                        Ident_User_S = rtz;
                                                    }
                                                    if (AdditFilter.EndsWith("OR")) AdditFilter = AdditFilter.Remove(AdditFilter.Length - 2, 2) + ")";
                                                }
                                            }
                                        }
                                    }
                                }
                                else { }
                                hg_.StatusObject = Its.STATUS_;
                                StreamWriter write = new StreamWriter(gd_name, false, Encoding.UTF8);
                                string cipherText = CryptorEngine.Decrypt(Its.Query, true);
                                write.Write(cipherText);
                                write.Close();
                                if (System.IO.File.Exists(gd_name))
                                {
                                    List<string> str_fld = new List<string>();
                                    List<OrmField> fld_orm = new List<OrmField>();
                                    List<OrmItemExpr> fld_expr = new List<OrmItemExpr>();


                                    string str_value = "";
                                    IcsmReport ics = new IcsmReport();
                                    OrmRs orm = new OrmRs();

                                    orm.Init(ConnectDB.Connect_);
                                    orm.m_allFetched = true;
                                    orm.AllFetched();

                                    ics.m_records.LinkTo(orm);
                                    try
                                    {
                                        ics.Load(gd_name);
                                        hg_.Setting_param.isCorrectQuery = true;
                                    }
                                    catch (Exception)
                                    {
                                        hg_.Setting_param.isCorrectQuery = false;
                                    }

                                    if (ics != null)
                                    {
                                        hg_.FILTER = filter;
                                        hg_.TABLE_NAME = ics.m_dat.m_tab;


                                        // Для статуса DSL
                                        if (Its.STATUS_ == TypeStatus.DSL)
                                        {
                                            if (LstDeleg != null)
                                            {
                                                string str_val = "";
                                                List<WebDelegatedStations> LDeleg = LstDeleg.FindAll(ty => ty.TABLE_NAME == hg_.TABLE_NAME && ty.IS_ACTIVATE == 1 && Ids_V.Contains(ty.USER_ID) && ty.QUERY_ID == Its.ID);
                                                foreach (WebDelegatedStations itrc_ in LDeleg)
                                                {
                                                    str_val += string.Format("'{0}'", itrc_.RECORD_ID) + ",";
                                                }
                                                if (!string.IsNullOrEmpty(str_val))
                                                {
                                                    str_val = str_val.Remove(str_val.Length - 1, 1);
                                                    AdditFilter += " AND ([ID] in (" + str_val + "))";
                                                }
                                                if (LDeleg == null)
                                                {
                                                    AdditFilter += " AND ([ID] = -1) ";
                                                }
                                                if (LDeleg != null)
                                                {
                                                    if (LDeleg.Count == 0)
                                                    {
                                                        AdditFilter += " AND ([ID] = -1) ";
                                                    }
                                                }
                                            }
                                        }
                                        if (Its.STATUS_ == TypeStatus.CON)
                                        {

                                        }
                                        if (ics.m_dat.m_list.Count() > 0)
                                        {
                                            for (int i = 0; i < ics.m_dat.m_list[0].m_query.lq.Count(); i++)
                                            {
                                                //if 
                                                if (!ics.m_dat.m_list[0].m_query.lq[i].m_isCustExpr)
                                                {
                                                    string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                                                    t = t.Replace(ics.m_dat.m_tab + ".", "");

                                                    if (hg_.Setting_param.GROUP_FIELD == t)
                                                    {
                                                        hg_.Setting_param.index_group_field = i + 2;
                                                    }
                                                    hg_.FLD.Add(t);
                                                    hg_.CAPTION_FLD.Add(ics.m_dat.m_list[0].m_query.lq[i].title);
                                                    hg_.FORMAT_FLD.Add(ics.m_dat.m_list[0].m_query.lq[i].format);
                                                    str_fld.Add(t);
                                                    OrmField ty_p = ConnectDB.GetOrmDataDesc(t, hg_.TABLE_NAME);
                                                    //////////////////////////
                                                    if (ty_p == null)
                                                    {
                                                        string FLD_STATE_FORMAT = "";
                                                        string FLD_STATE_value = "";
                                                        {
                                                            if (t.Contains("."))
                                                            {
                                                                FLD_STATE_value = t;
                                                                var count = t.Count(chr => chr == '.');
                                                                FLD_STATE_FORMAT = t.Replace(".", "(") + "".PadRight(count, ')');
                                                            }
                                                        }
                                                        if (FLD_STATE_FORMAT != "")
                                                        {
                                                            ty_p = ConnectDB.GetFieldFromOrm(hg_.TABLE_NAME, FLD_STATE_value);
                                                        }
                                                    }
                                                    //////////////////////////

                                                    fld_orm.Add(ty_p);
                                                }
                                                else {
                                                    OrmItemExpr nw_ = new OrmItemExpr();
                                                    nw_.m_expression = ics.m_dat.m_list[0].m_query.lq[i].m_CustExpr;
                                                    nw_.m_name = ics.m_dat.m_list[0].m_query.lq[i].title;
                                                    nw_.m_sp = ics.m_dat.m_list[0].m_query.lq[i].m_typeCustExpr;
                                                    nw_.m_fmt = ics.m_dat.m_list[0].m_query.lq[i].format;
                                                    if (hg_.Setting_param.GROUP_FIELD == nw_.m_name)
                                                    {
                                                        hg_.Setting_param.index_group_field = i + 2;
                                                    }

                                                    str_fld.Add(nw_.m_name);
                                                    OrmField ty_p = ConnectDB.GetOrmDataDesc(nw_.m_name, hg_.TABLE_NAME);
                                                    //////////////////////////
                                                    if (ty_p == null)
                                                    {
                                                        string FLD_STATE_FORMAT = "";
                                                        string FLD_STATE_value = "";
                                                        {
                                                            if (nw_.m_name.Contains("."))
                                                            {
                                                                FLD_STATE_value = nw_.m_name;
                                                                var count = nw_.m_name.Count(chr => chr == '.');
                                                                FLD_STATE_FORMAT = nw_.m_name.Replace(".", "(") + "".PadRight(count, ')');
                                                            }
                                                        }
                                                        if (FLD_STATE_FORMAT != "")
                                                        {
                                                            ty_p = ConnectDB.GetFieldFromOrm(hg_.TABLE_NAME, FLD_STATE_value);
                                                        }
                                                    }
                                                    //////////////////////////
                                                    fld_orm.Add(ty_p);
                                                    hg_.FLD.Add(nw_.m_name);
                                                    hg_.CAPTION_FLD.Add(nw_.m_name);
                                                    hg_.FORMAT_FLD.Add(nw_.m_fmt);
                                                    fld_expr.Add(nw_);
                                                }
                                            }
                                            hg_.Fld_Orm = fld_orm;
                                        }
                                        if (!hg_.Setting_param.IS_SQL_REQUEST)
                                        {
                                            if (hg_.SettingConstraint != null)
                                            {
                                                if (hg_.SettingConstraint.Count > 0)
                                                {
                                                    string[] OneConstraint = new string[hg_.FLD.Count];
                                                    for (int ca = 0; ca < OneConstraint.Count(); ca++)
                                                    {
                                                        OneConstraint[ca] = "";
                                                    }
                                                    foreach (WebConstraint cntr in hg_.SettingConstraint)
                                                    {
                                                        if ((cntr.MIN != ConnectDB.NullD) || (cntr.MAX != ConnectDB.NullD))
                                                        {
                                                            string NameFldLon = "";
                                                            for (int jh = 0; jh < hg_.CAPTION_FLD.Count(); jh++)
                                                            {
                                                                if (hg_.FLD[jh] == cntr.PATH)
                                                                {
                                                                    NameFldLon = hg_.FLD[jh];
                                                                    if (!string.IsNullOrEmpty(NameFldLon))
                                                                    {
                                                                        if (OneConstraint[jh].Length == 0)
                                                                        {
                                                                            if (cntr.INCLUDE)
                                                                                OneConstraint[jh] += string.Format(" ([{0}] between {1} AND {2}) OR ([{0}] IS NULL)", NameFldLon, cntr.MIN.ToString().Replace(",", "."), cntr.MAX.ToString().Replace(",", "."));
                                                                            else
                                                                                OneConstraint[jh] += string.Format(" ([{0}] not between {1} AND {2}) OR ([{0}] IS NULL) ", NameFldLon, cntr.MIN.ToString().Replace(",", "."), cntr.MAX.ToString().Replace(",", "."));
                                                                        }
                                                                        else {
                                                                            if (cntr.INCLUDE)
                                                                                OneConstraint[jh] += string.Format(" AND ([{0}] between {1} AND {2}) OR ([{0}] IS NULL)", NameFldLon, cntr.MIN.ToString().Replace(",", "."), cntr.MAX.ToString().Replace(",", "."));
                                                                            else
                                                                                OneConstraint[jh] += string.Format(" AND ([{0}] not between {1} AND {2}) OR ([{0}] IS NULL)", NameFldLon, cntr.MIN.ToString().Replace(",", "."), cntr.MAX.ToString().Replace(",", "."));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (cntr.STR_VALUE != null)
                                                        {
                                                            if (cntr.STR_VALUE != "")
                                                            {
                                                                string NameFldLon = "";
                                                                for (int jh = 0; jh < hg_.CAPTION_FLD.Count(); jh++)
                                                                {
                                                                    if (hg_.FLD[jh] == cntr.PATH)
                                                                    {
                                                                        NameFldLon = hg_.FLD[jh];
                                                                        if (!string.IsNullOrEmpty(NameFldLon))
                                                                        {
                                                                            if ((cntr.STR_VALUE.EndsWith("*")) || (cntr.STR_VALUE.StartsWith("*")))
                                                                            {
                                                                                char[] chs = cntr.STR_VALUE.ToArray();
                                                                                if (cntr.STR_VALUE.EndsWith("*"))
                                                                                    chs[chs.Count() - 1] = '%';
                                                                                if (cntr.STR_VALUE.StartsWith("*"))
                                                                                    chs[0] = '%';
                                                                                cntr.STR_VALUE = new string(chs);
                                                                                if (OneConstraint[jh].Length == 0)
                                                                                {
                                                                                    if (cntr.INCLUDE)
                                                                                        OneConstraint[jh] += string.Format(" ([{0}] LIKE '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                    else
                                                                                        OneConstraint[jh] += string.Format(" ([{0}] NOT LIKE '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                }
                                                                                else {
                                                                                    if (cntr.INCLUDE)
                                                                                        OneConstraint[jh] += string.Format(" AND ([{0}] LIKE '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                    else
                                                                                        OneConstraint[jh] += string.Format(" AND ([{0}] NOT LIKE '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                }
                                                                            }
                                                                            else {
                                                                                if (OneConstraint[jh].Length == 0)
                                                                                {
                                                                                    if (cntr.INCLUDE)
                                                                                        OneConstraint[jh] += string.Format(" ([{0}] = '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                    else
                                                                                        OneConstraint[jh] += string.Format(" ([{0}] <> '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                }
                                                                                else {
                                                                                    if (cntr.INCLUDE)
                                                                                        OneConstraint[jh] += string.Format(" AND ([{0}] = '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                    else
                                                                                        OneConstraint[jh] += string.Format(" AND ([{0}] <> '{1}') ", NameFldLon, cntr.STR_VALUE);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if ((cntr.DATE_VALUE != ConnectDB.NullT) || (cntr.DATE_VALUE_MAX != ConnectDB.NullT))
                                                        {
                                                            string NameFldLon = "";
                                                            for (int jh = 0; jh < hg_.CAPTION_FLD.Count(); jh++)
                                                            {
                                                                if (hg_.FLD[jh] == cntr.PATH)
                                                                {
                                                                    NameFldLon = hg_.FLD[jh];
                                                                    if (!string.IsNullOrEmpty(NameFldLon))
                                                                    {
                                                                        if (OneConstraint[jh].Length == 0)
                                                                        {
                                                                            if (cntr.INCLUDE)
                                                                                OneConstraint[jh] += string.Format(" ([{0}] between '{1}' AND '{2}') OR ([{0}] IS NULL)", NameFldLon, cntr.DATE_VALUE, cntr.DATE_VALUE_MAX);
                                                                            else
                                                                                OneConstraint[jh] += string.Format(" ([{0}] not between '{1}' AND '{2}') OR ([{0}] IS NULL)", NameFldLon, cntr.DATE_VALUE, cntr.DATE_VALUE_MAX);
                                                                        }
                                                                        else {
                                                                            if (cntr.INCLUDE)
                                                                                OneConstraint[jh] += string.Format(" AND ([{0}] between '{1}' AND '{2}') OR ([{0}] IS NULL)", NameFldLon, cntr.DATE_VALUE, cntr.DATE_VALUE_MAX);
                                                                            else
                                                                                OneConstraint[jh] += string.Format(" AND ([{0}] not between '{1}' AND '{2}') OR ([{0}] IS NULL)", NameFldLon, cntr.DATE_VALUE, cntr.DATE_VALUE_MAX);
                                                                        }

                                                                    }
                                                                }
                                                            }
                                                        }
                                                        // }
                                                    }

                                                    for (int ca = 0; ca < OneConstraint.Count(); ca++)
                                                    {
                                                        if (OneConstraint[ca] != null)
                                                        {
                                                            if (OneConstraint[ca].Length > 0)
                                                                AdditFilter += "  AND " + "(" + OneConstraint[ca] + ")";
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if ((Its.STATUS_ == TypeStatus.HCC) || (Its.STATUS_ == TypeStatus.ISV))
                                        {
                                            string NameFldLon = "";
                                            for (int jh = 0; jh < hg_.CAPTION_FLD.Count(); jh++)
                                            {
                                                if (hg_.CAPTION_FLD[jh].Contains("Ilguma"))
                                                {
                                                    NameFldLon = hg_.FLD[jh];
                                                    AdditFilter = string.Format(" AND ([{0}] is NOT NULL)", NameFldLon);
                                                    break;
                                                }
                                            }
                                        }
                                        if (Its.STATUS_ == TypeStatus.ISV)
                                        {
                                            string NameFldLon = "";
                                            for (int jh = 0; jh < hg_.CAPTION_FLD.Count(); jh++)
                                            {
                                                if (hg_.CAPTION_FLD[jh].Contains("STATUS"))
                                                {
                                                    NameFldLon = hg_.FLD[jh];
                                                    AdditFilter = string.Format(" AND ([{0}] IN ('A','C'))", NameFldLon);
                                                    break;
                                                }
                                            }
                                        }

                                        if (str_fld != null)
                                        {
                                            string Role_ = "";
                                            if (Ids_V.Count > 0) Curr_UD_User = Ids_V[0];
                                            Role_ = v_s.GetRoleUser(Curr_UD_User);
                                            if ((hg_.Setting_param.IS_TESTING_REQUEST) && (RolesUsers.Admin.ToString() != Role_)) return null; //continue;
                                            if (hg_.Setting_param.isCorrectQuery && !hg_.Setting_param.IS_SQL_REQUEST)
                                            {
                                                if (str_fld != null)
                                                {
                                                    string FilterRange = ""; int VAL_MAX_R_INTERVAL = -1;
                                                    if (hg_.Setting_param.IS_ENABLE_INTERVAL)
                                                    {
                                                        if (hg_.Setting_param.MIN_VAL_INTERVAL < hg_.Setting_param.MAX_VAL_INTERVAL)
                                                        {
                                                            VAL_MAX_R_INTERVAL = hg_.Setting_param.MAX_VAL_INTERVAL - hg_.Setting_param.MIN_VAL_INTERVAL;
                                                            if (VAL_MAX_R_INTERVAL > 0)
                                                            {
                                                                FilterRange = string.Format(" AND (([ID]>={0}) AND ([ID]<={1})) ", hg_.Setting_param.MIN_VAL_INTERVAL, hg_.Setting_param.MAX_VAL_INTERVAL);
                                                                VAL_MAX_R_INTERVAL = VAL_MAX_R_INTERVAL + 1;
                                                            }
                                                        }
                                                    }
                                                    if (isAccessUser(Curr_UD_User.ToString(), hg_.StatusObject))
                                                    {
                                                        if ((hg_.StatusObject == TypeStatus.HCC) || (hg_.StatusObject == TypeStatus.ISV))
                                                        {
                                                            if (str_fld.Count() > 0)
                                                                if (hg_.Val_Arr.Count == 0)
                                                                {
                                                                    bool isReloadQuery = true;
                                                                    if (QueryID!=ConnectDB.NullI) { if (Its.ID != QueryID) { isReloadQuery = false; } } else { isReloadQuery = false; }
                                                                    if (isReloadQuery)
                                                                    {
                                                                        if (!hg_.Setting_param.IS_ENABLE_INTERVAL)
                                                                        {
                                                                            if (string.IsNullOrEmpty(hg_.Setting_param.GROUP_FIELD))
                                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + AdditFilter, "[ID] DESC", Its.MAX_REC, fld_expr, ConnectDB.Connect_, false);
                                                                            else
                                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + AdditFilter, GetOrderByforGrouping(hg_.FLD, hg_.Setting_param.TOTAL_COUNT_GROUP, hg_.Setting_param.GROUP_FIELD), Its.MAX_REC, fld_expr, ConnectDB.Connect_, false);
                                                                        }
                                                                        else if ((hg_.Setting_param.IS_ENABLE_INTERVAL) && (VAL_MAX_R_INTERVAL > 0))
                                                                        {
                                                                            if (string.IsNullOrEmpty(hg_.Setting_param.GROUP_FIELD))
                                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + FilterRange + AdditFilter, "[ID] DESC", VAL_MAX_R_INTERVAL, fld_expr, ConnectDB.Connect_, false);
                                                                            else
                                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + FilterRange + AdditFilter, GetOrderByforGrouping(hg_.FLD, hg_.Setting_param.TOTAL_COUNT_GROUP, hg_.Setting_param.GROUP_FIELD), VAL_MAX_R_INTERVAL, fld_expr, ConnectDB.Connect_, false);
                                                                        }
                                                                    }
                                                                }
                                                        }
                                                        else {
                                                            if (str_fld.Count() > 0)
                                                            {
                                                                bool isReloadQuery = true;
                                                                if (QueryID != ConnectDB.NullI) { if (Its.ID != QueryID) { isReloadQuery = false; } } else { isReloadQuery = false; }
                                                                if (isReloadQuery)
                                                                {
                                                                    if (!hg_.Setting_param.IS_ENABLE_INTERVAL)
                                                                    {
                                                                        if (string.IsNullOrEmpty(hg_.Setting_param.GROUP_FIELD))
                                                                            hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + AdditFilter, "[ID] DESC", Its.MAX_REC, fld_expr, ConnectDB.Connect_, true);
                                                                        else
                                                                            hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + AdditFilter, GetOrderByforGrouping(hg_.FLD, hg_.Setting_param.TOTAL_COUNT_GROUP, hg_.Setting_param.GROUP_FIELD), Its.MAX_REC, fld_expr, ConnectDB.Connect_, true);
                                                                    }
                                                                    else if ((hg_.Setting_param.IS_ENABLE_INTERVAL) && (VAL_MAX_R_INTERVAL > 0))
                                                                    {
                                                                        if (string.IsNullOrEmpty(hg_.Setting_param.GROUP_FIELD))
                                                                            hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + FilterRange + AdditFilter, "[ID] DESC", VAL_MAX_R_INTERVAL, fld_expr, ConnectDB.Connect_, true);
                                                                        else
                                                                            hg_.Val_Arr = ConnectDB.ICSMQuery(ics.m_dat.m_tab, str_fld.ToArray(), filter + FilterRange + AdditFilter, GetOrderByforGrouping(hg_.FLD, hg_.Setting_param.TOTAL_COUNT_GROUP, hg_.Setting_param.GROUP_FIELD), VAL_MAX_R_INTERVAL, fld_expr, ConnectDB.Connect_, true);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }


                                                    int idxKey = -1;
                                                    for (int i = 0; i < hg_.Val_Arr.Count(); i++)
                                                    {
                                                        for (int j = 0; j < hg_.Val_Arr[i].Count(); j++)
                                                        {
                                                            if (hg_.CAPTION_FLD[j].Contains("ID"))
                                                            {
                                                                idxKey = j;
                                                            }
                                                            else {
                                                                hg_.Val_Arr[i][j] = hg_.Val_Arr[i][j];
                                                            }
                                                        }
                                                    }
                                                    if (idxKey > -1)
                                                    {
                                                        if (hg_.Val_Arr != null) { if (hg_.Val_Arr.Count > 0) { if (hg_.Val_Arr[0].Count() > 0) { int Twy; if (int.TryParse(hg_.Val_Arr[0][idxKey].ToString(), out Twy)) { hg_.Setting_param.MaxID = Twy; } } } }
                                                        if (hg_.Val_Arr != null) { if (hg_.Val_Arr.Count > 0) { if (hg_.Val_Arr[hg_.Val_Arr.Count - 1].Count() > 0) { int Twy; if (int.TryParse(hg_.Val_Arr[hg_.Val_Arr.Count - 1][idxKey].ToString(), out Twy)) { hg_.Setting_param.MinID = Twy; } } } }
                                                        if (hg_.StatusObject != TypeStatus.HCC)
                                                        {
                                                            if (hg_.Val_Arr.Count > 0)
                                                            {
                                                                if (hg_.Val_Arr.Count > 0)
                                                                {
                                                                    for (int tu = 0; tu < hg_.Val_Arr.Count(); tu++)
                                                                    {
                                                                        int Min = 0;
                                                                        int Max = 0;
                                                                        if (hg_.Val_Arr[0].Count() > 0) { if (int.TryParse(hg_.Val_Arr[0][idxKey].ToString(), out Max)) { }  }
                                                                        if ((tu % hg_.Setting_param.MAX_REC_PAGE == 0) && (tu > 0))
                                                                        {
                                                                            if (tu > hg_.Setting_param.MAX_REC_PAGE) { if (hg_.Val_Arr[0].Count() > 0) { if (int.TryParse(hg_.Val_Arr[tu - hg_.Setting_param.MAX_REC_PAGE][idxKey].ToString(), out Max)) { } } }
                                                                            if (hg_.Val_Arr[tu].Count() > 0) { if (int.TryParse(hg_.Val_Arr[tu][idxKey].ToString(), out Min)) { } }
                                                                            SettingIRPClass tx_ = new SettingIRPClass();
                                                                            tx_.MaxID = Max;
                                                                            tx_.MinID = Min;
                                                                            Temp_Pages_Indexes.Add(tx_);
                                                                        }
                                                                        if (Temp_Pages_Indexes.Count > 0)
                                                                        {
                                                                            if ((hg_.Val_Arr.Count() - tu) < hg_.Setting_param.MAX_REC_PAGE)
                                                                            {
                                                                                if (hg_.Val_Arr[hg_.Val_Arr.Count - 1].Count() > 0) { if (int.TryParse(hg_.Val_Arr[hg_.Val_Arr.Count - 1][idxKey].ToString(), out Min)) { } }
                                                                                SettingIRPClass tx_ = new SettingIRPClass();
                                                                                tx_.MaxID = Temp_Pages_Indexes[Temp_Pages_Indexes.Count - 1].MinID;
                                                                                tx_.MinID = Min;
                                                                                Temp_Pages_Indexes.Add(tx_);
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    if (hg_.Val_Arr.Count() <= hg_.Setting_param.MAX_REC_PAGE)
                                                                    {
                                                                        SettingIRPClass tx_ = new SettingIRPClass();
                                                                        tx_.MaxID = hg_.Setting_param.MaxID;
                                                                        tx_.MinID = hg_.Setting_param.MinID;
                                                                        Temp_Pages_Indexes.Add(tx_);
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            else if (hg_.Setting_param.IS_SQL_REQUEST)
                                            {
                                                if (hg_.Val_Arr.Count == 0)
                                                {
                                                    bool isReloadQuery = true;
                                                    if (QueryID != ConnectDB.NullI) { if (Its.ID != QueryID) { isReloadQuery = false; } } else { isReloadQuery = false; }
                                                    if (isReloadQuery)
                                                    {

                                                        string Sql_ = CryptorEngine.Decrypt(hg_.Setting_param.Query, true);
                                                        if (!string.IsNullOrEmpty(Sql_))
                                                        {
                                                            string TB_Name = "";
                                                            List<string> FLD_X = new List<string>(); string TblName = "";
                                                            List<KeyValuePair<string, Type>> L_Field = new List<KeyValuePair<string, Type>>();
                                                            hg_.Val_Arr = ConnectDB.ExecuteSQLCommand(ref FLD_X, Sql_, out TB_Name, hg_.Setting_param.MAX_REC, AdditFilter, out L_Field, ActiveUser);
                                                            hg_.FLD = FLD_X;
                                                            hg_.TABLE_NAME = TB_Name;
                                                            hg_.CAPTION_FLD = FLD_X;
                                                            hg_.FLD_TYPE = new List<Type>(); foreach (string it in hg_.FLD)
                                                            {
                                                                foreach (KeyValuePair<string, Type> item in L_Field)
                                                                {
                                                                    if (it == item.Key)
                                                                    {
                                                                        hg_.FLD_TYPE.Add(item.Value);
                                                                    }
                                                                }
                                                            }
                                                            hg_.Setting_param.isCorrectQuery = true;
                                                            for (int ity_f = 0; ity_f < hg_.FLD.Count(); ity_f++)
                                                            {
                                                                if (hg_.Setting_param.GROUP_FIELD == hg_.FLD[ity_f])
                                                                {
                                                                    hg_.Setting_param.index_group_field = ity_f + 2;
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    hg_.PagesIndexRange = Temp_Pages_Indexes.ToArray();
                                    System.IO.File.Delete(gd_name);
                                }

                            }
                        }
                    }


                    //foreach (Class_IRP_Object lst_search in Cl_IRP)
                    if (hg_ != null)
                    {
                        Class_IRP_Object lst_search = hg_;
                        {
                            if (!lst_search.Setting_param.IS_SQL_REQUEST)
                            {
                                for (int iii = 0; iii < lst_search.FORMAT_FLD.Count; iii++)
                                {
                                    {
                                        Semant sm = null;
                                        if (lst_search.Fld_Orm[iii] != null)
                                        {
                                            sm = lst_search.Fld_Orm[iii].Special;
                                            if (sm != null)
                                            {
                                                for (int tu = 0; tu < lst_search.Val_Arr.Count(); tu++)
                                                {
                                                    double P = ConnectDB.NullD;
                                                    if (double.TryParse(lst_search.Val_Arr[tu][iii].ToString(), out P))
                                                    {
                                                        string Val_calc = "";
                                                        try
                                                        {
                                                            Val_calc = sm.Display(P, false, lst_search.FORMAT_FLD[iii]);
                                                            if (Val_calc != null)
                                                            {
                                                                if (double.TryParse(Val_calc.ToUpper(), NumberStyles.Any, CultureInfo.InvariantCulture, out P))
                                                                {
                                                                    lst_search.Val_Arr[tu][iii] = (object)P;
                                                                }
                                                            }
                                                        }
                                                        catch (Exception) { }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                  
                }
            }
            catch (Exception) {

            }
            return hg_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="LPtr"></param>
        /// <returns></returns>
        public bool isSuccessConstraintsCheck(List<WebConstraint> Lconstraint, List<RecordPtrDB> LPtr)
        {
            bool Ret = true;
            List<KeyValuePair<string, bool[]>> MassRet = new List<KeyValuePair<string, bool[]>>();
            for (int jh = 0; jh < LPtr.Count(); jh++)
            {
                foreach (WebConstraint constraint in Lconstraint)
                {
                    if (LPtr[jh].LinkField == constraint.PATH)
                    {
                        List<WebConstraint> fnd = Lconstraint.FindAll(r => r.PATH == LPtr[jh].LinkField);
                        if (fnd != null)
                        {
                            if (fnd.Count > 0)
                            {
                                if (MassRet.FindAll(r => r.Key == LPtr[jh].LinkField).Count == 0)
                                {
                                    bool[] C_Constraint = new bool[fnd.Count]; for (int h = 0; h < C_Constraint.Count(); h++) { C_Constraint[h] = true; }
                                    int index = 0;
                                    foreach (WebConstraint cbn in fnd)
                                    {
                                        if ((cbn.MIN != ConnectDB.NullD) || (cbn.MAX != ConnectDB.NullD))
                                        {
                                            if (LPtr[jh].Value != null)
                                            {
                                                double vl;
                                                if (double.TryParse(LPtr[jh].Value.ToString(), out vl))
                                                {
                                                    if (cbn.INCLUDE)
                                                    {
                                                        if (!((((vl >= cbn.MIN) && (vl <= cbn.MAX)))))
                                                        {
                                                            C_Constraint[index] = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((((vl >= cbn.MIN) && (vl <= cbn.MAX))))
                                                        {
                                                            C_Constraint[index] = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if ((cbn.DATE_VALUE != ConnectDB.NullT) || (cbn.DATE_VALUE_MAX != ConnectDB.NullT))
                                        {
                                            if (LPtr[jh].Value != null)
                                            {
                                                DateTime vl;
                                                if (DateTime.TryParse(LPtr[jh].Value.ToString(), out vl))
                                                {
                                                    if (cbn.INCLUDE)
                                                    {
                                                        if (!(((vl >= cbn.DATE_VALUE) && (vl <= cbn.DATE_VALUE_MAX))))
                                                        {
                                                            C_Constraint[index] = false;
                                                        }
                                                    }
                                                    else {
                                                        if (((((vl >= cbn.DATE_VALUE) && (vl <= cbn.DATE_VALUE_MAX)))))
                                                        {
                                                            C_Constraint[index] = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (!string.IsNullOrEmpty(LPtr[jh].Value.ToString()))
                                        {
                                            if (cbn.STR_VALUE != null)
                                            {
                                                if (((cbn.STR_VALUE.EndsWith("*")) || (cbn.STR_VALUE.StartsWith("*"))) || ((cbn.STR_VALUE.EndsWith("%")) || (cbn.STR_VALUE.StartsWith("%"))))
                                                {
                                                    if (LPtr[jh].Value != null)
                                                    {
                                                        if (cbn.INCLUDE)
                                                        {

                                                            if (!LPtr[jh].Value.ToString().Contains(cbn.STR_VALUE.Replace("%", "").Replace("*", "")))
                                                            {
                                                                C_Constraint[index] = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (LPtr[jh].Value.ToString().Contains(cbn.STR_VALUE.Replace("%", "").Replace("*", "")))
                                                            {
                                                                C_Constraint[index] = false;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (LPtr[jh].Value != null)
                                                    {
                                                        if (cbn.INCLUDE)
                                                        {
                                                            if (LPtr[jh].Value.ToString() != cbn.STR_VALUE)
                                                            {
                                                                C_Constraint[index] = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (LPtr[jh].Value.ToString() == cbn.STR_VALUE)
                                                            {
                                                                C_Constraint[index] = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        index++;
                                    }
                                    MassRet.Add(new KeyValuePair<string, bool[]>(LPtr[jh].LinkField, C_Constraint));
                                }
                            }
                        }
                    }
                }
            }


            foreach (KeyValuePair<string, bool[]> constraint_x in MassRet)
            {
                bool[] res_bool = constraint_x.Value;
                bool isCheck = true;
                foreach (bool x in res_bool)
                {
                    if (x == false)
                    {
                        isCheck = false;
                        break;
                    }
                }
                if (!isCheck) { Ret = false; break; }
            }
            return Ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isSuccessCheckLicence(List<KeyValuePair<string, object>> LPtr, Class_IRP_Object irp_obj)
        {
            bool Ret = false;
            ConnectDB conn = new ConnectDB();
            if (((irp_obj.Setting_param.MAX_VAL > 0) || (irp_obj.Setting_param.MAX_VAL != ConnectDB.NullI)) && (irp_obj.Setting_param.MAX_VAL != 0))
            {
                foreach (KeyValuePair<string, object> lx in LPtr)
                {
                    if (lx.Key == "ID")
                    {
                        double vl;
                        if (double.TryParse(lx.Value.ToString(), out vl))
                        {
                            ModeCompareLicence rtc = conn.CheckLicense((int)vl, irp_obj.TABLE_NAME, irp_obj.Setting_param.MAX_VAL);
                            if (rtc == ModeCompareLicence.Equally)
                            {
                                Ret = true;
                            }
                            break;
                        }
                    }
                }
            }
            else { Ret = true; }
            return Ret;
        }

        public bool isSuccessCheckLicence(List<RecordPtrDB> LPtr, Class_IRP_Object irp_obj)
        {
            bool Ret = false;
            ConnectDB conn = new ConnectDB();
            if (((irp_obj.Setting_param.MAX_VAL > 0) || (irp_obj.Setting_param.MAX_VAL != ConnectDB.NullI)) && (irp_obj.Setting_param.MAX_VAL != 0))
            {
                DateTime STOP_DATE = ConnectDB.NullT; DateTime FIRST_DATE = ConnectDB.NullT;
                foreach (RecordPtrDB lx in LPtr)
                {
                    List<RecordPtrDB> Lst = conn.GetLinkData(irp_obj.TABLE_NAME, lx.LinkField);
                    //if (lx.FieldCaptionTo == "STOP_DATE")
                    if ((Lst.Find(r => r.NameTableTo == "LICENCE" && r.NameFieldForSetValue == "STOP_DATE") != null) || (lx.FieldCaptionTo == "Leidimo pabaiga"))
                    {
                        DateTime vl;
                        if (DateTime.TryParse(lx.Value.ToString(), out vl))
                        {
                            STOP_DATE = vl;
                        }
                    }
                    //if (lx.FieldCaptionTo == "FIRST_DATE")
                    if ((Lst.Find(r => r.NameTableTo == "LICENCE" && r.NameFieldForSetValue == "FIRST_DATE") != null) || (lx.FieldCaptionTo == "Leidimo pradžia"))
                    {
                        DateTime vl;
                        if (DateTime.TryParse(lx.Value.ToString(), out vl))
                        {
                            FIRST_DATE = vl;
                        }
                    }
                }
                ModeCompareLicence rtc = conn.CheckLicense(FIRST_DATE, STOP_DATE, irp_obj.Setting_param.MAX_VAL);
                if (rtc == ModeCompareLicence.Equally)
                {
                    Ret = true;
                }

            }
            else { Ret = true; }
            return Ret;
        }

        public string GetImplicitUnit(string fmt, typSemant m_type)
        {
            if (fmt.IsNull()) fmt = "Auto";
            string sfmt, specif;
            int iS = fmt.IndexOf('/');
            if (iS > 0) { sfmt = fmt.Substring(0, iS); specif = fmt.Substring(iS + 1); }
            else { sfmt = fmt; specif = null; }
            string res = "";
            switch (m_type)
            {
                case typSemant.tdBm:
                case typSemant.tdBWatts:
                case typSemant.tWatts:
                    if (sfmt[0] != 'A') res = sfmt;
                    break;
                case typSemant.tFreq:
                    if (sfmt == "Formatted") res = "MHz";
                    else if (sfmt.ToLower().EndsWith("hz")) res = sfmt;
                    break;
                case typSemant.tBw:
                    if (sfmt == "Formatted") res = "kHz";
                    else if (sfmt.ToLower().EndsWith("hz")) res = sfmt;
                    break;
                case typSemant.tSecond:
                    if (sfmt == "Formatted") res = "s";
                    else if (sfmt.ToLower().EndsWith("s")) res = sfmt;
                    break;
                default:
                    break;
            }
            return res;
        }

        public static Semant InitFrom(Semant rf, string sfmt)
        {
            string[] hertzUnit = { "??", "µHz", "mHz", "Hz", "kHz", "MHz", "GHz" };
            //*this= *ref; int i;
            if (string.IsNullOrEmpty(sfmt) || sfmt == "Auto") return rf;
            Semant re = new Semant("noname", rf.m_type);
            switch (rf.m_type)
            {
                case typSemant.tWatts:
                case typSemant.tdBm:
                case typSemant.tdBWatts: //"FmtWatts")
                    re.m_sym = null;
                    switch (sfmt[0])
                    {
                        case 'd':
                            if (sfmt == "dBW") { re.m_type = typSemant.tdBWatts; re.m_div = 0.0; re.m_min = -100; re.m_max = 100; }//dBW: 
                            else if (sfmt == "dBm") { re.m_type = typSemant.tdBm; re.m_div = 0.0; re.m_min = -100; re.m_max = 100; }
                            break;
                        case 'µ': re.m_type = typSemant.tWatts; re.m_div = 1e-6; re.m_min = 0; re.m_max = 1e9; break; //µW
                        case 'm': re.m_type = typSemant.tWatts; re.m_div = 1e-3; re.m_min = 0; re.m_max = 1e9; break; //mW	E=milli Watts (no unit)
                        case 'W':
                            W: re.m_type = typSemant.tWatts; re.m_div = 1.0; re.m_min = 0; re.m_max = 1e9; break; //W	E=Watts (no unit)
                        case 'k': re.m_type = typSemant.tWatts; re.m_div = 1e3; re.m_min = 0; re.m_max = 1e9; break; //kW	E=kilo Watts (no unit)
                        case 'M': re.m_type = typSemant.tWatts; re.m_div = 1e6; re.m_min = 0; re.m_max = 1e9; break; //MW	E=Mega Watts (no unit)
                        case 'G': re.m_type = typSemant.tWatts; re.m_div = 1e9; re.m_min = 0; re.m_max = 1e9; break; //GW	E=Giga Watts (no unit)
                        case 'A':
                            if ("AutoW,AutoWE,AutoW1".Has(sfmt))
                            {
                                if (rf.m_type != typSemant.tWatts) goto W; //else *this= *ref;
                            }
                            else if (sfmt == "AutoD")
                            {
                                if (rf.m_type == typSemant.tWatts) { re.m_type = typSemant.tdBWatts; re.m_div = 0.0; re.m_min = -100; re.m_max = 100; }//goto dBW
                            }
                            break;
                        default: break;
                    }
                    break;
                case typSemant.tFreq:
                    re.m_sym = null;
                    re.m_type = typSemant.tFreq; re.m_min = 0; re.m_max = 1e12;
                    re.m_div = 1e6; //default MHz
                    for (int i = 0; i < hertzUnit.Length; i++)
                        if (sfmt == hertzUnit[i]) { re.m_div = Math.Pow(10.0, (i - 3) * 3); break; }
                    break;
                default: break;
            }
            return re;
        }



        public List<BlockDataFind> GetFieldFromFormFinderCreate(QueryMetaD obj, SettingIRPClass Ir_, bool isIncludeChecker, Dictionary<string, object> Params)
        {
            List<BlockDataFind> LstBlockFndVal = new List<BlockDataFind>();
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            try {
                if (Ir_ != null) {
                    int countColumnVisible = Ir_.HIDDEN_COLUMNS;
                    int start_hide_idx = obj.Columns.Count() - countColumnVisible + 1;
                    for (int i = 0; i < obj.Columns.Count(); i++) {
                        if ((obj.Columns[i].Description == "ID") || (obj.Columns[i].Description == Ir_.Ident_User))
                            continue;
                        if (isIncludeChecker) {
                            if (ConnectDB.CheckFieldPrimary(obj.Columns[i].Description, obj.TableName)) {
                                continue;
                            }
                        }
                        if (i >= start_hide_idx) continue;
                        BlockDataFind bl_f = new BlockDataFind();
                        bl_f.NameField = obj.Columns[i].Description;
                        bl_f.CaptionField = obj.Columns[i].Title;
                        bl_f.TableName = obj.TableName;
                        bl_f.type_ = obj.Columns[i].Type;
                        foreach (KeyValuePair<string, object> P_ in Params) {
                            if (P_.Key == bl_f.NameField) { bl_f.Value = P_.Value; break; }
                        }
                        LstBlockFndVal.Add(bl_f);
                    }
                }
            }
            catch (Exception)
            {

            }
            return LstBlockFndVal;
        }


        public List<BlockDataFind> GetFieldFromFormFinder(QueryMetaD obj, SettingIRPClass Ir_,  bool isIncludeChecker, Dictionary<string, object> Params)
        {
            List<BlockDataFind> LstBlockFndVal = new List<BlockDataFind>();
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            try
            {
                int indx_ID_Val = -1;
                for (int gx = 0; gx < obj.Columns.Count(); gx++){
                    if (obj.Columns[gx].Description == "ID") {
                        indx_ID_Val = gx;
                        break;
                    }
                }

                if (Ir_ != null) {
                    for (int i = 0; i < obj.Columns.Count(); i++) {
                        if (isIncludeChecker) {
                            if ((ConnectDB.CheckFieldPrimary(obj.Columns[i].Description, obj.TableName)) || (obj.Columns[i].Description == Ir_.Ident_User)){
                                continue;
                            }
                        }
                        {
                            BlockDataFind bl_f = new BlockDataFind();
                            bl_f.NameField = obj.Columns[i].Description;
                            bl_f.CaptionField = obj.Columns[i].Title;
                            bl_f.TableName = obj.TableName;
                            bl_f.type_ = obj.Columns[i].Type;
                            foreach (KeyValuePair<string, object> P_ in Params) {
                                if (P_.Key == bl_f.NameField) { bl_f.Value = P_.Value; break; }
                            }
                            LstBlockFndVal.Add(bl_f);

                        }
                    }
                }
            }
            catch (Exception)
            {
              
            }
            return LstBlockFndVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IRP"></param>
        //public int SaveToOrmDataEdit(List<Class_IRP_Object> irpm, List<BlockDataFind> Val_Mass, List<List<RecordPtrDB>> Val_Mass_Add, int ID, string TypeName, int USER_ID, out int Max_Val, string TypeSect)
        public int SaveToOrmDataEdit(QueryMetaD obj, SettingIRPClass ir__, List<BlockDataFind> Val_Mass, List<List<RecordPtrDB>> Val_Mass_Add, int ID, int USER_ID, out int Max_Val)
        {
            int Ret_Val = 0;
            Max_Val = ConnectDB.NullI;
            try {
                List<RecordPtrDB> LPtr = new List<RecordPtrDB>();
                List<RecordPtrDB> LPtr_Check_Licence = new List<RecordPtrDB>();
                if (ir__ != null) {
                    Max_Val = ir__.MAX_VAL;
                    List<string> Active_List_FLD = new List<string>();
                    ConnectDB conn = new ConnectDB();
                    int idx_column = 0;
                    foreach (ColumnMetaD item in obj.Columns) {
                        if (ConnectDB.CheckFieldPrimary(item.Description, obj.TableName)) {
                            RecordPtrDB JK = new RecordPtrDB(); JK.FieldCaptionTo = "ID"; JK.LinkField = "ID"; JK.Value = ID; LPtr_Check_Licence.Add(JK);
                        }
                        else if (!ConnectDB.CheckFieldPrimary(item.Description, obj.TableName)) {
                            Active_List_FLD.Add(item.Description);
                            BlockDataFind BF = Val_Mass.Find(r => r.NameField == item.Description);
                            if (BF != null) {
                                int CaptionField_int = idx_column; string CaptionFld = "";
                                if (CaptionField_int != -1) {
                                    CaptionFld = obj.Columns[CaptionField_int].Title;
                                    OrmField ormF = obj.Columns[CaptionField_int].ormFld;
                                    string Form_ = obj.Columns[CaptionField_int].Format;
                                    Semant sm = null;
                                    if (ormF != null) {
                                        sm = ormF.Special;
                                        if (sm != null) {
                                            double P = ConnectDB.NullD;
                                            if (double.TryParse(BF.Value.ToString(), out P)) {
                                                string Val_calc = "";
                                                try
                                                {
                                                    // for freq
                                                    if (sm.m_type == typSemant.tBw) {
                                                        if (sm.mName.ToLower().EndsWith("hz")) {
                                                            int iS = sm.mName.IndexOf('/');
                                                            if (iS > 0) {
                                                                string sub = sm.mName.Substring(iS + 1);
                                                                Val_calc = Semant.Get("F/" + Form_).Display(P, false, sub);
                                                            }
                                                        }
                                                    }
                                                    else if (sm.m_type == typSemant.tFreq)  {
                                                        if (sm.mName.StartsWith("F/")) {
                                                            string To = sm.mName.Substring(2, sm.mName.Length - 2);
                                                            Val_calc = Semant.Get("F/" + Form_).Display(P, false, To);
                                                        }
                                                    }
                                                    else {
                                                        Semant rd = InitFrom(sm, Form_);
                                                        Val_calc = rd.Display(P, false, "dBm");
                                                    }
                                                    if (Val_calc != null) {
                                                        if (double.TryParse(Val_calc.ToUpper(), NumberStyles.Any, CultureInfo.InvariantCulture, out P))
                                                        {
                                                            BF.Value = (object)P;
                                                        }
                                                    }
                                                }
                                                catch (Exception) { }
                                            }

                                        }
                                    }
                                }
                                RecordPtrDB JK = new RecordPtrDB(); JK.FieldCaptionTo = CaptionFld; JK.LinkField = BF.NameField; JK.Value = BF.Value; LPtr.Add(JK);
                            }
                        }
                        idx_column++;
                    }

                    //LPtr_Check_Licence.AddRange(LPtr);
                    //if ((ir__.SettingConstraint != null) && (LPtr != null))
                    //{
                        //if ((isSuccessConstraintsCheck(ir__.SettingConstraint, LPtr) == false) || (isSuccessCheckLicence(LPtr_Check_Licence, ir__) == false))
                            //return ConnectDB.NullI;
                    //}

                    List<object> L_in = new List<object>();

                    foreach (BlockDataFind ity in Val_Mass)
                    {
                        L_in.Add(ity.Value);
                    }

                    // Специальная функция преобразования наименований полей, например Position.NAME в пары  (имя поля, имя таблицы)
                    //ConnectDB.GetFieldFromOrm(ir__.TABLE_NAME, Active_List_FLD.ToArray());

                    Ret_Val = conn.UpdateBlockData(ID, USER_ID, obj.TableName, LPtr);
                    //conn.TestEdit(ID, ir__.TABLE_NAME, Active_List_FLD.ToArray(), L_in.ToArray(), Val_Mass_Add, ConnectDB.Connect_, USER_ID);
                }

            }
            catch (Exception)
            {

            }
            return Ret_Val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="irpm"></param>
        /// <param name="Val_Mass"></param>
        /// <param name="ID"></param>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public string[] GetTablesEmptyLink_(List<Class_IRP_Object> irpm, int ID, string TypeName)
        {
            string[] Res = null;
            try {
                Class_IRP_Object ir__ = irpm.Find(r => r.Setting_param.NAME == TypeName);
                if (ir__ != null) {
                    List<string> Active_List_FLD = new List<string>();
                    ConnectDB conn = new ConnectDB();
                    foreach (string item in ir__.FLD) {
                        //if (!ConnectDB.CheckFieldNotNull(item, ir__.TABLE_NAME))
                        if (!ConnectDB.CheckFieldPrimary(item, ir__.TABLE_NAME)) {
                            Active_List_FLD.Add(item);
                        }
                    }
                    Res = conn.GetTablesEmptyLink(ID, ir__.TABLE_NAME, Active_List_FLD.ToArray(), ConnectDB.Connect_);
                }
            }
            catch (Exception)
            {

            }

            return Res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="irpm"></param>
        /// <param name="Val_Mass"></param>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public string[] GetTablesEmptyLink_(List<Class_IRP_Object> irpm, string TypeName)
        {
            string[] Res = null;
            try {
                Class_IRP_Object ir__ = irpm.Find(r => r.Setting_param.NAME == TypeName);
                if (ir__ != null) {
                    List<string> Active_List_FLD = new List<string>();
                    ConnectDB conn = new ConnectDB();
                    foreach (string item in ir__.FLD) {
                        //if (!ConnectDB.CheckFieldNotNull(item, ir__.TABLE_NAME))
                        if (!ConnectDB.CheckFieldPrimary(item, ir__.TABLE_NAME)) {
                            Active_List_FLD.Add(item);
                        }
                    }
                    Res = conn.GetTablesEmptyLink(ir__.TABLE_NAME, Active_List_FLD.ToArray(), ConnectDB.Connect_);
                }
            }
            catch (Exception)
            {
            }
            return Res;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public enum TypeRadioButton
    {
        Less,
        More,
        Equally,
        Unknown
    }

    public class BlockDataFind
    {
        public object Value { get; set; }
        public Type type_ { get; set; }
        public TypeRadioButton typeRadio_ { get; set; }
        public string NameField { get; set; }
        public string CaptionField { get; set; }
        public string TableName { get; set; }

        public BlockDataFind()
        {
            Value = new object();
            typeRadio_ = TypeRadioButton.Unknown;
        }
    }


}