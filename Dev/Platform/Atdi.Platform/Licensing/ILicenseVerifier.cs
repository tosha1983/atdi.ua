using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Licensing
{
    /// <summary>
    /// The license verification service
    /// </summary>
    public interface ILicenseVerifier
    {
        /// <summary>
        /// License verification method.
        /// In cases where the license check fails, an exception will be thrown:
        ///  - InvalidLicenseException
        /// </summary>
        /// <returns></returns>
        IVerificationResult Verify();
    }
}
