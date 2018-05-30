using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OrmCs;
using FormsCs;
using System.Text;
using DatalayerCs;
using System.Collections;
using System.Data;



namespace XICSM.WebQuery
{
    public class ClassORM
    {
        public static OrmField GetOrmDataDesc(string fld_check, string tableName) {
            OrmField rc = null;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null) {
                foreach (OrmField f1 in zeta.ClassFields) {
                    switch (f1.Nature) {
                        case OrmFieldNature.Column: {
                                OrmFieldF fjF = (OrmFieldF)f1;
                                if (fld_check == f1.Name) {
                                    if (fjF != null){
                                        rc = fjF;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return rc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Fld"></param>
        /// <param name="sql_"></param>
        /// <returns></returns>
        public static List<object[]> ExecuteSQLCommand(ref List<string> Fld, string sql_)
        {
            string sql; ANetDb d;
            List<object[]> RecordValues = new List<object[]>();
            List<object> RecordsVal = null;
            List<string> fld_f = new List<string>();
            List<KeyValuePair<string, string>> L_Field = new List<KeyValuePair<string, string>>();
            try {
                if (!string.IsNullOrEmpty(sql_)) {
                    if (OrmSchema.Linker != null) {
                        ICSM.IMDbConnection conn = ICSM.IM.GetDbConnection("LICENCE");
                        d = ANetDb.New("System.Data.SqlClient"); d.ConnectionString = string.Format(@"SERVER={0};UID={1};PWD={2};DATABASE={3};", conn.SERVER, conn.UID, conn.PWD, conn.DATABASE);
                        ANetRs rs = d.NewRecordset(); rs.Open(sql_); DataTable dft = rs.GetSchemaTable();
                        foreach (DataRow row in dft.Rows) {
                            var name = row["ColumnName"];
                            var size = row["ColumnSize"];
                            var dataType = row["DataTypeName"];
                            if (!L_Field.Contains(new KeyValuePair<string, string>(name.ToString(), dataType.ToString())))
                                L_Field.Add(new KeyValuePair<string, string>(name.ToString(), dataType.ToString()));
                        }
                        foreach (DataRow c in dft.Rows)  {
                            fld_f.Add(c[0].ToString());
                        }

                        if (fld_f.Count > 0) {
                            Fld = fld_f;
                            for (; !rs.IsEOF(); rs.MoveNext()){
                                RecordsVal = new List<object>();
                                int i = 0;
                                foreach (KeyValuePair<string, string> item in L_Field) {
                                    object value = -1;
                                    if (item.Value == "decimal") {
                                        value = rs.GetDouble(i);
                                    }
                                    else if (item.Value == "nvarchar") {
                                        value = rs.GetString(i);
                                    }
                                    else if (item.Value == "datetime") {
                                        value = rs.GetDatetime(i);
                                    }
                                    else  {
                                        value = rs.GetString(i);
                                    }
                                    RecordsVal.Add(value);
                                    i++;
                                }
                                RecordValues.Add(RecordsVal.ToArray());
                            }

                        }
                        rs.Destroy();
                    }
                }

            }
            catch (Exception ex) {
            }
            return RecordValues;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public static List<string> GetProperties(string Query, bool isDecrypt, bool isSQLMode)
        {
            List<string> str_fld = new List<string>();
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            if (!System.IO.Directory.Exists(appFolder + @"\Temp"))
                System.IO.Directory.CreateDirectory(appFolder + @"\Temp");
            string gd_name = Guid.NewGuid().ToString();
            gd_name = appFolder + @"\Temp\TmpIrp" + gd_name + ".IRP";
            StreamWriter write = new StreamWriter(gd_name, false, Encoding.UTF8);
            if (isDecrypt) {
                string cipherText = CryptorEngine.Decrypt(Query, true);
                if (isSQLMode)
                    ExecuteSQLCommand(ref str_fld, cipherText);
                else
                    write.Write(cipherText);
            }
            else { write.Write(Query);
                if (isSQLMode)
                    ExecuteSQLCommand(ref str_fld, Query);
                else write.Write(Query);
            }

            write.Close();
            Class_IRP_Object hg_ = new Class_IRP_Object();
            if (!isSQLMode) {
                if (System.IO.File.Exists(gd_name)) {
                    string str_value = "";
                    IcsmReport ics = new IcsmReport();
                    OrmRs orm = new OrmRs();
                    orm.Init(OrmSchema.Linker);
                    orm.m_allFetched = true;
                    orm.AllFetched();
                    ics.m_records.LinkTo(orm);
                    try{
                        ics.Load(gd_name);
                    }
                    catch (Exception) { }
                    if (ics != null) {
                        hg_.TABLE_NAME = ics.m_dat.m_tab;
                        if (ics.m_dat.m_list.Count() > 0){
                            for (int i = 0; i < ics.m_dat.m_list[0].m_query.lq.Count(); i++) {
                                if (!ics.m_dat.m_list[0].m_query.lq[i].m_isCustExpr) {
                                    string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                                    t = t.Replace(ics.m_dat.m_tab + ".", "");
                                    if (hg_.Setting_param.GROUP_FIELD == t) {
                                        hg_.Setting_param.index_group_field = i + 1;
                                    }
                                    hg_.FLD.Add(t);
                                    hg_.CAPTION_FLD.Add(ics.m_dat.m_list[0].m_query.lq[i].title);
                                    str_fld.Add(t);
                                }
                                else {
                                    OrmItemExpr nw_ = new OrmItemExpr();
                                    nw_.m_expression = ics.m_dat.m_list[0].m_query.lq[i].m_CustExpr;
                                    nw_.m_name = ics.m_dat.m_list[0].m_query.lq[i].title;
                                    nw_.m_sp = ics.m_dat.m_list[0].m_query.lq[i].m_typeCustExpr;
                                    if (hg_.Setting_param.GROUP_FIELD == nw_.m_name) {
                                        hg_.Setting_param.index_group_field = i + 1;
                                    }
                                    str_fld.Add(nw_.m_name);
                                    hg_.FLD.Add(nw_.m_name);
                                    hg_.CAPTION_FLD.Add(nw_.m_name);
                                }
                            }
                        }
                    }
                }
            }
            return str_fld;
        }

        public static Class_IRP_Object GetPropertiesAll(string Query, bool isDecrypt, bool isSQLMode)
        {
            List<string> str_fld = new List<string>();
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            if (!System.IO.Directory.Exists(appFolder + @"\Temp"))
                System.IO.Directory.CreateDirectory(appFolder + @"\Temp");
            string gd_name = Guid.NewGuid().ToString();
            gd_name = appFolder + @"\Temp\TmpIrp" + gd_name + ".IRP";
            StreamWriter write = new StreamWriter(gd_name, false, Encoding.UTF8);
            if (isDecrypt) {
                string cipherText = CryptorEngine.Decrypt(Query, true);
                if (isSQLMode)
                    ExecuteSQLCommand(ref str_fld, cipherText);
                else
                    write.Write(cipherText);
            }
            else { write.Write(Query);
                if (isSQLMode)
                    ExecuteSQLCommand(ref str_fld, Query);
                else write.Write(Query);
            }

            write.Close();
            Class_IRP_Object hg_ = new Class_IRP_Object();
            if (!isSQLMode) {
                if (System.IO.File.Exists(gd_name)) {
                    string str_value = "";
                    IcsmReport ics = new IcsmReport();
                    OrmRs orm = new OrmRs();
                    orm.Init(OrmSchema.Linker);
                    orm.m_allFetched = true;
                    orm.AllFetched();
                    ics.m_records.LinkTo(orm);
                    try {
                        ics.Load(gd_name);
                    }
                    catch (Exception) {
                    }
                    if (ics != null) {
                        hg_.TABLE_NAME = ics.m_dat.m_tab;
                        if (ics.m_dat.m_list.Count() > 0) {
                            for (int i = 0; i < ics.m_dat.m_list[0].m_query.lq.Count(); i++) {
                                if (!ics.m_dat.m_list[0].m_query.lq[i].m_isCustExpr) {
                                    string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                                    t = t.Replace(ics.m_dat.m_tab + ".", "");
                                    if (hg_.Setting_param.GROUP_FIELD == t) {
                                        hg_.Setting_param.index_group_field = i + 1;
                                    }
                                    hg_.FLD.Add(t);
                                    hg_.CAPTION_FLD.Add(ics.m_dat.m_list[0].m_query.lq[i].title);
                                    str_fld.Add(t);
                                }
                                else {
                                    OrmItemExpr nw_ = new OrmItemExpr();
                                    nw_.m_expression = ics.m_dat.m_list[0].m_query.lq[i].m_CustExpr;
                                    nw_.m_name = ics.m_dat.m_list[0].m_query.lq[i].title;
                                    nw_.m_sp = ics.m_dat.m_list[0].m_query.lq[i].m_typeCustExpr;
                                    if (hg_.Setting_param.GROUP_FIELD == nw_.m_name) {
                                        hg_.Setting_param.index_group_field = i + 1;
                                    }
                                    str_fld.Add(nw_.m_name);
                                    hg_.FLD.Add(nw_.m_name);
                                    hg_.CAPTION_FLD.Add(nw_.m_name);
                                }
                            }
                        }
                    }
                }
            }
            return hg_;
        }
    }

     
    


    /// <summary>
    /// 
    /// </summary>
    public enum ExtendedControlRight
    {
        WithoutExtension,
        OnlyView,
        FullRight
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TypeStatus
    {
        PUB,
        PRI,
        CUS,
        DSL,
        LT1,
        LT2,
        ISV,
        HCC,
        CON,
        CUR,
        EXP
    }

    /// <summary>
    /// 
    /// </summary>
    public class SettingIRPClass
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public int MAX_REC { get; set; }
        public TypeStatus STATUS_ { get; set; }
        public string DESCRIPTION { get; set; }
        public bool IS_VISIBLE { get; set; }
        public string Query { get; set; }
        public int MAX_COLUMNS { get; set; }
        public string Ident_User { get; set; }
        public bool isCorrectQuery { get; set; }
        public int MaxID { get; set; }
        public int MinID { get; set; }
        public int MAX_REC_PAGE { get; set; }
        public string ExtendedControlRight { get; set; }
        public string GROUP_FIELD { get; set; }
        public int index_group_field { get; set; }
        public bool IS_SQL_REQUEST { get; set; }
        public bool IS_VISIBLE_ALL_REC { get; set; }
        public string COMMENTS { get; set; }
        public bool IS_FILL_COLOR { get; set; }
        public string SYS_COORD { get; set; }
        public bool IS_SEND_NOTIFY_MESS { get; set; }
        public bool IS_TESTING_REQUEST { get; set; }
        public string EMAIL_QUESTION { get; set; }
        public string EMAIL_ANSWER { get; set; }
        public bool IS_ENABLE_INTERVAL { get; set; }
        public int MAX_VAL_INTERVAL { get; set; }
        public int MIN_VAL_INTERVAL { get; set; }
        public int MAX_VAL { get; set; }


        public SettingIRPClass()
        {
            index_group_field = -1;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SettingIRPClass()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class RecordPtrDB
    {

        public int JoinFromIndex { get; set; }
        public int JoinToIndex { get; set; }

        public int Precision { get; set; }
        public string FieldJoinFrom { get; set; }
        public string FieldCaptionFrom { get; set; }
        public string FieldJoinTo { get; set; }
        public string FieldCaptionTo { get; set; }
        public string NameTableFrom { get; set; }
        public string NameTableTo { get; set; }
        public string NameFieldForSetValue { get; set; }
        public string Name { get; set; }
        public string CaptionNameTable { get; set; }
        public int KeyValue { get; set; }
        public object Value { get; set; }
        public bool isNotNull { get; set; }
        public string DefVal { get; set; }
        public OrmVarType TypeVal { get; set; }
        public int Index { get; set; }
        public object OldVal { get; set; }
        public object NewVal { get; set; }
        public bool isMandatory { get; set; }
        public string LinkField { get; set; }
        public string NameLayer { get; set; }
        public object ident_loop { get; set; }

    }

    public class Class_IRP_Object
    {
        public List<RecordPtrDB> FLD_DETAIL { get; set; }
        public List<string> FLD { get; set; }
        public List<Type> FLD_TYPE { get; set; }
        public List<string> CAPTION_FLD { get; set; }
        public List<object[]> Val_Arr { get; set; }
        public string FILTER { get; set; }
        public string TABLE_NAME { get; set; }
        public SettingIRPClass Setting_param { get; set; }
        public TypeStatus StatusObject { get; set; }
        public List<OrmField> Fld_Orm { get; set; }
        public Dictionary<string, string> NameItemMenu_CUR = new Dictionary<string, string>();
        public Dictionary<string, string> NameItemMenu_EXP = new Dictionary<string, string>();
        public SettingIRPClass[] PagesIndexRange { get; set; }




        /// <summary>
        /// 
        /// </summary>
        public Class_IRP_Object()
        {
            FLD = new List<string>();
            CAPTION_FLD = new List<string>();
            Val_Arr = new List<dynamic[]>();
            Setting_param = new SettingIRPClass();
            FILTER = "";
            TABLE_NAME = "";
            FLD_TYPE = new List<Type>();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Class_IRP_Object()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            FLD = null;
            CAPTION_FLD = null;
            Val_Arr = null;
            Setting_param = null;
            FILTER = "";
            TABLE_NAME = "";
            GC.SuppressFinalize(this);
        }



    }

}
