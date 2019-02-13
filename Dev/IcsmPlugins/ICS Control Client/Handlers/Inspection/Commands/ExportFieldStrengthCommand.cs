using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.StartMeasurementsSO;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;
using FRM = System.Windows.Forms;

namespace XICSM.ICSControlClient.Handlers.InspectionCommnads
{
    public class Inspection
    {
        public int id;
        public int taskId;
        public int sectorId;
        public int stationId;
        public string stationName;
        public string status;
    }

    public class ExportFieldStrengthCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            List<Inspection> selectedInspections = new List<Inspection>();

            IMRecordset rs = new IMRecordset(context.TableName, IMRecordset.Mode.ReadWrite);
            rs.Select("ID,STATUS,InspTour.CUST_NBR1,Station.TABLE_ID,Station.ID,Station.NAME");
            rs.AddSelectionFrom(context.DataList, IMRecordset.WhereCopyOptions.SelectedLines);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                selectedInspections.Add(new Inspection { id = rs.GetI("ID"), taskId = rs.GetI("InspTour.CUST_NBR1"), status = rs.GetS("STATUS"), sectorId = rs.GetI("Station.TABLE_ID"), stationId = rs.GetI("Station.ID"), stationName = rs.GetS("Station.NAME") });
            }
            return CreateFilesCSV(selectedInspections.ToArray());
        }
        private static bool CreateFilesCSV(Inspection[] selectedInspections)
        {
            try
            {
                bool isFewRecords = selectedInspections.Count() > 1;
                string folderName = "";

                if (isFewRecords)
                {
                    FRM.FolderBrowserDialog sfd = new FRM.FolderBrowserDialog();
                    FRM.DialogResult result = sfd.ShowDialog();
                    if (result == FRM.DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.SelectedPath))
                        folderName = sfd.SelectedPath;
                    else
                        return true;
                }

                foreach (var insp in selectedInspections)
                {
                    List<int> measResultIds = new List<int>();

                    if (insp.id == IM.NullI || insp.taskId == IM.NullI || insp.sectorId == IM.NullI)
                    {
                        if (isFewRecords)
                            continue;
                        else
                        {
                            System.Windows.MessageBox.Show("Undefined value TaskId or SectorId");
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(insp.status) || insp.status.Equals("New", StringComparison.OrdinalIgnoreCase))
                    {
                        if (isFewRecords)
                            continue;
                        else
                        {
                            System.Windows.MessageBox.Show("No measurement result found for the station");
                            continue;
                        }
                    }

                    var shortMeasResults = SVC.SdrnsControllerWcfClient.GetShortMeasResultsByTypeAndTaskId(SDR.MeasurementType.MonitoringStations, insp.taskId);
                    if (isFewRecords)
                    {
                        foreach (var shortMeasResult in shortMeasResults)
                        {
                            measResultIds.Add(shortMeasResult.Id.MeasSdrResultsId);
                        }
                    }
                    else
                    {
                        var dlgForm = new FM.ExportFieldStrengthForm();
                        dlgForm._shortMeasResults = shortMeasResults;
                        dlgForm.ShowDialog();
                        dlgForm.Dispose();
                        measResultIds = dlgForm._measResultIds;
                    }
                    CreateFileCSV(measResultIds, insp, isFewRecords, folderName);
                }
                if (isFewRecords)
                    System.Windows.MessageBox.Show("Your files was generated and its ready for use.");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }

            return true;
        }
        private static void CreateFileCSV(List<int> MeasResultID, Inspection inspection, bool isFewRecords, string folderName)
        {
            try
            {
                var stationLevels = SVC.SdrnsControllerWcfClient.GetStationLevelsByTask(MeasResultID, inspection.taskId, inspection.sectorId);
                //var stationLevels = SVC.SdrnsControllerWcfClient.GetStationLevelsByTask(new List<int>(new int[] { 421, 422, 423 }), 265, 1348418);
                 
                int recCount = stationLevels.Count();
                if (recCount == 0 && !isFewRecords)
                {
                    System.Windows.MessageBox.Show("No data for export.");
                    return;
                }

                int i = 0;
                string[] output = new string[recCount + 1];
                output[0] += "Lon;Lat;Level";

                foreach (var stationLevel in stationLevels)
                {
                    output[i + 1] += stationLevel.Lon.ToString() + ";" + stationLevel.Lat.ToString() + ";" + stationLevel.Level_dBmkVm.ToString() + ";";
                    i++;
                }

                string fileName = "FS_Meas_Res_" + inspection.id.ToString() + "_" + inspection.stationId.ToString() + "_" + inspection.stationName + ".csv";
                if (isFewRecords)
                    System.IO.File.WriteAllLines(Path.Combine(folderName, fileName), output, System.Text.Encoding.UTF8);
                else
                {
                    FRM.SaveFileDialog sfd = new FRM.SaveFileDialog();
                    sfd.Filter = "CSV (*.csv)|*.csv";
                    sfd.FileName = fileName;
                    if (sfd.ShowDialog() == FRM.DialogResult.OK)
                    {
                        System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                        System.Windows.MessageBox.Show("Your file was generated and its ready for use.");
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }
    }
}
