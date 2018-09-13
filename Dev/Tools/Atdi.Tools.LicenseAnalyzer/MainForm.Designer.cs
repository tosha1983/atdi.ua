namespace Atdi.Tools.LicenseAnalyzer
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLicenseProductKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLicenseOwnerId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLicenseFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openLicenseFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtStartDate = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtStopDate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCreated = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txtLicenseProductKey);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLicenseOwnerId);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtLicenseFileName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 113);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // txtLicenseProductKey
            // 
            this.txtLicenseProductKey.Location = new System.Drawing.Point(77, 77);
            this.txtLicenseProductKey.Name = "txtLicenseProductKey";
            this.txtLicenseProductKey.Size = new System.Drawing.Size(303, 20);
            this.txtLicenseProductKey.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Product Key";
            // 
            // txtLicenseOwnerId
            // 
            this.txtLicenseOwnerId.Location = new System.Drawing.Point(77, 51);
            this.txtLicenseOwnerId.Name = "txtLicenseOwnerId";
            this.txtLicenseOwnerId.Size = new System.Drawing.Size(303, 20);
            this.txtLicenseOwnerId.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Owner ID";
            // 
            // txtLicenseFileName
            // 
            this.txtLicenseFileName.Location = new System.Drawing.Point(77, 25);
            this.txtLicenseFileName.Name = "txtLicenseFileName";
            this.txtLicenseFileName.Size = new System.Drawing.Size(356, 20);
            this.txtLicenseFileName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File name";
            // 
            // openLicenseFileDialog
            // 
            this.openLicenseFileDialog.FileName = "*.lic";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(439, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(29, 19);
            this.button1.TabIndex = 6;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(393, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 44);
            this.button2.TabIndex = 7;
            this.button2.Text = "Analyze";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtOwner);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.txtCompany);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.txtCreated);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtInstance);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtStopDate);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtStartDate);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txtProduct);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtType);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtNumber);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(12, 131);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(479, 186);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "  License data ";
            // 
            // txtStartDate
            // 
            this.txtStartDate.Location = new System.Drawing.Point(67, 150);
            this.txtStartDate.Name = "txtStartDate";
            this.txtStartDate.ReadOnly = true;
            this.txtStartDate.Size = new System.Drawing.Size(162, 20);
            this.txtStartDate.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 153);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Start Date";
            // 
            // txtProduct
            // 
            this.txtProduct.Location = new System.Drawing.Point(67, 45);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.ReadOnly = true;
            this.txtProduct.Size = new System.Drawing.Size(391, 20);
            this.txtProduct.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Product";
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(296, 19);
            this.txtType.Name = "txtType";
            this.txtType.ReadOnly = true;
            this.txtType.Size = new System.Drawing.Size(162, 20);
            this.txtType.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(235, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Type";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(67, 19);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.ReadOnly = true;
            this.txtNumber.Size = new System.Drawing.Size(162, 20);
            this.txtNumber.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Number";
            // 
            // txtStopDate
            // 
            this.txtStopDate.Location = new System.Drawing.Point(296, 150);
            this.txtStopDate.Name = "txtStopDate";
            this.txtStopDate.ReadOnly = true;
            this.txtStopDate.Size = new System.Drawing.Size(162, 20);
            this.txtStopDate.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Stop Date";
            // 
            // txtInstance
            // 
            this.txtInstance.Location = new System.Drawing.Point(296, 124);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.ReadOnly = true;
            this.txtInstance.Size = new System.Drawing.Size(162, 20);
            this.txtInstance.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(235, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Instance";
            // 
            // txtCreated
            // 
            this.txtCreated.Location = new System.Drawing.Point(67, 124);
            this.txtCreated.Name = "txtCreated";
            this.txtCreated.ReadOnly = true;
            this.txtCreated.Size = new System.Drawing.Size(162, 20);
            this.txtCreated.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Created";
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(67, 71);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.ReadOnly = true;
            this.txtCompany.Size = new System.Drawing.Size(391, 20);
            this.txtCompany.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Company";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // txtOwner
            // 
            this.txtOwner.Location = new System.Drawing.Point(67, 97);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.ReadOnly = true;
            this.txtOwner.Size = new System.Drawing.Size(391, 20);
            this.txtOwner.TabIndex = 23;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 100);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Owner";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 329);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "License Analyzer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtLicenseProductKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLicenseOwnerId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLicenseFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openLicenseFileDialog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCreated;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStopDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtStartDate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Label label9;
    }
}

