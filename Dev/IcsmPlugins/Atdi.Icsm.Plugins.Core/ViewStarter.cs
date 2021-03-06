﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.TextFormatting;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.Core
{
	public sealed class ViewStarter : LoggedObject
	{
		private readonly IServicesResolver _resolver;
		private readonly Dictionary<ViewForm, ViewForm> _forms;
		private readonly Dictionary<LongProcessWorker, LongProcessWorker> _processWorkers;
		private readonly Dictionary<ViewForm, ViewBase> _viewsByFroms;
		private readonly Dictionary<ViewBase, ViewForm> _formsByViews;

		public ViewStarter(IServicesResolver resolver, ILogger logger) : base(logger)
		{
			_resolver = resolver;
			_forms = new Dictionary<ViewForm, ViewForm>();
			_processWorkers = new Dictionary<LongProcessWorker, LongProcessWorker>();
			_formsByViews = new Dictionary<ViewBase, ViewForm>();
			_viewsByFroms = new Dictionary<ViewForm, ViewBase>();
		}

		public void Stop(ViewBase view)
		{
			var form = _formsByViews[view];
			form.Close();
		}

		public TView Start<TView>(bool isModal = false, Action<TView> action = null)
			where TView: ViewBase
		{
			var viewType = typeof(TView);
			var view = _resolver.Resolve<TView>();

			action?.Invoke(view);

			var xamlAttr = viewType.GetCustomAttribute<ViewXamlAttribute>();
			if (xamlAttr == null)
			{
				throw new InvalidOperationException($"Undefined ViewXamlAttribute in view type '{viewType}'");
			}
			var captionAttr = viewType.GetCustomAttribute<ViewCaptionAttribute>();
			var captionText = captionAttr?.Text;
			var xamlFilePath = $"{viewType.Assembly.GetName().Name}\\Xaml\\{xamlAttr.Name}";
			var viewForm = new ViewForm(xamlFilePath, view, captionText, this, this.Logger);

			
			if (xamlAttr.Width > 0 && xamlAttr.Height > 0)
			{
				viewForm.Size = new Size(xamlAttr.Width, xamlAttr.Height);
			}
			viewForm.WindowState = xamlAttr.WindowState;

			_forms.Add(viewForm, viewForm);
			_formsByViews.Add(view, viewForm);
			_viewsByFroms.Add(viewForm, view);

			if (isModal)
			{
				viewForm.ShowDialog();
				viewForm.Dispose();
				return null;
			}
			else
			{
				viewForm.Show();
				return view;
			}
		}

		internal void Close(ViewForm form)
		{
			if (_forms.ContainsKey(form))
			{
				_forms.Remove(form);
			}

			if (_viewsByFroms.ContainsKey(form))
			{
				var view = _viewsByFroms[form];
				_viewsByFroms.Remove(form);

				if (_formsByViews.ContainsKey(view))
				{
					_formsByViews.Remove(view);
				}

				try
				{
					view.Dispose();
				}
				catch (Exception e)
				{
					this.Logger.Exception("Plugins.Core", "ViewStarter", e, this);
				}
			}
		}


		public bool AskQuestion(string title, string question)
		{
			return MessageBox.Show(question, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}

		public void ShowException(string title, Exception e)
		{
			this.ShowException(title, "Something went wrong.", e);
		}

		public void ShowException(string title, string message, Exception e)
		{
			var text = new StringBuilder();
			text.AppendLine(message);
			text.AppendLine($"");
			text.AppendLine(e.Message);

			MessageBox.Show(text.ToString(), title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public bool ShowExceptionRequestingRetry(string title, Exception e)
		{
			return this.ShowExceptionRequestingRetry(title, "Something went wrong.", e);
		}

		public bool ShowExceptionRequestingRetry(string title, string message, Exception e)
		{
			var text = new StringBuilder();
			text.AppendLine(message);
			text.AppendLine($"");
			text.AppendLine(e.Message);

			return MessageBox.Show(text.ToString(), title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
		}

		public void StartLongProcess(LongProcessOptions options, Action<LongProcessToken> action)
		{
			var worker = _resolver.Resolve<LongProcessWorker>();
			_processWorkers.Add(worker, worker);
			worker.Start(options, action);
		}

		public void StartInUserContext(string title, string question, Action action, bool allowRetry = false)
		{
			if (this.AskQuestion(title, question))
			{
				this.StartInUserContext(title, action, allowRetry);
			}
		}

		public void StartInUserContext(string title, Action action, bool allowRetry = false)
		{
			var retry = false;
			do
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					this.Logger.Exception("Plugins.Core", "ViewStarter", title, e, this);
					if (allowRetry)
					{
						retry = this.ShowExceptionRequestingRetry(title, e);
					}
					else
					{
						retry = false;
						this.ShowException(title, e);
					}
				}

			} while (retry);
			
		}

		internal void Close(LongProcessWorker processWorker)
		{
			if (_processWorkers.ContainsKey(processWorker))
			{
				_processWorkers.Remove(processWorker);
			}
		}
	}
}
