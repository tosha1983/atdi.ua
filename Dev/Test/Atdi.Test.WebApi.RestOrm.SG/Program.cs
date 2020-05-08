using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrns.Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.Test.WebApi.RestOrm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            var endpoint = new WebApiEndpoint(new Uri("http://localhost:15020/"), "/appserver/v1");
            var dataContext = new WebApiDataContext("SDRN_Server_DB");

            var dataLayer = new WebApiDataLayer();
            var executor = dataLayer.GetExecutor(endpoint, dataContext);

            TestReadMethod(executor, dataLayer);

        }


        static void TestReadMethod(IQueryExecutor executor, WebApiDataLayer dataLayer)
        {
            var dateMin = DateTime.Now.AddDays(-30);
            var dateMax = DateTime.Now;
            var typeMeasurements = "SpectrumOccupation";
            var typeSpectrumOccupation = "FreqBandwidthOccupation";
            var channelFrequencyMin_MHz = 700;
            var channelFrequencyMax_MHz = 2400;
            var sensors = new long[4] { 17, 2, 13, 3};



            var lstMeasTask = new List<MeasTask>();
            var lstMeasTaskIds = new List<long>();


            var lstSensors = new List<Sensor>();
            var lstSensorsIds = new List<long>();

            var listSensors = BreakDownElemBlocks.BreakDown(sensors);
            for (int i = 0; i < listSensors.Count; i++)
            {
                var webQueryResLevels = dataLayer.GetBuilder<IResLevels>()
                .Read()
                .Select(
                   c => c.Id,
                   c => c.FreqMeas,
                   c => c.ValueSpect,
                   c => c.VMinLvl,
                   c => c.VMMaxLvl,
                   c => c.ValueLvl,
                   c => c.RES_MEAS.TimeMeas,
                   c => c.RES_MEAS.ScansNumber,
                   c => c.RES_MEAS.TypeMeasurements,
                   c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id,
                   c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Name,
                   c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id,
                   c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name,
                   c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId,
                   c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Title
                )
                .OrderByAsc(c => c.Id)
                .BeginFilter()
                    .Condition(c => c.RES_MEAS.TimeMeas, FilterOperator.LessEqual, dateMax)
                    .And()
                    .Condition(c => c.RES_MEAS.TimeMeas, FilterOperator.GreaterEqual, dateMin)
                    .And()
                    .Condition(c => c.RES_MEAS.TypeMeasurements, FilterOperator.Equal, typeMeasurements)
                    .And()
                    .Condition(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id, FilterOperator.In, listSensors[i].ToArray())
                     .And()
                    .Condition(c => c.FreqMeas, FilterOperator.Between, channelFrequencyMin_MHz, channelFrequencyMax_MHz)
                .EndFilter()
                .OnTop(100);

                var countResLevels = executor.Execute(webQueryResLevels);
                var recordsResLevels = executor.ExecuteAndFetch(webQueryResLevels, readerResLevels =>
                {
                    while (readerResLevels.Read())
                    {
                        if (!lstMeasTaskIds.Contains(readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id)))
                        {
                            lstMeasTaskIds.Add(readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id));


                            var measTask = new MeasTaskSpectrumOccupation();
                            measTask.Id = new MeasTaskIdentifier();
                            measTask.Id.Value = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            measTask.Name = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Name);


                            MeasurementType typeMeas;
                            if (Enum.TryParse<MeasurementType>(readerResLevels.GetValue(c => c.RES_MEAS.TypeMeasurements), out typeMeas))
                            {
                                measTask.TypeMeasurements = typeMeas;
                            }

                            var webQueryMeasOther = dataLayer.GetBuilder<IMeasOther>()
                            .Read()
                            .Select(
                            c => c.Id,
                            c => c.LevelMinOccup,
                            c => c.TypeSpectrumOccupation,
                            c => c.Nchenal
                            )
                            .OrderByAsc(c => c.Id)
                            .BeginFilter()
                                .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id))
                                .And()
                                .Condition(c => c.TypeSpectrumOccupation, FilterOperator.Equal, typeSpectrumOccupation)
                            .EndFilter()
                            .OnTop(1);

                            var countMeasOthers = executor.Execute(webQueryMeasOther);
                            if (countMeasOthers > 0)
                            {
                                measTask.SpectrumOccupationParameters = new SpectrumOccupationParameters();
                                var recordsMeasOther = executor.ExecuteAndFetch(webQueryMeasOther, readerMeasOther =>
                                 {
                                     while (readerMeasOther.Read())
                                     {
                                         SpectrumOccupationType spectrumOccupationType;
                                         if (Enum.TryParse<SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out spectrumOccupationType))
                                         {
                                             measTask.SpectrumOccupationParameters.TypeSpectrumOccupation = spectrumOccupationType;
                                         }
                                         measTask.SpectrumOccupationParameters.NChenal = readerMeasOther.GetValue(c => c.Nchenal);
                                         measTask.SpectrumOccupationParameters.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                                         break;
                                     }
                                     return true;
                                 });


                                var webQueryMeasFreqParam = dataLayer.GetBuilder<IMeasFreqParam>()
                               .Read()
                               .Select(
                               c => c.Id,
                               c => c.Mode,
                               c => c.Rgl,
                               c => c.Rgu,
                               c => c.Step
                               )
                               .OrderByAsc(c => c.Id)
                               .BeginFilter()
                                   .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id))
                               .EndFilter()
                               .OnTop(1);


                                var countMeasFreqParam = executor.Execute(webQueryMeasFreqParam);
                                var recordsMeasFreqParam = executor.ExecuteAndFetch(webQueryMeasFreqParam, readerMeasFreqParam =>
                                {
                                    measTask.MeasFreqParam = new MeasFreqParam();
                                    while (readerMeasFreqParam.Read())
                                    {
                                        measTask.MeasFreqParam.Step = readerMeasFreqParam.GetValue(c => c.Step);
                                        measTask.MeasFreqParam.RgU = readerMeasFreqParam.GetValue(c => c.Rgu);
                                        measTask.MeasFreqParam.RgL = readerMeasFreqParam.GetValue(c => c.Rgl);
                                        FrequencyMode frequencyMode;
                                        if (Enum.TryParse<FrequencyMode>(readerMeasFreqParam.GetValue(c => c.Mode), out frequencyMode))
                                        {
                                            measTask.MeasFreqParam.Mode = frequencyMode;
                                        }
                                        break;
                                    }
                                    return true;
                                });


                                var webQueryMeasDtParam = dataLayer.GetBuilder<IMeasDtParam>()
                           .Read()
                           .Select(
                           c => c.Id,
                           c => c.Mode,
                           c => c.Demod,
                           c => c.DetectType,
                           c => c.Ifattenuation,
                           c => c.MeasTime,
                           c => c.NumberTotalScan,
                           c => c.Preamplification,
                           c => c.Rbw,
                           c => c.ReferenceLevel,
                           c => c.Rfattenuation,
                           c => c.SwNumber,
                           c => c.Vbw
                           )
                           .OrderByAsc(c => c.Id)
                           .BeginFilter()
                               .Condition(c => c.MEAS_TASK.Id, FilterOperator.Equal, readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id))
                           .EndFilter()
                           .OnTop(1);

                                var countMeasDtParam = executor.Execute(webQueryMeasDtParam);
                                var recordsMeasDtParam = executor.ExecuteAndFetch(webQueryMeasDtParam, readerMeasDtParam =>
                                {
                                    measTask.MeasDtParam = new MeasDtParam();
                                    while (readerMeasDtParam.Read())
                                    {
                                        measTask.MeasDtParam.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                                        MeasurementMode measurementMode;
                                        if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out measurementMode))
                                        {
                                            measTask.MeasDtParam.Mode = measurementMode;
                                        }
                                        DetectingType detectingType;
                                        if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectingType))
                                        {
                                            measTask.MeasDtParam.DetectType = detectingType;
                                        }
                                        measTask.MeasDtParam.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation);
                                        measTask.MeasDtParam.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                                        measTask.MeasDtParam.NumberTotalScan = readerMeasDtParam.GetValue(c => c.NumberTotalScan);
                                        measTask.MeasDtParam.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification);
                                        measTask.MeasDtParam.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                                        measTask.MeasDtParam.ReferenceLevel = readerMeasDtParam.GetValue(c => c.ReferenceLevel);
                                        measTask.MeasDtParam.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation);
                                        measTask.MeasDtParam.SwNumber = readerMeasDtParam.GetValue(c => c.SwNumber);
                                        measTask.MeasDtParam.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                                        break;
                                    }
                                    return true;
                                });

                                if (!lstSensorsIds.Contains(readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id)))
                                {
                                    lstSensorsIds.Add(readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id));

                                    var sensor = new Sensor();
                                    sensor.Id = new SensorIdentifier();
                                    sensor.Id.Value = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id); 
                                    sensor.Name = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                                    sensor.Equipment = new SensorEquip();
                                    sensor.Equipment.TechId = readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);


                                    var sensorLocation = new SensorLocation();
                                    var webQuerySensorLocation = dataLayer.GetBuilder<ISensorLocation>()
                                 .Read()
                                 .Select(
                                 c => c.Id,
                                 c => c.Lon,
                                 c => c.Lat,
                                 c => c.Status,
                                 c => c.Asl,
                                 c => c.DateCreated
                                 )
                                 .OrderByAsc(c => c.Id)
                                 .BeginFilter()
                                     .Condition(c => c.SENSOR.Id, FilterOperator.Equal, readerResLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id))
                                     .And()
                                     .Condition(c => c.SENSOR.Status, FilterOperator.Equal, "A")
                                 .EndFilter()
                                 .OnTop(1);

                                    var recordsSensorLocation = executor.ExecuteAndFetch(webQuerySensorLocation, readerSensorLocation =>
                                    {
                                        while (readerSensorLocation.Read())
                                        {
                                            sensorLocation.ASL = readerSensorLocation.GetValue(c => c.Asl);
                                            sensorLocation.DataCreated = readerSensorLocation.GetValue(c => c.DateCreated);
                                            sensorLocation.Lon = readerSensorLocation.GetValue(c => c.Lon);
                                            sensorLocation.Lat = readerSensorLocation.GetValue(c => c.Lat);
                                            sensorLocation.Status = readerSensorLocation.GetValue(c => c.Status);
                                            break;
                                        }
                                        return true;
                                    });

                                    sensor.Locations = new SensorLocation[1] { sensorLocation };
                                    lstSensors.Add(sensor);
                                }
                            }
                            lstMeasTask.Add(measTask);
                        }
                    }
                    return true;
                });
            }
        }
    }
}
