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
    public class OnSendSOMeasResultsHandler : IMessageHandler<SendMeasResultSOToMasterServer, MeasResultContainer>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        public OnSendSOMeasResultsHandler(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }
        public void Handle(IIncomingEnvelope<SendMeasResultSOToMasterServer, MeasResultContainer> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnSendSOMeasResultsHandler, this))
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
                        if (measResult.FrequencySamples != null)
                        {
                            foreach (var freqSample in measResult.FrequencySamples)
                            {
                                var builderInsertResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().Insert();
                                builderInsertResLevels.SetValue(c => c.VMMaxLvl, freqSample.LevelMax_dBm);
                                builderInsertResLevels.SetValue(c => c.VMinLvl, freqSample.LevelMin_dBm);
                                builderInsertResLevels.SetValue(c => c.ValueLvl, freqSample.Level_dBm);
                                builderInsertResLevels.SetValue(c => c.ValueSpect, freqSample.Level_dBmkVm);
                                builderInsertResLevels.SetValue(c => c.OccupancySpect, freqSample.Occupation_Pt);
                                builderInsertResLevels.SetValue(c => c.FreqMeas, freqSample.Freq_MHz);
                                builderInsertResLevels.SetValue(c => c.RES_MEAS.Id, resMeas.Id);
                                scope.Executor.Execute(builderInsertResLevels);
                            }
                        }
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, resMeas.Id);
                        scope.Executor.Execute(builderInsertResLocSensorMeas);
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
