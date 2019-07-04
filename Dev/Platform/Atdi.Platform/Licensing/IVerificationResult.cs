using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Licensing
{
    /// <summary>
    /// The object of the license check result. 
    /// Contains a secured token that is required for use when accessing licensed objects or methods.
    /// </summary>
    public interface IVerificationResult
    {
        /// <summary>
        /// Secure license token
        /// </summary>
        byte[] Token { get; }

        /// <summary>
        /// License key
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The lifetime of the token after which it is necessary to request it again by checking the license
        /// </summary>
        DateTime? Lifetime { get; }
    }

   
}