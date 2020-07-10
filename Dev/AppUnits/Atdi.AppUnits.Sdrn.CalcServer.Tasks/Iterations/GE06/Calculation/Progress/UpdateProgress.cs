using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Idwm = Atdi.Contracts.Sdrn.DeepServices.IDWM;
using IdwmDataModel = Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class UpdateProgress
    {
        /// <summary>
        /// Отправка уведомления о проценте выполнения задачи
        /// </summary>
        /// <param name="paramLength1">Размерность внешнего цикла</param>
        /// <param name="paramLength2">Размерность внутреннего цикла</param>
        /// <param name="index1">Текущий индекс внешнего цикла</param>
        /// <param name="index2">Текущий индекс внутреннего цикла</param>
        /// <param name="percentComplete">переменная для обновления текущего значения процента</param>
        /// <param name="taskContext"></param>
        public static void UpdatePercentComplete100(int paramLength1, int paramLength2, int index1, int index2, ref int percentComplete, string message, ITaskContext taskContext)
        {
            if (paramLength1 > 0)
            {
                var clc = (double)(100.0 / (double)(paramLength1 * paramLength2));
                var newCalcPercent = (int)(((index1 + 1) * (index2 + 1)) * clc);
                if ((newCalcPercent - percentComplete) > 4)
                {
                    percentComplete = newCalcPercent;
                    taskContext.SendEvent(new CalcResultEvent<CurrentProgress>
                    {
                        Level = CalcResultEventLevel.Info,
                        Context = "Ge06CalcIteration",
                        Message = $"Percent complete for calculation {message}",
                        Data = new CurrentProgress
                        {
                            State = newCalcPercent
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Отправка уведомления о проценте выполнения задачи
        /// </summary>
        /// <param name="paramLength1"></param>
        /// <param name="paramLength2"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="additionalPercent"></param>
        /// <param name="percentComplete"></param>
        /// <param name="message"></param>
        /// <param name="taskContext"></param>
        public static void UpdatePercentComplete50(int paramLength1, int paramLength2, int index1, int index2, int additionalPercent, ref int percentComplete, string message, ITaskContext taskContext)
        {
            if (paramLength1 > 0)
            {
                var clc = (double)(50.0 / (double)(paramLength1 * paramLength2));
                var newCalcPercent = additionalPercent + (int)(((index1 + 1) * (index2 + 1)) * clc);
                if ((newCalcPercent - percentComplete) > 4)
                {
                    percentComplete = newCalcPercent;
                    taskContext.SendEvent(new CalcResultEvent<CurrentProgress>
                    {
                        Level = CalcResultEventLevel.Info,
                        Context = "Ge06CalcIteration",
                        Message = $"Percent complete for calculation {message}",
                        Data = new CurrentProgress
                        {
                            State = newCalcPercent
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Отправка уведомления о фиксированном проценте выполнения задачи
        /// </summary>
        /// <param name="percentComplete"></param>
        /// <param name="message"></param>
        /// <param name="taskContext"></param>
        public static void UpdatePercentComplete100(ref int percentComplete, string message, ITaskContext taskContext)
        {
            percentComplete = 100;
            taskContext.SendEvent(new CalcResultEvent<CurrentProgress>
            {
                Level = CalcResultEventLevel.Info,
                Context = "Ge06CalcIteration",
                Message = $"Percent complete for calculation {message}",
                Data = new CurrentProgress
                {
                    State = percentComplete
                }
            });

        }

    }
}
