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
            this.SuspendLayout();
            // 
            // ButtonCalcReferenceLevels
            // 
            this.ButtonCalcReferenceLevels.Location = new System.Drawing.Point(21, 34);
            this.ButtonCalcReferenceLevels.Name = "ButtonCalcReferenceLevels";
            this.ButtonCalcReferenceLevels.Size = new System.Drawing.Size(136, 23);
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
            this.buttonCalcGroupingEmitting1.Location = new System.Drawing.Point(21, 64);
            this.buttonCalcGroupingEmitting1.Name = "buttonCalcGroupingEmitting1";
            this.buttonCalcGroupingEmitting1.Size = new System.Drawing.Size(136, 23);
            this.buttonCalcGroupingEmitting1.TabIndex = 1;
            this.buttonCalcGroupingEmitting1.Text = "CalcGroupingEmitting";
            this.buttonCalcGroupingEmitting1.UseVisualStyleBackColor = true;
            // 
            // ButtonCalcSearchEmitting
            // 
            this.ButtonCalcSearchEmitting.Location = new System.Drawing.Point(21, 93);
            this.ButtonCalcSearchEmitting.Name = "ButtonCalcSearchEmitting";
            this.ButtonCalcSearchEmitting.Size = new System.Drawing.Size(136, 23);
            this.ButtonCalcSearchEmitting.TabIndex = 2;
            this.ButtonCalcSearchEmitting.Text = "CalcSearchEmitting";
            this.ButtonCalcSearchEmitting.UseVisualStyleBackColor = true;
            // 
            // ButtonCalcSearchInterruption
            // 
            this.ButtonCalcSearchInterruption.Location = new System.Drawing.Point(21, 122);
            this.ButtonCalcSearchInterruption.Name = "ButtonCalcSearchInterruption";
            this.ButtonCalcSearchInterruption.Size = new System.Drawing.Size(136, 23);
            this.ButtonCalcSearchInterruption.TabIndex = 3;
            this.ButtonCalcSearchInterruption.Text = "CalcSearchInterruption";
            this.ButtonCalcSearchInterruption.UseVisualStyleBackColor = true;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 261);
            this.Controls.Add(this.ButtonCalcSearchInterruption);
            this.Controls.Add(this.ButtonCalcSearchEmitting);
            this.Controls.Add(this.buttonCalcGroupingEmitting1);
            this.Controls.Add(this.ButtonCalcReferenceLevels);
            this.Name = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonCalcReferenceLevels;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonCalcGroupingEmitting1;
        private System.Windows.Forms.Button ButtonCalcSearchEmitting;
        private System.Windows.Forms.Button ButtonCalcSearchInterruption;
    }
}

