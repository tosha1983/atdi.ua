namespace Atdi.Test.CalcServer.LowFunction
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
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.IsShowPointValues = false;
            this.zedGraphControl1.Location = new System.Drawing.Point(494, 42);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.PointValueFormat = "G";
            this.zedGraphControl1.Size = new System.Drawing.Size(1077, 837);
            this.zedGraphControl1.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(26, 42);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(340, 92);
            this.button4.TabIndex = 1;
            this.button4.Text = "Тест";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // TestForm
            // 
            this.ClientSize = new System.Drawing.Size(1583, 912);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.zedGraphControl1);
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
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button button4;
    }
}

