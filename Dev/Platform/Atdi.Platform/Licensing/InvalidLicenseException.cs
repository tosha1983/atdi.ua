using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Licensing
{
    [Serializable]
    public class InvalidLicenseException : Exception
    {
        public InvalidLicenseException()
        {
        }

        public InvalidLicenseException(string message) : base(message)
        {
        }

        public InvalidLicenseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
