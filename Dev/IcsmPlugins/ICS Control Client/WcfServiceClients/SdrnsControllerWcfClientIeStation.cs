using System;
using System.Collections.Generic;
using ICSM;
using XICSM.ICSControlClient.Environment;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;

namespace XICSM.ICSControlClient.WcfServiceClients
{
    public class SdrnsControllerWcfClientIeStation : WcfServiceClientBase<ISdrnsControllerIeStation, SdrnsControllerWcfClientIeStation>
    {
        public SdrnsControllerWcfClientIeStation() : base("SdrnsController") { }

        public static long? ImportRefSpectrum(RefSpectrum refSpectrum)
        {
            return Execute(contract => contract.ImportRefSpectrum(refSpectrum));
        }
        public static bool DeleteRefSpectrum(long[] RefSpectrumIdsBySDRN)
        {
            return Execute(contract => contract.DeleteRefSpectrum(RefSpectrumIdsBySDRN));
        }
        public static RefSpectrum[] GetAllRefSpectrum()
        {
            return Execute(contract => contract.GetAllRefSpectrum());
        }
        public static DataSynchronizationProcess CurrentDataSynchronizationProcess()
        {
            return Execute(contract => contract.CurrentDataSynchronizationProcess());
        }
        public static bool RunDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] RefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended)
        {
            return Execute(contract => contract.RunDataSynchronizationProcess(dataSynchronization, RefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended));
        }
        public static DataSynchronizationProcess[] GetAllDataSynchronizationProcess()
        {
            return Execute(contract => contract.GetAllDataSynchronizationProcess());
        }
        public static Protocols[] GetProtocols()
        {
            return GetProtocolsByParameters(null, "", null, null, null, null, null, null, null, null, "", "", "", "", null, null);
        }
        public static Protocols[] GetProtocolsByParameters(long? processId,
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
            return Execute(contract => contract.GetProtocolsByParameters(processId, "", DateCreated, DateStart, DateStop, DateMeasDay, DateMeasMonth, DateMeasYear, freq, probability, standard, province, ownerName, permissionNumber, permissionStart, permissionStop));
        }
        public static HeadProtocols[] GetDetailProtocolsByParameters(long? processId,
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
            return Execute(contract => contract.GetDetailProtocolsByParameters(processId, "", DateCreated, DateStart, DateStop, DateMeasDay, DateMeasMonth, DateMeasYear, freq, probability, standard, province, ownerName, permissionNumber, permissionStart, permissionStop));
        }
    }
}
