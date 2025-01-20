using Sobaki.Domain.Contexts;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;

namespace Sobaki.Domain.Services
{
    public class MainNavService : INavService
    {
        private readonly MainContext _mainContext;
        private readonly Func<ViewModel> _func;

        public MainNavService(MainContext mainContext, Func<ViewModel> func = null)
        {
            _mainContext = mainContext;
            _func = func;
        }

        public void Pop()
        {
            _mainContext.PopViewModel();
        }

        public void Push()
        {
            _mainContext.PushViewModel(_func);
        }

        public void PopAndPush()
        {
            _mainContext.PopAndPushViewModel(_func);
        }
    }
}
