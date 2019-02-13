using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;


namespace XICSM.ICSControlClient.Models.Views
{
    public class ShortMeasurementResultsViewModel
    {
        [WpfColumn("Meas SDR Results ID", WidthRule = ColumnWidthRule.AutoSize, Width = 60, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int MeasSdrResultsId { get; set; }

        [WpfColumn("Number", WidthRule = ColumnWidthRule.AutoSize, Width = 120, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int Number { get; set; }

        [WpfColumn("Meas Task ID", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int MeasTaskId{ get; set; }

        [WpfColumn("Status", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Status { get; set; }

        [WpfColumn("Sub Meas Task ID", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int SubMeasTaskId { get; set; }

        [WpfColumn("Sub Meas Task Station ID", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int SubMeasTaskStationId { get; set; }

        [WpfColumn("Time Meas", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "", HeaderStyle = "")]
        public DateTime? TimeMeas { get; set; }

        [WpfColumn("Data Rank", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int? DataRank { get; set; }

        [WpfColumn("Measurement Type", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "", HeaderStyle = "")]
        public MeasurementType TypeMeasurements { get; set; }

        [WpfColumn("Sensor Name", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "", HeaderStyle = "")]
        public string SensorName { get; set; }

        [WpfColumn("Sensor Tech Id", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "", HeaderStyle = "")]
        public string SensorTechId { get; set; }

        [WpfColumn("Count Station Measurements", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int? CountStationMeasurements { get; set; }

        [WpfColumn("Count Unknown Station Measurements", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int? CountUnknownStationMeasurements { get; set; }
    }
}
