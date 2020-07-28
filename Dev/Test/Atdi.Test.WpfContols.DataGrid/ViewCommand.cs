using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Atdi.Icsm.Plugins.Core
{
	public class ViewCommand : ICommand
	{
		private readonly Action<object> _commandAction;
		private readonly Func<object, bool> _canExecute;

		public ViewCommand(Action<object> commandAction, Func<object, bool> canExecute = null)
		{
			this._commandAction = commandAction;
			this._canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return this._canExecute == null || this._canExecute(parameter);
		}
		public void Execute(object parameter)
		{
			this._commandAction(parameter);
		}

		protected virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}