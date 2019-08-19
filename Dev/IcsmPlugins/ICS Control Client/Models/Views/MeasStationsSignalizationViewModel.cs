using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasStationsSignalizationViewModel
    {
        [WpfColumn("IcsmTable", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string IcsmTable { get; set; }

        [WpfColumn("IcsmId", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public long IcsmId { get; set; }

        [WpfColumn("StationName", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StationName { get; set; }

        [WpfColumn("Standart", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Standart { get; set; }

        [WpfColumn("Status", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Status { get; set; }

        [WpfColumn("Lon", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Lon { get; set; }

        [WpfColumn("Lat", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Lat { get; set; }

        [WpfColumn("Agl", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Agl { get; set; }

        [WpfColumn("Eirp", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Eirp { get; set; }

        [WpfColumn("Bw", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Bw { get; set; }

        [WpfColumn("Freq", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Freq { get; set; }

        [WpfColumn("Owner", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Owner { get; set; }

        [WpfColumn("RelivedLevel", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double RelivedLevel { get; set; }

        [WpfColumn("Distance", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Distance { get; set; }
    }
}
