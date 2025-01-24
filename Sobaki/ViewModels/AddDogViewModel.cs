using Sobaki.Domain.Utilities;
using System;

namespace Sobaki.ViewModels
{
    public class AddDogViewModel : ViewModel
    {


        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
