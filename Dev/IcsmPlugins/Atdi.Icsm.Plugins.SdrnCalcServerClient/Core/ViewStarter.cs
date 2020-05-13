using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Forms;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Core
{
	public sealed class ViewStarter : LoggedObject
	{
		private readonly IServicesResolver _resolver;
		private readonly Dictionary<ViewForm, ViewForm> _forms;

		public ViewStarter(IServicesResolver resolver, ILogger logger) : base(logger)
		{
			_resolver = resolver;
			_forms = new Dictionary<ViewForm, ViewForm>();
		}

		public void Start<TView>(bool isModal = false, Action<TView> action = null)
			where TView: WpfViewModelBase
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

			var viewForm = new ViewForm(xamlAttr.Name, view, captionText, this, this.Logger);
			_forms.Add(viewForm, viewForm);
			if (isModal)
			{
				viewForm.ShowDialog();
				viewForm.Dispose();
			}
			else
			{
				viewForm.Show();
			}


		}

		public void Close(ViewForm viewForm)
		{
			if (_forms.ContainsKey(viewForm))
			{
				_forms.Remove(viewForm);
			}
		}
	}
}
