using Sobaki.Domain.IServices;
using System;
using System.Windows.Input;

namespace Sobaki.Domain.Commands
{
    internal class PopAndPushCommand : ICommand
    {
        private readonly INavService _navService;

        public PopAndPushCommand(INavService navService)
        {
            _navService = navService;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.PopAndPush();
        }

        public event EventHandler CanExecuteChanged;
    }
}
