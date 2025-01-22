using Sobaki.Domain.Commands;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class AdminCallViewModel : ViewModel
    {
        public ICommand PopCommand { get; private set; }

        public AdminCallViewModel(INavService back)
        {
            PopCommand = new PopCommand(back);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
