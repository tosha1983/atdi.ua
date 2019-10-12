using Atdi.Contracts.WcfServices.Sdrn.Device;
using System;
using System.Collections.Generic;

namespace Atdi.WcfServices.Sdrn.Device
{
    internal class ServiceEnvironment : IServiceEnvironment
    {
        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly Modules.Licensing.VerificationResult _licensing;

        public ServiceEnvironment(SdrnServerDescriptor serverDescriptor, Modules.Licensing.VerificationResult licensing)
        {
            this._serverDescriptor = serverDescriptor;
            this._licensing = licensing;
        }

        public string Instance => _serverDescriptor.SensorName;

        public string SdrnServerInstance => _serverDescriptor.SdrnServer;

        public string LicenseNumber => _licensing.LicenseNumber;

        public DateTime LicenseStopDate => _licensing.StopDate;

        public DateTime LicenseStartDate => _licensing.StartDate;

        public IDictionary<string, string> AllowedSensors => _serverDescriptor.AllowedSensors;
    }
}
