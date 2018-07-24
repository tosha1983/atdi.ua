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
            this.textBox_countIteration = new System.Windows.Forms.TextBox();
            this.textBox_CountThreads = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.checkBox_ExecQuery = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_time_period = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_MaxCountRecords = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_cnt_iteration2 = new System.Windows.Forms.TextBox();
            this.button_run_time = new System.Windows.Forms.Button();
            this.textBox_thread_end = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_maxcount_record2 = new System.Windows.Forms.TextBox();
            this.textBox_start_thread = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox_Insert
            // 
            this.checkBox_Insert.AutoSize = true;
            this.checkBox_Insert.Location = new System.Drawing.Point(35, 147);
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
            this.checkBox_Update.Location = new System.Drawing.Point(35, 170);
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
            this.checkBox_Delete.Location = new System.Drawing.Point(35, 193);
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
            this.checkBox_ExecuteQueryCustomExpr.Location = new System.Drawing.Point(35, 216);
            this.checkBox_ExecuteQueryCustomExpr.Name = "checkBox_ExecuteQueryCustomExpr";
            this.checkBox_ExecuteQueryCustomExpr.Size = new System.Drawing.Size(185, 17);
            this.checkBox_ExecuteQueryCustomExpr.TabIndex = 3;
            this.checkBox_ExecuteQueryCustomExpr.Text = "ExecuteQuery+CustomExpression";
            this.checkBox_ExecuteQueryCustomExpr.UseVisualStyleBackColor = true;
            this.checkBox_ExecuteQueryCustomExpr.CheckedChanged += new System.EventHandler(this.checkBox_ExecuteQueryCustomExpr_CheckedChanged);
            // 
            // textBox_countIteration
            // 
            this.textBox_countIteration.Location = new System.Drawing.Point(97, 13);
            this.textBox_countIteration.Name = "textBox_countIteration";
            this.textBox_countIteration.Size = new System.Drawing.Size(100, 20);
            this.textBox_countIteration.TabIndex = 9;
            this.textBox_countIteration.Text = "100";
            // 
            // textBox_CountThreads
            // 
            this.textBox_CountThreads.Location = new System.Drawing.Point(150, 36);
            this.textBox_CountThreads.Name = "textBox_CountThreads";
            this.textBox_CountThreads.Size = new System.Drawing.Size(50, 20);
            this.textBox_CountThreads.TabIndex = 10;
            this.textBox_CountThreads.Text = "10";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(649, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(35, 265);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(738, 227);
            this.textBox3.TabIndex = 13;
            // 
            // checkBox_ExecQuery
            // 
            this.checkBox_ExecQuery.AutoSize = true;
            this.checkBox_ExecQuery.Location = new System.Drawing.Point(35, 242);
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
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Count iteration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Count threads per operation";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Time";
            // 
            // textBox_time_period
            // 
            this.textBox_time_period.Location = new System.Drawing.Point(63, 13);
            this.textBox_time_period.Name = "textBox_time_period";
            this.textBox_time_period.Size = new System.Drawing.Size(78, 20);
            this.textBox_time_period.TabIndex = 20;
            this.textBox_time_period.Text = "10000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Max count return records";
            // 
            // textBox_MaxCountRecords
            // 
            this.textBox_MaxCountRecords.Location = new System.Drawing.Point(137, 58);
            this.textBox_MaxCountRecords.Name = "textBox_MaxCountRecords";
            this.textBox_MaxCountRecords.Size = new System.Drawing.Size(63, 20);
            this.textBox_MaxCountRecords.TabIndex = 21;
            this.textBox_MaxCountRecords.Text = "200";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(35, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(738, 123);
            this.tabControl1.TabIndex = 23;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.textBox_countIteration);
            this.tabPage1.Controls.Add(this.textBox_MaxCountRecords);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.textBox_CountThreads);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(730, 97);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Iteration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.textBox_cnt_iteration2);
            this.tabPage2.Controls.Add(this.button_run_time);
            this.tabPage2.Controls.Add(this.textBox_thread_end);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.textBox_maxcount_record2);
            this.tabPage2.Controls.Add(this.textBox_start_thread);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBox_time_period);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(730, 97);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Time period";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(147, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Count iteration";
            // 
            // textBox_cnt_iteration2
            // 
            this.textBox_cnt_iteration2.Location = new System.Drawing.Point(238, 13);
            this.textBox_cnt_iteration2.Name = "textBox_cnt_iteration2";
            this.textBox_cnt_iteration2.Size = new System.Drawing.Size(100, 20);
            this.textBox_cnt_iteration2.TabIndex = 30;
            this.textBox_cnt_iteration2.Text = "100";
            // 
            // button_run_time
            // 
            this.button_run_time.Location = new System.Drawing.Point(640, 10);
            this.button_run_time.Name = "button_run_time";
            this.button_run_time.Size = new System.Drawing.Size(75, 23);
            this.button_run_time.TabIndex = 29;
            this.button_run_time.Text = "Run";
            this.button_run_time.UseVisualStyleBackColor = true;
            this.button_run_time.Click += new System.EventHandler(this.button_run_time_Click);
            // 
            // textBox_thread_end
            // 
            this.textBox_thread_end.Location = new System.Drawing.Point(424, 40);
            this.textBox_thread_end.Name = "textBox_thread_end";
            this.textBox_thread_end.Size = new System.Drawing.Size(50, 20);
            this.textBox_thread_end.TabIndex = 27;
            this.textBox_thread_end.Text = "10";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(249, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(166, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Count threads per operation (End)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Max count return records";
            // 
            // textBox_maxcount_record2
            // 
            this.textBox_maxcount_record2.Location = new System.Drawing.Point(149, 64);
            this.textBox_maxcount_record2.Name = "textBox_maxcount_record2";
            this.textBox_maxcount_record2.Size = new System.Drawing.Size(63, 20);
            this.textBox_maxcount_record2.TabIndex = 25;
            this.textBox_maxcount_record2.Text = "200";
            // 
            // textBox_start_thread
            // 
            this.textBox_start_thread.Location = new System.Drawing.Point(193, 40);
            this.textBox_start_thread.Name = "textBox_start_thread";
            this.textBox_start_thread.Size = new System.Drawing.Size(50, 20);
            this.textBox_start_thread.TabIndex = 23;
            this.textBox_start_thread.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(169, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Count threads per operation (Start)";
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 504);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.checkBox_ExecQuery);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.checkBox_ExecuteQueryCustomExpr);
            this.Controls.Add(this.checkBox_Delete);
            this.Controls.Add(this.checkBox_Update);
            this.Controls.Add(this.checkBox_Insert);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.CheckBox checkBox_Insert;
        System.Windows.Forms.CheckBox checkBox_Update;
        System.Windows.Forms.CheckBox checkBox_Delete;
        System.Windows.Forms.CheckBox checkBox_ExecuteQueryCustomExpr;
        System.Windows.Forms.TextBox textBox_countIteration;
        System.Windows.Forms.TextBox textBox_CountThreads;
        System.Windows.Forms.Button button1;
        System.Windows.Forms.TextBox textBox3;
        System.Windows.Forms.CheckBox checkBox_ExecQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_time_period;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_MaxCountRecords;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox_thread_end;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_maxcount_record2;
        private System.Windows.Forms.TextBox textBox_start_thread;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_run_time;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_cnt_iteration2;
    }
}