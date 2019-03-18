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
            this.ButtonCalcGroupingEmitting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonCalcGroupingEmitting
            // 
            this.ButtonCalcGroupingEmitting.Location = new System.Drawing.Point(21, 34);
            this.ButtonCalcGroupingEmitting.Name = "ButtonCalcGroupingEmitting";
            this.ButtonCalcGroupingEmitting.Size = new System.Drawing.Size(136, 23);
            this.ButtonCalcGroupingEmitting.TabIndex = 0;
            this.ButtonCalcGroupingEmitting.Text = "CalcGroupingEmitting";
            this.ButtonCalcGroupingEmitting.UseVisualStyleBackColor = true;
            this.ButtonCalcGroupingEmitting.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 261);
            this.Controls.Add(this.ButtonCalcGroupingEmitting);
            this.Name = "TestForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonCalcGroupingEmitting;
    }
}

