using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class ShortSensorViewModel
    {
        [WpfColumn("ID", WidthRule = ColumnWidthRule.AutoSize, Width = 50, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public int Id { get; set; }

        [WpfColumn("Name", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string Name { get; set; }

        [WpfColumn("Status", WidthRule = ColumnWidthRule.AutoSize, Width = 100, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string Status { get; set; }

        [WpfColumn("Lower Freq", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public double? LowerFreq { get; set; }

        [WpfColumn("Equip Manufacturer", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string EquipManufacturer { get; set; }

        [WpfColumn("Equip Name", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string EquipName { get; set; }

        [WpfColumn("Equip Code", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string EquipCode { get; set; }

        [WpfColumn("Antenna Gain, MAX", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellRighted", HeaderStyle = "")]
        public double? AntGainMax { get; set; }

        [WpfColumn("Antenna Manufacturer", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string AntManufacturer { get; set; }

        [WpfColumn("Antenna Name", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string AntName { get; set; }

        [WpfColumn("Upper Freq", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellRighted", HeaderStyle = "")]
        public double? UpperFreq { get; set; }

        [WpfColumn("Created By", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string CreatedBy { get; set; }

        [WpfColumn("Rx Loss", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "DataGridCellRighted", HeaderStyle = "")]
        public double? RxLoss { get; set; }

        [WpfColumn("Eouse Date", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "", HeaderStyle = "")]
        public DateTime? EouseDate { get; set; }

        [WpfColumn("Biuse Date", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "", HeaderStyle = "")]
        public DateTime? BiuseDate { get; set; }

        [WpfColumn("Network ID", WidthRule = ColumnWidthRule.AutoSize, Width = 60, CellStyle = "DataGridCellCentered", HeaderStyle = "DataGridHeaderCentered")]
        public string NetworkId { get; set; }

        [WpfColumn("Administration", WidthRule = ColumnWidthRule.AutoSize, Width = 150, CellStyle = "", HeaderStyle = "")]
        public string Administration { get; set; }

        [WpfColumn("Created Date", WidthRule = ColumnWidthRule.AutoSize, Width = 80, CellStyle = "", HeaderStyle = "")]
        public DateTime? DateCreated { get; set; }
    }
}
