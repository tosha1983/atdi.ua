using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Atdi.DataModels.Sdrns.Device;
using Castle.Windsor;
using RabbitMQ.Client;
using Atdi.Modules.Sdrn.MessageBus;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class SaveMeasSdrResults
    {
        public void Handle(Message messageResponse, ILogger logger)
        {
            MessageObject dataU = Concumer.UnPackObject(messageResponse);
            MeasSdrResults measSdrResults = dataU.Object as MeasSdrResults;
            ClassesDBGetResult bbGetRes = new ClassesDBGetResult(logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(logger);
            {
                if (measSdrResults != null)
                {
                    int? ID = -1;
                    string Status_Original = measSdrResults.status;
                    MeasurementResults msReslts = ClassConvertToSDRResults.GenerateMeasResults(measSdrResults);
                    if (msReslts.TypeMeasurements == MeasurementType.SpectrumOccupation) msReslts.Status = Status_Original;
                    if (msReslts.MeasurementsResults != null)
                    {
                        if (msReslts.MeasurementsResults.Count() > 0)
                        {
                            if (msReslts.MeasurementsResults is LevelMeasurementOnlineResult)
                            {
                                msReslts.Status = "O";
                                logger.Trace(string.Format("Start save online results..."));
                                ID = bbGetRes.SaveResultToDB(msReslts);
                                if (ID > 0)
                                {
                                    logger.Trace(string.Format("Success save online results..."));
                                }
                            }
                            else
                            {
                                logger.Trace(string.Format("Start save results..."));
                                ID = bbGetRes.SaveResultToDB(msReslts);
                                if (ID > 0)
                                {
                                    logger.Trace(string.Format("Success save results..."));
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Trace(string.Format("Start save results..."));
                        ID = bbGetRes.SaveResultToDB(msReslts);
                        if (ID > 0)
                        {
                            logger.Trace(string.Format("Success save results..."));
                        }
                    }
                }
                bbGetRes.Dispose();
                conv.Dispose();
            }
        }
    }
}

