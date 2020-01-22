namespace XICSM.ICSControlClient.Forms
{
    partial class gridFilterNumeric
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
            this.label2 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.txtFrom = new NetPlugins2.IcsDouble();
            this.txtTo = new NetPlugins2.IcsDouble();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(163, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "To";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(108, 53);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(45, 24);
            this.txtFrom.Margin = new System.Windows.Forms.Padding(0);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(103, 18);
            this.txtFrom.Subtype = "Number";
            this.txtFrom.TabIndex = 5;
            this.txtFrom.ValueChanged += new System.EventHandler(this.txtFrom_ValueChanged);
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(186, 24);
            this.txtTo.Margin = new System.Windows.Forms.Padding(0);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(103, 18);
            this.txtTo.Subtype = "Number";
            this.txtTo.TabIndex = 6;
            this.txtTo.ValueChanged += new System.EventHandler(this.txtTo_ValueChanged);
            // 
            // gridFilterNumeric
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 88);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.txtFrom);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "gridFilterNumeric";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnApply;
        private NetPlugins2.IcsDouble txtFrom;
        private NetPlugins2.IcsDouble txtTo;
    }
}