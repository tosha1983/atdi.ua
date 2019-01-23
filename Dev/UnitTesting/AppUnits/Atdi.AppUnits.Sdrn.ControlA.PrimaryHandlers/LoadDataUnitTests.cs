using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers
{
    [TestClass]
    public class LoadDataUnitTests
    {
        [TestMethod]
        public void Test_LoadActiveTaskSdrResults()
        {
            LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
            var res = loadDataMeasTask.LoadActiveTaskSdrResults();
        }

        [TestMethod]
        public void Test_GetAllMeasTaskSDR()
        {
            LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
            var res = loadDataMeasTask.GetAllMeasTaskSDR();
        }

        [TestMethod]
        public void Test_FindMeasTaskSDR()
        {
            LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
            int SensorId = 1;
            int MeasTaskId = 1;
            int MeasSubTaskStationId = 1;
            int MeasSubTaskId = 1;
            var res = loadDataMeasTask.FindMeasTaskSDR(SensorId, MeasTaskId, MeasSubTaskStationId, MeasSubTaskId);
        }

        [TestMethod]
        public void Test_GetMaxIdFromResults()
        {
            LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
            var res = loadDataMeasTask.GetMaxIdFromResults();
        }

        [TestMethod]
        public void Test_GetIdentTaskFromMeasTaskSDR()
        {
            LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
            int id = 122344;
            int? MeasTaskId = 0;
            int? MeasSubTaskId = 1;
            int? MeasSubTaskStationId = 0;
            int? SensorId = 0;
            loadDataMeasTask.GetIdentTaskFromMeasTaskSDR(id, ref MeasTaskId, ref MeasSubTaskId, ref MeasSubTaskStationId, ref SensorId);
        }

        [TestMethod]
        public void Test_LoadDataMeasByTaskId()
        {
            LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
            loadDataMeasTask.LoadDataMeasByTaskId(1);
        }
    }
}
