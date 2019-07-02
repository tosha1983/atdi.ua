using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILicensingManager
    {
        /// <summary>
        /// Registration method of license verifier type
        /// </summary>
        /// <param name="type">The type of license verifier implementaion</param>
        void RegisterVerifier(Type type);

        /// <summary>
        /// License verification method.
        /// In cases where the license check fails, an exception will be thrown:
        ///  - InvalidLicenseException
        /// </summary>
        /// <param name="licenseKey">The license key to check</param>
        /// <returns>The verification result object</returns>
        IVerificationResult Verify(string licenseKey);

        /// <summary>
        /// The method ensures a protected token license, if the token is expired, an attempt will be made to get it again. 
        /// In cases where the license check fails, an exception will be thrown:
        ///  - InvalidLicenseException
        /// </summary>
        /// <param name="licenseKey">The license key to get a secure token.</param>
        /// <returns>The secure token</returns>
        byte[] EnsureToken(string licenseKey);
    }
}
