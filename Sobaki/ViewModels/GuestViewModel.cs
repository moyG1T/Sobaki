using Sobaki.Data.Remote;
using Sobaki.Domain.Commands;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Sobaki.ViewModels
{
    public class GuestViewModel : ViewModel
    {
        private readonly StrayDogzEntities _db;

        public ICommand CallAdminCommand { get; }
        public ICommand PushAuthCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand TakeDogCommand { get; }

        public ObservableCollection<Dog> Dogs { get; set; } = new ObservableCollection<Dog>();

        public GuestViewModel(INavService auth, INavService call, StrayDogzEntities db)
        {
            PushAuthCommand = new PushCommand(auth);
            CallAdminCommand = new PushCommand(call);
            TakeDogCommand = new RelayAsyncCommand(TakeDog);
            _db = db;

            Task.Run(LoadDogs);
        }

        private async Task TakeDog(object param)
        {
            if (param is Dog dog)
            {
                var givenDog = new GivenDog
                {
                    DogId = dog.Id,
                    Timestamp = DateTime.Now,
                };

                _db.GivenDogs.Add(givenDog);
                await _db.SaveChangesAsync();

                MessageBox.Show("Спасибо!");
            }
        }

        private async Task LoadDogs()
        {
            var dogs = await _db.Dogs
                .Where(it => it.GivenDogs.Count == 0 && it.DeadDogs.Count == 0)
                .ToListAsync();

            Dogs = new ObservableCollection<Dog>(dogs);
            OnPropertyChanged(nameof(Dogs));
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
