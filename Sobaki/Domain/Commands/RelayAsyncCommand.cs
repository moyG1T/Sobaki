using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sobaki.Domain.Commands
{
    public class RelayAsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<object, Task> _paramedExecute;

        public RelayAsyncCommand(Func<Task> execute)
        {
            _execute = execute;
        }

        public RelayAsyncCommand(Func<object, Task> paramedExecute)
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
