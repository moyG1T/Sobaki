using System;
using System.Windows.Input;

namespace Sobaki.Domain.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _paramedExecute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action<object> paramedExecute)
        {
            _paramedExecute = paramedExecute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute?.Invoke();
            _paramedExecute?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
