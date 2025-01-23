using Sobaki.Domain.Commands;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class AdminPanelViewModel : ViewModel
    {

        public ICommand LogoutCommand { get; }
        public ICommand PushEmployeesCommand { get; }
        public ICommand PushDogsCommand { get; }
        public ICommand PushCagesCommand { get; }

        public AdminPanelViewModel(INavService logout, INavService employees, INavService dogs, INavService cages)
        {
            LogoutCommand = new PopAndPushCommand(logout);
            PushEmployeesCommand = new PushCommand(employees);
            PushDogsCommand = new PushCommand(dogs);
            PushCagesCommand = new PushCommand(cages);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
