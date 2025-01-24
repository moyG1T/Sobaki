using Sobaki.Data.Remote;
using Sobaki.Domain.Commands;
using Sobaki.Domain.Contexts;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class AddReceptionViewModel : ViewModel
    {
        private Dog _selectedDog;
        private readonly ReceptionContext _receptionContext;
        private readonly UserContext _userContext;
        private readonly StrayDogzEntities _db;

        public Dog SelectedDog
        {
            get { return _selectedDog; }
            set { _selectedDog = value; OnPropertyChanged(); }
        }

        private string _diseaseText;
        public string DiseaseText
        {
            get { return _diseaseText; }
            set { _diseaseText = value; OnPropertyChanged(); }
        }
        private string _recText;
        public string RecText
        {
            get { return _recText; }
            set { _recText = value; OnPropertyChanged(); }
        }
        private string _commText;
        public string CommText
        {
            get { return _commText; }
            set { _commText = value; OnPropertyChanged(); }
        }


        public List<Dog> Dogs { get; set; }

        public ICommand PopCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public AddReceptionViewModel(INavService back, ReceptionContext receptionContext, UserContext userContext, StrayDogzEntities db)
        {
            PopCommand = new PopCommand(back);
            SaveChangesCommand = new RelayAsyncCommand(SaveChangesAsync);

            _receptionContext = receptionContext;
            _userContext = userContext;
            _db = db;

            _ = LoadDogs();
        }

        private async Task LoadDogs()
        {
            Dogs = await _db.Dogs.ToListAsync();
            OnPropertyChanged(nameof(Dogs));
        }

        private async Task SaveChangesAsync()
        {
            if (SelectedDog == null
                || string.IsNullOrEmpty(DiseaseText)
                || string.IsNullOrEmpty(RecText)
                || string.IsNullOrEmpty(CommText))
            {
                MessageBox.Show("Заполните поля");
                return;
            }

            try
            {
                var reception = new Reception
                {
                    Comment = CommText,
                    Disease = DiseaseText,
                    DogId = SelectedDog.Id,
                    Recommendation = RecText,
                    Timestamp = DateTime.Now,
                    VeterinarId = _userContext.User.Id,
                };

                _db.Receptions.Add(reception);
                await _db.SaveChangesAsync();

                _receptionContext.AddReceptionNotification(reception);

                PopCommand?.Execute(this);
                MessageBox.Show("Добавлено");
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так :[");
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
