using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Common;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.AppServices;
using Atdi.AppServer.AppServices.SdrnsController;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using Atdi.SDNRS.AppServer.Sheduler;
using Atdi.SDNRS.AppServer.BusManager;
using XMLLibrary;
using Atdi.AppServer.AppService.SdrnsController;
using Atdi.SDNRS.AppServer.ManageDB;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDR.Server.Utils;


namespace Atdi.AppServer.RunServices
{ 
    public class RunServiceComponent : IAppServerComponent
    {
        private readonly string _name;
        private ILogger _logger;

        public RunServiceComponent()
        {
            this._name = "RunServiceComponent";
        }

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.AppService;

        string IAppServerComponent.Name => this._name;

        void IAppServerComponent.Activate()
        {
            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
            GlobalInit.Initialization();
            ClassesDBGetResult DbGetRes = new ClassesDBGetResult();
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults();
            ///
            // Начальная инициализация (загрузка конфигурационных данных)
            /*
            System.Threading.Thread tt = new System.Threading.Thread(() => {
                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
                    if (GlobalInit.LST_MeasurementResults.Count == 0) {
                        MeasurementResults[] msResltConv = conv.ConvertTo_SDRObjects(DbGetRes.ReadlAllResultFromDB());
                        if (msResltConv != null) {
                            foreach (MeasurementResults inb in msResltConv.ToArray()) {
                                GlobalInit.LST_MeasurementResults.Add(inb);
                            }
                        }
                    }
            });
            tt.Start();
            tt.Join();
            */


              System.Threading.Thread tsg = new System.Threading.Thread(() => {
                ClassesDBGetTasks cl = new ClassesDBGetTasks();

                ClassConvertTasks ts = new ClassConvertTasks();
                Task<MeasTask[]> task = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB());
                task.Wait();
                List<MeasTask> mts_ = task.Result.ToList();
                //List<MeasTask> mts_ = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB()).ToList();
                foreach (MeasTask mtsd in mts_.ToArray()) {
                    if (((GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == mtsd.Id.Value) == null))) {
                        MeasTask fnd = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == mtsd.Id.Value);
                        if (fnd != null)
                            GlobalInit.LIST_MEAS_TASK.ReplaceAll<MeasTask>(fnd, mtsd);
                        else GlobalInit.LIST_MEAS_TASK.Add(mtsd);

                    }
                }
                cl.Dispose();
                ts.Dispose();
            });
            tsg.Start();
            
           

           Sheduler_Up_Meas_SDR_Results Sc_Up_Meas_SDR = new Sheduler_Up_Meas_SDR_Results(); Sc_Up_Meas_SDR.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeUpdateMeasResult);
            
           ShedulerReceiveStatusMeastaskSDR sc = new ShedulerReceiveStatusMeastaskSDR();
           sc.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeUpdateMeasTaskStatus);
           

           ShedulerCheckActivitySensor CheckActivitySensor = new ShedulerCheckActivitySensor();
           CheckActivitySensor.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._RescanActivitySensor);
           
           ShedulerGetMeasTask getMeasTask = new ShedulerGetMeasTask(); getMeasTask.ShedulerRepeatStart(20);


           //Sheduler_ArchiveSDRResults arch_sdrRes = new Sheduler_ArchiveSDRResults(); arch_sdrRes.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeArchiveResult);
           
           ShedulerCheckStart Quartz = new ShedulerCheckStart();
           Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ReloadStart);
           


        }

        void IAppServerComponent.Deactivate()
        {
            ;
        }

        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            _logger.Trace("Component RunServiceComponent: Uninstalled");
        }
    }
}
