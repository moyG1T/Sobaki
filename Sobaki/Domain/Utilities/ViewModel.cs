using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sobaki.Domain.Utilities
{
    public abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public abstract void Dispose();
    }
}
