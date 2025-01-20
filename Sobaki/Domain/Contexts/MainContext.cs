using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;

namespace Sobaki.Domain.Contexts
{
    public class MainContext
    {
		private Stack<ViewModel> _history = new Stack<ViewModel>();

		public ViewModel CurrentViewModel => _history.Peek();

		public void PushViewModel(Func<ViewModel> func)
        {
            _history.Push(func());

            ViewModelChanged?.Invoke();
        }

		public void PopViewModel()
		{
			if (_history.Count > 0)
			{
				var viewModel = _history.Pop();
				viewModel?.Dispose();
				ViewModelChanged?.Invoke(); 
			}
		}

		public void PopAndPushViewModel(Func<ViewModel> func)
		{
            if (_history.Count > 0)
            {
				foreach (var viewModel in _history)
				{
					viewModel?.Dispose();
				}
				_history.Clear();

				_history.Push(func());

                ViewModelChanged?.Invoke();
            }
        }

		public event Action ViewModelChanged;
	}
}
