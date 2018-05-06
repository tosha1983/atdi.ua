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

        /// <summary>
        /// Извлечение детальных характеристик запроса
        /// </summary>
        /// <param name="IRP"></param>
        /// <param name="QueryID"></param>
        /// <param name="User_ID"></param>
        /// <returns></returns>
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
                        string cipherText = CryptorEngine.Decrypt(Its.Query, true);
                        {
                            List<OrmField> fld_orm = new List<OrmField>();
                            IcsmReport ics = new IcsmReport();
                            OrmRs orm = new OrmRs();
                            orm.Init(ConnectDB.Connect_);
                            orm.m_allFetched = true;
                            orm.AllFetched();
                            ics.m_records.LinkTo(orm);
                            try  {
                                Frame f = new Frame();
                                int x1 = cipherText.IndexOf("\r\n");
                                cipherText = cipherText.Remove(0, x1+2);
                                int x2 = cipherText.IndexOf("\r\n");
                                cipherText = cipherText.Remove(0, x2+2);
                                InChannelString strx = new InChannelString(cipherText);
                                f.Load(strx);
                                ics.SetConfig(f);
                                isCorrectQuery = true;
                            }
                            catch (Exception ex) {
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
                                            metaCol.isCustExpr = false;
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
                                            //metaCol.settIRP = Its;
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
                                        else
                                        {
                                            string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                                            t = t.Replace(ics.m_dat.m_tab + ".", "");
                                            ColumnMetaD metaCol = new ColumnMetaD();
                                            metaCol.isCustExpr = true;
                                            //metaCol.Description = t;
                                            metaCol.Format = ics.m_dat.m_list[0].m_query.lq[i].format;
                                            OrmItemExpr nw_ = new OrmItemExpr();
                                            nw_.m_expression = ics.m_dat.m_list[0].m_query.lq[i].m_CustExpr;
                                            nw_.m_name = ics.m_dat.m_list[0].m_query.lq[i].title;
                                            metaCol.Description = nw_.m_name;
                                            nw_.m_sp = ics.m_dat.m_list[0].m_query.lq[i].m_typeCustExpr;
                                            nw_.m_fmt = ics.m_dat.m_list[0].m_query.lq[i].format;
                                            OrmField ty_p = ConnectDB.GetOrmDataDesc(nw_.m_name, ics.m_dat.m_tab);
                                            if (ty_p == null) {
                                                string FLD_STATE_FORMAT = ""; string FLD_STATE_value = "";
                                                if (t.Contains(".")) {
                                                    FLD_STATE_value = t;
                                                    var count = t.Count(chr => chr == '.');
                                                    FLD_STATE_FORMAT = t.Replace(".", "(") + "".PadRight(count, ')');
                                                }
                                                if (FLD_STATE_FORMAT != "") {
                                                    ty_p = ConnectDB.GetFieldFromOrm(ics.m_dat.m_tab, FLD_STATE_value);
                                                }
                                            }
                                            metaCol.ormFld = ty_p;
                                            metaCol.ormItemExpr = nw_;
                                            colMeta.Add(metaCol);
                                        }

                                    }
                                    metaData.Columns = colMeta.ToArray();
                                }
                            }
                        }
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
                    metaData.settIRP = Its;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ID_USER"></param>
        /// <param name="IRP"></param>
        /// <param name="QueryID"></param>
        /// <param name="QD"></param>
        /// <returns></returns>
        public Class_IRP_Object ExecuteSQL(string filter, int ID_USER, QueryMetaD QD)
        {
            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            List<SettingIRPClass[]> LIr_Pages = new List<SettingIRPClass[]>();
            Class_IRP_Object hg_ = new Class_IRP_Object();
            try
            {
                //ID текущего пользователя
                int Curr_UD_User = ID_USER;
                //Получаем аспомогательные сведения о текущем запросе
                SettingIRPClass Its = QD.settIRP;
                    if (Its != null)
                    {
                        hg_.Setting_param = Its;
                        // если запрос разрешен на выполнение
                        if (Its.IS_VISIBLE == true)
                        {
                        //формирование фильтра  для случая, когда необходимо фильтровать записи, относящиеся к заданному владельцу Ident_User (например, когда необходимо выдать только станции, которые создал текущий user)
                        string AdditFilter = "";
                            if (!string.IsNullOrEmpty(Its.Ident_User))
                            {
                                if (Curr_UD_User != ConnectDB.NullI)
                                {
                                    AdditFilter += " AND (";
                                    AdditFilter += string.Format(" ([{0}] = {1}) OR", Its.Ident_User, Curr_UD_User);
                                    if (AdditFilter.EndsWith("OR")) AdditFilter = AdditFilter.Remove(AdditFilter.Length - 2, 2) + ")";
                                }
                            }
                            hg_.StatusObject = Its.STATUS_;
                            {
                                // получаем списки всех полей заданного запроса 
                                List<string> str_fld = new List<string>();
                                List<OrmField> fld_orm = new List<OrmField>();
                                List<OrmItemExpr> fld_expr = new List<OrmItemExpr>();
                                {
                                    hg_.FILTER = filter;
                                    hg_.TABLE_NAME = QD.TableName; // здесь храниться имя основной таблицы запроса
                                    if (QD.Columns.Count() > 0) {
                                        for (int i = 0; i < QD.Columns.Count(); i++) {
                                            // если поле не Custom Expression
                                            if (!QD.Columns[i].isCustExpr) {
                                                str_fld.Add(QD.Columns[i].Description);
                                                fld_orm.Add(QD.Columns[i].ormFld);
                                                hg_.FLD.Add(QD.Columns[i].Description);
                                            }
                                        // если поле Custom Expression
                                        else {
                                                str_fld.Add(QD.Columns[i].Description);
                                                fld_expr.Add(QD.Columns[i].ormItemExpr);
                                                hg_.FLD.Add(QD.Columns[i].Description);
                                        }
                                        }
                                        hg_.Fld_Orm = fld_orm;
                                    }
                                    if (str_fld != null)
                                    {
                                    // если запрос с пометкой IS_TESTING_REQUEST - не выполняем
                                    if (hg_.Setting_param.IS_TESTING_REQUEST) return null; //continue;
                                    // если запрос для случая IRP - данних
                                    if (!hg_.Setting_param.IS_SQL_REQUEST)
                                        {
                                            if (str_fld != null)
                                            {
                                                string FilterRange = ""; int VAL_MAX_R_INTERVAL = -1;
                                                // если включена опция - по интервалу (от минимального ID до максимального ID)
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

                                                if (str_fld.Count() > 0)
                                                {
                                                // если  опция - по интервалу выключена (в данном случае используется параметр Its.MAX_REC - который определяет максимально допустимое число строк результирующей выборки)
                                                if (!hg_.Setting_param.IS_ENABLE_INTERVAL)
                                                        {
                                                            // если опция группировки выключена
                                                            if (string.IsNullOrEmpty(hg_.Setting_param.GROUP_FIELD))
                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(QD.TableName, str_fld.ToArray(), filter + AdditFilter, "[ID] DESC", Its.MAX_REC, fld_expr, ConnectDB.Connect_, true);
                                                            else
                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(QD.TableName, str_fld.ToArray(), filter + AdditFilter, GetOrderByforGrouping(hg_.FLD, hg_.Setting_param.TOTAL_COUNT_GROUP, hg_.Setting_param.GROUP_FIELD), Its.MAX_REC, fld_expr, ConnectDB.Connect_, true);
                                                        }
                                                        else if ((hg_.Setting_param.IS_ENABLE_INTERVAL) && (VAL_MAX_R_INTERVAL > 0))
                                                        {
                                                            // если опция группировки выключена
                                                            if (string.IsNullOrEmpty(hg_.Setting_param.GROUP_FIELD))
                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(QD.TableName, str_fld.ToArray(), filter + FilterRange + AdditFilter, "[ID] DESC", VAL_MAX_R_INTERVAL, fld_expr, ConnectDB.Connect_, true);
                                                            else
                                                                hg_.Val_Arr = ConnectDB.ICSMQuery(QD.TableName, str_fld.ToArray(), filter + FilterRange + AdditFilter, GetOrderByforGrouping(hg_.FLD, hg_.Setting_param.TOTAL_COUNT_GROUP, hg_.Setting_param.GROUP_FIELD), VAL_MAX_R_INTERVAL, fld_expr, ConnectDB.Connect_, true);
                                                        }
                                                }
                                            }
                                        }
                                        // если запрос для случая SQL - кода
                                        else if (hg_.Setting_param.IS_SQL_REQUEST)
                                        {
                                            if (hg_.Val_Arr.Count == 0)
                                            {
                                                    string Sql_ = CryptorEngine.Decrypt(hg_.Setting_param.Query, true);
                                                    if (!string.IsNullOrEmpty(Sql_))
                                                    {
                                                        string TB_Name = "";
                                                        List<string> FLD_X = new List<string>(); string TblName = "";
                                                        List<KeyValuePair<string, Type>> L_Field = new List<KeyValuePair<string, Type>>();
                                                        //вызов команды, выполняющей SQL - запрос 
                                                        hg_.Val_Arr = ConnectDB.ExecuteSQLCommand(ref FLD_X, Sql_, out TB_Name, hg_.Setting_param.MAX_REC, AdditFilter, out L_Field, Its.ID.ToString());
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
                                                    }
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                
            }
            catch (Exception ex)
            {

            }
            return hg_;
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
              
            }
            return LstBlockFndVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IRP"></param>
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
                                }
                                RecordPtrDB JK = new RecordPtrDB(); JK.FieldCaptionTo = CaptionFld; JK.LinkField = BF.NameField; JK.Value = BF.Value; LPtr.Add(JK);
                            }
                        }
                        idx_column++;
                    }
                    List<object> L_in = new List<object>();
                    foreach (BlockDataFind ity in Val_Mass)
                    {
                        L_in.Add(ity.Value);
                    }
                    // Специальная функция преобразования наименований полей, например Position.NAME в пары  (имя поля, имя таблицы)
                    Ret_Val = conn.UpdateBlockData(ID, USER_ID, obj.TableName, LPtr);
                }

            }
            catch (Exception ex)
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