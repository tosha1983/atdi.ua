namespace Atdi.Test.WebQuery.WinForm
{
    partial class TestForm
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
            this.checkBox_Insert = new System.Windows.Forms.CheckBox();
            this.checkBox_Update = new System.Windows.Forms.CheckBox();
            this.checkBox_Delete = new System.Windows.Forms.CheckBox();
            this.checkBox_ExecuteQueryCustomExpr = new System.Windows.Forms.CheckBox();
            this.checkBox_Authorization = new System.Windows.Forms.CheckBox();
            this.checkBox_QueryGroups = new System.Windows.Forms.CheckBox();
            this.checkBox_QueryMetaData = new System.Windows.Forms.CheckBox();
            this.textBox_countIteration = new System.Windows.Forms.TextBox();
            this.textBox_CountThreads = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.checkBox_ExecQuery = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_delay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_MaxCountRecords = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBox_Insert
            // 
            this.checkBox_Insert.AutoSize = true;
            this.checkBox_Insert.Location = new System.Drawing.Point(40, 107);
            this.checkBox_Insert.Name = "checkBox_Insert";
            this.checkBox_Insert.Size = new System.Drawing.Size(52, 17);
            this.checkBox_Insert.TabIndex = 0;
            this.checkBox_Insert.Text = "Insert";
            this.checkBox_Insert.UseVisualStyleBackColor = true;
            this.checkBox_Insert.CheckedChanged += new System.EventHandler(this.checkBox_Insert_CheckedChanged);
            // 
            // checkBox_Update
            // 
            this.checkBox_Update.AutoSize = true;
            this.checkBox_Update.Location = new System.Drawing.Point(40, 130);
            this.checkBox_Update.Name = "checkBox_Update";
            this.checkBox_Update.Size = new System.Drawing.Size(61, 17);
            this.checkBox_Update.TabIndex = 1;
            this.checkBox_Update.Text = "Update";
            this.checkBox_Update.UseVisualStyleBackColor = true;
            this.checkBox_Update.CheckedChanged += new System.EventHandler(this.checkBox_Update_CheckedChanged);
            // 
            // checkBox_Delete
            // 
            this.checkBox_Delete.AutoSize = true;
            this.checkBox_Delete.Location = new System.Drawing.Point(40, 153);
            this.checkBox_Delete.Name = "checkBox_Delete";
            this.checkBox_Delete.Size = new System.Drawing.Size(57, 17);
            this.checkBox_Delete.TabIndex = 2;
            this.checkBox_Delete.Text = "Delete";
            this.checkBox_Delete.UseVisualStyleBackColor = true;
            this.checkBox_Delete.CheckedChanged += new System.EventHandler(this.checkBox_Delete_CheckedChanged);
            // 
            // checkBox_ExecuteQueryCustomExpr
            // 
            this.checkBox_ExecuteQueryCustomExpr.AutoSize = true;
            this.checkBox_ExecuteQueryCustomExpr.Location = new System.Drawing.Point(40, 176);
            this.checkBox_ExecuteQueryCustomExpr.Name = "checkBox_ExecuteQueryCustomExpr";
            this.checkBox_ExecuteQueryCustomExpr.Size = new System.Drawing.Size(185, 17);
            this.checkBox_ExecuteQueryCustomExpr.TabIndex = 3;
            this.checkBox_ExecuteQueryCustomExpr.Text = "ExecuteQuery+CustomExpression";
            this.checkBox_ExecuteQueryCustomExpr.UseVisualStyleBackColor = true;
            this.checkBox_ExecuteQueryCustomExpr.CheckedChanged += new System.EventHandler(this.checkBox_ExecuteQueryCustomExpr_CheckedChanged);
            // 
            // checkBox_Authorization
            // 
            this.checkBox_Authorization.AutoSize = true;
            this.checkBox_Authorization.Location = new System.Drawing.Point(248, 107);
            this.checkBox_Authorization.Name = "checkBox_Authorization";
            this.checkBox_Authorization.Size = new System.Drawing.Size(87, 17);
            this.checkBox_Authorization.TabIndex = 4;
            this.checkBox_Authorization.Text = "Authorization";
            this.checkBox_Authorization.UseVisualStyleBackColor = true;
            this.checkBox_Authorization.CheckedChanged += new System.EventHandler(this.checkBox_Authorization_CheckedChanged);
            // 
            // checkBox_QueryGroups
            // 
            this.checkBox_QueryGroups.AutoSize = true;
            this.checkBox_QueryGroups.Location = new System.Drawing.Point(247, 130);
            this.checkBox_QueryGroups.Name = "checkBox_QueryGroups";
            this.checkBox_QueryGroups.Size = new System.Drawing.Size(88, 17);
            this.checkBox_QueryGroups.TabIndex = 5;
            this.checkBox_QueryGroups.Text = "QueryGroups";
            this.checkBox_QueryGroups.UseVisualStyleBackColor = true;
            // 
            // checkBox_QueryMetaData
            // 
            this.checkBox_QueryMetaData.AutoSize = true;
            this.checkBox_QueryMetaData.Location = new System.Drawing.Point(247, 153);
            this.checkBox_QueryMetaData.Name = "checkBox_QueryMetaData";
            this.checkBox_QueryMetaData.Size = new System.Drawing.Size(101, 17);
            this.checkBox_QueryMetaData.TabIndex = 6;
            this.checkBox_QueryMetaData.Text = "QueryMetaData";
            this.checkBox_QueryMetaData.UseVisualStyleBackColor = true;
            this.checkBox_QueryMetaData.CheckedChanged += new System.EventHandler(this.checkBox_QueryMetaData_CheckedChanged);
            // 
            // textBox_countIteration
            // 
            this.textBox_countIteration.Location = new System.Drawing.Point(122, 31);
            this.textBox_countIteration.Name = "textBox_countIteration";
            this.textBox_countIteration.Size = new System.Drawing.Size(100, 20);
            this.textBox_countIteration.TabIndex = 9;
            this.textBox_countIteration.Text = "100";
            // 
            // textBox_CountThreads
            // 
            this.textBox_CountThreads.Location = new System.Drawing.Point(175, 54);
            this.textBox_CountThreads.Name = "textBox_CountThreads";
            this.textBox_CountThreads.Size = new System.Drawing.Size(50, 20);
            this.textBox_CountThreads.TabIndex = 10;
            this.textBox_CountThreads.Text = "16";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(684, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(684, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Clear all";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(40, 225);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(738, 227);
            this.textBox3.TabIndex = 13;
            // 
            // checkBox_ExecQuery
            // 
            this.checkBox_ExecQuery.AutoSize = true;
            this.checkBox_ExecQuery.Location = new System.Drawing.Point(40, 202);
            this.checkBox_ExecQuery.Name = "checkBox_ExecQuery";
            this.checkBox_ExecQuery.Size = new System.Drawing.Size(93, 17);
            this.checkBox_ExecQuery.TabIndex = 14;
            this.checkBox_ExecQuery.Text = "ExecuteQuery";
            this.checkBox_ExecQuery.UseVisualStyleBackColor = true;
            this.checkBox_ExecQuery.CheckedChanged += new System.EventHandler(this.checkBox_ExecQuery_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Count iteration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Count threads per operation";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Delay between query";
            this.label4.Visible = false;
            // 
            // textBox_delay
            // 
            this.textBox_delay.Location = new System.Drawing.Point(144, 6);
            this.textBox_delay.Name = "textBox_delay";
            this.textBox_delay.Size = new System.Drawing.Size(78, 20);
            this.textBox_delay.TabIndex = 20;
            this.textBox_delay.Text = "0";
            this.textBox_delay.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Max count return records";
            // 
            // textBox_MaxCountRecords
            // 
            this.textBox_MaxCountRecords.Location = new System.Drawing.Point(162, 76);
            this.textBox_MaxCountRecords.Name = "textBox_MaxCountRecords";
            this.textBox_MaxCountRecords.Size = new System.Drawing.Size(63, 20);
            this.textBox_MaxCountRecords.TabIndex = 21;
            this.textBox_MaxCountRecords.Text = "200";
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 467);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_MaxCountRecords);
            this.Controls.Add(this.textBox_delay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox_ExecQuery);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_CountThreads);
            this.Controls.Add(this.textBox_countIteration);
            this.Controls.Add(this.checkBox_QueryMetaData);
            this.Controls.Add(this.checkBox_QueryGroups);
            this.Controls.Add(this.checkBox_Authorization);
            this.Controls.Add(this.checkBox_ExecuteQueryCustomExpr);
            this.Controls.Add(this.checkBox_Delete);
            this.Controls.Add(this.checkBox_Update);
            this.Controls.Add(this.checkBox_Insert);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.CheckBox checkBox_Insert;
        System.Windows.Forms.CheckBox checkBox_Update;
        System.Windows.Forms.CheckBox checkBox_Delete;
        System.Windows.Forms.CheckBox checkBox_ExecuteQueryCustomExpr;
        System.Windows.Forms.CheckBox checkBox_Authorization;
        System.Windows.Forms.CheckBox checkBox_QueryGroups;
        System.Windows.Forms.CheckBox checkBox_QueryMetaData;
        System.Windows.Forms.TextBox textBox_countIteration;
        System.Windows.Forms.TextBox textBox_CountThreads;
        System.Windows.Forms.Button button1;
        System.Windows.Forms.Button button2;
        System.Windows.Forms.TextBox textBox3;
        System.Windows.Forms.CheckBox checkBox_ExecQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_delay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_MaxCountRecords;
    }
}