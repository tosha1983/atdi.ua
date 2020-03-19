using Atdi.Contracts.WcfServices.Sdrn.Server;
using System;
using System.ServiceModel;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using DM = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;

namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SdrnsControllerIeStation : WcfServiceBase<ISdrnsControllerIeStation>, ISdrnsControllerIeStation
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;

        public SdrnsControllerIeStation(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, IPipelineSite pipelineSite, ILogger logger)
        {
            this._eventEmitter = eventEmitter;
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._pipelineSite = pipelineSite;
        }


        /// <summary>
        /// Import RefSpectrum into DB
        /// </summary>
        /// <param name="refSpectrum"></param>
        /// <returns></returns>

        public long? ImportRefSpectrum(RefSpectrum refSpectrum)
        {
            var importRefSpectrum = new ImportRefSpectrumData(_dataLayer, _logger);
            return importRefSpectrum.ImportSpectrum(refSpectrum);
        }


        /// <summary>
        /// Get all RefSpectrum
        /// </summary>
        /// <returns></returns>
        public RefSpectrum[] GetAllRefSpectrum()
        {
            var utils = new Utils(_dataLayer, _logger);
            return utils.GetAllRefSpectrum();
        }

        /// <summary>
        /// Data all DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        public DataSynchronizationProcess CurrentDataSynchronizationProcess()
        {
            var utils = new Utils(_dataLayer, _logger);
            return utils.CurrentDataSynchronizationProcess();
        }

        public bool DeleteRefSpectrum(long[] RefSpectrumIdsBySDRN)
        {
            var importRefSpectrum = new ImportRefSpectrumData(_dataLayer, _logger);
            return importRefSpectrum.DeleteRefSpectrum(RefSpectrumIdsBySDRN);
        }


        /// <summary>
        /// Run DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        public bool RunDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] RefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended)
        {
            var runSynchroProcess = new RunSynchroProcess(_dataLayer, _logger);
            return runSynchroProcess.RunDataSynchronizationProcess(dataSynchronization, RefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended);
        }

        /// <summary>
        /// Get Protocols by parameters
        /// </summary>
        /// <param name="createdBy"> DataSynchronizationBase.CreatedBy</param>
        /// <param name="DateCreated">DataSynchronizationBase.CreatedBy</param>
        /// <param name="DateMeas">Protocols.DateMeas</param>
        /// <param name="DateStart">Protocols.DateMeas</param>
        /// <param name="DateStop">Protocols.DateMeas</param>
        /// <param name="freq">Protocols.Freq_Mhz</param>
        /// <param name="probability">ProtocolsWithEmittings.probability</param>
        /// <param name="standard">StationExtended.standard</param>
        /// <param name="province">StationExtended.Province</param>
        /// <param name="ownerName">StationExtended.OwnerName</param>
        /// <param name="permissionNumber">IProtocols.permissionNumber</param>
        /// <param name="permissionStart">IProtocols.permissionStart</param>
        /// <param name="permissionStop">IProtocols.PermissionStop</param>
        /// <returns></returns>
        public Protocols[] GetProtocolsByParameters(long? processId,
                                                    string createdBy,
                                                    DateTime? DateCreated,
                                                    DateTime? DateStart,
                                                    DateTime? DateStop,
                                                    short? DateMeasDay,
                                                    short? DateMeasMonth,
                                                    short? DateMeasYear,
                                                    double? freq,
                                                    double? probability,
                                                    string standard,
                                                    string province,
                                                    string ownerName,
                                                    string permissionNumber,
                                                    DateTime? permissionStart,
                                                    DateTime? permissionStop)
        {
            var loadProtocols = new LoadProtocols(_dataLayer, _logger);
            return loadProtocols.GetProtocolsByParameters(processId, createdBy,
                                                    DateCreated,
                                                    DateStart,
                                                    DateStop,
                                                    DateMeasDay,
                                                    DateMeasMonth,
                                                    DateMeasYear,
                                                    freq,
                                                    probability,
                                                    standard,
                                                    province,
                                                    ownerName,
                                                    permissionNumber,
                                                    permissionStart,
                                                    permissionStop);
        }

        /// <summary>
        /// Get Protocols by parameters
        /// </summary>
        /// <param name="createdBy"> DataSynchronizationBase.CreatedBy</param>
        /// <param name="DateCreated">DataSynchronizationBase.CreatedBy</param>
        /// <param name="DateMeas">Protocols.DateMeas</param>
        /// <param name="DateStart">Protocols.DateMeas</param>
        /// <param name="DateStop">Protocols.DateMeas</param>
        /// <param name="freq">Protocols.Freq_Mhz</param>
        /// <param name="probability">ProtocolsWithEmittings.probability</param>
        /// <param name="standard">StationExtended.standard</param>
        /// <param name="province">StationExtended.Province</param>
        /// <param name="ownerName">StationExtended.OwnerName</param>
        /// <param name="permissionNumber">IProtocols.permissionNumber</param>
        /// <param name="permissionStart">IProtocols.permissionStart</param>
        /// <param name="permissionStop">IProtocols.PermissionStop</param>
        /// <returns></returns>
        public HeadProtocols[] GetDetailProtocolsByParameters(long? processId,
                                                    string createdBy,
                                                    DateTime? DateCreated,
                                                    DateTime? DateStart,
                                                    DateTime? DateStop,
                                                    short? DateMeasDay,
                                                    short? DateMeasMonth,
                                                    short? DateMeasYear,
                                                    double? freq,
                                                    double? probability,
                                                    string standard,
                                                    string province,
                                                    string ownerName,
                                                    string permissionNumber,
                                                    DateTime? permissionStart,
                                                    DateTime? permissionStop)
        {
            var loadProtocols = new LoadProtocols(_dataLayer, _logger);
            return loadProtocols.GetDetailProtocolsByParameters(processId, createdBy,
                                                    DateCreated,
                                                    DateStart,
                                                    DateStop,
                                                    DateMeasDay,
                                                    DateMeasMonth,
                                                    DateMeasYear,
                                                    freq,
                                                    probability,
                                                    standard,
                                                    province,
                                                    ownerName,
                                                    permissionNumber,
                                                    permissionStart,
                                                    permissionStop);
        }

        public DataSynchronizationProcess[] GetAllDataSynchronizationProcess()
        {
            var utils = new Utils(_dataLayer, _logger);
            return utils.GetAllDataSynchronizationProcess();
        }
    }
}


