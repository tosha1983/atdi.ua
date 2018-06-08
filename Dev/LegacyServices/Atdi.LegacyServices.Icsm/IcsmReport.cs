using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Atdi.LegacyServices.Icsm
{ 
    public enum type { tNull = 0, tSym = 1, tStr = 2, tFra = 3, tInt = 4, tDou = 5, tChr = 6, tTim = 7 };
    public enum OrmOp
    {
        opNop = 0, opGt = 1, opGe = 2, opLt = 3, opLe = 4, opEq = 5, opNeq = 6, //same 'operation' in Datalayer.h
        opLike = 7, opIn = 8, opNotIn = 9, opNotLike = 10, opNull = 11, opNotNull = 12
    };

   
    public static class Utils
    {
        public static int NullI = 2147483647;
        public static double NullD = 1E-99;
        public static DateTime NullT = new DateTime(1, 1, 1, 0, 0, 0);


        public static bool IsNull(this DateTime dat)
        {
            return dat.Ticks == 0;
        }
        public static bool IsNull(this double dat)
        {
            return dat == 1e-99;
        }
        public static bool IsNull(this int dat)
        {
            return dat == 0x7FFFFFFF;
        }
        public static bool IsNull(this string dat)
        {
            return string.IsNullOrEmpty(dat);
        }
        public static bool IsNull(object x)
        {
            if (x == null) return false;
            if (x.ToString() == NullI.ToString()
                || x.ToString() == NullT.ToString()
                || x.ToString() == NullD.ToString()
                || x.ToString() == "" || x == null) { return true; }
            else { return false; }
        }
        public static string ToStringNull(this DateTime dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static string ToStringNull(this double dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static string ToStringNull(this int dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static string ToStringNull(this string dat)
        {
            if (dat.IsNull()) return "";
            else return dat.ToString();
        }
        public static bool IsNotNull(this DateTime dat)
        {
            return dat.Ticks != 0;
        }
        public static bool IsNotNull(object x)
        {
            if (x.ToString() != NullI.ToString()
                && x.ToString() != NullT.ToString()
                && x.ToString() != NullD.ToString()
                && x.ToString() != "" && x != null) { return true; }
            else { return false; }
        }
        public static bool IsNotNull(this double dat)
        {
            return dat != 1e-99;
        }
        public static bool IsNotNull(this int dat)
        {
            return dat != 0x7FFFFFFF;
        }
        public static bool IsNotNull(this string dat)
        {
            return !string.IsNullOrEmpty(dat);
        }

        public static double ParseDouble(this string s)
        {
            double v;
            if (s == null || !double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out v)) v = NullD;
            return v;
        }

        public static int IndexFirstNonWhite(this string buff, int ifrm = 0)
        {
            if (string.IsNullOrEmpty(buff)) return 0;
            int ip = ifrm;
            while (ip < buff.Length && char.IsWhiteSpace(buff[ip])) ++ip;
            return ip;
        }


        static public bool ParseDouble(this string buf, ref int idx, out double val, bool allowThousandSeparator = true)
        {
            val = 1e-99;
            int ip = buf.IndexFirstNonWhite(idx);
            string cand = "";
            if (ip == buf.Length) return true;
            if (ip < buf.Length && buf[ip] == '-') { cand += '-'; ip = buf.IndexFirstNonWhite(ip + 1); }
            else if (ip < buf.Length && buf[ip] == '+') ip = buf.IndexFirstNonWhite(ip + 1);
            bool lastDig = false;
            num1:
            while (ip < buf.Length && char.IsDigit(buf[ip])) { cand += buf[ip]; lastDig = true; ip++; }
            if (allowThousandSeparator && lastDig && ip + 3 < buf.Length && buf[ip] == ' '
                && char.IsDigit(buf[ip + 1]) && char.IsDigit(buf[ip + 2]) && char.IsDigit(buf[ip + 3])
                && (ip + 4 == buf.Length || !char.IsDigit(buf[ip + 4]))) { ++ip; goto num1; }  //skip white space separator of thousands
            if (ip < buf.Length && (buf[ip] == ',' || buf[ip] == '.'))
            {
                cand += '.'; ip++;
                while (ip < buf.Length && char.IsDigit(buf[ip])) { cand += buf[ip]; lastDig = true; ip++; }
            }
            if (!lastDig) return false;
            ip = buf.IndexFirstNonWhite(ip);
            if (ip < buf.Length && (buf[ip] == 'e' || buf[ip] == 'E'))
            {
                int ip2 = buf.IndexFirstNonWhite(ip + 1);
                if (ip2 < buf.Length && (buf[ip2] == '-' || buf[ip2] == '+' || char.IsDigit(buf[ip2])))
                {
                    cand += 'e'; ip = ip2;
                    if (ip < buf.Length && buf[ip] == '-') { cand += '-'; ip = buf.IndexFirstNonWhite(ip + 1); }
                    else if (ip < buf.Length && buf[ip] == '+') ip = buf.IndexFirstNonWhite(ip + 1);
                    if (ip == buf.Length || !char.IsDigit(buf[ip])) return false;
                    while (ip < buf.Length && char.IsDigit(buf[ip])) { cand += buf[ip]; ip++; }
                }
            }
            try { val = Convert.ToDouble(cand, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS")); idx = ip; return true; }
            catch { return false; }
        }


        public static Encoding GetFileEncoding(string srcFile)
        {
            Encoding enc = Encoding.Default; //Ansi codepage!
            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            try
            {
                FileStream file = new FileStream(srcFile, FileMode.Open);
                file.Read(buffer, 0, 5);
                file.Close();
                if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                    enc = Encoding.UTF8;
                else if (buffer[0] == 0xff && buffer[1] == 0xfe)
                    enc = Encoding.Unicode;
                else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                    enc = Encoding.UTF32;
                else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                    enc = Encoding.UTF7;
            }
            catch (Exception) { }
            return enc;
        }
    }



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
        public frame GetConfig()
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
        public void SetConfig(frame f)
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

    public class Frame
    {
        public Frame() { fRoot = fLast = null; }
        public Frame(frame f) { fRoot = fLast = f; }
        public static implicit operator frame(Frame ff) { return ff.fRoot; }

        public void Add(string prop, string value) { add(prop, new frameString(value)); }
        public void Add(string prop, int value) { add(prop, new frameInt(value)); }
        public void Add(string prop, double value) { add(prop, new frameDouble(value)); }
        public void Add(string prop, frame value) { add(prop, new frameFrame(value)); }
        public void Add(string prop, DateTime value) { add(prop, new frameDate(value)); }

        public void Set(string prop, string value) { set(prop, new frameString(value)); }
        public void Set(string prop, int value) { set(prop, new frameInt(value)); }
        public void Set(string prop, double value) { set(prop, new frameDouble(value)); }
        public void Set(string prop, frame value) { set(prop, new frameFrame(value)); }
        public void Set(string prop, DateTime value) { set(prop, new frameDate(value)); }

        public bool Get(string prop, out string value)
        {
            value = null;
            frame p = get(prop);
            if (p == null) return false;
            if (p is frameString) { value = ((frameString)p).value; return true; }
            return false;
        }
        public bool Get(string prop, out bool value)
        {
            int val; bool found = Get(prop, out val);
            value = val != 0 && val != 0x7FFFFFFF;
            return found;
        }
        public bool Get(string prop, out int value)
        {
            value = 0x7FFFFFFF;
            frame p = get(prop);
            if (p == null) return false;
            if (p is frameInt) { value = ((frameInt)p).value; return true; }
            return false;
        }
        public bool Get(string prop, out double value)
        {
            value = 1e-99;
            frame p = get(prop);
            if (p == null) return false;
            if (p is frameDouble) { value = ((frameDouble)p).value; return true; }
            if (p is frameInt) { int n = ((frameInt)p).value; if (n != 0x7FFFFFFF) value = n; return true; }
            return false;
        }
        public bool Get(string prop, out Frame value)
        {
            value = null;
            frame p = get(prop);
            if (p == null) return false;
            if (p is frameFrame) { value = new Frame(((frameFrame)p).value); return true; }
            return false;
        }
        public bool Get(string prop, out DateTime value)
        {
            value = new DateTime();
            frame p = get(prop);
            if (p == null) return false;
            if (p is frameDate) { value = ((frameDate)p).value; return true; }
            return false;
        }

        private frame get(string prop)
        {
            frame e;
            for (e = fLast; e != null; e = e.next) if (e.prop == prop) return fLast = e;
            for (e = fRoot; e != null && e != fLast; e = e.next) if (e.prop == prop) return fLast = e;
            return null;
        }

        private void add(string prop, frame p)
        {
            p.next = null;
            p.prop = prop;
            if (fRoot == null) fRoot = p;
            else
            {
                Debug.Assert(fLast != null);
                while (fLast.next != null) fLast = fLast.next;
                fLast.next = p;
            }
            fLast = p;
        }
        private void set(string prop, frame p)
        {
            p.prop = prop;
            if (fLast != null)
            {
                frame e = fLast;
                while (e.next != null)
                {
                    frame ee = e.next;
                    if (ee.prop == prop) { p.next = ee.next; e.next = p; fLast = e; return; }
                    e = ee;
                }
            }
            if (fRoot != null)
            {
                frame e = fRoot;
                if (e.prop == prop) { p.next = e.next; fLast = fRoot = p; return; }
                while (e.next != null && e != fLast)
                {
                    frame ee = e.next;
                    if (ee.prop == prop) { p.next = ee.next; e.next = p; fLast = e; return; }
                    e = ee;
                }
            }
            //have to add it:
            if (fRoot == null) fLast = fRoot = p;
            else
            {
                Debug.Assert(fLast != null);
                while (fLast.next != null) fLast = fLast.next;
                fLast.next = p;
            }
        }

        private frame fRoot;
        private frame fLast;

        public void Load(InChannel c)
        {
            string id = null;
            c.ReadToken();
            if (c.tokType != type.tChr || c.tokC != '{') c.Wanted("'{'");
            for (; ; )
            {
                c.ReadToken();
                if (c.tokType == type.tInt) { id = c.tokI.ToString(); if (c.tokI < 0) c.Wanted("positive int"); }
                else if (c.tokType == type.tSym) { id = c.tokB; }
                else if (c.tokType == type.tChr && c.tokC == '}') break;
                else c.Wanted("property or '}'");
                c.ReadToken();
                if (c.tokType != type.tChr || c.tokC != '=') c.Wanted("=");
                c.ReadToken();
                switch (c.tokType)
                {
                    case type.tSym: Add(id, c.tokB); break;
                    case type.tStr: Add(id, c.tokS); break;
                    case type.tInt: Add(id, c.tokI); break;
                    case type.tTim: Add(id, c.tokT); break;
                    case type.tDou: Add(id, c.tokD); break;
                    case type.tChr:
                        {
                            if (c.tokC != '{') c.Wanted("value");
                            c.UnreadToken();
                            Frame f2 = new Frame();
                            f2.Load(c);
                            Add(id, f2);
                        }
                        break;
                    default: Debug.Assert(false); break;
                }
            }
        }

    }
    public class frame
    {
        public frame next;
        public string prop;
    }
    public class frameString : frame
    {
        public frameString(string v) { value = v; }
        public string value;
    }
    public class frameInt : frame
    {
        public frameInt(int v) { value = v; }
        public int value;
    }
    public class frameDouble : frame
    {
        public frameDouble(double v) { value = v; }
        public double value;
    }
    public class frameFrame : frame
    {
        public frameFrame(frame v) { value = v; }
        public frame value;
    }
    public class frameDate : frame
    {
        public frameDate(DateTime v) { value = v; }
        public DateTime value;
    }

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
        public void SetConfig(frame f)
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

    public class IcsmReport
    {
        public IcsmReport() { m_dat = new reportitem();/* m_globparam = new GlobalParameters(); */ m_records = new Query(); }
        public string m_desc;
        public string m_techno; //SpecIrpTechno M=metafile, F=input form report C=Crystal 'U'pdate (I=Itu) R=rtf
        public int m_renderBy;
        public reportitem m_dat;
        public Query m_records; //record selection/ordering
        //public GlobalParameters m_globparam;
        public string m_filepath; // c:\toto\rep\unrep.irp
                                  //int m_dirlen; // strlen("c:\toto\rep\")  Path.GetDirectoryName(m_filepath)
        public int m_lastRaccNum; //techno 'R' only
        public string m_script;

        public void Clear(bool alsoPath)
        {
            m_dat.Clear();
            //m_globparam.Clear();
            m_records.Clear();
            m_desc = null;
            m_script = null;
            if (alsoPath)
            {
                m_filepath = null;
                m_techno = null;
            }
        }
        public void SetConfig(frame f)
        {
            Clear(false);
            Frame p = new Frame(f);
            Frame pp;
            if (!p.Get("Techno", out m_techno)) m_techno = "M";
            p.Get("DESCRIPTION", out m_desc);
            p.Get("PARAMETERS", out pp);
            //m_globparam.SetConfig(pp);
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


    public class InChannel
    {

        public InChannel() { Flush(); unread = false; noNegativeNumbers = false; }
        public void Flush() { pBuf = 0; Buffer = ""; unread = false; }
        public string Name;
        public void ReadToken()
        {
            if (unread) { unread = false; return; }
            SkipSpaces();
            if (pBuf == Buffer.Length) { tokType = type.tChr; tokC = (char)0; return; } //EOF!!
            char c = Buffer[pBuf];
            if (IsIdentChar(c, true) || c == '\'')
            {
                if (c == 'D' && pBuf + 3 < Buffer.Length && Buffer[pBuf + 1] == 'a' && Buffer[pBuf + 2] == 't' && Buffer[pBuf + 3] == '(')
                    readDatTim(); //"Dat("
                else readSymbol();
            }
            else if (char.IsDigit(c)) readNumeric(1);
            else if (!noNegativeNumbers && c == '-' && pBuf + 1 < Buffer.Length && char.IsDigit(Buffer[pBuf + 1])) { ++pBuf; readNumeric(-1); }
            else if (!noNegativeNumbers && c == '+' && pBuf + 1 < Buffer.Length && char.IsDigit(Buffer[pBuf + 1])) { ++pBuf; readNumeric(1); }
            else if (c == '"') readString('"');
            else if (c == '<' && pBuf + 2 < Buffer.Length && Buffer[pBuf + 1] == '0' && Buffer[pBuf + 2] == '>') //"<0>"
            { tokType = type.tSym; tokB = null; pBuf += 3; }
            else { tokType = type.tChr; tokC = c; ++pBuf; }
        }

        public void UnreadToken() { unread = true; }

        public void Wanted(string s)
        {
            string err = string.Format("ERROR: {0} wanted.", s);
            throw new Exception(err + "Corrupted data");
        }

        public static bool IsIdentChar(char b, bool asFirst)
        {
            if (b >= 'A' && b <= 'Z') return true;
            if (b >= 'a' && b <= 'z') return true;
            if (!asFirst && b >= '0' && b <= '9') return true;
            if (b == '_') return true;
            return false;
        }

        public bool Eof()
        {
            SkipSpaces();
            return pBuf == Buffer.Length;
        }
        public bool ReadText(StringBuilder buff) //LPTSTR &buff,int &nchMaxBuff
        {
            ReadToken();
            if (tokType != type.tSym || tokB != "IMMEDIATETEXT") { unread = true; return false; }
            int nchtot, nlines;
            ReadToken(); nchtot = tokType == type.tInt ? tokI : -1;
            ReadToken(); nlines = tokType == type.tInt ? tokI : -1;

            if (nlines < 0 || nchtot < 0) throw new Exception("Corrupted IMMEDIATETEXT");
            if (nlines == 0) return true;

            Flush();
            buff.EnsureCapacity(nchtot);
            buff.Clear();
            //LPTSTR p= buff; int reste=nchMaxBuff;
            for (int i = 0; i < nlines; i++)
            {
                FillBuff();
                int lr = Buffer.Length;
                if (lr > 0 && Buffer[lr - 1] == '\n')
                {
                    --lr; if (lr > 0 && Buffer[lr - 1] == '\r') --lr;
                }
                int nf = i > 0 ? 2 : 0;
                if (buff.Length + nf + lr > buff.Capacity) throw new Exception("Corrupted ImmText");
                if (nf > 0) buff.Append("\r\n");
                buff.Append(Buffer, 0, lr);
            }
            Flush();
            return true;
        }
        public void DisableNegativeNumbers() { noNegativeNumbers = true; }

        public type tokType;
        public int tokI;
        public string tokB;
        public double tokD;
        public char tokC;
        public DateTime tokT;
        public string tokS;

        public int pBuf;
        public void SkipSpaces()
        {
            loop:
            while (pBuf < Buffer.Length && char.IsWhiteSpace(Buffer[pBuf])) pBuf++;
            if (pBuf == Buffer.Length || (Buffer[pBuf] == '/' && pBuf + 1 < Buffer.Length && Buffer[pBuf + 1] == '/'))
            {
                FillBuff();
                if (pBuf < Buffer.Length) goto loop;
            }
            return;
        }
        public string Buffer;

        public virtual void FillBuff()
        {
            throw new Exception("FillBuff not overriden");  // fills Buffer; empty string iff EOF
        }
       
        private bool unread;
        private void readString(char sep)
        {
            Debug.Assert(Buffer[pBuf] == sep);
            tokS = "";
            ++pBuf;
            string line = "";
            for (; ; )
            {
                if (pBuf == Buffer.Length) Wanted(new string(sep, 1));
                else if (Buffer[pBuf] == sep) { ++pBuf; tokS += line; break; }
                else if (Buffer[pBuf] == '\\')
                {
                    ++pBuf;
                    char c = pBuf < Buffer.Length ? Buffer[pBuf] : (char)0;
                    if (c == 'n') { line += '\n'; ++pBuf; }
                    else if (c == 'r') { line += '\r'; ++pBuf; }
                    else if (c == 't') { line += '\t'; ++pBuf; }
                    else if (c == '\\') { line += '\\'; ++pBuf; }
                    else if (c == '"') { line += '"'; ++pBuf; }
                    else if (c == '\'') { line += '\''; ++pBuf; }
                    else if (char.IsDigit(c))
                    {
                        int i = c - '0'; ++pBuf;
                        Debug.Assert(i >= 0 && i <= 9);
                        if (pBuf < Buffer.Length && char.IsDigit(Buffer[pBuf])) i = (i << 3) + Buffer[pBuf++] - '0';
                        if (pBuf < Buffer.Length && char.IsDigit(Buffer[pBuf])) i = (i << 3) + Buffer[pBuf++] - '0';
                        line += (char)i;
                    }
                    else if (c == 0 || (c == '\n' && pBuf + 1 == Buffer.Length) || (c == '\r' && pBuf + 2 == Buffer.Length && Buffer[pBuf + 1] == '\n'))
                    {
                        tokS += line; line = "";
                        FillBuff();
                    }
                    else Wanted("r,t,n,\\,\",0-9,EOL");
                }
                else line += Buffer[pBuf++];
            }
            tokType = type.tStr;
        }
        private void readSymbol()
        {
            if (Buffer[pBuf] == '\'')
            {
                readString('\'');
                tokType = type.tSym;
                tokB = tokS;
            }
            else
            {
                int t = pBuf + 1;
                while (t < Buffer.Length && IsIdentChar(Buffer[t], false)) ++t;
                tokType = type.tSym;
                tokB = Buffer.Substring(pBuf, t - pBuf);
                pBuf = t;
            }
        }
        private bool noNegativeNumbers;

     

        private void readNumeric(int sig)
        {
            int tot = 0;
            int s = pBuf;
            Debug.Assert(char.IsDigit(Buffer[s]));
            do tot = tot * 10 + (Buffer[s++] - '0'); while (s < Buffer.Length && char.IsDigit(Buffer[s]));
            if (s < Buffer.Length && (Buffer[s] == '.' || Buffer[s] == 'e' || Buffer[s] == 'E'))
            {
                tokType = type.tDou;
                Buffer.ParseDouble(ref pBuf, out tokD);
                tokD = tokD * sig;
            }
            else
            {
                pBuf = s;
                tokType = type.tInt;
                tokI = sig * tot;
            }
        }
        private void readDatTim()
        {
            pBuf += 4; //'Dat('
            tokType = type.tTim;
            tokT = Utils.NullT;
            int fp = Buffer.IndexOf(')', pBuf);
            if (fp < 0) Wanted(")");
            int jj = 0, mm = 0, aa = 0, hh = 0, mn = 0, ss = 0; // jj/mm/aaaa,hh:mm:ss
            int nok = 0;
            string[] comps = Buffer.Substring(pBuf, fp - pBuf).Split(',');
            if (comps.GetLength(0) > 0)
            {
                string[] jma = comps[0].Split('/');
                if (jma.GetLength(0) == 3
                    && int.TryParse(jma[0], out jj) && jj >= 1 && jj < 31
                    && int.TryParse(jma[1], out mm) && mm >= 1 && mm <= 12
                    && int.TryParse(jma[2], out aa))
                {
                    nok = 3;
                    if (comps.GetLength(0) > 1)
                    {
                        string[] hms = comps[1].Split(':');
                        if (hms.GetLength(0) == 3
                            && int.TryParse(hms[0], out hh) && hh >= 0 && hh < 24
                            && int.TryParse(hms[1], out mn) && mn >= 0 && mn < 60
                            && int.TryParse(hms[2], out ss) && ss >= 0 && ss < 60) nok = 6;
                    }
                }
            }
            if (nok == 3) tokT = new DateTime(aa, mm, jj);
            if (nok == 6) tokT = new DateTime(aa, mm, jj, hh, mn, ss);
            pBuf = fp + 1;
        }
    }

    public class InChannelString : InChannel
    {
        public InChannelString(string s)
        {
            if (s == null) s = "";
            Text = s;
            pRead = 0;
            pLast = 0;
            lineNum = 0;
        }
        public override void FillBuff()
        {
            int s = Math.Max(Text.IndexOf('\r', pRead), Text.IndexOf('\n', pRead));
            if (s >= 0) { ++s; if (s < Text.Length && Text[s - 1] == '\r' && Text[s] == '\n') ++s; }
            else s = Text.Length;
            Buffer = Text.Substring(pRead, s - pRead); pBuf = 0;
            pLast = pRead;
            pRead = s;
            ++lineNum;
        }
        public int OffsetCurLine() { return pLast; }
        public int NumCurLine() { return lineNum; }
        private int lineNum;
        private int pRead;
        private int pLast;
        private string Text;

        public static void Extract(out string prevLine, out String curLine, out int numLine, out int numCol, string script, int loc)
        {
            numLine = 0; numCol = 0;
            curLine = "";
            InChannelString cin = new InChannelString(script);
            for (; ; )
            {
                cin.FillBuff();
                prevLine = curLine;
                curLine = cin.Buffer;
                int sz = curLine.Length;
                if (sz == 0)
                {
                    numLine = cin.NumCurLine();
                    numCol = 1;
                    break;
                }
                if (curLine.EndsWith("\r\n")) curLine = curLine.Substring(0, sz - 2);
                int offset = cin.OffsetCurLine();
                if (offset + sz >= loc)
                {
                    numLine = cin.NumCurLine();
                    numCol = loc - offset + 1;
                    break;
                }
            }
            if (curLine.IsNull()) curLine = "<End of script>";
        }


    };

    public class InChannelFile : InChannel, IDisposable
    {
        public void Dispose() { Close(); }
        public InChannelFile(string filName)//throw
        {
            Name = filName;
            f = new StreamReader(filName, Utils.GetFileEncoding(filName));
            fName = filName;
            fclos = true;
            line = 0;
        }
        public InChannelFile(StreamReader s)
        {
            f = s;
            fclos = false;
            line = 0;
        }
        public override void FillBuff()
        {
            Buffer = ""; pBuf = 0;
            if (f == null) return;
            if (f.EndOfStream) { Close(); return; }
            Buffer = f.ReadLine();
            pBuf = 0;
            line++;
        }

        public int GetCountLinesRead() { return line; }

        StreamReader f;
        string fName;
        public void Close()
        {
            if (f != null)
            {
                if (fclos) f.Close();
                f = null;
            }
        }
        int line;
        bool fclos;
    }




}
   

