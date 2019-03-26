using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.CreateMeasTask;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using AAC = Atdi.DataModels.DataConstraint;

namespace XICSM.ICSControlClient.Handlers.TourCommnads
{
    public class CreateMeasTaskCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.CreateMeasTask,
                        CreateMeasTaskByTour
                    );
        }

        private static bool CreateMeasTaskByTour(int tourId)
        {
            var tour = Repository.ReadEntityById<DM.Tour>(tourId);
            if (!ValidateTour(tour))
            {
                return false;
            }

            var inspactions = FindInspections(tour);
            if(inspactions == null || inspactions.Length == 0)
            {
                throw new InvalidOperationException($"Not found any inspection records by the tour #{tour.Id}");
            }

            var measTask = PreparedMeasTaskInfo(tour, inspactions);
            SaveTaskToLog(measTask);

            var measTaskId = WCF.SdrnsControllerWcfClient.CreateMeasTask(measTask);
            if (measTaskId == IM.NullI)
            {
                throw new InvalidOperationException($"Could not create a meas task by the tour #{tour.Id}");
            }

            tour.Status = MD.Tours.Statuses.Dur;
            tour.MeasTaskId = measTaskId;
            Repository.UpdateEntity(tour);

            return true;
        }

        private static DM.Inspection[] FindInspections(DM.Tour tour)
        {
            return Repository.GetEntities<DM.Inspection>(source =>
            {
                source.SetWhere(MD.Inspection.Fields.TourId, IMRecordset.Operation.Eq, tour.Id);
            });
        }

        private static void SaveTaskToLog(SDR.MeasTask measTask)
        {
            try
            {
                var data = new StringBuilder();

                data.AppendLine($"Created meas task:");
                data.AppendLine($"Name: {measTask.Name}");
                data.AppendLine($"Mode: {measTask.ExecutionMode}");
                data.AppendLine($"Created: {measTask.DateCreated}");
                data.AppendLine($"Stations: {measTask.Stations.Length}");
                data.AppendLine($"StationsForMeasurements: {measTask.StationsForMeasurements.Length}");

                Logger.WriteInfo(PluginMetadata.Processes.CreateMeasTask, data.ToString());
            }
            catch(Exception e)
            {
                Logger.WriteExeption(PluginMetadata.Processes.SaveTaskToLog, e);
            }

        }
        private static SDR.MeasTask PreparedMeasTaskInfo(DM.Tour tour, DM.Inspection[] inspections)
        {
            var measTask = new SDR.MeasTask()
            {
                Name = tour.MeasTaskName,
                ExecutionMode = SDR.MeasTaskExecutionMode.Manual,
                DateCreated = DateTime.Now,
                CreatedBy = IM.ConnectedUser(),
                MeasDtParam = PreparedDetectedParam(tour),
                MeasTimeParamList = new SDR.MeasTimeParamList() { PerStart = tour.StartDate, PerStop = tour.StopDate },
                Stations = PreparedStations(tour), // new SDR.MeasStation[] { } ,
                StationsForMeasurements = PreparedStationDataForMeasurements(tour, inspections),
                
            };

            return measTask;
        }

        private static SDR.MeasStation[] PreparedStations(DM.Tour tour)
        {
            var stations = new List<SDR.MeasStation>();

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
                        RightOperand = new AAC.StringValueOperand { Type = AAC.OperandType.Value, Value = tour.SensorName}
                    },
                    new AAC.ConditionExpression
                    {
                        Type = AAC.ConditionType.Expression,
                        Operator = AAC.ConditionOperator.Equal,
                        LeftOperand = new AAC.ColumnOperand { Type = AAC.OperandType.Column, ColumnName = "TechId", Source = "Sensor"},
                        RightOperand = new AAC.StringValueOperand { Type = AAC.OperandType.Value, Value = tour.SensorEquipTechId}
                    }
                }
            };
            var result = WCF.SdrnsControllerWcfClient.GetSensors(constraint);
                   
            result = result.Where(u => u.Name == tour.SensorName && u.Equipment.TechId == tour.SensorEquipTechId).ToArray();

            if (result.Length > 0)
            {
                stations.AddRange(
                    result.Select(sensor => new SDR.MeasStation
                    {
                        StationId = new SDR.MeasStationIdentifier
                        {
                            Value = sensor.Id.Value
                        }
                    })
                );
            }
            return stations.ToArray();
        }

        private static SDR.MeasDtParam PreparedDetectedParam(DM.Tour tour)
        {
            var detectedParam = new SDR.MeasDtParam()
            {
                TypeMeasurements = SDR.MeasurementType.MonitoringStations
            };

            return detectedParam;
        }

        private static SDR.StationDataForMeasurements[] PreparedStationDataForMeasurements(DM.Tour tour, DM.Inspection[] inspections)
        {
            var stations = new List<SDR.StationDataForMeasurements>();

            var applications = new Dictionary<int, DM.NfraApplication>();

            inspections.ToList().ForEach(inspection =>
            {
                if (MD.MobStations.TableName.Equals(inspection.StationTableName, StringComparison.OrdinalIgnoreCase))
                {
                    var mobStation = Repository.ReadEntityById<DM.MobStation>(inspection.StationTableId);
                    

                    var application = Repository.ReadFirstEntity<DM.NfraApplication>(source => 
                    {
                        source.SetWhere(MD.NfraApplication.Fields.ObjTable, IMRecordset.Operation.Eq, inspection.StationTableName);
                        source.SetAdditional($"({inspection.StationTableId} in ([{MD.NfraApplication.Fields.ObjId1}], [{MD.NfraApplication.Fields.ObjId2}], [{MD.NfraApplication.Fields.ObjId3}], [{MD.NfraApplication.Fields.ObjId4}], [{MD.NfraApplication.Fields.ObjId5}], [{MD.NfraApplication.Fields.ObjId6}]))");
                    });

                    if (application == null)
                    {
                        throw new InvalidOperationException($"Not found record of {MD.NfraApplication.TableName} by ObjId #{inspection.StationTableId} and TableName '{inspection.StationTableName}'");
                    }

                    mobStation.Frequencies = Repository
                        .GetEntities<DM.MobStationFrequencies>(
                                source => source.SetWhere(MD.MobStationFrequencies.Fields.StationId, IMRecordset.Operation.Eq, mobStation.Id)
                            );

                    if (applications.ContainsKey(application.Id))
                    {
                        applications[application.Id].MobStations.Add(mobStation);
                    }
                    else
                    {
                        application.MobStations = new List<DM.MobStation>();
                        application.MobStations.Add(mobStation);
                        applications.Add(application.Id, application);
                    }
                }
            });

            applications.Values.ToList().ForEach(app =>
            {
                var firstStation = app.MobStations[0];
                var stationData = new SDR.StationDataForMeasurements
                {
                    Owner = new SDR.OwnerData
                    {
                        Id = firstStation.OwnerRef.Id,
                        OwnerName = firstStation.OwnerRef.Name,
                        Addres = firstStation.OwnerRef.Address,
                        Code = firstStation.OwnerRef.Code,
                        OKPO = firstStation.OwnerRef.RegistNum,
                        Zip = firstStation.OwnerRef.PostCode
                    },
                    IdStation = string.IsNullOrEmpty(firstStation.CustTxt13) ? firstStation.Name.TryToInt() : firstStation.CustTxt13.TryToInt(),
                    Site = new SDR.SiteStationForMeas
                    {
                        Adress = firstStation.PositionRef.Address,
                        Lat = firstStation.PositionRef.Latitude,
                        Lon = firstStation.PositionRef.Longitude,
                        Region = firstStation.PositionRef.SubProvince
                    },
                    Sectors = app.MobStations.Select(mobStation => 
                            new SDR.SectorStationForMeas
                            {
                                AGL = mobStation.Agl,
                                IdSector = mobStation.Id,
                                EIRP = mobStation.Power,
                                Azimut = mobStation.Azimut,
                                BW = mobStation.BW,
                                ClassEmission = mobStation.DesignEmission,
                                MaskBW = new SDR.MaskElements[] { },
                                Frequencies = mobStation.Frequencies.Select(
                                    freq => new SDR.FrequencyForSectorFormICSM
                                    {
                                        ChannalNumber = freq.ChannelTxRef.Channel,
                                        Frequency = freq.ChannelTxRef.Freq,
                                        Id = freq.Id,
                                        IdPlan = freq.ChannelTxRef.PlanId
                                    }).ToArray()
                            }).ToArray(),
                    Standart = firstStation.Standart,
                    LicenseParameter = new SDR.PermissionForAssignment
                    {
                        CloseDate = app.DozvDateCancel,
                        DozvilName = app.DozvNum,
                        EndDate = app.DozvDateTo,
                        StartDate = app.DozvDateFrom,
                        Id = app.Id
                    }
                };

                stations.Add(stationData);
            });

            return stations.ToArray();
        }

        private static bool ValidateTour(DM.Tour tour)
        {
            var messages = new StringBuilder();
            bool result = true;

            if (!tour.IsNewState)
            {
                messages.AppendLine($"Incorrect the status of the tour #{tour.Id}");
                result = false;
            }

            if (tour.StartDate.IsNull())
            {
                messages.AppendLine($"Undefined a start date of the tour #{tour.Id}");
                result = false;
            }

            if (tour.StopDate.IsNull())
            {
                messages.AppendLine($"Undefined a stop date of the tour #{tour.Id}");
                result = false;
            }

            if (tour.MeasTaskId != IM.NullI)
            {
                messages.AppendLine($"For the tour #{tour.Id} the meas task already creted with Id #{tour.MeasTaskId}");
                result = false;
            }

            if (string.IsNullOrEmpty(tour.MeasTaskName))
            {
                messages.AppendLine($"Undefined a meas task name of the tour #{tour.Id}");
                result = false;
            }

            if (string.IsNullOrEmpty(tour.SensorName))
            {
                messages.AppendLine($"Undefined a sensor name of the tour #{tour.Id}");
                result = false;
            }

            if (string.IsNullOrEmpty(tour.SensorEquipTechId))
            {
                messages.AppendLine($"Undefined a sensor equipment tech ID of the tour #{tour.Id}");
                result = false;
            }

            if (!result)
            {
                Logger.WriteWarning(PluginMetadata.Processes.CreateMeasTask, messages.ToString(), true);
            }

            return result;
        }
    }
}
