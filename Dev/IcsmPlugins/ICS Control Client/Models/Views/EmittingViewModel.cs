using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class EmittingViewModel
    {
        [WpfColumn("StartFrequency_MHz", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double StartFrequency_MHz { get; set; }

        [WpfColumn("StopFrequency_MHz", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double StopFrequency_MHz { get; set; }

        [WpfColumn("CurentPower_dBm", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double CurentPower_dBm { get; set; }

        [WpfColumn("ReferenceLevel_dBm", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double ReferenceLevel_dBm { get; set; }

        [WpfColumn("MeanDeviationFromReference", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double MeanDeviationFromReference { get; set; }

        [WpfColumn("TriggerDeviationFromReference", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double TriggerDeviationFromReference { get; set; }

        [WpfColumn("Bandwidth_kHz", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double Bandwidth_kHz { get; set; }

        [WpfColumn("CorrectnessEstimations", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public bool CorrectnessEstimations { get; set; }

        [WpfColumn("TraceCount", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int TraceCount { get; set; }

        [WpfColumn("SignalLevel_dBm", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public float SignalLevel_dBm { get; set; }

        [WpfColumn("RollOffFactor", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double RollOffFactor { get; set; }

        [WpfColumn("StandardBW", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double StandardBW { get; set; }

        [WpfColumn("SensorName", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string SensorName { get; set; }

        public WorkTime[] WorkTimes { get; set; }
        public Spectrum Spectrum { get; set; }
        public LevelsDistribution LevelsDistribution { get; set; }
    }
}
