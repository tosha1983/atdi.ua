namespace XICSM.ICSControlClient.Forms
{
    partial class CalcSODlg2Form
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeMeas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeMeasurements = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentLon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentLat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(967, 468);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(867, 468);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(12, 468);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 10;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
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
            this.Id,
            this.TimeMeas,
            this.DataRank,
            this.Number,
            this.Status,
            this.TypeMeasurements,
            this.CurrentLon,
            this.CurrentLat,
            this.SensorName});
            this.dataGrid.Location = new System.Drawing.Point(0, -4);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(1057, 455);
            this.dataGrid.TabIndex = 11;
            // 
            // Id
            // 
            this.Id.HeaderText = "ID";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // TimeMeas
            // 
            this.TimeMeas.HeaderText = "Time Meas";
            this.TimeMeas.Name = "TimeMeas";
            this.TimeMeas.ReadOnly = true;
            // 
            // DataRank
            // 
            this.DataRank.HeaderText = "DataRank";
            this.DataRank.Name = "DataRank";
            this.DataRank.ReadOnly = true;
            // 
            // Number
            // 
            this.Number.HeaderText = "Number";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // TypeMeasurements
            // 
            this.TypeMeasurements.HeaderText = "TypeMeasurements";
            this.TypeMeasurements.Name = "TypeMeasurements";
            this.TypeMeasurements.ReadOnly = true;
            // 
            // CurrentLon
            // 
            this.CurrentLon.HeaderText = "CurrentLon";
            this.CurrentLon.Name = "CurrentLon";
            this.CurrentLon.ReadOnly = true;
            // 
            // CurrentLat
            // 
            this.CurrentLat.HeaderText = "CurrentLat";
            this.CurrentLat.Name = "CurrentLat";
            this.CurrentLat.ReadOnly = true;
            // 
            // SensorName
            // 
            this.SensorName.HeaderText = "SensorName";
            this.SensorName.Name = "SensorName";
            this.SensorName.ReadOnly = true;
            this.SensorName.Width = 150;
            // 
            // CalcSODlg2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 503);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "CalcSODlg2Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Measurement results";
            this.Load += new System.EventHandler(this.CalcSODlg2Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeMeas;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeMeasurements;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentLon;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentLat;
        private System.Windows.Forms.DataGridViewTextBoxColumn SensorName;
    }
}