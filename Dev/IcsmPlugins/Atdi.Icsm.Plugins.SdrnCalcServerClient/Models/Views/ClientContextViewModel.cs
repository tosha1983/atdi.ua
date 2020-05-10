using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Models.Views
{
    public class ClientContextViewModel
    {
        [WpfColumn("Id", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public long Id { get; set; }

        [WpfColumn("Name", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Name { get; set; }

        [WpfColumn("Note", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Note { get; set; }

        [WpfColumn("OwnerInstance", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string OwnerInstance { get; set; }

        [WpfColumn("OwnerContextId", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public Guid OwnerContextId { get; set; }

        [WpfColumn("CreatedDate", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public DateTimeOffset CreatedDate { get; set; }

        [WpfColumn("TypeCode", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public byte TypeCode { get; set; }

        [WpfColumn("TypeName", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string TypeName { get; set; }

        [WpfColumn("StatusCode", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public byte StatusCode { get; set; }

        [WpfColumn("StatusName", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StatusName { get; set; }

        [WpfColumn("StatusNote", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string StatusNote { get; set; }
    }
}
