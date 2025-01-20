using Sobaki.Domain.IServices;
using System;
using System.Windows.Input;

namespace Sobaki.Domain.Commands
{
    public class PopCommand : ICommand
    {
        private readonly INavService _navService;

        public PopCommand(INavService navService)
        {
            _navService = navService;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.Pop();
        }

        public event EventHandler CanExecuteChanged;
    }
}
