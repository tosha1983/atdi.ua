using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;

namespace XICSM.ICSControlClient.Models.Views
{
    public class RefSpectrumViewModel
    {
        [WpfColumn("Id", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public long? Id { get; set; }

        [WpfColumn("FileName", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string FileName { get; set; }

        [WpfColumn("DateCreated", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public DateTime DateCreated { get; set; }

        [WpfColumn("CreatedBy", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string CreatedBy { get; set; }

        [WpfColumn("CountImportRecords", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int? CountImportRecords { get; set; }

        [WpfColumn("MinFreqMHz", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double? MinFreqMHz { get; set; }

        [WpfColumn("MaxFreqMHz", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double? MaxFreqMHz { get; set; }

        [WpfColumn("CountSensors", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int? CountSensors { get; set; }

        public DataRefSpectrum[] DataRefSpectrum { get; set; }
    }
}
