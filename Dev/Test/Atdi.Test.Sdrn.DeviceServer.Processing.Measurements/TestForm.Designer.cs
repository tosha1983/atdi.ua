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
            this.SuspendLayout();
            // 
            // ButtonCalcReferenceLevels
            // 
            this.ButtonCalcReferenceLevels.Location = new System.Drawing.Point(28, 42);
            this.ButtonCalcReferenceLevels.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.buttonCalcGroupingEmitting1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCalcGroupingEmitting1.Name = "buttonCalcGroupingEmitting1";
            this.buttonCalcGroupingEmitting1.Size = new System.Drawing.Size(181, 28);
            this.buttonCalcGroupingEmitting1.TabIndex = 1;
            this.buttonCalcGroupingEmitting1.Text = "CalcGroupingEmitting";
            this.buttonCalcGroupingEmitting1.UseVisualStyleBackColor = true;
            // 
            // ButtonCalcSearchEmitting
            // 
            this.ButtonCalcSearchEmitting.Location = new System.Drawing.Point(28, 114);
            this.ButtonCalcSearchEmitting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ButtonCalcSearchEmitting.Name = "ButtonCalcSearchEmitting";
            this.ButtonCalcSearchEmitting.Size = new System.Drawing.Size(181, 28);
            this.ButtonCalcSearchEmitting.TabIndex = 2;
            this.ButtonCalcSearchEmitting.Text = "CalcSearchEmitting";
            this.ButtonCalcSearchEmitting.UseVisualStyleBackColor = true;
            // 
            // ButtonCalcSearchInterruption
            // 
            this.ButtonCalcSearchInterruption.Location = new System.Drawing.Point(28, 150);
            this.ButtonCalcSearchInterruption.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 321);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ButtonCalcSearchInterruption);
            this.Controls.Add(this.ButtonCalcSearchEmitting);
            this.Controls.Add(this.buttonCalcGroupingEmitting1);
            this.Controls.Add(this.ButtonCalcReferenceLevels);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
    }
}

