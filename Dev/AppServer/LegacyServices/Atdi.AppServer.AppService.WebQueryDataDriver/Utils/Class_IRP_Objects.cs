using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OrmCs;
using FormsCs;
using System.Text;


namespace Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities
{

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
        public bool IS_VALID_LICENSE { get; set; }
        public int TOTAL_COUNT_GROUP { get; set; }
        public int HIDDEN_COLUMNS { get; set; }
        public string TYPE2 { get; set; }
        public string TYPE3 { get; set; }
        public string ADM_NAME { get; set; }
        public string TYPE { get; set; }
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

    public class ColumnMetaD
    {
        public Type Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public uint Position { get; set; }

        public uint Width { get; set; }

        public uint Order { get; set; }

        public uint Rank { get; set; }

        public uint Show { get; set; }

        public string Format { get; set; }
        public OrmField ormFld { get; set; }
    public ColumnMetaD()
        {
            Title = "";
            Description = "";
            Position = 0;
            Width = 0;
            Order = 0;
            Rank = (uint)ConnectDB.NullI;
            Show = 0;
            ormFld = new OrmField();
        }

    }

    public class QueryMetaD
    {
    
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Techno { get; set; }
        public string TableName { get; set; }

        public ColumnMetaD[] Columns { get; set; }

        public QueryMetaD()
        {
            Name = "";
            Title = "";
            Description = "";
            Techno = "";
            TableName = "";
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class Class_IRP_Object
    {
        public List<RecordPtrDB> FLD_DETAIL { get; set; }
        public List<string> FLD { get; set; }
        public List<Type> FLD_TYPE { get; set; }
        public List<string> CAPTION_FLD { get; set; }
        public List<string> FORMAT_FLD { get; set; }
        public List<object[]> Val_Arr { get; set; }
        public string FILTER { get; set; }
        public string TABLE_NAME { get; set; }
        public SettingIRPClass Setting_param { get; set; }
        public List<WebConstraint> SettingConstraint { get; set; }
        public List<WebAddParams> SettingAddParams { get; set; }
        public string FormatConstraint { get; set; }
        public TypeStatus StatusObject { get; set; }
        public List<OrmField> Fld_Orm { get; set; }
        public SettingIRPClass[] PagesIndexRange { get; set; }
        



        /// <summary>
        /// 
        /// </summary>
        public Class_IRP_Object()
        {
            FLD = new List<string>();
            CAPTION_FLD = new List<string>();
            FORMAT_FLD = new List<string>();
            Val_Arr = new List<dynamic[]>();
            Setting_param = new SettingIRPClass();
            SettingAddParams = new List<WebAddParams>();
            SettingConstraint = new List<WebConstraint>();
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
