using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.DataConstraint;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// The public contract of the service of the controller of the SDR Networks 
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface ISdrnsControllerIeStation
    {

        /// <summary>
        /// Import RefSpectrum into DB SDRN
        /// </summary>
        /// <param name="refSpectrum"></param>
        /// <returns></returns>
        [OperationContract]
        long? ImportRefSpectrum(RefSpectrum refSpectrum);

        /// <summary>
        /// Delete RefSpectrum from DB SDRN
        /// </summary>
        /// <param name="refSpectrum"></param>
        /// <returns></returns>
        [OperationContract]
        bool DeleteRefSpectrum(long[] RefSpectrumIdsBySDRN);

        /// <summary>
        /// Get all DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataSynchronizationProcess[] GetAllDataSynchronizationProcess();

        /// <summary>
        /// Get all RefSpectrum
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        RefSpectrum[] GetAllRefSpectrum();

        /// <summary>
        /// Data all DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataSynchronizationProcess CurrentDataSynchronizationProcess();


        /// <summary>
        /// Run DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool RunDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] RefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended);


        /// <summary>
        /// Get Protocols by parameters
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Protocols[] GetProtocolsByParameters(       long? processId,
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
                                                    DateTime? permissionStop);
        /// <summary>
        /// Get Protocols by parameters
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        HeadProtocols[] GetDetailProtocolsByParameters(long? processId,
                                                    string createdBy,
                                                    DateTime? DateCreated,
                                                    DateTime? DateStart,
                                                    DateTime? DateStop,
                                                    short? DateMeasDay,
                                                    short? DateMeasMonth,
                                                    short? DateMeasYear,
                                                    double? freqStart,
                                                    double? freqStop,
                                                    double? probability,
                                                    string standard,
                                                    string province,
                                                    string ownerName,
                                                    string permissionNumber,
                                                    DateTime? permissionStart,
                                                    DateTime? permissionStop,
                                                    string statusMeas);

    }
}
