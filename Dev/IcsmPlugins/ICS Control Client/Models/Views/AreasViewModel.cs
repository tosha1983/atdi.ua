using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;

namespace XICSM.ICSControlClient.Models.Views
{
    public class AreasViewModel
    {
        [WpfColumn("IdentifierFromICSM", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int IdentifierFromICSM { get; set; }

        [WpfColumn("Name", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Name { get; set; }

        [WpfColumn("TypeArea", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string TypeArea { get; set; }

        [WpfColumn("CreatedBy", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string CreatedBy { get; set; }

        [WpfColumn("DateCreated", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public DateTime? DateCreated { get; set; }

        public DataLocation[] Location { get; set; }
    }
}
