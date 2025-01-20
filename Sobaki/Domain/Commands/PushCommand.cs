using Sobaki.Domain.IServices;
using System;
using System.Windows.Input;

namespace Sobaki.Domain.Commands
{
    public class PushCommand : ICommand
    {
        private readonly INavService _navService;

        public PushCommand(INavService navService)
        {
            _navService = navService;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.Push();
        }

        public event EventHandler CanExecuteChanged;
    }
}
