using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    public struct SelectData
    {
        public OrmOp opG; //opNop or opGt or opGe
        public OrmOp opL; //opNop or opLt or opLe
        public OrmOp opE; //opNop or opEq or opNeq or opLike or opNotLike or opNull or opNotNull
        public OrmOp opI; //opNop or opIn or opNotIn
        public string paramG, paramL, paramE, paramI;
        public int includeNull; //for dates and opG/opL only
        public void Clear() { opG = opL = opE = opI = OrmOp.opNop; includeNull = 0; paramG = paramL = paramE = paramI = ""; }
        public bool HasFilter() { return opG != OrmOp.opNop || opL != OrmOp.opNop || opE != OrmOp.opNop || opI != OrmOp.opNop; }
        public bool Equals(SelectData p)
        {
            if (opG != p.opG || opL != p.opL || opE != p.opE || opI != p.opI) return false;
            if (opG != OrmOp.opNop && paramG != p.paramG) return false;
            if (opL != OrmOp.opNop && paramL != p.paramL) return false;
            if (opE != OrmOp.opNop && opE != OrmOp.opNull && opE != OrmOp.opNotNull && paramE != p.paramE) return false;
            if (opI != OrmOp.opNop && paramI != p.paramI) return false;
            if ((opG != OrmOp.opNop || opL != OrmOp.opNop) && includeNull != p.includeNull) return false;
            return true;
        }
        public frameobject GetConfig()
        {
            Frame bb = new Frame();
            bool inu = false;
            if (opG == OrmOp.opGe) { bb.Add(">=", paramG); inu = true; }
            if (opG == OrmOp.opGt) { bb.Add(">", paramG); inu = true; }
            if (opL == OrmOp.opLe) { bb.Add("<=", paramL); inu = true; }
            if (opL == OrmOp.opLt) { bb.Add("<", paramL); inu = true; }
            if (inu) bb.Add("includNull", includeNull);
            if (opE == OrmOp.opLike) bb.Add("Like", paramE);
            if (opE == OrmOp.opNotLike) bb.Add("NotLike", paramE);
            if (opE == OrmOp.opEq) bb.Add("=", paramE);
            if (opE == OrmOp.opNeq) bb.Add("<>", paramE);
            if (opE == OrmOp.opNull) bb.Add("Null", paramE);
            if (opE == OrmOp.opNotNull) bb.Add("NotNull", paramE);
            if (opI == OrmOp.opIn) bb.Add("In", paramI);
            if (opI == OrmOp.opNotIn) bb.Add("NotIn", paramI);
            return bb;
        }
        public void SetConfig(frameobject f)
        {
            string s; Frame p = new Frame(f);
            Clear();
            bool inu = false;
            if (p.Get(">=", out s)) { paramG = s; opG = OrmOp.opGe; inu = true; }
            if (p.Get(">", out s)) { paramG = s; opG = OrmOp.opGt; inu = true; }
            if (p.Get("<=", out s)) { paramL = s; opL = OrmOp.opLe; inu = true; }
            if (p.Get("<", out s)) { paramL = s; opL = OrmOp.opLt; inu = true; }
            if (inu && !p.Get("includNull", out includeNull)) includeNull = 0;
            if (p.Get("Like", out s)) { paramE = s; opE = OrmOp.opLike; }
            if (p.Get("NotLike", out s)) { paramE = s; opE = OrmOp.opNotLike; }
            if (p.Get("=", out s)) { paramE = s; opE = OrmOp.opEq; }
            if (p.Get("<>", out s)) { paramE = s; opE = OrmOp.opNeq; }
            if (p.Get("Null", out s)) { paramE = s; opE = OrmOp.opNull; }
            if (p.Get("NotNull", out s)) { paramE = s; opE = OrmOp.opNotNull; }
            if (p.Get("In", out s)) { paramI = s; opI = OrmOp.opIn; }
            if (p.Get("NotIn", out s)) { paramI = s; opI = OrmOp.opNotIn; }
        }
    }
}
