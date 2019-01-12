using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class ShortResultsMeasurementsStationViewModel
    {
        [WpfColumn("StationId", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StationId { get; set; }

        [WpfColumn("SectorId", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int? SectorId { get; set; }

        [WpfColumn("Status", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Status { get; set; }

        [WpfColumn("GlobalSID", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string GlobalSID { get; set; }

        [WpfColumn("MeasGlobalSID", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string MeasGlobalSID { get; set; }

        [WpfColumn("Central Frequency Meas", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double? GeneralResultCentralFrequencyMeas { get; set; }

        [WpfColumn("Standard", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Standard { get; set; }

        [WpfColumn("Station Locations", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "", HeaderStyle = "")]
        public SiteStationForMeas[] StationLocations { get; set; }
    }
}
