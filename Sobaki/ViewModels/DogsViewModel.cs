
using Sobaki.Domain.Utilities;
using System;

namespace Sobaki.ViewModels
{
    public class DogsViewModel : ViewModel
    {

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
