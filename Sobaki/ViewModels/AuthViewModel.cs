using Sobaki.Data.Remote;
using Sobaki.Domain.Commands;
using Sobaki.Domain.Contexts;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class AuthViewModel : ViewModel
    {
        private readonly UserContext _userContext;
        private readonly StrayDogzEntities _db;

        private string _loginText;
        public string LoginText
        {
            get => _loginText;
            set
            {
                _loginText = value; OnPropertyChanged();
            }
        }

        public ICommand PopCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand PushAdminCommand { get; private set; }

        public AuthViewModel(INavService back, INavService admin, UserContext userContext, StrayDogzEntities db)
        {
            PopCommand = new PopCommand(back);
            LoginCommand = new RelayAsyncCommand(LoginAsync);
            PushAdminCommand = new PushCommand(admin);

            _userContext = userContext;
            _db = db;
        }

        private async Task LoginAsync(object param)
        {
            if (param is PasswordBox passwordBox)
            {
                var password = passwordBox.Password;

                var user = await _db.Employees
                    .FirstOrDefaultAsync(it => it.Login == LoginText && it.Password == password);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин");
                    return;
                }

                _userContext.User = user;

                switch (user.PostId)
                {
                    case 2:
                        PushAdminCommand?.Execute(null);
                        break;
                    default:
                        MessageBox.Show("Что-то пошло не так :[");
                        passwordBox.Password = string.Empty;
                        break;
                }
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
