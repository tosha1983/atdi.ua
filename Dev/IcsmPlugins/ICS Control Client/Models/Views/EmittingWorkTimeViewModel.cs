using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class EmittingWorkTimeViewModel
    {
        [WpfColumn("StartEmitting", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public DateTime StartEmitting { get; set; }

        [WpfColumn("StopEmitting", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public DateTime StopEmitting { get; set; }

        [WpfColumn("HitCount", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int HitCount { get; set; }

        [WpfColumn("PersentAvailability", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public float PersentAvailability { get; set; }
    }
}
