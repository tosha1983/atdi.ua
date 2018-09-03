using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.LegacyServices.Icsm
{

    
    public class Query {

        public struct paramNTV
        {
            public string paramName;
            public OrmDataCoding paramType;
            public string paramValue;
        }

        public Query() { Clear(); }
        public Query(string table, string oqlFilter) { Clear(); logTab = table; mwhere = oqlFilter; }
		public string logTab;
		public Querytem[] lq;
		public int bagid;
		public int superbagid;
		public string mwhere; 
        public paramNTV[] Params; 
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

       
		public void SetConfig(frameobject f)
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
