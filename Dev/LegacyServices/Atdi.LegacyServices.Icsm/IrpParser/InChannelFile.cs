using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Atdi.LegacyServices.Icsm
{
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
