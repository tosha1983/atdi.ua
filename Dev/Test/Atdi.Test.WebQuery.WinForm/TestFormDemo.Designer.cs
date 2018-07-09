namespace Atdi.Test.WebQuery.WinForm
{
    partial class TestFormDemo
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
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox_ExecQuery = new System.Windows.Forms.CheckBox();
            this.dataGridView_ExecuteQuery = new System.Windows.Forms.DataGridView();
            this.comboBox_Insert = new System.Windows.Forms.ComboBox();
            this.comboBox_Update = new System.Windows.Forms.ComboBox();
            this.comboBox_Delete = new System.Windows.Forms.ComboBox();
            this.comboBox_ExecuteQueryCustExpr = new System.Windows.Forms.ComboBox();
            this.comboBox_ExecuteQuery = new System.Windows.Forms.ComboBox();
            this.textBox_ID_Insert = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Upate_ID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Delete_ID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView_ExecuteQuery_CustomExpress = new System.Windows.Forms.DataGridView();
            this.textBox_limit_Cust_Expr = new System.Windows.Forms.TextBox();
            this.textBox_Limit_Query = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_time_insert = new System.Windows.Forms.Label();
            this.label_time_update = new System.Windows.Forms.Label();
            this.label_time_delete = new System.Windows.Forms.Label();
            this.label_time_executeQueryCustExpr = new System.Windows.Forms.Label();
            this.label_time_ExecuteQuery = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExecuteQuery)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExecuteQuery_CustomExpress)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox_Insert
            // 
            this.checkBox_Insert.AutoSize = true;
            this.checkBox_Insert.Location = new System.Drawing.Point(16, 16);
            this.checkBox_Insert.Name = "checkBox_Insert";
            this.checkBox_Insert.Size = new System.Drawing.Size(70, 17);
            this.checkBox_Insert.TabIndex = 0;
            this.checkBox_Insert.Text = "Insert_13";
            this.checkBox_Insert.UseVisualStyleBackColor = true;
            this.checkBox_Insert.CheckedChanged += new System.EventHandler(this.checkBox_Insert_CheckedChanged);
            // 
            // checkBox_Update
            // 
            this.checkBox_Update.AutoSize = true;
            this.checkBox_Update.Location = new System.Drawing.Point(16, 39);
            this.checkBox_Update.Name = "checkBox_Update";
            this.checkBox_Update.Size = new System.Drawing.Size(79, 17);
            this.checkBox_Update.TabIndex = 1;
            this.checkBox_Update.Text = "Update_47";
            this.checkBox_Update.UseVisualStyleBackColor = true;
            this.checkBox_Update.CheckedChanged += new System.EventHandler(this.checkBox_Update_CheckedChanged);
            // 
            // checkBox_Delete
            // 
            this.checkBox_Delete.AutoSize = true;
            this.checkBox_Delete.Location = new System.Drawing.Point(16, 62);
            this.checkBox_Delete.Name = "checkBox_Delete";
            this.checkBox_Delete.Size = new System.Drawing.Size(75, 17);
            this.checkBox_Delete.TabIndex = 2;
            this.checkBox_Delete.Text = "Delete_13";
            this.checkBox_Delete.UseVisualStyleBackColor = true;
            this.checkBox_Delete.CheckedChanged += new System.EventHandler(this.checkBox_Delete_CheckedChanged);
            // 
            // checkBox_ExecuteQueryCustomExpr
            // 
            this.checkBox_ExecuteQueryCustomExpr.AutoSize = true;
            this.checkBox_ExecuteQueryCustomExpr.Location = new System.Drawing.Point(16, 100);
            this.checkBox_ExecuteQueryCustomExpr.Name = "checkBox_ExecuteQueryCustomExpr";
            this.checkBox_ExecuteQueryCustomExpr.Size = new System.Drawing.Size(203, 17);
            this.checkBox_ExecuteQueryCustomExpr.TabIndex = 3;
            this.checkBox_ExecuteQueryCustomExpr.Text = "ExecuteQuery+CustomExpression_46";
            this.checkBox_ExecuteQueryCustomExpr.UseVisualStyleBackColor = true;
            this.checkBox_ExecuteQueryCustomExpr.CheckedChanged += new System.EventHandler(this.checkBox_ExecuteQueryCustomExpr_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button1.Location = new System.Drawing.Point(631, 146);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 24);
            this.button1.TabIndex = 11;
            this.button1.Text = "Виконати";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox_ExecQuery
            // 
            this.checkBox_ExecQuery.AutoSize = true;
            this.checkBox_ExecQuery.Location = new System.Drawing.Point(16, 123);
            this.checkBox_ExecQuery.Name = "checkBox_ExecQuery";
            this.checkBox_ExecQuery.Size = new System.Drawing.Size(111, 17);
            this.checkBox_ExecQuery.TabIndex = 14;
            this.checkBox_ExecQuery.Text = "ExecuteQuery_13";
            this.checkBox_ExecQuery.UseVisualStyleBackColor = true;
            this.checkBox_ExecQuery.CheckedChanged += new System.EventHandler(this.checkBox_ExecQuery_CheckedChanged);
            // 
            // dataGridView_ExecuteQuery
            // 
            this.dataGridView_ExecuteQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ExecuteQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ExecuteQuery.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_ExecuteQuery.Name = "dataGridView_ExecuteQuery";
            this.dataGridView_ExecuteQuery.Size = new System.Drawing.Size(728, 295);
            this.dataGridView_ExecuteQuery.TabIndex = 24;
            // 
            // comboBox_Insert
            // 
            this.comboBox_Insert.FormattingEnabled = true;
            this.comboBox_Insert.Location = new System.Drawing.Point(206, 12);
            this.comboBox_Insert.Name = "comboBox_Insert";
            this.comboBox_Insert.Size = new System.Drawing.Size(238, 21);
            this.comboBox_Insert.TabIndex = 25;
            this.comboBox_Insert.SelectedValueChanged += new System.EventHandler(this.comboBox_Insert_SelectedValueChanged);
            // 
            // comboBox_Update
            // 
            this.comboBox_Update.FormattingEnabled = true;
            this.comboBox_Update.Location = new System.Drawing.Point(206, 35);
            this.comboBox_Update.Name = "comboBox_Update";
            this.comboBox_Update.Size = new System.Drawing.Size(238, 21);
            this.comboBox_Update.TabIndex = 26;
            this.comboBox_Update.SelectedIndexChanged += new System.EventHandler(this.comboBox_Update_SelectedIndexChanged);
            this.comboBox_Update.SelectedValueChanged += new System.EventHandler(this.comboBox_Update_SelectedValueChanged);
            // 
            // comboBox_Delete
            // 
            this.comboBox_Delete.FormattingEnabled = true;
            this.comboBox_Delete.Location = new System.Drawing.Point(206, 58);
            this.comboBox_Delete.Name = "comboBox_Delete";
            this.comboBox_Delete.Size = new System.Drawing.Size(238, 21);
            this.comboBox_Delete.TabIndex = 27;
            this.comboBox_Delete.SelectedValueChanged += new System.EventHandler(this.comboBox_Delete_SelectedValueChanged);
            // 
            // comboBox_ExecuteQueryCustExpr
            // 
            this.comboBox_ExecuteQueryCustExpr.FormattingEnabled = true;
            this.comboBox_ExecuteQueryCustExpr.Location = new System.Drawing.Point(225, 96);
            this.comboBox_ExecuteQueryCustExpr.Name = "comboBox_ExecuteQueryCustExpr";
            this.comboBox_ExecuteQueryCustExpr.Size = new System.Drawing.Size(219, 21);
            this.comboBox_ExecuteQueryCustExpr.TabIndex = 28;
            this.comboBox_ExecuteQueryCustExpr.SelectedValueChanged += new System.EventHandler(this.comboBox_ExecuteQueryCustExpr_SelectedValueChanged);
            // 
            // comboBox_ExecuteQuery
            // 
            this.comboBox_ExecuteQuery.FormattingEnabled = true;
            this.comboBox_ExecuteQuery.Location = new System.Drawing.Point(206, 121);
            this.comboBox_ExecuteQuery.Name = "comboBox_ExecuteQuery";
            this.comboBox_ExecuteQuery.Size = new System.Drawing.Size(238, 21);
            this.comboBox_ExecuteQuery.TabIndex = 29;
            this.comboBox_ExecuteQuery.SelectedValueChanged += new System.EventHandler(this.comboBox_ExecuteQuery_SelectedValueChanged);
            // 
            // textBox_ID_Insert
            // 
            this.textBox_ID_Insert.Location = new System.Drawing.Point(511, 12);
            this.textBox_ID_Insert.Name = "textBox_ID_Insert";
            this.textBox_ID_Insert.Size = new System.Drawing.Size(80, 20);
            this.textBox_ID_Insert.TabIndex = 30;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(451, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 13);
            this.label9.TabIndex = 31;
            this.label9.Text = "ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(451, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "ID";
            // 
            // textBox_Upate_ID
            // 
            this.textBox_Upate_ID.Location = new System.Drawing.Point(511, 35);
            this.textBox_Upate_ID.Name = "textBox_Upate_ID";
            this.textBox_Upate_ID.Size = new System.Drawing.Size(79, 20);
            this.textBox_Upate_ID.TabIndex = 32;
            this.textBox_Upate_ID.Text = "18501";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(451, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "ID";
            // 
            // textBox_Delete_ID
            // 
            this.textBox_Delete_ID.Location = new System.Drawing.Point(511, 58);
            this.textBox_Delete_ID.Name = "textBox_Delete_ID";
            this.textBox_Delete_ID.Size = new System.Drawing.Size(80, 20);
            this.textBox_Delete_ID.TabIndex = 34;
            this.textBox_Delete_ID.Text = "1322743";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "Результат виконання запиту";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(16, 180);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 327);
            this.tabControl1.TabIndex = 41;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView_ExecuteQuery);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(734, 301);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ExecuteQuery";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView_ExecuteQuery_CustomExpress);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(734, 301);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ExecuteQuery + CustommExpress";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView_ExecuteQuery_CustomExpress
            // 
            this.dataGridView_ExecuteQuery_CustomExpress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ExecuteQuery_CustomExpress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ExecuteQuery_CustomExpress.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ExecuteQuery_CustomExpress.Name = "dataGridView_ExecuteQuery_CustomExpress";
            this.dataGridView_ExecuteQuery_CustomExpress.Size = new System.Drawing.Size(734, 301);
            this.dataGridView_ExecuteQuery_CustomExpress.TabIndex = 25;
            // 
            // textBox_limit_Cust_Expr
            // 
            this.textBox_limit_Cust_Expr.Location = new System.Drawing.Point(530, 97);
            this.textBox_limit_Cust_Expr.Name = "textBox_limit_Cust_Expr";
            this.textBox_limit_Cust_Expr.Size = new System.Drawing.Size(60, 20);
            this.textBox_limit_Cust_Expr.TabIndex = 42;
            this.textBox_limit_Cust_Expr.Text = "200";
            // 
            // textBox_Limit_Query
            // 
            this.textBox_Limit_Query.Location = new System.Drawing.Point(530, 123);
            this.textBox_Limit_Query.Name = "textBox_Limit_Query";
            this.textBox_Limit_Query.Size = new System.Drawing.Size(61, 20);
            this.textBox_Limit_Query.TabIndex = 43;
            this.textBox_Limit_Query.Text = "200";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(597, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Час обробки:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(596, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Час обробки:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(597, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 46;
            this.label6.Text = "Час обробки:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(596, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Час обробки:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(597, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 48;
            this.label8.Text = "Час обробки:";
            // 
            // label_time_insert
            // 
            this.label_time_insert.AutoSize = true;
            this.label_time_insert.Location = new System.Drawing.Point(679, 16);
            this.label_time_insert.Name = "label_time_insert";
            this.label_time_insert.Size = new System.Drawing.Size(13, 13);
            this.label_time_insert.TabIndex = 49;
            this.label_time_insert.Text = "0";
            // 
            // label_time_update
            // 
            this.label_time_update.AutoSize = true;
            this.label_time_update.Location = new System.Drawing.Point(679, 38);
            this.label_time_update.Name = "label_time_update";
            this.label_time_update.Size = new System.Drawing.Size(13, 13);
            this.label_time_update.TabIndex = 50;
            this.label_time_update.Text = "0";
            // 
            // label_time_delete
            // 
            this.label_time_delete.AutoSize = true;
            this.label_time_delete.Location = new System.Drawing.Point(679, 61);
            this.label_time_delete.Name = "label_time_delete";
            this.label_time_delete.Size = new System.Drawing.Size(13, 13);
            this.label_time_delete.TabIndex = 51;
            this.label_time_delete.Text = "0";
            // 
            // label_time_executeQueryCustExpr
            // 
            this.label_time_executeQueryCustExpr.AutoSize = true;
            this.label_time_executeQueryCustExpr.Location = new System.Drawing.Point(679, 101);
            this.label_time_executeQueryCustExpr.Name = "label_time_executeQueryCustExpr";
            this.label_time_executeQueryCustExpr.Size = new System.Drawing.Size(13, 13);
            this.label_time_executeQueryCustExpr.TabIndex = 52;
            this.label_time_executeQueryCustExpr.Text = "0";
            // 
            // label_time_ExecuteQuery
            // 
            this.label_time_ExecuteQuery.AutoSize = true;
            this.label_time_ExecuteQuery.Location = new System.Drawing.Point(679, 130);
            this.label_time_ExecuteQuery.Name = "label_time_ExecuteQuery";
            this.label_time_ExecuteQuery.Size = new System.Drawing.Size(13, 13);
            this.label_time_ExecuteQuery.TabIndex = 53;
            this.label_time_ExecuteQuery.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(451, 101);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 54;
            this.label10.Text = "Ліміт записів";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(451, 124);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(73, 13);
            this.label11.TabIndex = 55;
            this.label11.Text = "Ліміт записів";
            // 
            // TestFormDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 533);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label_time_ExecuteQuery);
            this.Controls.Add(this.label_time_executeQueryCustExpr);
            this.Controls.Add(this.label_time_delete);
            this.Controls.Add(this.label_time_update);
            this.Controls.Add(this.label_time_insert);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Limit_Query);
            this.Controls.Add(this.textBox_limit_Cust_Expr);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Delete_ID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Upate_ID);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox_ID_Insert);
            this.Controls.Add(this.comboBox_ExecuteQuery);
            this.Controls.Add(this.comboBox_ExecuteQueryCustExpr);
            this.Controls.Add(this.comboBox_Delete);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox_Update);
            this.Controls.Add(this.comboBox_Insert);
            this.Controls.Add(this.checkBox_ExecQuery);
            this.Controls.Add(this.checkBox_ExecuteQueryCustomExpr);
            this.Controls.Add(this.checkBox_Delete);
            this.Controls.Add(this.checkBox_Update);
            this.Controls.Add(this.checkBox_Insert);
            this.Name = "TestFormDemo";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExecuteQuery)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExecuteQuery_CustomExpress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.CheckBox checkBox_Insert;
        System.Windows.Forms.CheckBox checkBox_Update;
        System.Windows.Forms.CheckBox checkBox_Delete;
        System.Windows.Forms.CheckBox checkBox_ExecuteQueryCustomExpr;
        System.Windows.Forms.Button button1;
        System.Windows.Forms.CheckBox checkBox_ExecQuery;
        private System.Windows.Forms.DataGridView dataGridView_ExecuteQuery;
        private System.Windows.Forms.ComboBox comboBox_Insert;
        private System.Windows.Forms.ComboBox comboBox_Update;
        private System.Windows.Forms.ComboBox comboBox_Delete;
        private System.Windows.Forms.ComboBox comboBox_ExecuteQueryCustExpr;
        private System.Windows.Forms.ComboBox comboBox_ExecuteQuery;
        private System.Windows.Forms.TextBox textBox_ID_Insert;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Upate_ID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Delete_ID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView_ExecuteQuery_CustomExpress;
        private System.Windows.Forms.TextBox textBox_limit_Cust_Expr;
        private System.Windows.Forms.TextBox textBox_Limit_Query;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_time_insert;
        private System.Windows.Forms.Label label_time_update;
        private System.Windows.Forms.Label label_time_delete;
        private System.Windows.Forms.Label label_time_executeQueryCustExpr;
        private System.Windows.Forms.Label label_time_ExecuteQuery;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}