namespace XICSM.ICSControlClient.Forms
{
    partial class StationListForm
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
            this.icsDBList_Station = new NetPlugins2.IcsDBList();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Location = new System.Drawing.Point(1209, 557);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(220, 35);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // icsDBList_Station
            // 
            this.icsDBList_Station.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.icsDBList_Station.BackColor = System.Drawing.SystemColors.Control;
            this.icsDBList_Station.ConfigName = null;
            this.icsDBList_Station.Filter = null;
            this.icsDBList_Station.Location = new System.Drawing.Point(11, 14);
            this.icsDBList_Station.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.icsDBList_Station.Name = "icsDBList_Station";
            this.icsDBList_Station.Param1 = 0;
            this.icsDBList_Station.Param2 = 0;
            this.icsDBList_Station.Size = new System.Drawing.Size(1451, 519);
            this.icsDBList_Station.TabIndex = 2;
            this.icsDBList_Station.Table = null;
            this.icsDBList_Station.OnQuery += new System.EventHandler(this.icsDBList_Station_OnRequery);
            // 
            // StationListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1473, 607);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.icsDBList_Station);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StationListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stations";
            this.Load += new System.EventHandler(this.StationListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private NetPlugins2.IcsDBList icsDBList_Station;
    }
}