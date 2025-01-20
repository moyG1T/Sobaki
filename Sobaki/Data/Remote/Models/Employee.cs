using Sobaki.Domain.Utilities;

namespace Sobaki.Data.Remote
{
    public partial class Employee : ObservableObject
    {
        private string _login;
        public string Login
        {
            get => _login; set
            {
                _login = value; OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password; set
            {
                _password = value; OnPropertyChanged();
            }
        }
    }
}
