using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using INP = System.Windows.Input;
using System.Windows.Controls;
using System.Collections;
using XICSM.ICSControlClient.Models;
using System.Timers;
using XICSM.ICSControlClient.Forms;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Globalization;
using TR = System.Threading;
using Microsoft.VisualBasic;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.ViewModels
{
    public class SavedTaskViewModel : WpfViewModelBase
    {
        private ShortMeasTaskViewModel _currentShortMeasTask;
        private IList _currentShortMeasTasks;
        private MeasTaskViewModel _currentMeasTask;
        private ShortMeasTaskViewModel[] _shortMeasTasks;
        public WpfCommand DeleteMeasTaskCommand { get; set; }
        public WpfCommand ActivateMeasTaskCommand { get; set; }
        public WpfCommand RefreshMeasTaskCommand { get; set; }

        public SavedTaskViewModel()
        {
            this.ActivateMeasTaskCommand = new WpfCommand(this.OnActivateMeasTaskCommand);
            this.DeleteMeasTaskCommand = new WpfCommand(this.OnDeleteMeasTaskCommand);
            this.RefreshMeasTaskCommand = new WpfCommand(this.OnRefreshMeasTaskCommand);
            ReloadShortMeasTasks();
        }
        public ShortMeasTaskViewModel CurrentShortMeasTask
        {
            get => this._currentShortMeasTask;
            set => this.Set(ref this._currentShortMeasTask, value, () => { ReloadMeasTaskDetail(this._currentShortMeasTask); });
        }
        public IList CurrentShortMeasTasks
        {
            get => this._currentShortMeasTasks;
            set => this.Set(ref this._currentShortMeasTasks, value);
        }
        public MeasTaskViewModel CurrentMeasTask
        {
            get => this._currentMeasTask;
            set => this.Set(ref this._currentMeasTask, value, () => { });
        }
        public ShortMeasTaskViewModel[] ShortMeasTasks
        {
            get => this._shortMeasTasks;
            set => this.Set(ref this._shortMeasTasks, value, () => { });
        }
        private void ReloadShortMeasTasks()
        {
            var listMeasTask = new List<ShortMeasTaskViewModel>();

            string _endpointUrls = PluginHelper.GetRestApiEndPoint();

            if (string.IsNullOrEmpty(_endpointUrls))
                return;

            using (var wc = new HttpClient())
            {
                string filter = $"(Status eq 'S')";
                string fields = "Status,Name,Type,DateCreated,CreatedBy,Id";
                string request = $"{_endpointUrls}api/orm/data/SDRN_Server_DB/Atdi.DataModels.Sdrns.Server.Entities/MeasTask?select={fields}&filter={filter}&orderBy=Id";
                var response = wc.GetAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var dicFields = new Dictionary<string, int>();
                    var data = JsonConvert.DeserializeObject<DataSetResult>(response.Content.ReadAsStringAsync().Result);

                    foreach (var field in data.Fields)
                        dicFields[field.Path] = field.Index;
                    foreach (object[] record in data.Records)
                    {
                        var task = new ShortMeasTaskViewModel();
                        task.Status = (string)record[dicFields["Status"]];
                        task.StatusFull = PluginHelper.GetFullTaskStatus(task.Status);
                        task.Name = (string)record[dicFields["Name"]];
                        MeasurementType typeMeasurements;
                        if (Enum.TryParse<MeasurementType>((string)record[dicFields["Type"]], out typeMeasurements))
                        {
                            task.TypeMeasurements = typeMeasurements;
                            task.TypeMeasurementsString = typeMeasurements.ToString();
                        }

                        task.DateCreated = (DateTime?)record[dicFields["DateCreated"]];
                        task.CreatedBy = (string)record[dicFields["CreatedBy"]];
                        task.Id = Convert.ToInt64(record[dicFields["Id"]]);
                        listMeasTask.Add(task);
                    }
                }
            }

            //var endpoint = PluginHelper.GetEndpoint();
            //var dataContext = new WebApiDataContext("SDRN_Server_DB");
            //var dataLayer = new WebApiDataLayer();
            //var webQuery = dataLayer.GetBuilder<IMeasTask>()
            //    .Read()
            //    .Select(c => c.Status, c => c.Name, c => c.Type, c => c.DateCreated, c => c.CreatedBy, c => c.Id)
            //    .Filter(c => c.Status, "S");
            //var executor = dataLayer.GetExecutor(endpoint, dataContext);
            //var records = executor.ExecuteAndFetch(webQuery, reader =>
            //{
            //    while (reader.Read())
            //    {
            //        var task = new ShortMeasTaskViewModel();
            //        task.Status = reader.GetValue(c => c.Status);
            //        task.StatusFull = PluginHelper.GetFullTaskStatus(task.Status);
            //        task.Name = reader.GetValue(c => c.Name);
            //        MeasurementType typeMeasurements;
            //        if (Enum.TryParse<MeasurementType>(reader.GetValue(c => c.Type), out typeMeasurements))
            //        {
            //            task.TypeMeasurements = typeMeasurements;
            //            task.TypeMeasurementsString = typeMeasurements.ToString();
            //        }

            //        task.DateCreated = reader.GetValue(c => c.DateCreated);
            //        task.CreatedBy = reader.GetValue(c => c.CreatedBy);
            //        task.Id = reader.GetValue(c => c.Id);
            //        listMeasTask.Add(task);
            //    }
            //    return true;
            //});

            //var sdrTasks = SVC.SdrnsControllerWcfClient.GetShortMeasTasks();

            this.ShortMeasTasks = listMeasTask.ToArray();
        }
        private void ReloadMeasTaskDetail(ShortMeasTaskViewModel measTask)
        {
            var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(measTask.Id);
            CurrentMeasTask = Mappers.Map(task);
        }
        private void OnDeleteMeasTaskCommand(object parameter)
        {
            if (this.CurrentShortMeasTask == null && this.CurrentShortMeasTasks == null)
                return;

            var result = System.Windows.Forms.MessageBox.Show("Are you sure?", $"Delete the meas task(s)", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (result != System.Windows.Forms.DialogResult.Yes)
                return;

            if (this.CurrentShortMeasTasks != null)
            {
                foreach (ShortMeasTaskViewModel task in this.CurrentShortMeasTasks)
                    SVC.SdrnsControllerWcfClient.DeleteMeasTaskById(task.Id);
            }
            else
            {
                SVC.SdrnsControllerWcfClient.DeleteMeasTaskById(this.CurrentShortMeasTask.Id);
            }

            this.ReloadShortMeasTasks();
        }
        private void OnActivateMeasTaskCommand(object parameter)
        {
            if (this.CurrentShortMeasTask != null)
            {
                var measTaskForm = new FM.MeasTaskForm(this.CurrentShortMeasTask.Id);
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
            }
        }
        private void OnRefreshMeasTaskCommand(object parameter)
        {
            this.ReloadShortMeasTasks();
        }
    }
}
