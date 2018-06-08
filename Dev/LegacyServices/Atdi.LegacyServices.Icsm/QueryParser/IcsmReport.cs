using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Atdi.LegacyServices.Icsm
{ 
    public class IcsmReport
    {
        public IcsmReport() { m_dat = new reportitem();/* m_globparam = new GlobalParameters(); */ m_records = new Query(); }
        public string m_desc;
        public string m_techno; //SpecIrpTechno M=metafile, F=input form report C=Crystal 'U'pdate (I=Itu) R=rtf
        public int m_renderBy;
        public reportitem m_dat;
        public Query m_records; //record selection/ordering
        public string m_filepath; // c:\toto\rep\unrep.irp
        public int m_lastRaccNum; //techno 'R' only
        public string m_script;

        public void Clear(bool alsoPath)
        {
            m_dat.Clear();
            m_records.Clear();
            m_desc = null;
            m_script = null;
            if (alsoPath)
            {
                m_filepath = null;
                m_techno = null;
            }
        }
        public void SetConfig(frameobject f)
        {
            Clear(false);
            Frame p = new Frame(f);
            Frame pp;
            if (!p.Get("Techno", out m_techno)) m_techno = "M";
            p.Get("DESCRIPTION", out m_desc);
            p.Get("PARAMETERS", out pp);
            p.Get("Data", out pp);
            m_dat.SetConfig(pp);
            if (!p.Get("FILTER", out pp)) p.Get("Params", out pp);
            m_records.SetConfig(pp);
            if (!p.Get("B", out m_renderBy)) m_renderBy = 0;
        }

        public bool Load(string fname)
        {
            bool res = false;
            try
            {
                using (StreamReader f = new StreamReader(fname, Utils.GetFileEncoding(fname)))
                {
                    m_filepath = fname;

                    string skip1 = f.ReadLine();
                    string skip2 = f.ReadLine();
                    InChannelFile ch = new InChannelFile(f);
                    Frame p = new Frame();
                    p.Load(ch);
                    SetConfig(p);
                    StringBuilder sb = new StringBuilder();
                    ch.ReadText(sb);
                    m_script = sb.ToString();
                    res = true;
                }
            }
            catch (Exception e)
            {
                
            }
            return res;
        }
    }


}
   

