using Atdi.Contracts.WcfServices.Sdrn.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Device
{
    class ServiceEnvironment : IServiceEnvironment
    {
        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly Modules.Licensing.VerificationResult _licensing;

        public ServiceEnvironment(SdrnServerDescriptor serverDescriptor, Modules.Licensing.VerificationResult licensing)
        {
            this._serverDescriptor = serverDescriptor;
            this._licensing = licensing;
        }

        public string Instance => _serverDescriptor.Instance;

        public string SdrnServerInstance => _serverDescriptor.ServerInstance;

        public string LicenseNumber => _licensing.LicenseNumber;

        public DateTime LicenseStopDate => _licensing.StopDate;

        public DateTime LicenseStartDate => _licensing.StartDate;

        public IDictionary<string, string> AllowedSensors => _serverDescriptor.AllowedSensors;
    }
}
