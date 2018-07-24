using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Data.OleDb;


namespace DAL
{

    [Serializable]
    [DataContract]
    public enum TypeDb
    {
        ORACLE,
        MSSQL,
        UNKNOWN
    }
    [Serializable]
    [DataContract]
    public enum enumRulesAccess
    {
        Insert,
        Update,
        Delete,
        Select
    }

    [Serializable]
    [DataContract]
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
        public int Index { get; set; }
        public object OldVal { get; set; }
        public object NewVal { get; set; }
        public bool isMandatory { get; set; }
        public string LinkField { get; set; }
        public string NameLayer { get; set; }
        public object ident_loop { get; set; }
        
    }



    /// <summary>
    /// Класс, обеспечивающий соединение с базой данных,
    /// начальную инициализацию, обмен данными с таблицами СУБД
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConnectDB
    {
        public static int NullI = 2147483647;
        public static double NullD = 1E-99;
        public static DateTime NullT = new DateTime(1, 1, 1, 0, 0, 0);
      
    }


}
