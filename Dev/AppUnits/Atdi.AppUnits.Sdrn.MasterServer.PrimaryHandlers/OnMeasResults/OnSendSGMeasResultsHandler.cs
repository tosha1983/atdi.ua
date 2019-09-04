using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.Sdrn.Server;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers
{
    public class OnSendSGMeasResultsHandler : IMessageHandler<SendMeasResultSGToMasterServer, MeasResultContainer>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        public OnSendSGMeasResultsHandler(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }
        public void Handle(IIncomingEnvelope<SendMeasResultSGToMasterServer, MeasResultContainer> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnSendSGMeasResultsHandler, this))
            {
                var deliveryObject = envelope.DeliveryObject;
                SaveMeasResult(deliveryObject.MeasResult);
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
        private bool SaveMeasResult(MeasResults measResult)
        {
            try
            {
                if (measResult == null)
                    return true;

                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                    builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                    builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                    builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                    builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                    builderInsertIResMeas.SetValue(c => c.SUBTASK_SENSOR.Id, long.TryParse(measResult.TaskId, out long subMeasTaskSensorId) ? subMeasTaskSensorId : 0);
                    var resMeas = scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                    if (resMeas.Id > 0)
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, resMeas.Id);
                        scope.Executor.Execute(builderInsertResLocSensorMeas);

                        if (measResult.RefLevels != null)
                        {
                            var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                            builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, measResult.RefLevels.StartFrequency_Hz);
                            builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, measResult.RefLevels.StepFrequency_Hz);
                            builderInsertReferenceLevels.SetValue(c => c.RefLevels, measResult.RefLevels.levels);
                            builderInsertReferenceLevels.SetValue(c => c.RES_MEAS.Id, resMeas.Id);
                            scope.Executor.Execute<MD.IReferenceLevels_PK>(builderInsertReferenceLevels);
                        }

                        if (measResult.Emittings != null)
                        {
                            foreach (Emitting emitting in measResult.Emittings)
                            {
                                var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                                builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emitting.CurentPower_dBm);
                                builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emitting.MeanDeviationFromReference);
                                builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emitting.ReferenceLevel_dBm);
                                builderInsertEmitting.SetValue(c => c.TriggerDeviationFromReference, emitting.TriggerDeviationFromReference);
                                builderInsertEmitting.SetValue(c => c.RES_MEAS.Id, resMeas.Id);
                                builderInsertEmitting.SetValue(c => c.SensorId, emitting.SensorId);
                                if (emitting.EmittingParameters != null)
                                {
                                    builderInsertEmitting.SetValue(c => c.RollOffFactor, emitting.EmittingParameters.RollOffFactor);
                                    builderInsertEmitting.SetValue(c => c.StandardBW, emitting.EmittingParameters.StandardBW);
                                }
                                builderInsertEmitting.SetValue(c => c.StartFrequency_MHz, emitting.StartFrequency_MHz);
                                builderInsertEmitting.SetValue(c => c.StopFrequency_MHz, emitting.StopFrequency_MHz);

                                if (emitting.LevelsDistribution != null)
                                {
                                    builderInsertEmitting.SetValue(c => c.LevelsDistributionCount, emitting.LevelsDistribution.Count);
                                    builderInsertEmitting.SetValue(c => c.LevelsDistributionLvl, emitting.LevelsDistribution.Levels);
                                }

                                if (emitting.SignalMask != null)
                                {
                                    builderInsertEmitting.SetValue(c => c.Loss_dB, emitting.SignalMask.Loss_dB);
                                    builderInsertEmitting.SetValue(c => c.Freq_kHz, emitting.SignalMask.Freq_kHz);
                                }

                                var valInsReferenceEmitting = scope.Executor.Execute<MD.IEmitting_PK>(builderInsertEmitting);
                                if (valInsReferenceEmitting.Id > 0)
                                {
                                    if (emitting.WorkTimes != null)
                                    {
                                        foreach (WorkTime workTime in emitting.WorkTimes)
                                        {
                                            var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Insert();
                                            builderInsertIWorkTime.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                            builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                            builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                            builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                            builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                            scope.Executor.Execute(builderInsertIWorkTime);
                                        }
                                    }

                                    if (emitting.Spectrum != null)
                                    {
                                        var builderInsertISpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Insert();
                                        builderInsertISpectrum.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                        builderInsertISpectrum.SetValue(c => c.CorrectnessEstimations, emitting.Spectrum.СorrectnessEstimations == true ? 1 : 0);
                                        builderInsertISpectrum.SetValue(c => c.Contravention, emitting.Spectrum.Contravention == true ? 1 : 0);
                                        builderInsertISpectrum.SetValue(c => c.Bandwidth_kHz, emitting.Spectrum.Bandwidth_kHz);
                                        builderInsertISpectrum.SetValue(c => c.MarkerIndex, emitting.Spectrum.MarkerIndex);
                                        builderInsertISpectrum.SetValue(c => c.SignalLevel_dBm, emitting.Spectrum.SignalLevel_dBm);
                                        builderInsertISpectrum.SetValue(c => c.SpectrumStartFreq_MHz, emitting.Spectrum.SpectrumStartFreq_MHz);
                                        builderInsertISpectrum.SetValue(c => c.SpectrumSteps_kHz, emitting.Spectrum.SpectrumSteps_kHz);
                                        builderInsertISpectrum.SetValue(c => c.T1, emitting.Spectrum.T1);
                                        builderInsertISpectrum.SetValue(c => c.T2, emitting.Spectrum.T2);
                                        builderInsertISpectrum.SetValue(c => c.TraceCount, emitting.Spectrum.TraceCount);
                                        builderInsertISpectrum.SetValue(c => c.Levels_dBm, emitting.Spectrum.Levels_dBm);
                                        scope.Executor.Execute<MD.ISpectrum_PK>(builderInsertISpectrum);
                                    }
                                }

                                if (emitting.SysInfos != null)
                                {
                                    foreach (SignalingSysInfo sysInfo in emitting.SysInfos)
                                    {
                                        var builderInsertSysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().Insert();
                                        builderInsertSysInfo.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                        builderInsertSysInfo.SetValue(c => c.BandWidth_Hz, sysInfo.BandWidth_Hz);
                                        builderInsertSysInfo.SetValue(c => c.BSIC, sysInfo.BSIC);
                                        builderInsertSysInfo.SetValue(c => c.ChannelNumber, sysInfo.ChannelNumber);
                                        builderInsertSysInfo.SetValue(c => c.CID, sysInfo.CID);
                                        builderInsertSysInfo.SetValue(c => c.CtoI, sysInfo.CtoI);
                                        builderInsertSysInfo.SetValue(c => c.Freq_Hz, sysInfo.Freq_Hz);
                                        builderInsertSysInfo.SetValue(c => c.LAC, sysInfo.LAC);
                                        builderInsertSysInfo.SetValue(c => c.Level_dBm, sysInfo.Level_dBm);
                                        builderInsertSysInfo.SetValue(c => c.MCC, sysInfo.MCC);
                                        builderInsertSysInfo.SetValue(c => c.MNC, sysInfo.MNC);
                                        builderInsertSysInfo.SetValue(c => c.Power, sysInfo.Power);
                                        builderInsertSysInfo.SetValue(c => c.RNC, sysInfo.RNC);
                                        builderInsertSysInfo.SetValue(c => c.Standard, sysInfo.Standard);
                                        var valInsSysInfo = scope.Executor.Execute<MD.ISignalingSysInfo_PK>(builderInsertSysInfo);
                                        if (valInsSysInfo.Id > 0 && sysInfo.WorkTimes != null)
                                        {
                                            foreach (WorkTime workTime in sysInfo.WorkTimes)
                                            {
                                                var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().Insert();
                                                builderInsertIWorkTime.SetValue(c => c.SYSINFO.Id, valInsSysInfo.Id);
                                                builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                                builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                                builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                                builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                                scope.Executor.Execute(builderInsertIWorkTime);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }
    }
}
