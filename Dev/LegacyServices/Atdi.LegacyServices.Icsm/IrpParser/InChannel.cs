using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Atdi.LegacyServices.Icsm
{
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

            //if (nlines < 0 || nchtot < 0) throw new Exception("Corrupted IMMEDIATETEXT");
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
                //if (buff.Length + nf + lr > buff.Capacity) throw new Exception("Corrupted ImmText");
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
            
        }

        private bool unread;
        private void readString(char sep)
        {
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
}
