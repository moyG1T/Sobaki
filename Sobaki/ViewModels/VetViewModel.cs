using Sobaki.Data.Remote;
using Sobaki.Domain.Commands;
using Sobaki.Domain.Contexts;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class VetViewModel : ViewModel
    {
        private readonly ReceptionContext _receptionContext;
        private readonly StrayDogzEntities _db;

        private List<Reception> _initialReceptions = new List<Reception>();
        private ObservableCollection<Reception> _receptions = new ObservableCollection<Reception>();
        public ObservableCollection<Reception> Receptions
        {
            get => _receptions;
            set
            {
                _receptions = value; OnPropertyChanged();
            }
        }

        public List<Dog> Dogs { get; set; }

        private Dog _selectedDog;
        public Dog SelectedDog
        {
            get { return _selectedDog; }
            set
            {
                _selectedDog = value;
                OnPropertyChanged();

                if (value is null)
                {
                    _ = LoadDogReceptions(0);
                }
                else
                {
                    _ = LoadDogReceptions(value.Id);
                }
            }
        }

        public ICommand PopCommand { get; }
        public ICommand PushReceptionCommand { get; }
        public ICommand PushDogCommand { get; }
        public ICommand RemoveSelectedDogCommand { get; }

        public VetViewModel(INavService guest, INavService reception, INavService dog, ReceptionContext receptionContext, StrayDogzEntities db)
        {
            PopCommand = new PopAndPushCommand(guest);
            PushReceptionCommand = new PushCommand(reception);
            PushDogCommand = new PushCommand(dog);
            RemoveSelectedDogCommand = new RelayCommand(RemoveSelectedDog);

            _receptionContext = receptionContext;
            _db = db;

            _receptionContext.ReceptionAdded += AddReception;

            _ = LoadContent();
        }

        private async Task LoadContent()
        {
            Dogs = await _db.Dogs.ToListAsync();
            OnPropertyChanged(nameof(Dogs));

            _initialReceptions = await _db.Receptions
                .OrderByDescending(it => it.Timestamp)
                .Take(20)
                .ToListAsync();

            Receptions = new ObservableCollection<Reception>(_initialReceptions);
        }

        private void RemoveSelectedDog()
        {
            SelectedDog = null;
        }

        private async Task LoadDogReceptions(int dogId)
        {
            if (dogId == 0)
            {
                Receptions = new ObservableCollection<Reception>(_initialReceptions);
            }
            else
            {
                var dogReceptions = await _db.Receptions
                    .Where(it => it.DogId == dogId)
                    .ToListAsync();

                Receptions = new ObservableCollection<Reception>(dogReceptions);
            }
        }

        private void AddReception(Reception reception)
        {
            _initialReceptions.Add(reception);
            Receptions.Add(reception);
        }

        public override void Dispose()
        {
            _receptionContext.ReceptionAdded -= AddReception;

            GC.SuppressFinalize(this);
        }
    }
}
