using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Models.Views
{
    public class ProjectMapViewModel
    {
        [WpfColumn("Id", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public long Id { get; set; }

        [WpfColumn("MapName", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string MapName { get; set; }

        [WpfColumn("MapNote", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string MapNote { get; set; }

        [WpfColumn("OwnerInstance", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string OwnerInstance { get; set; }

        [WpfColumn("OwnerMapId", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public Guid OwnerMapId { get; set; }

        [WpfColumn("CreatedDate", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public DateTimeOffset CreatedDate { get; set; }

        [WpfColumn("StatusCode", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public byte StatusCode { get; set; }

        [WpfColumn("StatusName", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StatusName { get; set; }

        [WpfColumn("StatusNote", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StatusNote { get; set; }

        [WpfColumn("StepUnit", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StepUnit { get; set; }
    }
}
