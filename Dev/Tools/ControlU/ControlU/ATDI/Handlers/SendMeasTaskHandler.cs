using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class SendMeasTaskHandler : MessageHandlerBase<DM.MeasTask>
    {
        public List<ControlU.DB.LocalMeasSdrTask_v2> Receivedtasks;
        private readonly IBusGate _gate;

        public SendMeasTaskHandler(IBusGate gate)
            : base("SendMeasTask")
        {
            this._gate = gate;
            Receivedtasks = new List<ControlU.DB.LocalMeasSdrTask_v2>() { };
        }

        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            try
            {
                //Console.WriteLine($"Recieved meas task with ID = '{message.Data.TaskId}'");
                if (message.Data != null)
                {
                    try
                    {
                        //некоторая обработка таска
                        DM.MeasTask mt = message.Data;

                        //if (mt.Stations.Length > 0)
                        //{
                        ControlU.DB.LocalMeasSdrTask_v2 data = new ControlU.DB.LocalMeasSdrTask_v2();
                        bool find = false;
                        for (int j = 0; j < Receivedtasks.Count; j++)
                        {
                            if (mt.TaskId == Receivedtasks[j].MeasTask.task_id)
                            { find = true; }
                        }
                        if (find == false)
                        {
                            ControlU.ATDI.AtdiDataConverter dc = new ControlU.ATDI.AtdiDataConverter();
                            data = dc.ConvertToLocal(mt);

                            if (data.Errors == "")
                            {
                                Receivedtasks.Add(data);
                                ControlU.MainWindow.db_v2.SaveReceivedTask_v2(Receivedtasks);
                            }
                        }

                        
                        // }

                        // подтверждаем обработку
                        if (data.Errors == "")
                            message.Result = MessageHandlingResult.Confirmed;
                        else
                        {
                            message.ReasonFailure = data.Errors;
                            message.Result = MessageHandlingResult.Error;
                            ControlU.App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                ((ControlU.SplashWindow)ControlU.App.Current.MainWindow).m_mainWindow.Message = "Принят таск №" + data.MeasTask.task_id + " c ошибкой " + data.Errors;
                            }));

                        }
                    }
                    catch (Exception exp)
                    {
                        ControlU.App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            ControlU.MainWindow.exp.ExceptionData = new ControlU.ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));

                    }
                }
                else
                {
                    message.Result = MessageHandlingResult.Reject;
                    //message.Result = MessageHandlingResult.Error;
                    //message.ReasonFailure = "Текст ошибки";
                }
            }
            catch (Exception exp)
            {
                ControlU.App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ControlU.MainWindow.exp.ExceptionData = new ControlU.ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));

            }
        }
    }
}
