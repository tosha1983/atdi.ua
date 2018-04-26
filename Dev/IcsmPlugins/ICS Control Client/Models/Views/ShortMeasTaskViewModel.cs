using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class ShortMeasTaskViewModel
    {
        [WpfColumn("ID", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int Id { get; set; }

        [WpfColumn("Order", WidthRule = ColumnWidthRule.AutoSize, Width = 50)]
        public int OrderId { get; set; }

        [WpfColumn("Status", WidthRule = ColumnWidthRule.AutoSize, Width = 100)]
        public string Status { get; set; }

        [WpfColumn("Type", WidthRule = ColumnWidthRule.AutoSize, Width = 100)]
        public string Type { get; set; }

        [WpfColumn("Name", WidthRule = ColumnWidthRule.AutoSize, Width = 200)]
        public string Name { get; set; }

        [WpfColumn("Execution Mode", WidthRule = ColumnWidthRule.AutoSize, Width = 100)]
        public MeasTaskExecutionMode ExecutionMode { get; set; }

        [WpfColumn("Task Type", WidthRule = ColumnWidthRule.AutoSize, Width = 100)]
        public MeasTaskType Task { get; set; }

        [WpfColumn("Prio", WidthRule = ColumnWidthRule.AutoSize, Width = 50)]
        public int? Prio { get; set; }

        [WpfColumn("Result Type", WidthRule = ColumnWidthRule.AutoSize, Width = 150)]
        public MeasTaskResultType ResultType { get; set; }

        [WpfColumn("Measurements Type", WidthRule = ColumnWidthRule.AutoSize, Width = 100)]
        public MeasurementType TypeMeasurements { get; set; }

        [WpfColumn("MAX Time, Bs", WidthRule = ColumnWidthRule.AutoSize, Width = 60)]
        public int? MaxTimeBs { get; set; }

        [WpfColumn("Created Date", WidthRule = ColumnWidthRule.AutoSize, Width = 80)]
        public DateTime? DateCreated { get; set; }

        [WpfColumn("Created By", WidthRule = ColumnWidthRule.AutoSize, Width = 150)]
        public string CreatedBy { get; set; }
    }
}
