using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Licensing
{
    class LicensingManager : ILicensingManager
    {
        public byte[] EnsureToken(string licenseKey)
        {
            throw new NotImplementedException();
        }

        public void RegisterVerifier(Type type)
        {
            throw new NotImplementedException();
        }

        public IVerificationResult Verify(string licenseKey)
        {
            throw new NotImplementedException();
        }
    }

    //class ProtectedLibLicenseVerifier : ILicenseVerifier
    //{
    //    private class VerificationResult : IVerificationResult
    //    {
    //        public byte[] Token { get; set; }

    //        public string Key { get; set; }

    //        public DateTime? Lifetime { get; set; }
    //    }

    //    [DllImport("ProtectedLib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "checkLicense")]
    //    public static extern byte[] CheckLicense(byte[] licenseBody);

        


    //    private readonly IConfig _config;

    //    public ProtectedLibLicenseVerifier(IConfig config)
    //    {
    //        this._config = config;
    //    }

    //    public IVerificationResult Verify()
    //    {
    //        var fileName = _config.FileName;
    //        var licenseBody =  File.ReadAllBytes(fielName);

            
    //        var token = CheckLicense(licenseBody);
    //        if (token == null)
    //        {
    //            throw new InvalidLicenseException();
    //        }

    //        var result = new VerificationResult
    //        {
    //            Key = "ProtectedLib.dll",
    //            Lifetime = DateTime.Now.AddHours(1),
    //            Token = token
    //        };

    //        return result;
    //    }

    //    [DllImport("ProtectedLib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "workMethod1")]
    //    public static extern byte[] WorkMethod1(byte[] securetyToken, int param1, float[] param2, double[][] param3);

    //    [DllImport("ProtectedLib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "workMethod2")]
    //    public static extern byte[] WorkMethod2(byte[] securetyToken, int param1, float[] param2, double[][] param3);

    //    [DllImport("ProtectedLib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "workMethod3")]
    //    public static extern byte[] WorkMethod3(byte[] securetyToken, int param1, float[] param2, double[][] param3);

    //    private void SomeClientMethod()
    //    {
    //        ILicensingManager licensingManager = container.Resolve<ILicensingManager>();
    //        const string protecteLibLicenseKey = "ProtectedLib.dll";

    //        try
    //        {
    //            var token = licensingManager.EnsureToken(protecteLibLicenseKey);

    //            // now, we can invoke WorkMethod to ProtectedLib.dll
    //            WorkMethod1(token, 1, new float[] { }, null);

    //            //...
    //            WorkMethod2(token, 2, new float[] { }, null);

    //            // ...
    //            WorkMethod3(token, 3, new float[] { }, null);
    //        }
    //        catch (InvalidLicenseException)
    //        {
    //            // ... to do 
    //            throw;
    //        }
    //        catch (LicenseTokenExpiredException)
    //        {
    //            // ... to do 
    //            throw;
    //        }

    //    }
    //}


    

    
}
