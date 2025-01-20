using Sobaki.Domain.Contexts;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;

namespace Sobaki.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly MainContext _mainContext;

        public ViewModel CurrentViewModel => _mainContext.CurrentViewModel;

        public MainViewModel(INavService auth, INavService main, MainContext mainContext)
        {
            _mainContext = mainContext;

            _mainContext.ViewModelChanged += OnViewModelChanged;
        }

        private void OnViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            _mainContext.ViewModelChanged -= OnViewModelChanged;

            GC.SuppressFinalize(this);
        }
    }
}
