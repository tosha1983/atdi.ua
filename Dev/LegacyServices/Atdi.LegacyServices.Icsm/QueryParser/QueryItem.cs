using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    
    public class Querytem
    {
       
    

        public string title;
        public int colWidth; //pixels in 100dpi
        public QueryQsho show; //QshowSEL | QshoCOL
        public string logTab;
        public SelectData m_s;

        public bool m_isCustExpr;
        public string m_CustExpr;
        public Semant m_typeCustExpr;
        public string path; //1st tab can be table alias!

        public string format;
        public string inputstyle;
        public Ordering ord;
        public int ord_rank;
     
        public void Clear() { }

        public void SetConfig(frameobject f)
        {
            Clear();
            Frame b = new Frame(f), bb, pi;
            b.Get("Title", out title);
            b.Get("Width", out colWidth);
            b.Get("Table", out logTab);
            if (b.Get("Config", out bb))
            {
                if (bb.Get("Data", out pi))
                {
                    m_s.SetConfig(pi);
                    if (!bb.Get("IsCust", out m_isCustExpr)) m_isCustExpr = false;
                    bb.Get("Expr", out m_CustExpr);
                    string m_typeS;
                    if (bb.Get("Type", out m_typeS)) m_typeCustExpr = Semant.Get(m_typeS);
                }
                else m_s.SetConfig(bb); //item...
            }
            b.Get("Format", out format);
            b.Get("InputStyle", out inputstyle);
            int or;
            b.Get("Order", out or); ord = (Ordering)or;
            b.Get("Rank", out ord_rank);
            //b.Get(sym_ModLstBox,modLstBx);
            b.Get("PATH", out path);
            if (b.Get("Show", out or)) show = (QueryQsho)or; else show = title.IsNotNull() ? QueryQsho.QshoSEL | QueryQsho.QshoCOL : QueryQsho.QshoNONE;
        }
    }
}
