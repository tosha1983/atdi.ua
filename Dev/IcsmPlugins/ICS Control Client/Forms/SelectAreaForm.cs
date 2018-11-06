using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSM;
//using DM = XICSM.ICSControlClient.Models.SynchroInspections;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SelectAreaForm : Form
    {
        public int tourId;
        public SelectAreaForm()
        {
            InitializeComponent();
        }
        private void SelectAreaForm_Load(object sender, EventArgs e)
        {
            icsDBList_Areas.Init();
            icsDBList_Areas.AddColumn("AREA.ID", "ID", "Auto", 48);
            icsDBList_Areas.AddColumn("AREA.NAME", "Area Name", "Auto", 354);
            icsDBList_Areas.AddColumn("AREA.DENSITY", "Type_of_the_area", "Auto", 142);
            icsDBList_Areas.AddColumn("AREA.FAMILY", "Family_of_the_area", "Auto", 195);
            icsDBList_Areas.AddColumn("AREA.DistanceToPoint", "Distance to the reference point", "Auto", 142);
            icsDBList_Areas.AddColumn("AREA.LONGITUDE", "Longitude (Deg)", "Auto", 159);
            icsDBList_Areas.AddColumn("AREA.LATITUDE", "Latitude (Deg)", "Auto", 159);
            icsDBList_Areas.AddColumn("AREA.CSYS", "Coordinate system ", "Auto", 159);
            icsDBList_Areas.AddColumn("AREA.COUNTRY", "Country", "Auto", 51);
            icsDBList_Areas.AddColumn("AREA.DATE_CREATED", "DATE_CREATED", "Auto", 177);
            icsDBList_Areas.AddColumn("AREA.POINTS", "List of vertices for the polygon description", "Auto", 354);
            icsDBList_Areas.Table = "AREA";
            icsDBList_Areas.Requery();

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string points = "";
            string csys = "";

            if (icsDBList_Areas.GetCurrentLineData("POINTS", ref points))
            {
                if (!icsDBList_Areas.GetCurrentLineData("CSYS", ref csys) || ((!"4DEC".Equals(csys, StringComparison.OrdinalIgnoreCase)) && (!"4DMS".Equals(csys, StringComparison.OrdinalIgnoreCase))))
                {
                    MessageBox.Show("Polygon mast have coordinate system 4DEC or 4DMS", "SelectAreaCommand", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var source = new IMRecordset("INSP_TOUR", IMRecordset.Mode.ReadWrite);
                source.Select("ID,CUST_TXT9");
                source.SetWhere("ID", IMRecordset.Operation.Eq, tourId.ToString());
                using (source.OpenWithScope())
                {
                    if (source.IsEOF())
                    {
                        throw new InvalidOperationException($"Not found a record of INSP_TOUR by Id #{tourId.ToString()}");
                    }

                    var newpoints = ConvertPoints(points, csys);
                    source.Edit();
                    source.Put("CUST_TXT9", newpoints);
                    source.Update();
                }
            }
            this.Close();
        }
        private string ConvertPoints(string inputPoints, string csys)
        {
            string outputPoints = "";
            if (string.IsNullOrEmpty(inputPoints))
                return outputPoints;

            foreach (var a in inputPoints.Split(new[] { "\r\n" }, StringSplitOptions.None))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    string[] b = a.Split(new[] { "\t" }, StringSplitOptions.None);
                    if (b.Length == 2)
                    {
                        string coord1 = "";
                        string coord2 = "";
                        if ("4DMS".Equals(csys, StringComparison.OrdinalIgnoreCase))
                        {
                            double k1;
                            double k2;
                            if (double.TryParse(b[0].Replace('.', ','), out k1) && double.TryParse(b[1].Replace('.', ','), out k2))
                            {
                                coord1 = IMPosition.Dms2Dec(k1).ToString().Replace(',', '.');
                                coord2 = IMPosition.Dms2Dec(k2).ToString().Replace(',', '.');
                            }
                        }
                        else
                        {
                            coord1 = b[0];
                            coord2 = b[1];
                        }
                        if (string.IsNullOrEmpty(outputPoints))
                            outputPoints = string.Join(",", new[] { coord2, coord1 });
                        else
                            outputPoints = outputPoints + "," + string.Join(",", new[] { coord2, coord1 });
                    }
                }
            }
            return outputPoints;
        }
    }
}
