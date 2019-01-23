using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers
{
    [TestClass]
    public class SaveDataUnitTests
    {

        

        [TestMethod]
        public void Test_ArchiveMeasTaskSDR()
        {
            SaveMeasTaskSDR saveMeasTaskSDR = new SaveMeasTaskSDR();
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrTask measSdrTask = new AppServer.Contracts.Sdrns.MeasSdrTask();
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            measSdrTask.GroupeTypeMeasForMobEquipment = new AppServer.Contracts.Sdrns.MeasurementType[1] { AppServer.Contracts.Sdrns.MeasurementType.AmplModulation };
            measSdrTask.Id = 1;
            measSdrTask.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.AmplModulation;
            measSdrTask.MeasFreqParam = new AppServer.Contracts.Sdrns.MeasFreqParam();
            measSdrTask.MeasFreqParam.MeasFreqs = new AppServer.Contracts.Sdrns.MeasFreq[1];
            measSdrTask.MeasFreqParam.MeasFreqs[0] = new AppServer.Contracts.Sdrns.MeasFreq();
            measSdrTask.MeasFreqParam.MeasFreqs[0].Freq = 11;
            measSdrTask.MeasFreqParam.Mode = AppServer.Contracts.Sdrns.FrequencyMode.FrequencyList;
            measSdrTask.MeasFreqParam.RgL = 2;
            measSdrTask.MeasFreqParam.RgU = 4;
            measSdrTask.MeasFreqParam.Step = 4;
            measSdrTask.MeasLocParam = new AppServer.Contracts.Sdrns.MeasLocParam[1] { new AppServer.Contracts.Sdrns.MeasLocParam { ASL = 100, Id = new AppServer.Contracts.Sdrns.MeasLocParamIdentifier { Value = 2 }, Lat = 30, Lon = 10, MaxDist = 10 } };
            measSdrTask.MeasSDRParam = new AppServer.Contracts.Sdrns.MeasSdrParam();
            measSdrTask.MeasSDRParam.DetectTypeSDR = AppServer.Contracts.Sdrns.DetectingType.Avarage;
            measSdrTask.MeasSDRParam.MeasTime = 1;
            measSdrTask.MeasSDRParam.PreamplificationSDR = 2;
            measSdrTask.MeasSDRParam.RBW = 200;
            measSdrTask.MeasSDRParam.ref_level_dbm = -20;
            measSdrTask.MeasSDRParam.RfAttenuationSDR = 20;
            measSdrTask.MeasSDRParam.VBW = 400;
            measSdrTask.MeasSDRSOParam = new AppServer.Contracts.Sdrns.MeasSdrSOParam();
            measSdrTask.MeasSDRSOParam.LevelMinOccup = 100;
            measSdrTask.MeasSDRSOParam.NChenal = 10;
            measSdrTask.MeasSDRSOParam.TypeSO = AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation;
            measSdrTask.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasSubTaskId.Value = 1;
            measSdrTask.MeasSubTaskStationId = 1;
            measSdrTask.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasTaskId.Value = 1;
            measSdrTask.NumberScanPerTask = 34;
            measSdrTask.PerInterval = 200;
            measSdrTask.prio = 10;
            measSdrTask.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrTask.SensorId.Value = 1;
            measSdrTask.status = "N";
            measSdrTask.SwNumber = 2;
            measSdrTask.Time_start = DateTime.Now;
            measSdrTask.Time_stop = DateTime.Now.AddHours(1);
            measSdrTask.TypeM = AppServer.Contracts.Sdrns.SpectrumScanType.RealTime;
            bool res = saveMeasTaskSDR.ArchiveMeasTaskSDR(measSdrTask, "X");
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_DeleteMeasTaskSDR()
        {
            SaveMeasTaskSDR saveMeasTaskSDR = new SaveMeasTaskSDR();
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrTask measSdrTask = new AppServer.Contracts.Sdrns.MeasSdrTask();
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            measSdrTask.GroupeTypeMeasForMobEquipment = new AppServer.Contracts.Sdrns.MeasurementType[1] { AppServer.Contracts.Sdrns.MeasurementType.AmplModulation };
            measSdrTask.Id = 1;
            measSdrTask.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.AmplModulation;
            measSdrTask.MeasFreqParam = new AppServer.Contracts.Sdrns.MeasFreqParam();
            measSdrTask.MeasFreqParam.MeasFreqs = new AppServer.Contracts.Sdrns.MeasFreq[1];
            measSdrTask.MeasFreqParam.MeasFreqs[0] = new AppServer.Contracts.Sdrns.MeasFreq();
            measSdrTask.MeasFreqParam.MeasFreqs[0].Freq = 11;
            measSdrTask.MeasFreqParam.Mode = AppServer.Contracts.Sdrns.FrequencyMode.FrequencyList;
            measSdrTask.MeasFreqParam.RgL = 2;
            measSdrTask.MeasFreqParam.RgU = 4;
            measSdrTask.MeasFreqParam.Step = 4;
            measSdrTask.MeasLocParam = new AppServer.Contracts.Sdrns.MeasLocParam[1] { new AppServer.Contracts.Sdrns.MeasLocParam { ASL = 100, Id = new AppServer.Contracts.Sdrns.MeasLocParamIdentifier { Value = 2 }, Lat = 30, Lon = 10, MaxDist = 10 } };
            measSdrTask.MeasSDRParam = new AppServer.Contracts.Sdrns.MeasSdrParam();
            measSdrTask.MeasSDRParam.DetectTypeSDR = AppServer.Contracts.Sdrns.DetectingType.Avarage;
            measSdrTask.MeasSDRParam.MeasTime = 1;
            measSdrTask.MeasSDRParam.PreamplificationSDR = 2;
            measSdrTask.MeasSDRParam.RBW = 200;
            measSdrTask.MeasSDRParam.ref_level_dbm = -20;
            measSdrTask.MeasSDRParam.RfAttenuationSDR = 20;
            measSdrTask.MeasSDRParam.VBW = 400;
            measSdrTask.MeasSDRSOParam = new AppServer.Contracts.Sdrns.MeasSdrSOParam();
            measSdrTask.MeasSDRSOParam.LevelMinOccup = 100;
            measSdrTask.MeasSDRSOParam.NChenal = 10;
            measSdrTask.MeasSDRSOParam.TypeSO = AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation;
            measSdrTask.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasSubTaskId.Value = 1;
            measSdrTask.MeasSubTaskStationId = 1;
            measSdrTask.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasTaskId.Value = 1;
            measSdrTask.NumberScanPerTask = 34;
            measSdrTask.PerInterval = 200;
            measSdrTask.prio = 10;
            measSdrTask.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrTask.SensorId.Value = 1;
            measSdrTask.status = "N";
            measSdrTask.SwNumber = 2;
            measSdrTask.Time_start = DateTime.Now;
            measSdrTask.Time_stop = DateTime.Now.AddHours(1);
            measSdrTask.TypeM = AppServer.Contracts.Sdrns.SpectrumScanType.RealTime;
            bool res = saveMeasTaskSDR.DeleteMeasTaskSDR(measSdrTask);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_CreateNewMeasTaskSDR()
        {
            SaveMeasTaskSDR saveMeasTaskSDR = new SaveMeasTaskSDR();
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrTask measSdrTask = new AppServer.Contracts.Sdrns.MeasSdrTask();
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            measSdrTask.GroupeTypeMeasForMobEquipment = new AppServer.Contracts.Sdrns.MeasurementType[1] { AppServer.Contracts.Sdrns.MeasurementType.AmplModulation };
            measSdrTask.Id = 1;
            measSdrTask.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.AmplModulation;
            measSdrTask.MeasFreqParam = new AppServer.Contracts.Sdrns.MeasFreqParam();
            measSdrTask.MeasFreqParam.MeasFreqs = new AppServer.Contracts.Sdrns.MeasFreq[1];
            measSdrTask.MeasFreqParam.MeasFreqs[0] = new AppServer.Contracts.Sdrns.MeasFreq();
            measSdrTask.MeasFreqParam.MeasFreqs[0].Freq = 11;
            measSdrTask.MeasFreqParam.Mode = AppServer.Contracts.Sdrns.FrequencyMode.FrequencyList;
            measSdrTask.MeasFreqParam.RgL = 2;
            measSdrTask.MeasFreqParam.RgU = 4;
            measSdrTask.MeasFreqParam.Step = 4;
            measSdrTask.MeasLocParam = new AppServer.Contracts.Sdrns.MeasLocParam[1] { new AppServer.Contracts.Sdrns.MeasLocParam { ASL = 100, Id = new AppServer.Contracts.Sdrns.MeasLocParamIdentifier { Value = 2 }, Lat = 30, Lon = 10, MaxDist = 10 } };
            measSdrTask.MeasSDRParam = new AppServer.Contracts.Sdrns.MeasSdrParam();
            measSdrTask.MeasSDRParam.DetectTypeSDR = AppServer.Contracts.Sdrns.DetectingType.Avarage;
            measSdrTask.MeasSDRParam.MeasTime = 1;
            measSdrTask.MeasSDRParam.PreamplificationSDR = 2;
            measSdrTask.MeasSDRParam.RBW = 200;
            measSdrTask.MeasSDRParam.ref_level_dbm = -20;
            measSdrTask.MeasSDRParam.RfAttenuationSDR = 20;
            measSdrTask.MeasSDRParam.VBW = 400;
            measSdrTask.MeasSDRSOParam = new AppServer.Contracts.Sdrns.MeasSdrSOParam();
            measSdrTask.MeasSDRSOParam.LevelMinOccup = 100;
            measSdrTask.MeasSDRSOParam.NChenal = 10;
            measSdrTask.MeasSDRSOParam.TypeSO = AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation;
            measSdrTask.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasSubTaskId.Value = 1;
            measSdrTask.MeasSubTaskStationId = 1;
            measSdrTask.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasTaskId.Value = 1;
            measSdrTask.NumberScanPerTask = 34;
            measSdrTask.PerInterval = 200;
            measSdrTask.prio = 10;
            measSdrTask.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrTask.SensorId.Value = 1;
            measSdrTask.status = "N";
            measSdrTask.SwNumber = 2;
            measSdrTask.Time_start = DateTime.Now;
            measSdrTask.Time_stop = DateTime.Now.AddHours(1);
            measSdrTask.TypeM = AppServer.Contracts.Sdrns.SpectrumScanType.RealTime;
            bool res = saveMeasTaskSDR.CreateNewMeasTaskSDR(measSdrTask);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_SaveStatusMeasTaskSDR()
        {
            SaveMeasTaskSDR saveMeasTaskSDR = new SaveMeasTaskSDR();
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrTask measSdrTask = new AppServer.Contracts.Sdrns.MeasSdrTask();
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            measSdrTask.GroupeTypeMeasForMobEquipment = new AppServer.Contracts.Sdrns.MeasurementType[1] { AppServer.Contracts.Sdrns.MeasurementType.AmplModulation };
            measSdrTask.Id = 1;
            measSdrTask.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.AmplModulation;
            measSdrTask.MeasFreqParam = new AppServer.Contracts.Sdrns.MeasFreqParam();
            measSdrTask.MeasFreqParam.MeasFreqs = new AppServer.Contracts.Sdrns.MeasFreq[1];
            measSdrTask.MeasFreqParam.MeasFreqs[0] = new AppServer.Contracts.Sdrns.MeasFreq();
            measSdrTask.MeasFreqParam.MeasFreqs[0].Freq = 11;
            measSdrTask.MeasFreqParam.Mode = AppServer.Contracts.Sdrns.FrequencyMode.FrequencyList;
            measSdrTask.MeasFreqParam.RgL = 2;
            measSdrTask.MeasFreqParam.RgU = 4;
            measSdrTask.MeasFreqParam.Step = 4;
            measSdrTask.MeasLocParam = new AppServer.Contracts.Sdrns.MeasLocParam[1] { new AppServer.Contracts.Sdrns.MeasLocParam { ASL = 100, Id = new AppServer.Contracts.Sdrns.MeasLocParamIdentifier { Value = 2 }, Lat = 30, Lon = 10, MaxDist = 10 } };
            measSdrTask.MeasSDRParam = new AppServer.Contracts.Sdrns.MeasSdrParam();
            measSdrTask.MeasSDRParam.DetectTypeSDR = AppServer.Contracts.Sdrns.DetectingType.Avarage;
            measSdrTask.MeasSDRParam.MeasTime = 1;
            measSdrTask.MeasSDRParam.PreamplificationSDR = 2;
            measSdrTask.MeasSDRParam.RBW = 200;
            measSdrTask.MeasSDRParam.ref_level_dbm = -20;
            measSdrTask.MeasSDRParam.RfAttenuationSDR = 20;
            measSdrTask.MeasSDRParam.VBW = 400;
            measSdrTask.MeasSDRSOParam = new AppServer.Contracts.Sdrns.MeasSdrSOParam();
            measSdrTask.MeasSDRSOParam.LevelMinOccup = 100;
            measSdrTask.MeasSDRSOParam.NChenal = 10;
            measSdrTask.MeasSDRSOParam.TypeSO = AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation;
            measSdrTask.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasSubTaskId.Value = 1;
            measSdrTask.MeasSubTaskStationId = 1;
            measSdrTask.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrTask.MeasTaskId.Value = 1;
            measSdrTask.NumberScanPerTask = 34;
            measSdrTask.PerInterval = 200;
            measSdrTask.prio = 10;
            measSdrTask.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrTask.SensorId.Value = 1;
            measSdrTask.status = "N";
            measSdrTask.SwNumber = 2;
            measSdrTask.Time_start = DateTime.Now;
            measSdrTask.Time_stop = DateTime.Now.AddHours(1);
            measSdrTask.TypeM = AppServer.Contracts.Sdrns.SpectrumScanType.RealTime;
            bool res = saveMeasTaskSDR.SaveStatusMeasTaskSDR(measSdrTask);
            Assert.AreEqual(res, true);
        }


        [TestMethod]
        public void Test_SaveStatusMeasTaskSDRResults()
        {
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrResults measSdrResults = new AppServer.Contracts.Sdrns.MeasSdrResults();
            measSdrResults.DataMeas = System.DateTime.Now;
            measSdrResults.Freqs = new float[5] { 1, 2, 3, 4, 5 };
            AppServer.Contracts.Sdrns.FSemples fSemples = new AppServer.Contracts.Sdrns.FSemples();
            fSemples.Freq = 200;
            fSemples.Id = 1;
            fSemples.LeveldBm = -20;
            fSemples.LeveldBmkVm = 10;
            fSemples.LevelMaxdBm = 30;
            fSemples.LevelMindBm = -30;
            fSemples.OcupationPt = 10;
            measSdrResults.FSemples = new AppServer.Contracts.Sdrns.FSemples[1] { fSemples };
            measSdrResults.Level = new float[5] { 10.1f, 20, 13.1f, 14, 15 };
            measSdrResults.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.MonitoringStations;
            measSdrResults.MeasSDRLoc = new AppServer.Contracts.Sdrns.LocationSensorMeasurement();
            measSdrResults.MeasSDRLoc.ASL = 100;
            measSdrResults.MeasSDRLoc.Lat = 50;
            measSdrResults.MeasSDRLoc.Lon = 30;
            measSdrResults.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasSubTaskId.Value = 1;
            measSdrResults.MeasSubTaskStationId = 1;
            measSdrResults.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasTaskId.Value = 1;
            measSdrResults.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasTaskId.Value = 1;
            measSdrResults.NN = 1;
            measSdrResults.ResultsBandwidth = new AppServer.Contracts.Sdrns.MeasSdrBandwidthResults();
            measSdrResults.ResultsBandwidth.BandwidthkHz = 200;
            measSdrResults.ResultsBandwidth.MarkerIndex = 1;
            measSdrResults.ResultsBandwidth.T1 = 1;
            measSdrResults.ResultsBandwidth.T2 = 4;
            measSdrResults.ResultsBandwidth.СorrectnessEstimations = true;
            AppServer.Contracts.Sdrns.ResultsMeasurementsStation resultsMeasurementsStation = new AppServer.Contracts.Sdrns.ResultsMeasurementsStation();
            resultsMeasurementsStation.CentralFrequencyMeas_MHz = 200;
            resultsMeasurementsStation.GeneralResult = new AppServer.Contracts.Sdrns.MeasurementsParameterGeneral();
            resultsMeasurementsStation.GeneralResult.CentralFrequency = 50;
            measSdrResults.ResultsMeasStation = new AppServer.Contracts.Sdrns.ResultsMeasurementsStation[1] { resultsMeasurementsStation };
            measSdrResults.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrResults.SensorId.Value = 1;
            measSdrResults.status = "C";
            measSdrResults.SwNumber = 1;
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            bool res = SaveMeasSDRResults.SaveStatusMeasTaskSDRResults(measSdrResults, "Z");
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_SaveisSendMeasTaskSDRResults()
        {
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrResults measSdrResults = new AppServer.Contracts.Sdrns.MeasSdrResults();
            measSdrResults.DataMeas = System.DateTime.Now;
            measSdrResults.Freqs = new float[5] { 1, 2, 3, 4, 5 };
            AppServer.Contracts.Sdrns.FSemples fSemples = new AppServer.Contracts.Sdrns.FSemples();
            fSemples.Freq = 200;
            fSemples.Id = 1;
            fSemples.LeveldBm = -20;
            fSemples.LeveldBmkVm = 10;
            fSemples.LevelMaxdBm = 30;
            fSemples.LevelMindBm = -30;
            fSemples.OcupationPt = 10;
            measSdrResults.FSemples = new AppServer.Contracts.Sdrns.FSemples[1] { fSemples };
            measSdrResults.Level = new float[5] { 10.1f, 20, 13.1f, 14, 15 };
            measSdrResults.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.MonitoringStations;
            measSdrResults.MeasSDRLoc = new AppServer.Contracts.Sdrns.LocationSensorMeasurement();
            measSdrResults.MeasSDRLoc.ASL = 100;
            measSdrResults.MeasSDRLoc.Lat = 50;
            measSdrResults.MeasSDRLoc.Lon = 30;
            measSdrResults.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasSubTaskId.Value = 1;
            measSdrResults.MeasSubTaskStationId = 1;
            measSdrResults.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasTaskId.Value = 1;
            measSdrResults.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasTaskId.Value = 1;
            measSdrResults.NN = 1;
            measSdrResults.ResultsBandwidth = new AppServer.Contracts.Sdrns.MeasSdrBandwidthResults();
            measSdrResults.ResultsBandwidth.BandwidthkHz = 200;
            measSdrResults.ResultsBandwidth.MarkerIndex = 1;
            measSdrResults.ResultsBandwidth.T1 = 1;
            measSdrResults.ResultsBandwidth.T2 = 4;
            measSdrResults.ResultsBandwidth.СorrectnessEstimations = true;
            AppServer.Contracts.Sdrns.ResultsMeasurementsStation resultsMeasurementsStation = new AppServer.Contracts.Sdrns.ResultsMeasurementsStation();
            resultsMeasurementsStation.CentralFrequencyMeas_MHz = 200;
            resultsMeasurementsStation.GeneralResult = new AppServer.Contracts.Sdrns.MeasurementsParameterGeneral();
            resultsMeasurementsStation.GeneralResult.CentralFrequency = 50;
            measSdrResults.ResultsMeasStation = new AppServer.Contracts.Sdrns.ResultsMeasurementsStation[1] { resultsMeasurementsStation };
            measSdrResults.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrResults.SensorId.Value = 1;
            measSdrResults.status = "C";
            measSdrResults.SwNumber = 1;
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            bool res = SaveMeasSDRResults.SaveisSendMeasTaskSDRResults(measSdrResults);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_SaveMeasResultSDR()
        {
            Fake.FakeLogger fakeLogger = new Fake.FakeLogger();
            SaveMeasSDRResults saveMeasSDRResults = new SaveMeasSDRResults();
            AppServer.Contracts.Sdrns.MeasSdrResults measSdrResults = new AppServer.Contracts.Sdrns.MeasSdrResults();
            measSdrResults.DataMeas = System.DateTime.Now;
            measSdrResults.Freqs = new float[5] { 1, 2, 3, 4, 5 };
            AppServer.Contracts.Sdrns.FSemples fSemples = new AppServer.Contracts.Sdrns.FSemples();
            fSemples.Freq = 200;
            fSemples.Id = 1;
            fSemples.LeveldBm = -20;
            fSemples.LeveldBmkVm = 10;
            fSemples.LevelMaxdBm = 30;
            fSemples.LevelMindBm = -30;
            fSemples.OcupationPt = 10;
            measSdrResults.FSemples = new AppServer.Contracts.Sdrns.FSemples[1] { fSemples };
            measSdrResults.Level = new float[5] { 10.1f, 20, 13.1f, 14, 15 };
            measSdrResults.MeasDataType = AppServer.Contracts.Sdrns.MeasurementType.MonitoringStations;
            measSdrResults.MeasSDRLoc = new AppServer.Contracts.Sdrns.LocationSensorMeasurement();
            measSdrResults.MeasSDRLoc.ASL = 100;
            measSdrResults.MeasSDRLoc.Lat = 50;
            measSdrResults.MeasSDRLoc.Lon = 30;
            measSdrResults.MeasSubTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasSubTaskId.Value = 1;
            measSdrResults.MeasSubTaskStationId = 1;
            measSdrResults.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasTaskId.Value = 1;
            measSdrResults.MeasTaskId = new AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            measSdrResults.MeasTaskId.Value = 1;
            measSdrResults.NN = 1;
            measSdrResults.ResultsBandwidth = new AppServer.Contracts.Sdrns.MeasSdrBandwidthResults();
            measSdrResults.ResultsBandwidth.BandwidthkHz = 200;
            measSdrResults.ResultsBandwidth.MarkerIndex = 1;
            measSdrResults.ResultsBandwidth.T1 = 1;
            measSdrResults.ResultsBandwidth.T2 = 4;
            measSdrResults.ResultsBandwidth.СorrectnessEstimations = true;
            AppServer.Contracts.Sdrns.ResultsMeasurementsStation resultsMeasurementsStation = new AppServer.Contracts.Sdrns.ResultsMeasurementsStation();
            resultsMeasurementsStation.CentralFrequencyMeas_MHz = 200;
            resultsMeasurementsStation.GeneralResult = new AppServer.Contracts.Sdrns.MeasurementsParameterGeneral();
            resultsMeasurementsStation.GeneralResult.CentralFrequency = 50;
            measSdrResults.ResultsMeasStation = new AppServer.Contracts.Sdrns.ResultsMeasurementsStation[1] { resultsMeasurementsStation };
            measSdrResults.SensorId = new AppServer.Contracts.Sdrns.SensorIdentifier();
            measSdrResults.SensorId.Value = 1;
            measSdrResults.status = "C";
            measSdrResults.SwNumber = 1;
            Atdi.AppUnits.Sdrn.ControlA.Bus.Launcher._logger = fakeLogger;
            bool res = saveMeasSDRResults.SaveMeasResultSDR(measSdrResults, "C");
            Assert.AreEqual(res, true);
        }
    }
}
