﻿namespace XICSM.WebQuery
{
    partial class AdminFormWebQuery
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
            this.textBox_code = new System.Windows.Forms.TextBox();
            this.comboBox_group = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtUserIdent = new System.Windows.Forms.TextBox();
            this.labelIdentUser = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.labelLevelAccess = new System.Windows.Forms.Label();
            this.labelIRPFile = new System.Windows.Forms.Label();
            this.buttonOpenIRP = new System.Windows.Forms.Button();
            this.textBoxIRPFilePath = new System.Windows.Forms.TextBox();
            this.textBoxDescrQuery = new System.Windows.Forms.TextBox();
            this.labelDescrQuery = new System.Windows.Forms.Label();
            this.LblName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBoxEditQuery = new System.Windows.Forms.GroupBox();
            this.panelSaveChangeQuery = new System.Windows.Forms.Panel();
            this.button_Constraints = new System.Windows.Forms.Button();
            this.ButtonSaveChangeQuery = new System.Windows.Forms.Button();
            this.textBoxQuery = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ButtonSaveAllChange = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxEditQuery.SuspendLayout();
            this.panelSaveChangeQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_code);
            this.groupBox1.Controls.Add(this.comboBox_group);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.labelLevelAccess);
            this.groupBox1.Controls.Add(this.labelIRPFile);
            this.groupBox1.Controls.Add(this.buttonOpenIRP);
            this.groupBox1.Controls.Add(this.textBoxIRPFilePath);
            this.groupBox1.Controls.Add(this.textBoxDescrQuery);
            this.groupBox1.Controls.Add(this.labelDescrQuery);
            this.groupBox1.Controls.Add(this.LblName);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(525, 768);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Panel Setting Web Query";
            // 
            // textBox_code
            // 
            this.textBox_code.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_code.Location = new System.Drawing.Point(110, 88);
            this.textBox_code.Name = "textBox_code";
            this.textBox_code.Size = new System.Drawing.Size(338, 20);
            this.textBox_code.TabIndex = 31;
            // 
            // comboBox_group
            // 
            this.comboBox_group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_group.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_group.FormattingEnabled = true;
            this.comboBox_group.Location = new System.Drawing.Point(110, 175);
            this.comboBox_group.Name = "comboBox_group";
            this.comboBox_group.Size = new System.Drawing.Size(390, 21);
            this.comboBox_group.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Taskforce group:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtUserIdent);
            this.groupBox2.Controls.Add(this.labelIdentUser);
            this.groupBox2.Location = new System.Drawing.Point(6, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 44);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mode visible records";
            // 
            // txtUserIdent
            // 
            this.txtUserIdent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserIdent.Location = new System.Drawing.Point(289, 13);
            this.txtUserIdent.Name = "txtUserIdent";
            this.txtUserIdent.Size = new System.Drawing.Size(205, 20);
            this.txtUserIdent.TabIndex = 17;
            // 
            // labelIdentUser
            // 
            this.labelIdentUser.AutoSize = true;
            this.labelIdentUser.Location = new System.Drawing.Point(6, 16);
            this.labelIdentUser.Name = "labelIdentUser";
            this.labelIdentUser.Size = new System.Drawing.Size(277, 13);
            this.labelIdentUser.TabIndex = 15;
            this.labelIdentUser.Text = "Field name identifies the users (example EMPLOYEE_ID):";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ButtonClose);
            this.panel1.Controls.Add(this.ButtonSaveAllChange);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 721);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(519, 44);
            this.panel1.TabIndex = 13;
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonClose.Location = new System.Drawing.Point(276, 12);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(193, 23);
            this.ButtonClose.TabIndex = 3;
            this.ButtonClose.Text = "Close";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // labelLevelAccess
            // 
            this.labelLevelAccess.AutoSize = true;
            this.labelLevelAccess.Location = new System.Drawing.Point(9, 89);
            this.labelLevelAccess.Name = "labelLevelAccess";
            this.labelLevelAccess.Size = new System.Drawing.Size(35, 13);
            this.labelLevelAccess.TabIndex = 12;
            this.labelLevelAccess.Text = "Code:";
            // 
            // labelIRPFile
            // 
            this.labelIRPFile.AutoSize = true;
            this.labelIRPFile.Location = new System.Drawing.Point(9, 57);
            this.labelIRPFile.Name = "labelIRPFile";
            this.labelIRPFile.Size = new System.Drawing.Size(95, 13);
            this.labelIRPFile.TabIndex = 10;
            this.labelIRPFile.Text = "IRP file with query:";
            // 
            // buttonOpenIRP
            // 
            this.buttonOpenIRP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenIRP.Location = new System.Drawing.Point(458, 57);
            this.buttonOpenIRP.Name = "buttonOpenIRP";
            this.buttonOpenIRP.Size = new System.Drawing.Size(42, 23);
            this.buttonOpenIRP.TabIndex = 5;
            this.buttonOpenIRP.Text = "...";
            this.buttonOpenIRP.UseVisualStyleBackColor = true;
            this.buttonOpenIRP.Click += new System.EventHandler(this.buttonOpenIRP_Click);
            // 
            // textBoxIRPFilePath
            // 
            this.textBoxIRPFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIRPFilePath.Location = new System.Drawing.Point(110, 58);
            this.textBoxIRPFilePath.Name = "textBoxIRPFilePath";
            this.textBoxIRPFilePath.Size = new System.Drawing.Size(338, 20);
            this.textBoxIRPFilePath.TabIndex = 4;
            // 
            // textBoxDescrQuery
            // 
            this.textBoxDescrQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescrQuery.Location = new System.Drawing.Point(12, 221);
            this.textBoxDescrQuery.Multiline = true;
            this.textBoxDescrQuery.Name = "textBoxDescrQuery";
            this.textBoxDescrQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescrQuery.Size = new System.Drawing.Size(504, 476);
            this.textBoxDescrQuery.TabIndex = 3;
            // 
            // labelDescrQuery
            // 
            this.labelDescrQuery.AutoSize = true;
            this.labelDescrQuery.Location = new System.Drawing.Point(12, 205);
            this.labelDescrQuery.Name = "labelDescrQuery";
            this.labelDescrQuery.Size = new System.Drawing.Size(87, 13);
            this.labelDescrQuery.TabIndex = 2;
            this.labelDescrQuery.Text = "Decription query:";
            // 
            // LblName
            // 
            this.LblName.AutoSize = true;
            this.LblName.Location = new System.Drawing.Point(12, 32);
            this.LblName.Name = "LblName";
            this.LblName.Size = new System.Drawing.Size(67, 13);
            this.LblName.TabIndex = 1;
            this.LblName.Text = "Name query:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(110, 32);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(338, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // groupBoxEditQuery
            // 
            this.groupBoxEditQuery.Controls.Add(this.panelSaveChangeQuery);
            this.groupBoxEditQuery.Controls.Add(this.textBoxQuery);
            this.groupBoxEditQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxEditQuery.Location = new System.Drawing.Point(0, 0);
            this.groupBoxEditQuery.Name = "groupBoxEditQuery";
            this.groupBoxEditQuery.Size = new System.Drawing.Size(554, 768);
            this.groupBoxEditQuery.TabIndex = 1;
            this.groupBoxEditQuery.TabStop = false;
            this.groupBoxEditQuery.Text = "Body query";
            // 
            // panelSaveChangeQuery
            // 
            this.panelSaveChangeQuery.Controls.Add(this.button_Constraints);
            this.panelSaveChangeQuery.Controls.Add(this.ButtonSaveChangeQuery);
            this.panelSaveChangeQuery.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSaveChangeQuery.Location = new System.Drawing.Point(3, 718);
            this.panelSaveChangeQuery.Name = "panelSaveChangeQuery";
            this.panelSaveChangeQuery.Size = new System.Drawing.Size(548, 47);
            this.panelSaveChangeQuery.TabIndex = 6;
            // 
            // button_Constraints
            // 
            this.button_Constraints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Constraints.Location = new System.Drawing.Point(345, 13);
            this.button_Constraints.Name = "button_Constraints";
            this.button_Constraints.Size = new System.Drawing.Size(184, 23);
            this.button_Constraints.TabIndex = 10;
            this.button_Constraints.Text = "Constraints...";
            this.button_Constraints.UseVisualStyleBackColor = true;
            this.button_Constraints.Click += new System.EventHandler(this.button_Constraints_Click);
            // 
            // ButtonSaveChangeQuery
            // 
            this.ButtonSaveChangeQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSaveChangeQuery.Location = new System.Drawing.Point(143, 13);
            this.ButtonSaveChangeQuery.Name = "ButtonSaveChangeQuery";
            this.ButtonSaveChangeQuery.Size = new System.Drawing.Size(184, 23);
            this.ButtonSaveChangeQuery.TabIndex = 1;
            this.ButtonSaveChangeQuery.Text = "Save Query";
            this.ButtonSaveChangeQuery.UseVisualStyleBackColor = true;
            this.ButtonSaveChangeQuery.Click += new System.EventHandler(this.ButtonSaveChangeQuery_Click);
            // 
            // textBoxQuery
            // 
            this.textBoxQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxQuery.Location = new System.Drawing.Point(9, 16);
            this.textBoxQuery.Multiline = true;
            this.textBoxQuery.Name = "textBoxQuery";
            this.textBoxQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxQuery.Size = new System.Drawing.Size(542, 693);
            this.textBoxQuery.TabIndex = 5;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 768);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialogIRP";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxEditQuery);
            this.splitContainer1.Size = new System.Drawing.Size(1083, 768);
            this.splitContainer1.SplitterDistance = 525;
            this.splitContainer1.TabIndex = 3;
            // 
            // ButtonSaveAllChange
            // 
            this.ButtonSaveAllChange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonSaveAllChange.Location = new System.Drawing.Point(58, 12);
            this.ButtonSaveAllChange.Name = "ButtonSaveAllChange";
            this.ButtonSaveAllChange.Size = new System.Drawing.Size(193, 23);
            this.ButtonSaveAllChange.TabIndex = 2;
            this.ButtonSaveAllChange.Text = "Save all changes";
            this.ButtonSaveAllChange.UseVisualStyleBackColor = true;
            this.ButtonSaveAllChange.Click += new System.EventHandler(this.ButtonSaveAllChange_Click);
            // 
            // AdminFormWebQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1086, 768);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdminFormWebQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin form web query editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBoxEditQuery.ResumeLayout(false);
            this.groupBoxEditQuery.PerformLayout();
            this.panelSaveChangeQuery.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LblName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxDescrQuery;
        private System.Windows.Forms.Label labelDescrQuery;
        private System.Windows.Forms.TextBox textBoxIRPFilePath;
        private System.Windows.Forms.GroupBox groupBoxEditQuery;
        private System.Windows.Forms.TextBox textBoxQuery;
        private System.Windows.Forms.Panel panelSaveChangeQuery;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button ButtonSaveChangeQuery;
        private System.Windows.Forms.Button buttonOpenIRP;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label labelLevelAccess;
        private System.Windows.Forms.Label labelIRPFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.TextBox txtUserIdent;
        private System.Windows.Forms.Label labelIdentUser;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_Constraints;
        private System.Windows.Forms.ComboBox comboBox_group;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_code;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ButtonSaveAllChange;
    }
}