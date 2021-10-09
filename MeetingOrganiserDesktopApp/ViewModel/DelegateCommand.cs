using System;
using System.Windows.Input;

namespace MeetingOrganiserDesktopApp.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<Object> execute;
        private readonly Func<Object, Boolean> canExecute;

        public event EventHandler CanExecuteChanged { 
            add { CommandManager.RequerySuggested += value; } 
            remove { CommandManager.RequerySuggested -= value; } 
        }

        public DelegateCommand(Action<Object> execute) : this(null, execute) { }

        public DelegateCommand(Func<Object, Boolean> canExecute, Action<Object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public Boolean CanExecute(Object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            execute(parameter);
        }
    }
}
