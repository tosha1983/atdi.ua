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
            this.icsDBList1 = new NetPlugins2.IcsDBList();
            this.SuspendLayout();
            // 
            // icsDBList1
            // 
            this.icsDBList1.BackColor = System.Drawing.SystemColors.Control;
            this.icsDBList1.ConfigName = null;
            this.icsDBList1.Filter = null;
            this.icsDBList1.Location = new System.Drawing.Point(0, 0);
            this.icsDBList1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.icsDBList1.Name = "icsDBList1";
            this.icsDBList1.Param1 = 0;
            this.icsDBList1.Param2 = 0;
            this.icsDBList1.Size = new System.Drawing.Size(789, 451);
            this.icsDBList1.TabIndex = 0;
            this.icsDBList1.Table = null;
            // 
            // SelectAreaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.icsDBList1);
            this.Name = "SelectAreaForm";
            this.Text = "SelectArea";
            this.ResumeLayout(false);

        }

        #endregion

        private NetPlugins2.IcsDBList icsDBList1;
    }
}