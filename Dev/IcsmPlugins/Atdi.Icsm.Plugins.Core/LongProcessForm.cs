using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atdi.Icsm.Plugins.Core
{
	public partial class LongProcessForm : Form
	{
		private readonly LongProcessToken _processToken;
		private CancellationTokenSource _abortSource;

		public LongProcessForm(LongProcessToken processToken, CancellationTokenSource abortSource)
		{
			this._processToken = processToken;
			_abortSource = abortSource;
			InitializeComponent();

			this.btnAbort.Enabled = processToken.Options.CanAbort;
			this.btnAbort.Visible = processToken.Options.CanAbort;

			this.btnStop.Enabled = processToken.Options.CanStop;
			this.btnStop.Visible = processToken.Options.CanStop;

			this.btnResume.Enabled = false;
			this.btnResume.Visible = processToken.Options.CanStop;

			this.txtLog.Visible = processToken.Options.UseLog;
			this.barProgress.Visible = processToken.Options.UseProgressBar;

			if (processToken.Options.UseProgressBar)
			{
				this.barProgress.Maximum = processToken.Options.MaxValue;
				this.barProgress.Minimum = processToken.Options.MinValue;

				if (processToken.Options.ValueKind == LongProcessValueKind.Infinity)
				{
					barProgress.Style = ProgressBarStyle.Marquee;
				}
				else
				{
					barProgress.Style = ProgressBarStyle.Blocks;
				}

				barProgress.Step = 1;
			}

			txtTitle.Text = processToken.Options.Title;
			txtNote.Text = processToken.Options.Note;
		}

		internal void AddLogMessage(string message)
		{
			if (!string.IsNullOrEmpty(txtLog.Text))
			{
				txtLog.Text = message + Environment.NewLine + txtLog.Text;
			}
			else
			{
				txtLog.Text = message;
			}
		}

		internal void UpdateState(int value, string title, string note)
		{
			if (_processToken.Options.UseProgressBar)
			{
				barProgress.Value = value;
				barProgress.Refresh();
			}

			if (!string.IsNullOrEmpty(title))
			{
				txtTitle.Text = title;
				txtTitle.Refresh();
			}
			if (!string.IsNullOrEmpty(title))
			{
				txtNote.Text = note;
				txtNote.Refresh();
			}
		}

		private void btnAbort_Click(object sender, EventArgs e)
		{
			_abortSource.Cancel();
			btnAbort.Enabled = false;
		}
	}
}
