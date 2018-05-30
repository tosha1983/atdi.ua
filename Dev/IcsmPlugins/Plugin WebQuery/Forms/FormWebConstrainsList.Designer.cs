namespace XICSM.WebQuery
{
    partial class FormWebConstrainsList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebConstrainsList));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_add_new = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_close = new System.Windows.Forms.Button();
            this.listView_constraints_lst = new System.Windows.Forms.ListView();
            this.button_edit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(708, 362);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_edit);
            this.groupBox3.Controls.Add(this.button_close);
            this.groupBox3.Controls.Add(this.button_delete);
            this.groupBox3.Controls.Add(this.button_add_new);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(3, 295);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(702, 64);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listView_constraints_lst);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(702, 279);
            this.panel1.TabIndex = 2;
            // 
            // button_add_new
            // 
            this.button_add_new.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.button_add_new.Location = new System.Drawing.Point(9, 19);
            this.button_add_new.Name = "button_add_new";
            this.button_add_new.Size = new System.Drawing.Size(127, 23);
            this.button_add_new.TabIndex = 0;
            this.button_add_new.Text = "Add new constraint";
            this.button_add_new.UseVisualStyleBackColor = true;
            this.button_add_new.Click += new System.EventHandler(this.button_add_new_Click);
            // 
            // button_delete
            // 
            this.button_delete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.button_delete.Location = new System.Drawing.Point(275, 19);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(127, 23);
            this.button_delete.TabIndex = 1;
            this.button_delete.Text = "Delete constraint";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_close
            // 
            this.button_close.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_close.Location = new System.Drawing.Point(614, 19);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 23);
            this.button_close.TabIndex = 2;
            this.button_close.Text = "Close";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button3_Click);
            // 
            // listView_constraints_lst
            // 
            this.listView_constraints_lst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_constraints_lst.Location = new System.Drawing.Point(0, 0);
            this.listView_constraints_lst.Name = "listView_constraints_lst";
            this.listView_constraints_lst.Size = new System.Drawing.Size(702, 279);
            this.listView_constraints_lst.TabIndex = 0;
            this.listView_constraints_lst.UseCompatibleStateImageBehavior = false;
            // 
            // button_edit
            // 
            this.button_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.button_edit.Location = new System.Drawing.Point(142, 19);
            this.button_edit.Name = "button_edit";
            this.button_edit.Size = new System.Drawing.Size(127, 23);
            this.button_edit.TabIndex = 3;
            this.button_edit.Text = "Edit constraint";
            this.button_edit.UseVisualStyleBackColor = true;
            this.button_edit.Click += new System.EventHandler(this.button_edit_Click);
            // 
            // FormWebConstrainsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 362);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(724, 400);
            this.Name = "FormWebConstrainsList";
            this.Text = "Constraints editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_add_new;
        private System.Windows.Forms.ListView listView_constraints_lst;
        private System.Windows.Forms.Button button_edit;
    }
}