using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    public class reportitem
    {
        public reportitem() { Clear(); }
        public char m_tag;
        public string m_item;
        public string m_tab;

        public string m_as;
        public reportitem[] m_list;
        public string m_blkname;
        public string m_lang;
        public char m_we;

        public const int rtfNOEMPTY = 2;
        public const int rtfPAGEBREAK = 1;
        public const int rtfSECTIONBREAK = 4;
        public int m_rtfOpt;
        public int m_rtfFactLevel;
        public Query m_query;
        //public OrmItem[] m_fk; 
        //public OrmItem m_fkff; 
        public void Clear() //deletes list
        {
            m_list = new reportitem[0];
            m_tab = m_item = m_as = null;
            m_tag = '\0';
            m_blkname = "";
            m_lang = null;
            numblk = 0;
            //m_fk = null;
            //m_fkff = null;
            m_query = new Query();
            m_rtfFactLevel = 0;
            //PRep=null;
        }
        //frame GetConfig();
        public void SetConfig(frameobject f)
        {
            Clear();
            Frame p = new Frame(f), bb;
            int itag;
            p.Get("TYPE", out itag); m_tag = (char)itag;
            p.Get("Item", out m_item);
            p.Get("Table", out m_tab);
            p.Get("As", out m_as);
            p.Get("Name", out m_blkname); if (m_blkname == null) m_blkname = "";
            if (p.Get("Query", out bb)) m_query.SetConfig(bb);
            if (!p.Get("Options", out m_rtfOpt)) m_rtfOpt = 0;
            if (!p.Get("FACTOR_M", out m_rtfFactLevel)) m_rtfFactLevel = 0;
            if ((m_tag == 'C' || m_tag == 'R' || m_tag == 'M') && m_query.logTab == null) m_query.logTab = m_tab;
            List<reportitem> li = new List<reportitem>();
            for (int i = 0; p.Get(i.ToString(), out bb); i++)
            {
                reportitem r = new reportitem();
                r.SetConfig(bb);
                li.Add(r);
            }
            m_list = li.ToArray();
        }
        public int numblk, numrec; //at report generation time
                                   //PredefinedReport PRep; ////only 'P', at report generation time
        public reportitem[] Array1() { return new reportitem[] { this }; }
    }
}
