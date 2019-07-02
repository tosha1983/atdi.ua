using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Licensing
{
    [Serializable]
    public class LicenseTokenExpiredException : Exception
    {
        public LicenseTokenExpiredException()
        {
        }

        public LicenseTokenExpiredException(string message) : base(message)
        {
        }

        public LicenseTokenExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LicenseTokenExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
