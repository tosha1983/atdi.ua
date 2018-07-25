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
    public class ClassORM
    {

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
