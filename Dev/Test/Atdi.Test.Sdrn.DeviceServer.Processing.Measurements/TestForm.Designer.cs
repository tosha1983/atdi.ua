namespace Atdi.Test.Sdrn.DeviceServer.Processing.Measurements
{
    partial class TestForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonCalcReferenceLevels = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonCalcGroupingEmitting1 = new System.Windows.Forms.Button();
            this.ButtonCalcSearchEmitting = new System.Windows.Forms.Button();
            this.ButtonCalcSearchInterruption = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.zedGraphControl_P1 = new ZedGraph.ZedGraphControl();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonCalcReferenceLevels
            // 
            this.ButtonCalcReferenceLevels.Location = new System.Drawing.Point(28, 42);
            this.ButtonCalcReferenceLevels.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonCalcReferenceLevels.Name = "ButtonCalcReferenceLevels";
            this.ButtonCalcReferenceLevels.Size = new System.Drawing.Size(181, 28);
            this.ButtonCalcReferenceLevels.TabIndex = 0;
            this.ButtonCalcReferenceLevels.Text = "CalcReferenceLevels";
            this.ButtonCalcReferenceLevels.UseVisualStyleBackColor = true;
            this.ButtonCalcReferenceLevels.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dat";
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonCalcGroupingEmitting1
            // 
            this.buttonCalcGroupingEmitting1.Location = new System.Drawing.Point(28, 79);
            this.buttonCalcGroupingEmitting1.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCalcGroupingEmitting1.Name = "buttonCalcGroupingEmitting1";
            this.buttonCalcGroupingEmitting1.Size = new System.Drawing.Size(181, 28);
            this.buttonCalcGroupingEmitting1.TabIndex = 1;
            this.buttonCalcGroupingEmitting1.Text = "CalcGroupingEmitting";
            this.buttonCalcGroupingEmitting1.UseVisualStyleBackColor = true;
            // 
            // ButtonCalcSearchEmitting
            // 
            this.ButtonCalcSearchEmitting.Location = new System.Drawing.Point(28, 114);
            this.ButtonCalcSearchEmitting.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonCalcSearchEmitting.Name = "ButtonCalcSearchEmitting";
            this.ButtonCalcSearchEmitting.Size = new System.Drawing.Size(181, 28);
            this.ButtonCalcSearchEmitting.TabIndex = 2;
            this.ButtonCalcSearchEmitting.Text = "CalcSearchEmitting";
            this.ButtonCalcSearchEmitting.UseVisualStyleBackColor = true;
            // 
            // ButtonCalcSearchInterruption
            // 
            this.ButtonCalcSearchInterruption.Location = new System.Drawing.Point(28, 150);
            this.ButtonCalcSearchInterruption.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonCalcSearchInterruption.Name = "ButtonCalcSearchInterruption";
            this.ButtonCalcSearchInterruption.Size = new System.Drawing.Size(181, 28);
            this.ButtonCalcSearchInterruption.TabIndex = 3;
            this.ButtonCalcSearchInterruption.Text = "CalcSearchInterruption";
            this.ButtonCalcSearchInterruption.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(181, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(28, 253);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(391, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Atdi.Modules.Sdrn.Calculation.SpecializedCalculation";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // zedGraphControl_P1
            // 
            this.zedGraphControl_P1.IsShowPointValues = false;
            this.zedGraphControl_P1.Location = new System.Drawing.Point(483, 34);
            this.zedGraphControl_P1.Name = "zedGraphControl_P1";
            this.zedGraphControl_P1.PointValueFormat = "G";
            this.zedGraphControl_P1.Size = new System.Drawing.Size(1195, 620);
            this.zedGraphControl_P1.TabIndex = 6;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(37, 403);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(172, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1690, 676);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.zedGraphControl_P1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ButtonCalcSearchInterruption);
            this.Controls.Add(this.ButtonCalcSearchEmitting);
            this.Controls.Add(this.buttonCalcGroupingEmitting1);
            this.Controls.Add(this.ButtonCalcReferenceLevels);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonCalcReferenceLevels;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonCalcGroupingEmitting1;
        private System.Windows.Forms.Button ButtonCalcSearchEmitting;
        private System.Windows.Forms.Button ButtonCalcSearchInterruption;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private ZedGraph.ZedGraphControl zedGraphControl_P1;
        private System.Windows.Forms.Button button3;
    }
}

