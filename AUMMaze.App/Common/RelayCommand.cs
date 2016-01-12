using System;
using System.Windows.Input;

namespace AUMMaze.App.Common
{
	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	internal class RelayCommand : ICommand
	{
		#region Fields

		/// <summary>
		/// Action.
		/// </summary>
		readonly Action _execute;

		/// <summary>
		/// CanExecute.
		/// </summary>
		readonly Func<bool> _canExecute;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public RelayCommand(Action execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion Constructors

		#region ICommand Members

		/// <summary>
		/// Returns whether delegated method can be executed.
		/// </summary>
		/// <param name="parameter">Delegate to method</param>
		/// <returns>Can execute.</returns>
		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute();
		}

		/// <summary>
		/// CanExecute changed event.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
					CommandManager.RequerySuggested += value;
			}
			remove
			{
				if (_canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}

		/// <summary>
		/// Executes given method.
		/// </summary>
		/// <param name="parameter">Delegate to method.</param>
		public void Execute(object parameter)
		{
			_execute();
		}

		#endregion ICommand Members
	}
}