namespace XICSM.ICSControlClient.Forms
{
    partial class SelectAreaForm
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
            this.icsDBList_Areas = new NetPlugins2.IcsDBList();
            this.btnSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // icsDBList_Areas
            // 
            this.icsDBList_Areas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.icsDBList_Areas.BackColor = System.Drawing.SystemColors.Control;
            this.icsDBList_Areas.ConfigName = null;
            this.icsDBList_Areas.Filter = null;
            this.icsDBList_Areas.Location = new System.Drawing.Point(11, 11);
            this.icsDBList_Areas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.icsDBList_Areas.Name = "icsDBList_Areas";
            this.icsDBList_Areas.Param1 = 0;
            this.icsDBList_Areas.Param2 = 0;
            this.icsDBList_Areas.Size = new System.Drawing.Size(1451, 519);
            this.icsDBList_Areas.TabIndex = 0;
            this.icsDBList_Areas.Table = null;
            // 
            // btnSelect
            // 
            this.btnSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Location = new System.Drawing.Point(1209, 554);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(220, 35);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // SelectAreaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1473, 607);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.icsDBList_Areas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectAreaForm";
            this.Text = "SelectArea";
            this.Load += new System.EventHandler(this.SelectAreaForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private NetPlugins2.IcsDBList icsDBList_Areas;
        private System.Windows.Forms.Button btnSelect;
    }
}