using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.LegacyServices.Icsm
{

    [Flags]
    public enum QueryQNO {
	    QNO_NONE=	0,
        QNO_ORDER= 1, //Query cannot have order by
        QNO_WHERE= 2, //Query cannot have where
        QNO_SELECT= 4, //Query cannot have select
        QNO_UNUSED= 8, //Query must be cleansed
        QNO_EXTJOIN=  16, //Query must have no join (RCOMP joins OK)
        QNO_RCOMPJOIN=  32, //Query must have no RCOMP join (join to it own component)
        QNO_BADFORINPUT= 64, //IDs, DATE_CREATED... excluded
        QNO_BAG= 128, //Bag not allowed
        QO_RTFTITLES= 256, //title of columns .,_,a-z,0-9
        QO_SELECT= 512, //All items must be retrieved .,_,a-z,0-9
        QNO_MULTICOL= 1024, //Only one column maximum
    }
    [Flags]
    public enum QueryQsho {
        QshoNONE=0,
        QshoSEL= 1, //item fetched (in select clause)
        QshoCOL= 3, //item fetched and visible in some column
    }
    [Flags]
    public enum QueryLinkOpt {
        Filter=1,
        Order=2,
        Fetch=4,
        ALL=255
    }

    public enum OrmDataCoding
    { //idem itemtype
        tvalNULL = 0,
        tvalNUMBER = 1,
        tvalSTRING = 2,
        tvalDATETIME = 3,
        tvalBINARY = 4,
        tvalGUID = 5
    }

    public enum Ordering { oNone = 0, oAsc = 1, oDesc = 2 };

    public enum typSemant
    {
        tNorm = 0, tFreq = 1, tWatts = 2, tLongi = 3, tLati = 4, tNumber = 5, tDate = 6,
        tHour = 7, tDist = 8, tdBWatts = 9, tCurrency = 10, tBw = 11, tStri = 12, tdBm = 13,
        tM = 14, tdB = 15, tDeg = 16, tCombo = 17, tBool = 18, tKelvins = 19, tdBWpHz = 20,
        tPattern = 21, tAsl = 22, tAgl = 23, tEDeg = 24, tmVpm = 25, tComboNum = 26, tdBmuVm = 27,
        tComboUser = 28, tMinpDay = 29, tDesigEm = 30, tCsys = 31, tForeignId = 32,
        tInteger = 33, tMbitps = 34, tM2 = 35, tComboWrkf = 36, tmSm = 37, tdBkW = 38, tSecond = 39, tHour2 = 40,
        tPerc = 41, tDatMsec = 42, tListCombo = 43, tListComboUser = 44, tPolygonItu = 45,
        tTelsys = 46, tItuSrvc = 47, tTariff = 48, tKm2 = 49, tGuid = 50, tTons = 51, tComboUserNum = 52, tBinary = 53,
        tFolder = 54,
        //COMPLETER AUSSI typSpecial dans IMainApp.h
    };

    public class Semant
    {
        public typSemant m_type;
        public string m_sym;
        //tCombo,tComboNum,tComboUser,tComboUserNum: combo name
        //tComboWrkf : table name
        //tForeignId : joined table
        public double m_div;
        //tFreq & tM & tSecond only 
        // tCombo(Num,User) : =is coded 
        public double m_min, m_max;
        //tStr: m_max is max length 
        //tCombo(Num,User): m_max is pixel width
        //tListCombo(User) : m_max is TLCOMB_ option
        //others: min & max value

        public string mName; //name of special, ex: "eri_Country", "F/MHz"

        public Semant(string name, typSemant t)
        {
            mName = name;
            m_type = t; m_div = 0.0; m_min = m_max = 1e-99; m_sym = null;
            Dico[name] = this;
        }

        static Dictionary<string, Semant> Dico = new Dictionary<string, Semant>();
        public enum Tlcomb
        {
            NORMAL = 0, UPPERCASE = 1,
            EXTREM = 2 //Add comma at extremities 
        }

        static public Semant Get(string specname)
        {
            if (specname.IsNull()) return null;
            Semant res = null;
            if (Dico.TryGetValue(specname, out res)) return res;
            if (specname.StartsWith("eri_"))
            {
                res = new Semant(specname, typSemant.tCombo);
                res.m_sym = specname.Substring(4); res.m_max = 400; res.m_div = 1;
            }
            else if (specname.StartsWith("lov_"))
            {
                res = new Semant(specname, typSemant.tComboUser);
                res.m_sym = specname.Substring(4); res.m_max = 400; res.m_div = 1;
            }
            else if (specname.StartsWith("stat_"))
            {
                res = new Semant(specname, typSemant.tComboWrkf);
                res.m_sym = specname.Substring(5); res.m_max = 400;
            }
            else if (specname.StartsWith("fk_"))
            {
                res = new Semant(specname, typSemant.tForeignId);
                res.m_sym = specname.Substring(3);
            }
            else if (specname.StartsWith("list_eri_"))
            {
                res = new Semant(specname, typSemant.tListCombo);
                res.m_sym = specname.Substring(9); res.m_div = (int)Tlcomb.NORMAL;
            }
            else if (specname.StartsWith("list_lov_"))
            {
                res = new Semant(specname, typSemant.tListComboUser);
                res.m_sym = specname.Substring(9); res.m_div = (int)Tlcomb.NORMAL;
            }
            else if (specname.StartsWith("String("))
            {
                res = new Semant(specname, typSemant.tStri);
                res.m_max = int.Parse(specname.Substring(7, specname.Length - 8));
            }
            else if (specname.StartsWith("Integer("))
            {
                res = new Semant(specname, typSemant.tInteger);
                int i = specname[8] - '0'; double mm = Math.Pow(10, i) - 1; res.m_max = mm - 1; res.m_min = -mm;
            }
            else if (specname == "Folder") { res = new Semant(specname, typSemant.tFolder); res.m_max = 256; }
            //if (res!=null) Dico[specname] = res;
            return res;
        }
    }

        public class Querytem {
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

	    //if query opened...
	    //public OrmItem qit; 

	    //non static
	    //public Querytem();
	    //void operator =(Querytem qu);
        public void Clear() { }
	    //CQItem *AllocItem();
	    public void SetConfig(frame f)
        {
           	Clear();
	        Frame b= new Frame(f),bb,pi;
	        b.Get("Title",out title);
	        b.Get("Width",out colWidth);
	        b.Get("Table",out logTab);
	        if (b.Get("Config",out bb)) {
	            if (bb.Get("Data",out pi)) {
                    m_s.SetConfig(pi);
		            if (!bb.Get("IsCust",out m_isCustExpr)) m_isCustExpr= false;
                    bb.Get("Expr",out m_CustExpr);
                    string m_typeS;
                    if (bb.Get("Type",out m_typeS)) m_typeCustExpr= Semant.Get(m_typeS);
                    }
                else m_s.SetConfig(bb); //item...
                }
	        b.Get("Format",out format);
	        b.Get("InputStyle",out inputstyle);
	        int or;
	        b.Get("Order",out or); ord= (Ordering)or;
	        b.Get("Rank",out ord_rank);
	        //b.Get(sym_ModLstBox,modLstBx);
	        b.Get("PATH",out path);
            if (b.Get("Show",out or)) show= (QueryQsho)or; else show= title.IsNotNull() ? QueryQsho.QshoSEL|QueryQsho.QshoCOL : QueryQsho.QshoNONE;
        }
	    };

    public struct paramNTV  {
        public string paramName;
		public OrmDataCoding paramType;
		public string paramValue;
    }
    public class Query {
        public Query() { Clear(); }
        public Query(string table, string oqlFilter) { Clear(); logTab = table; mwhere = oqlFilter; }
		public string logTab;
		public Querytem[] lq;
		public int bagid;
		public int superbagid;
		public string mwhere; //additional filter
        public paramNTV[] Params; //Obsolete!!  int nParams; TCHAR paramName[MAX_QUERY_PARAMETERS][31];  itemtype paramType[MAX_QUERY_PARAMETERS];  TCHAR paramValue[MAX_QUERY_PARAMETERS][101];
		public bool distinct;
		
        public bool HasSort() //true if has one item with order set.
        {
	        if (lq!=null) foreach(Querytem l in lq) if (l.ord!=Ordering.oNone) return true;
	        return false;
        }
        public bool HasFilter() //true if has one filter
        {
            if (bagid!=Utils.NullI || superbagid!=Utils.NullI) return true;
            if (mwhere.IsNotNull()) return true;
            if (lq != null) foreach (Querytem l in lq) if (l.m_s.HasFilter()) return true;
            return false;
        }
        public void Clear() { lq = null; Params = null; mwhere = null; bagid = superbagid = Utils.NullI; }

        private static int cmpSortQuerytindex(Querytem t1, Querytem t2) 
        {
            return t1.ord_rank- t2.ord_rank;
        }

       
		public void SetConfig(frame f)
        {
	        Clear();
	        Frame b= new Frame(f),bb,bp;
	        b.Get("Distinct",out distinct);
	        b.Get("Table",out logTab);
	        b.Get("mWhere",out mwhere);
	        b.Get("Bag",out bagid);
	        b.Get("Superbag",out superbagid);
	        List<Querytem> ll= new List<Querytem>();
	        for(int n=1;b.Get(n.ToString(),out bb);n++) {
		        Querytem l= new Querytem();
		        l.SetConfig(bb);
		        ll.Add(l);
		        }
            lq= ll.ToArray();
	        if (logTab==null && lq.Length>0) logTab= lq[0].logTab;
	        Params=null;
	        if (b.Get("Params",out bp)) {
                List<paramNTV> lpa= new List<paramNTV>();
                for(int i=1;;i++) {
                    if (!bp.Get(i.ToString(), out bb)) break;
                    paramNTV pa= new paramNTV();
			        bb.Get("Name",out pa.paramName);
			        int itt;
			        if (!bb.Get("Type",out itt)) itt=0;
			        pa.paramType= (OrmDataCoding)itt;
			        bb.Get("Val",out pa.paramValue);
                    lpa.Add(pa);
			        }
                Params= lpa.ToArray();
		        }
        }

	    };

}
