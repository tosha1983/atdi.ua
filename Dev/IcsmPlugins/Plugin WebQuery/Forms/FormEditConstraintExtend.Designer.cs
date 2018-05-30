namespace XICSM.WebQuery
{
    partial class FormEditConstraintExtend
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditConstraintExtend));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.groupBox_DateTime = new System.Windows.Forms.GroupBox();
            this.icsDateTime_to = new NetPlugins2.IcsDateTime();
            this.icsDateTime_from = new NetPlugins2.IcsDateTime();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox_String = new System.Windows.Forms.GroupBox();
            this.textBox_str_value = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox_Numerical = new System.Windows.Forms.GroupBox();
            this.icsDouble_to = new NetPlugins2.IcsDouble();
            this.icsDouble_from = new NetPlugins2.IcsDouble();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_include = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_path = new System.Windows.Forms.ComboBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox_DateTime.SuspendLayout();
            this.groupBox_String.SuspendLayout();
            this.groupBox_Numerical.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox_DateTime);
            this.groupBox1.Controls.Add(this.groupBox_String);
            this.groupBox1.Controls.Add(this.groupBox_Numerical);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 309);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button_cancel);
            this.groupBox6.Controls.Add(this.button_ok);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox6.Location = new System.Drawing.Point(3, 256);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(379, 50);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(230, 18);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(109, 23);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(12, 18);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(113, 23);
            this.button_ok.TabIndex = 0;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // groupBox_DateTime
            // 
            this.groupBox_DateTime.Controls.Add(this.icsDateTime_to);
            this.groupBox_DateTime.Controls.Add(this.icsDateTime_from);
            this.groupBox_DateTime.Controls.Add(this.label5);
            this.groupBox_DateTime.Controls.Add(this.label7);
            this.groupBox_DateTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox_DateTime.Location = new System.Drawing.Point(3, 206);
            this.groupBox_DateTime.Name = "groupBox_DateTime";
            this.groupBox_DateTime.Size = new System.Drawing.Size(379, 50);
            this.groupBox_DateTime.TabIndex = 4;
            this.groupBox_DateTime.TabStop = false;
            this.groupBox_DateTime.Text = "DateTime restrictions";
            // 
            // icsDateTime_to
            // 
            this.icsDateTime_to.Location = new System.Drawing.Point(256, 20);
            this.icsDateTime_to.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.icsDateTime_to.Name = "icsDateTime_to";
            this.icsDateTime_to.Size = new System.Drawing.Size(103, 18);
            this.icsDateTime_to.TabIndex = 9;
            this.icsDateTime_to.Value = new System.DateTime(((long)(0)));
            this.icsDateTime_to.ValueChanged += new System.EventHandler(this.icsDateTime_to_ValueChanged);
            // 
            // icsDateTime_from
            // 
            this.icsDateTime_from.Location = new System.Drawing.Point(51, 22);
            this.icsDateTime_from.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.icsDateTime_from.Name = "icsDateTime_from";
            this.icsDateTime_from.Size = new System.Drawing.Size(103, 18);
            this.icsDateTime_from.TabIndex = 8;
            this.icsDateTime_from.Value = new System.DateTime(((long)(0)));
            this.icsDateTime_from.ValueChanged += new System.EventHandler(this.icsDateTime_from_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(227, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "To";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "From";
            // 
            // groupBox_String
            // 
            this.groupBox_String.Controls.Add(this.textBox_str_value);
            this.groupBox_String.Controls.Add(this.label6);
            this.groupBox_String.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox_String.Location = new System.Drawing.Point(3, 156);
            this.groupBox_String.Name = "groupBox_String";
            this.groupBox_String.Size = new System.Drawing.Size(379, 50);
            this.groupBox_String.TabIndex = 3;
            this.groupBox_String.TabStop = false;
            this.groupBox_String.Text = "String restrictions";
            // 
            // textBox_str_value
            // 
            this.textBox_str_value.Location = new System.Drawing.Point(82, 21);
            this.textBox_str_value.Name = "textBox_str_value";
            this.textBox_str_value.Size = new System.Drawing.Size(277, 20);
            this.textBox_str_value.TabIndex = 5;
            this.textBox_str_value.TextChanged += new System.EventHandler(this.textBox_str_value_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "String value";
            // 
            // groupBox_Numerical
            // 
            this.groupBox_Numerical.Controls.Add(this.icsDouble_to);
            this.groupBox_Numerical.Controls.Add(this.icsDouble_from);
            this.groupBox_Numerical.Controls.Add(this.label3);
            this.groupBox_Numerical.Controls.Add(this.label4);
            this.groupBox_Numerical.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox_Numerical.Location = new System.Drawing.Point(3, 94);
            this.groupBox_Numerical.Name = "groupBox_Numerical";
            this.groupBox_Numerical.Size = new System.Drawing.Size(379, 62);
            this.groupBox_Numerical.TabIndex = 2;
            this.groupBox_Numerical.TabStop = false;
            this.groupBox_Numerical.Text = "Numerical restrictions";
            // 
            // icsDouble_to
            // 
            this.icsDouble_to.Location = new System.Drawing.Point(256, 24);
            this.icsDouble_to.Margin = new System.Windows.Forms.Padding(0);
            this.icsDouble_to.Name = "icsDouble_to";
            this.icsDouble_to.Size = new System.Drawing.Size(103, 18);
            this.icsDouble_to.Subtype = null;
            this.icsDouble_to.TabIndex = 7;
            this.icsDouble_to.ValueChanged += new System.EventHandler(this.icsDouble_to_ValueChanged);
            // 
            // icsDouble_from
            // 
            this.icsDouble_from.Location = new System.Drawing.Point(51, 24);
            this.icsDouble_from.Margin = new System.Windows.Forms.Padding(0);
            this.icsDouble_from.Name = "icsDouble_from";
            this.icsDouble_from.Size = new System.Drawing.Size(103, 18);
            this.icsDouble_from.Subtype = null;
            this.icsDouble_from.TabIndex = 6;
            this.icsDouble_from.ValueChanged += new System.EventHandler(this.icsDouble_from_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "To";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "From";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_include);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.comboBox_path);
            this.groupBox2.Controls.Add(this.textBox_name);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(379, 78);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Common parameters";
            // 
            // checkBox_include
            // 
            this.checkBox_include.AutoSize = true;
            this.checkBox_include.Location = new System.Drawing.Point(12, 51);
            this.checkBox_include.Name = "checkBox_include";
            this.checkBox_include.Size = new System.Drawing.Size(61, 17);
            this.checkBox_include.TabIndex = 4;
            this.checkBox_include.Text = "Include";
            this.checkBox_include.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path";
            // 
            // comboBox_path
            // 
            this.comboBox_path.FormattingEnabled = true;
            this.comboBox_path.Items.AddRange(new object[] {
            "ID",
            "POWER"});
            this.comboBox_path.Location = new System.Drawing.Point(231, 23);
            this.comboBox_path.Name = "comboBox_path";
            this.comboBox_path.Size = new System.Drawing.Size(128, 21);
            this.comboBox_path.TabIndex = 2;
            this.comboBox_path.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_path_DrawItem);
            this.comboBox_path.DropDown += new System.EventHandler(this.comboBox_path_DropDown);
            this.comboBox_path.DropDownClosed += new System.EventHandler(this.comboBox_path_DropDownClosed);
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(51, 24);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(140, 20);
            this.textBox_name.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // FormEditConstraintExtend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 309);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(401, 347);
            this.MinimumSize = new System.Drawing.Size(401, 347);
            this.Name = "FormEditConstraintExtend";
            this.Text = "FormEditConstraintExtend";
            this.groupBox1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox_DateTime.ResumeLayout(false);
            this.groupBox_DateTime.PerformLayout();
            this.groupBox_String.ResumeLayout(false);
            this.groupBox_String.PerformLayout();
            this.groupBox_Numerical.ResumeLayout(false);
            this.groupBox_Numerical.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_path;
        private System.Windows.Forms.GroupBox groupBox_Numerical;
        private NetPlugins2.IcsDouble icsDouble_to;
        private NetPlugins2.IcsDouble icsDouble_from;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_include;
        private System.Windows.Forms.GroupBox groupBox_String;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_str_value;
        private System.Windows.Forms.GroupBox groupBox_DateTime;
        private NetPlugins2.IcsDateTime icsDateTime_to;
        private NetPlugins2.IcsDateTime icsDateTime_from;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}