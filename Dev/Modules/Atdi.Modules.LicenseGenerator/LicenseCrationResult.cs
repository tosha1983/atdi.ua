using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.LicenseGenerator
{
    public class LicenseCrationResult
    {
        public string IvAsBase64;
        public int KeySize;
        public byte[] Body;
    }
}
