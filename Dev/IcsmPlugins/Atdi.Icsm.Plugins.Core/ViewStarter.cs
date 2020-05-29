using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.Core
{
	public sealed class ViewStarter : LoggedObject
	{
		private readonly IServicesResolver _resolver;
		private readonly Dictionary<ViewForm, ViewForm> _forms;

		private readonly Dictionary<ViewForm, ViewBase> _viewsByFroms;
		private readonly Dictionary<ViewBase, ViewForm> _formsByViews;

		public ViewStarter(IServicesResolver resolver, ILogger logger) : base(logger)
		{
			_resolver = resolver;
			_forms = new Dictionary<ViewForm, ViewForm>();
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

		public void Close(ViewForm form)
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
	}
}
