using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.StartMeasurementsSO;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using AAC = Atdi.DataModels.DataConstraint;
using FM = XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient.Handlers.AllotmentCommnads
{
    public class StartMeasurementsSOCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.StartMeasurementsSO,
                        CreateMeasTaskByAllotment
                    );
        }

        private static bool CreateMeasTaskByAllotment(int allotmentId)
        {
            var allotment = Repository.ReadEntityById<DM.Allotment>(allotmentId);
            if (!ValidateAllotment(allotment))
            {
                return false;
            }

            var freqPlanChens = FindFreqPlanChens(allotment);
            if (freqPlanChens == null || freqPlanChens.Length == 0)
            {
                throw new InvalidOperationException($"Not found any FREQ_PLAN_CHAN records by the tour #{allotment.Id}");
            }

            //var measTask = PreparedMeasTaskInfo(allotment, freqPlanChens);
            //SaveTaskToLog(measTask);

            //var measTaskId = WCF.SdrnsControllerWcfClient.CreateMeasTask(measTask);
            //if (measTaskId == IM.NullI)
            //{
            //    throw new InvalidOperationException($"Could not create a meas task by the allotment #{allotment.Id}");
            //}

            allotment.Status = MD.Allotments.Statuses.Dur;
            allotment.MeasTaskId = CreateMeasTask(allotmentId);
            if (allotment.MeasTaskId > 0)
                Repository.UpdateEntity(allotment);

            return true;
        }
        private static long CreateMeasTask(int allotmentId)
        {
            try
            {
                var measTaskForm = new FM.MeasTaskForm(allotmentId, SDR.MeasurementType.SpectrumOccupation);
                measTaskForm.ShowDialog();
                measTaskForm.Dispose();
                return measTaskForm.TaskId;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                return 0;
            }
        }

        private static DM.FreqPlanChan[] FindFreqPlanChens(DM.Allotment allotment)
        {
            return Repository.GetEntities<DM.FreqPlanChan>(source =>
            {
                source.SetWhere(MD.FreqPlanChan.Fields.PlanId, IMRecordset.Operation.Eq, allotment.PlanRef.Id);
            });
        }

        private static void SaveTaskToLog(SDR.MeasTask measTask)
        {
            try
            {
                var data = new StringBuilder();

                data.AppendLine($"Created meas task by SO:");
                data.AppendLine($"Name: {measTask.Name}");
                data.AppendLine($"Mode: {measTask.ExecutionMode}");
                data.AppendLine($"Created: {measTask.DateCreated}");
                if (measTask.Sensors != null)
                {
                    data.AppendLine($"Stations: {measTask.Sensors.Length}");
                }
                else
                {
                    data.AppendLine($"Stations: is null");
                }
                
                //data.AppendLine($"StationsForMeasurements: {measTask.StationsForMeasurements.Length}");

                Logger.WriteInfo(PluginMetadata.Processes.CreateMeasTask, data.ToString());
            }
            catch (Exception e)
            {
                Logger.WriteExeption(PluginMetadata.Processes.SaveTaskToLog, e);
            }

        }

        private static SDR.MeasTask PreparedMeasTaskInfo(DM.Allotment allotment, DM.FreqPlanChan[] freqPlanChans)
        {
            var measTask = new SDR.MeasTask()
            {
                Name = allotment.CustText1,
                ExecutionMode = SDR.MeasTaskExecutionMode.Automatic,
                Task = SDR.MeasTaskType.Scan,
                DateCreated = DateTime.Now,
                CreatedBy = IM.ConnectedUser(),
                Sensors = PreparedStations(allotment),
                MeasDtParam = PreparedDetectedParam(allotment),
                MeasFreqParam = PreparedFreqParam(allotment, freqPlanChans),
                MeasOther = PreparedOther(allotment),
                MeasTimeParamList = PreparedTimeParamList(allotment)
            };

            return measTask;
        }

        private static SDR.MeasSensor[] PreparedStations(DM.Allotment allotment)
        {
            var stations = new List<SDR.MeasSensor>();

            var constraint = new AAC.ComplexCondition()
            {

                Operator = AAC.LogicalOperator.And,
                Conditions = new AAC.ConditionExpression[]
                {
                    new AAC.ConditionExpression
                    {
                        Type = AAC.ConditionType.Expression,
                        Operator = AAC.ConditionOperator.Equal,
                        LeftOperand = new AAC.ColumnOperand { Type = AAC.OperandType.Column, ColumnName = "Name", Source = "Sensor"},
                        RightOperand = new AAC.StringValueOperand { Type = AAC.OperandType.Value, Value = allotment.CustText2}
                    },
                    new AAC.ConditionExpression
                    {
                        Type = AAC.ConditionType.Expression,
                        Operator = AAC.ConditionOperator.Equal,
                        LeftOperand = new AAC.ColumnOperand { Type = AAC.OperandType.Column, ColumnName = "TechId", Source = "Sensor"},
                        RightOperand = new AAC.StringValueOperand { Type = AAC.OperandType.Value, Value = allotment.CustText3}
                    }
                }
            };
            var result = WCF.SdrnsControllerWcfClient.GetSensors(constraint);
            result = result.Where(u => u.Name == allotment.CustText2 && u.Equipment.TechId == allotment.CustText3).ToArray();
            if (result.Length > 0)
            {
                stations.AddRange(
                    result.Select(sensor => new SDR.MeasSensor
                    {
                        SensorId = new SDR.MeasSensorIdentifier
                        {
                            Value = sensor.Id.Value
                        }
                    })
                );
            }
            return stations.ToArray();
        }

        private static SDR.MeasDtParam PreparedDetectedParam(DM.Allotment allotment)
        {
            var detectedParam = new SDR.MeasDtParam()
            {
                TypeMeasurements = SDR.MeasurementType.SpectrumOccupation,
                RBW = allotment.PlanRef.Bandwidth,
                VBW = allotment.PlanRef.Bandwidth,
                RfAttenuation = 0,
                IfAttenuation = 0,
                MeasTime = 0.003,
                DetectType = SDR.DetectingType.Average,
                Preamplification = 0
            };

            return detectedParam;
        }

        private static SDR.MeasFreqParam PreparedFreqParam(DM.Allotment allotment, DM.FreqPlanChan[] freqPlanChans)
        {
            var freqParam = new SDR.MeasFreqParam()
            {
                Mode = SDR.FrequencyMode.FrequencyList,
                RgL = freqPlanChans.Min(c => c.Freq),
                RgU = freqPlanChans.Max(c => c.Freq),
                Step = allotment.PlanRef.ChannelSep == 0 || allotment.PlanRef.ChannelSep == IM.NullD ? allotment.PlanRef.Bandwidth : allotment.PlanRef.ChannelSep,
                MeasFreqs = freqPlanChans.Select(c=> new SDR.MeasFreq { Freq = c.Freq }).ToArray()
            };

            return freqParam;
        }

        private static SDR.MeasOther PreparedOther(DM.Allotment allotment)
        {
            var other = new SDR.MeasOther()
            {
                SwNumber = 10,
                TypeSpectrumScan = SDR.SpectrumScanType.Sweep,
                TypeSpectrumOccupation = "FBO".Equals(allotment.CustText4, StringComparison.OrdinalIgnoreCase) ? SDR.SpectrumOccupationType.FreqBandwidthOccupation : SDR.SpectrumOccupationType.FreqChannelOccupation,
                LevelMinOccup = allotment.CustNum3 == 0 || allotment.CustNum3 == IM.NullD ? -80 : allotment.CustNum3,
                NChenal = 10
            };

            return other;
        }

        private static SDR.MeasTimeParamList PreparedTimeParamList(DM.Allotment allotment)
        {
            var today = DateTime.Today;
            var timeParamList = new SDR.MeasTimeParamList()
            {
                PerStart = allotment.CustDate1,
                PerStop = allotment.CustDate2,
                TimeStart = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0),
                TimeStop = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59),
                PerInterval = allotment.CustNum2 == 0 || allotment.CustNum2 == IM.NullD ? 3600 : 3600 * allotment.CustNum2
            };

            return timeParamList;
        }

      

        private static bool ValidateAllotment(DM.Allotment allotment)
        {
            var messages = new StringBuilder();
            bool result = true;

            if (!allotment.IsNewState)
            {
                messages.AppendLine($"Incorrect the status of the allotment #{allotment.Id}");
                result = false;
            }

            if (!MD.Allotments.UserTypes.M.Equals(allotment.UserType, StringComparison.OrdinalIgnoreCase))
            {
                messages.AppendLine($"Incorrect the user type of the allotment #{allotment.Id}");
                result = false;
            }

            if (allotment.PlanRef == null || allotment.PlanRef.Id == IM.NullI)
            {
                messages.AppendLine($"Undefined channelling plan");
                result = false;
            }

            //if (allotment.CustDate1.IsNull())
            //{
            //    messages.AppendLine($"Undefined a CUST_DAT1 of the allotment #{allotment.Id}");
            //    result = false;
            //}

            //if (allotment.CustDate2.IsNull())
            //{
            //    messages.AppendLine($"Undefined a CUST_DAT2 of the allotment #{allotment.Id}");
            //    result = false;
            //}

            if (string.IsNullOrEmpty(allotment.CustText1))
            {
                messages.AppendLine($"Undefined a CUST_TXT1 of the allotment #{allotment.Id}");
                result = false;
            }

            //if (string.IsNullOrEmpty(allotment.CustText2))
            //{
            //    messages.AppendLine($"Undefined a CUST_TXT2 of the allotment #{allotment.Id}");
            //    result = false;
            //}

            //if (string.IsNullOrEmpty(allotment.CustText3))
            //{
            //    messages.AppendLine($"Undefined a CUST_TXT3 of the allotment #{allotment.Id}");
            //    result = false;
            //}

            if (!result)
            {
                Logger.WriteWarning(PluginMetadata.Processes.CreateMeasTask, messages.ToString(), true);
            }

            return result;
        }
    }
}
