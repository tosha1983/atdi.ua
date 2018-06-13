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

        public static List<string> GetProperties(string Query, bool isDecrypt)
        {
            List<string> str_fld = new List<string>();
            Class_IRP_Object hg_ = new Class_IRP_Object();
            IcsmReport ics = new IcsmReport();
            Frame f = new Frame();
            int x1 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x1 + 2);
            int x2 = Query.IndexOf("\r\n");
            Query = Query.Remove(0, x2 + 2);
            InChannelString strx = new InChannelString(Query);
            f.Load(strx);
            ics.SetConfig(f);
            if (ics != null)   {
                hg_.TABLE_NAME = ics.m_dat.m_tab;
                if (ics.m_dat.m_list.Count() > 0)    {
                    for (int i = 0; i < ics.m_dat.m_list[0].m_query.lq.Count(); i++)    {
                        if (!ics.m_dat.m_list[0].m_query.lq[i].m_isCustExpr)   {
                            string t = ics.m_dat.m_list[0].m_query.lq[i].path;
                            t = t.Replace(ics.m_dat.m_tab + ".", "");
                            hg_.FLD.Add(t);
                            hg_.CAPTION_FLD.Add(ics.m_dat.m_list[0].m_query.lq[i].title);
                            str_fld.Add(t);
                        }
                        else {
                            OrmItemExpr nw_ = new OrmItemExpr();
                            nw_.m_expression = ics.m_dat.m_list[0].m_query.lq[i].m_CustExpr;
                            nw_.m_name = ics.m_dat.m_list[0].m_query.lq[i].title;
                            nw_.m_sp = ics.m_dat.m_list[0].m_query.lq[i].m_typeCustExpr;
                            str_fld.Add(nw_.m_name);
                            hg_.FLD.Add(nw_.m_name);
                            hg_.CAPTION_FLD.Add(nw_.m_name);
                        }
                    }
                }
            }
            return str_fld;
        }
    }
      
    


    public class Class_IRP_Object
    { 
        public List<string> FLD { get; set; }
        public List<Type> FLD_TYPE { get; set; }
        public List<string> CAPTION_FLD { get; set; }
        public string FILTER { get; set; }
        public string TABLE_NAME { get; set; }


        public Class_IRP_Object()
        {
            FLD = new List<string>();
            CAPTION_FLD = new List<string>();
            FLD_TYPE = new List<Type>();
        }


        ~Class_IRP_Object()
        {
            Dispose();
        }


        public void Dispose()
        {
            FLD = null;
            CAPTION_FLD = null;
            FILTER = "";
            TABLE_NAME = "";
            GC.SuppressFinalize(this);
        }

    }

}
