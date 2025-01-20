using Sobaki.Data.Remote;
using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class GuestViewModel : ViewModel
    {
        public ICommand CallAdminCommand { get; }
        public ICommand PushAuthCommand { get; }
        public ICommand StartCommand { get; }

        public List<Dog> Dogs { get; set; } = new List<Dog>();

        public GuestViewModel()
        {

        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
