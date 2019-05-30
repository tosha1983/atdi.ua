namespace XICSM.ICSControlClient.Forms
{
    partial class MeasStationsSignalizationDlg1Form
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblBw = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.icsDistance = new NetPlugins2.IcsDouble();
            this.icsBw = new NetPlugins2.IcsDouble();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Distance, km";
            // 
            // lblBw
            // 
            this.lblBw.AutoSize = true;
            this.lblBw.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblBw.Location = new System.Drawing.Point(12, 98);
            this.lblBw.Name = "lblBw";
            this.lblBw.Size = new System.Drawing.Size(71, 20);
            this.lblBw.TabIndex = 1;
            this.lblBw.Text = "BW, kHz";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(118, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // icsDistance
            // 
            this.icsDistance.Location = new System.Drawing.Point(133, 41);
            this.icsDistance.Margin = new System.Windows.Forms.Padding(0);
            this.icsDistance.Name = "icsDistance";
            this.icsDistance.Size = new System.Drawing.Size(154, 18);
            this.icsDistance.Subtype = "Number";
            this.icsDistance.TabIndex = 5;
            this.icsDistance.ValueChanged += new System.EventHandler(this.icsDistance_ValueChanged);
            // 
            // icsBw
            // 
            this.icsBw.Location = new System.Drawing.Point(133, 100);
            this.icsBw.Margin = new System.Windows.Forms.Padding(0);
            this.icsBw.Name = "icsBw";
            this.icsBw.Size = new System.Drawing.Size(154, 18);
            this.icsBw.Subtype = null;
            this.icsBw.TabIndex = 6;
            this.icsBw.ValueChanged += new System.EventHandler(this.icsBw_ValueChanged);
            // 
            // MeasStationsSignalizationDlg1Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 179);
            this.Controls.Add(this.icsBw);
            this.Controls.Add(this.icsDistance);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblBw);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeasStationsSignalizationDlg1Form";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Station in ICSM - Input values";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblBw;
        private System.Windows.Forms.Button button1;
        private NetPlugins2.IcsDouble icsDistance;
        private NetPlugins2.IcsDouble icsBw;
    }
}