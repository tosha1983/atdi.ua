namespace XICSM.ICSControlClient.Forms
{
    partial class CalcSODlg3Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.btnGetCsv = new System.Windows.Forms.Button();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.Frequency_MHz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Occupation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationIDs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.countStation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oc23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(1169, 699);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click_1);
            // 
            // btnGetCsv
            // 
            this.btnGetCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetCsv.Location = new System.Drawing.Point(1269, 699);
            this.btnGetCsv.Name = "btnGetCsv";
            this.btnGetCsv.Size = new System.Drawing.Size(75, 23);
            this.btnGetCsv.TabIndex = 12;
            this.btnGetCsv.Text = "Get CSV";
            this.btnGetCsv.UseVisualStyleBackColor = true;
            this.btnGetCsv.Click += new System.EventHandler(this.btnGetCsv_Click);
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Frequency_MHz,
            this.hit,
            this.Occupation,
            this.StationIDs,
            this.countStation,
            this.Oc0,
            this.Oc1,
            this.Oc2,
            this.Oc3,
            this.Oc4,
            this.Oc5,
            this.Oc6,
            this.Oc7,
            this.Oc8,
            this.Oc9,
            this.Oc10,
            this.Oc11,
            this.Oc12,
            this.Oc13,
            this.Oc14,
            this.Oc15,
            this.Oc16,
            this.Oc17,
            this.Oc18,
            this.Oc19,
            this.Oc20,
            this.Oc21,
            this.Oc22,
            this.Oc23});
            this.dataGrid.Location = new System.Drawing.Point(-3, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(1378, 692);
            this.dataGrid.TabIndex = 13;
            // 
            // Frequency_MHz
            // 
            this.Frequency_MHz.HeaderText = "Frequency MHz";
            this.Frequency_MHz.Name = "Frequency_MHz";
            this.Frequency_MHz.ReadOnly = true;
            this.Frequency_MHz.Width = 70;
            // 
            // hit
            // 
            this.hit.HeaderText = "Hit";
            this.hit.Name = "hit";
            this.hit.ReadOnly = true;
            this.hit.Width = 70;
            // 
            // Occupation
            // 
            this.Occupation.HeaderText = "Occupation";
            this.Occupation.Name = "Occupation";
            this.Occupation.ReadOnly = true;
            this.Occupation.Width = 70;
            // 
            // StationIDs
            // 
            this.StationIDs.HeaderText = "StationIDs";
            this.StationIDs.Name = "StationIDs";
            this.StationIDs.ReadOnly = true;
            this.StationIDs.Width = 70;
            // 
            // countStation
            // 
            this.countStation.HeaderText = "countStation";
            this.countStation.Name = "countStation";
            this.countStation.ReadOnly = true;
            this.countStation.Width = 70;
            // 
            // Oc0
            // 
            this.Oc0.HeaderText = "0-1";
            this.Oc0.Name = "Oc0";
            this.Oc0.ReadOnly = true;
            this.Oc0.Width = 40;
            // 
            // Oc1
            // 
            this.Oc1.HeaderText = "1-2";
            this.Oc1.Name = "Oc1";
            this.Oc1.ReadOnly = true;
            this.Oc1.Width = 40;
            // 
            // Oc2
            // 
            this.Oc2.HeaderText = "2-3";
            this.Oc2.Name = "Oc2";
            this.Oc2.ReadOnly = true;
            this.Oc2.Width = 40;
            // 
            // Oc3
            // 
            this.Oc3.HeaderText = "3-4";
            this.Oc3.Name = "Oc3";
            this.Oc3.ReadOnly = true;
            this.Oc3.Width = 40;
            // 
            // Oc4
            // 
            this.Oc4.HeaderText = "4-5";
            this.Oc4.Name = "Oc4";
            this.Oc4.ReadOnly = true;
            this.Oc4.Width = 40;
            // 
            // Oc5
            // 
            this.Oc5.HeaderText = "5-6";
            this.Oc5.Name = "Oc5";
            this.Oc5.ReadOnly = true;
            this.Oc5.Width = 40;
            // 
            // Oc6
            // 
            this.Oc6.HeaderText = "6-7";
            this.Oc6.Name = "Oc6";
            this.Oc6.ReadOnly = true;
            this.Oc6.Width = 40;
            // 
            // Oc7
            // 
            this.Oc7.HeaderText = "7-8";
            this.Oc7.Name = "Oc7";
            this.Oc7.ReadOnly = true;
            this.Oc7.Width = 40;
            // 
            // Oc8
            // 
            this.Oc8.HeaderText = "8-9";
            this.Oc8.Name = "Oc8";
            this.Oc8.ReadOnly = true;
            this.Oc8.Width = 40;
            // 
            // Oc9
            // 
            this.Oc9.HeaderText = "9-10";
            this.Oc9.Name = "Oc9";
            this.Oc9.ReadOnly = true;
            this.Oc9.Width = 40;
            // 
            // Oc10
            // 
            this.Oc10.HeaderText = "10-11";
            this.Oc10.Name = "Oc10";
            this.Oc10.ReadOnly = true;
            this.Oc10.Width = 40;
            // 
            // Oc11
            // 
            this.Oc11.HeaderText = "11-12";
            this.Oc11.Name = "Oc11";
            this.Oc11.ReadOnly = true;
            this.Oc11.Width = 40;
            // 
            // Oc12
            // 
            this.Oc12.HeaderText = "12-13";
            this.Oc12.Name = "Oc12";
            this.Oc12.ReadOnly = true;
            this.Oc12.Width = 40;
            // 
            // Oc13
            // 
            this.Oc13.HeaderText = "13-14";
            this.Oc13.Name = "Oc13";
            this.Oc13.ReadOnly = true;
            this.Oc13.Width = 40;
            // 
            // Oc14
            // 
            this.Oc14.HeaderText = "14-15";
            this.Oc14.Name = "Oc14";
            this.Oc14.ReadOnly = true;
            this.Oc14.Width = 40;
            // 
            // Oc15
            // 
            this.Oc15.HeaderText = "15-16";
            this.Oc15.Name = "Oc15";
            this.Oc15.ReadOnly = true;
            this.Oc15.Width = 40;
            // 
            // Oc16
            // 
            this.Oc16.HeaderText = "16-17";
            this.Oc16.Name = "Oc16";
            this.Oc16.ReadOnly = true;
            this.Oc16.Width = 40;
            // 
            // Oc17
            // 
            this.Oc17.HeaderText = "17-18";
            this.Oc17.Name = "Oc17";
            this.Oc17.ReadOnly = true;
            this.Oc17.Width = 40;
            // 
            // Oc18
            // 
            this.Oc18.HeaderText = "18-19";
            this.Oc18.Name = "Oc18";
            this.Oc18.ReadOnly = true;
            this.Oc18.Width = 40;
            // 
            // Oc19
            // 
            this.Oc19.HeaderText = "19-20";
            this.Oc19.Name = "Oc19";
            this.Oc19.ReadOnly = true;
            this.Oc19.Width = 40;
            // 
            // Oc20
            // 
            this.Oc20.HeaderText = "20-21";
            this.Oc20.Name = "Oc20";
            this.Oc20.ReadOnly = true;
            this.Oc20.Width = 40;
            // 
            // Oc21
            // 
            this.Oc21.HeaderText = "21-22";
            this.Oc21.Name = "Oc21";
            this.Oc21.ReadOnly = true;
            this.Oc21.Width = 40;
            // 
            // Oc22
            // 
            this.Oc22.HeaderText = "22-23";
            this.Oc22.Name = "Oc22";
            this.Oc22.ReadOnly = true;
            this.Oc22.Width = 40;
            // 
            // Oc23
            // 
            this.Oc23.HeaderText = "23-24";
            this.Oc23.Name = "Oc23";
            this.Oc23.ReadOnly = true;
            this.Oc23.Width = 40;
            // 
            // CalcSODlg3Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 729);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btnGetCsv);
            this.Controls.Add(this.btnOk);
            this.Name = "CalcSODlg3Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Measurement results";
            this.Load += new System.EventHandler(this.CalcSODlg3Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnGetCsv;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Frequency_MHz;
        private System.Windows.Forms.DataGridViewTextBoxColumn hit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Occupation;
        private System.Windows.Forms.DataGridViewTextBoxColumn StationIDs;
        private System.Windows.Forms.DataGridViewTextBoxColumn countStation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc0;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc20;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc21;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc22;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oc23;
    }
}