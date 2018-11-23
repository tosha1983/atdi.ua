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



namespace XICSM.Atdi.Icsm.Plugins.WebQuery
{
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


    public class ClassORM
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static RecordPtrDB GetTableFromORM(string fld_check, string tableName)
        {
            RecordPtrDB rc = new RecordPtrDB();
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Join:
                            {

                                OrmFieldJ fj = (OrmFieldJ)f1;
                                OrmJoin joi = fj.Join;
                                OrmTable tc = joi.JoinedTable;


                                string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);

                                if (fld_check == f1.Name)
                                {
                                    rc.NameTableTo = tc.Name;
                                    rc.Name = f1.Name;
                                    if (joi.From.Count() > 0) rc.FieldJoinFrom = joi.From[0].Name;
                                    if (joi.To.Count() > 0) rc.FieldJoinTo = joi.To[0].Name;
                                    if (joi.To.Count() > 0) { if (joi.To[0].DDesc != null) rc.TypeVal = joi.To[0].DDesc.ClassType; rc.Precision = joi.To[0].DDesc.Precision; }
                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }

        public static bool CheckField(string fld_check, string tableName)
        {
            bool rc = false;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;

                                if (fld_check == f1.Name)
                                {
                                    rc = true;
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
        /// <param name="ID"></param>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        public static List<RecordPtrDB> GetLinkData(string tableName, string fld)
        {
            string TableName2 = "";
            Dictionary<string, List<RecordPtrDB>> recDB_ = new Dictionary<string, List<RecordPtrDB>>();
            List<string> LstTable = new List<string>();
            TableName2 = tableName;
            string[] Spl = null;
            List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();
            RecordPtrDB recDB = new RecordPtrDB();
            recDB.NameTableTo = tableName;

            if (fld.IndexOf(".") > 0) { Spl = fld.Split(new char[] { '.' }); }

            if (Spl != null)
            {
                for (int r = 0; r < Spl.Count(); r++)
                {

                    if (r < Spl.Count() - 1)
                    {
                        recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                        recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                        Spl[r] = recDB.NameTableTo;
                        if (!LstTable.Contains(recDB.NameTableFrom))
                            LstTable.Add(recDB.NameTableFrom);

                        recDB.LinkField = fld;
                        recDB_Lst.Add(recDB);
                    }
                    else
                    {
                        recDB = new RecordPtrDB();
                        recDB.NameTableTo = recDB_Lst[recDB_Lst.Count() - 1].NameTableTo;
                        recDB.Name = recDB_Lst[recDB_Lst.Count() - 1].Name;
                        recDB.NameFieldForSetValue = Spl[r];
                        recDB.FieldJoinTo = Spl[r];

                        if (!LstTable.Contains(recDB.NameTableTo))
                            LstTable.Add(recDB.NameTableTo);

                        recDB.LinkField = fld;
                        recDB_Lst.Add(recDB);
                    }
                    tableName = GetTableFromORM(Spl[r], tableName).NameTableTo;
                }
            }
            else
            {
                recDB = new RecordPtrDB();
                recDB.NameTableTo = tableName;
                recDB.NameFieldForSetValue = fld;
                if (!LstTable.Contains(recDB.NameTableTo))
                    LstTable.Add(recDB.NameTableTo);

                recDB.LinkField = fld;
                recDB_Lst.Add(recDB);
            }
            return recDB_Lst;
        }



        public static string GetTableName(string Query)
        {
            string str_TableName = "";
            var ics = new IcsmReport();
            Frame f = new Frame();
            int x1 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x1 + 2);
            int x2 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x2 + 2);
            InChannelString strx = new InChannelString(Query);
            f.Load(strx);
            ics.SetConfig(f);
            if (ics != null)
            {
                str_TableName = ics.m_dat.m_tab;
            }
            return str_TableName;
        }


        public static List<string> GetProperties(string Query, bool isDecrypt)
        {
            var str_fld = new List<string>();
            var hg_ = new Class_IRP_Object();
            var ics = new IcsmReport();
            Frame f = new Frame();
            int x1 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x1 + 2);
            int x2 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x2 + 2);
            InChannelString strx = new InChannelString(Query);
            f.Load(strx);
            ics.SetConfig(f);
            if (ics != null)   {
                hg_.tablename = ics.m_dat.m_tab;
                if (ics.m_dat.m_list.Count() > 0)    {
                    for (int i = 0; i < ics.m_dat.m_list[0].m_query.lq.Count(); i++)    {
                        if (!ics.m_dat.m_list[0].m_query.lq[i].m_isCustExpr)   {
                            string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                            t = t.Replace(ics.m_dat.m_tab + ".", "");
                            hg_.fld.Add(t);
                            hg_.captionfld.Add(ics.m_dat.m_list[0].m_query.lq[i].title);
                            str_fld.Add(t);
                        }
                        else {
                            OrmItemExpr nw_ = new OrmItemExpr();
                            nw_.m_expression = ics.m_dat.m_list[0].m_query.lq[i].m_CustExpr;
                            nw_.m_name = ics.m_dat.m_list[0].m_query.lq[i].title;
                            nw_.m_sp = ics.m_dat.m_list[0].m_query.lq[i].m_typeCustExpr;
                            str_fld.Add(nw_.m_name);
                            hg_.fld.Add(nw_.m_name);
                            hg_.captionfld.Add(nw_.m_name);
                        }
                    }
                }
            }
            return str_fld;
        }
    }
      
    


    public class Class_IRP_Object
    { 
        public List<string> fld { get; set; }
        public List<Type> fldtype { get; set; }
        public List<string> captionfld { get; set; }
        public string filter { get; set; }
        public string tablename { get; set; }


        public Class_IRP_Object()
        {
            fld = new List<string>();
            captionfld = new List<string>();
            fldtype = new List<Type>();
        }


        ~Class_IRP_Object()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }

}
