namespace Atdi.Icsm.Plugins.Core
{
	partial class LongProcessForm
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
			this.txtTitle = new System.Windows.Forms.Label();
			this.txtNote = new System.Windows.Forms.TextBox();
			this.barProgress = new System.Windows.Forms.ProgressBar();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.btnAbort = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnResume = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtTitle
			// 
			this.txtTitle.AutoSize = true;
			this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtTitle.Location = new System.Drawing.Point(6, 15);
			this.txtTitle.Name = "txtTitle";
			this.txtTitle.Size = new System.Drawing.Size(157, 13);
			this.txtTitle.TabIndex = 0;
			this.txtTitle.Text = "Performed lengthy process";
			// 
			// txtNote
			// 
			this.txtNote.HideSelection = false;
			this.txtNote.Location = new System.Drawing.Point(4, 44);
			this.txtNote.Multiline = true;
			this.txtNote.Name = "txtNote";
			this.txtNote.ReadOnly = true;
			this.txtNote.Size = new System.Drawing.Size(413, 52);
			this.txtNote.TabIndex = 4;
			this.txtNote.TabStop = false;
			this.txtNote.Text = "Process note ...";
			// 
			// barProgress
			// 
			this.barProgress.Location = new System.Drawing.Point(4, 102);
			this.barProgress.Name = "barProgress";
			this.barProgress.Size = new System.Drawing.Size(413, 23);
			this.barProgress.Step = 1;
			this.barProgress.TabIndex = 2;
			this.barProgress.Value = 35;
			// 
			// txtLog
			// 
			this.txtLog.Location = new System.Drawing.Point(4, 131);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(413, 213);
			this.txtLog.TabIndex = 5;
			this.txtLog.TabStop = false;
			this.txtLog.Text = "Log";
			// 
			// btnAbort
			// 
			this.btnAbort.Enabled = false;
			this.btnAbort.Location = new System.Drawing.Point(423, 44);
			this.btnAbort.Name = "btnAbort";
			this.btnAbort.Size = new System.Drawing.Size(75, 23);
			this.btnAbort.TabIndex = 1;
			this.btnAbort.Text = "Abort";
			this.btnAbort.UseVisualStyleBackColor = true;
			this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(423, 73);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 2;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnResume
			// 
			this.btnResume.Enabled = false;
			this.btnResume.Location = new System.Drawing.Point(422, 102);
			this.btnResume.Name = "btnResume";
			this.btnResume.Size = new System.Drawing.Size(75, 23);
			this.btnResume.TabIndex = 3;
			this.btnResume.Text = "Resume";
			this.btnResume.UseVisualStyleBackColor = true;
			// 
			// LongProcessForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(502, 368);
			this.ControlBox = false;
			this.Controls.Add(this.btnResume);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnAbort);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.barProgress);
			this.Controls.Add(this.txtNote);
			this.Controls.Add(this.txtTitle);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LongProcessForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ICSM Plugin: Performed process";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label txtTitle;
		private System.Windows.Forms.TextBox txtNote;
		private System.Windows.Forms.ProgressBar barProgress;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.Button btnAbort;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnResume;
	}
}