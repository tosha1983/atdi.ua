using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
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


    }
}
