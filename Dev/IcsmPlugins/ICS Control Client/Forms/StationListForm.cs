using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XICSM.ICSControlClient.Forms
{
    public partial class StationListForm : Form
    {
        public string stationIDs;
        public StationListForm()
        {
            InitializeComponent();
        }
        private void StationListForm_Load(object sender, EventArgs e)
        {
            icsDBList_Station.Init();
            icsDBList_Station.AddColumn("MOB_STATION.ID", "ID", "Auto", 48);
            icsDBList_Station.AddColumn("MOB_STATION.NAME", "Name", "Auto", 354);
            icsDBList_Station.AddColumn("MOB_STATION.REMARK", "Remarks", "Auto", 354);
            icsDBList_Station.AddColumn("MOB_STATION.Position.LONGITUDE", "LONGITUDE", "Auto", 100);
            icsDBList_Station.AddColumn("MOB_STATION.Position.LATITUDE", "LATITUDE", "Auto", 100);
            icsDBList_Station.Table = "MOB_STATION";
            icsDBList_Station.Requery();
        }
        private void icsDBList_Station_OnRequery(object sender, EventArgs e)
        {
            icsDBList_Station.SetFilter(string.Format(" ([ID] in ({0})) ", stationIDs));
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
