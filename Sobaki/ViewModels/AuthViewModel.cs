using Sobaki.Domain.Utilities;
using System;

namespace Sobaki.ViewModels
{
    public class AuthViewModel : ViewModel
    {


        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
