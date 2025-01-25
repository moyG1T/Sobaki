using Sobaki.Data.Remote;
using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using Sobaki.Domain.Commands;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Contexts;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;

namespace Sobaki.ViewModels
{
    public class DogEditViewModel : ViewModel
    {
        public List<Gender> Genders { get; set; }
        private Gender _selectedGender;
        public Gender SelectedGender
        {
            get { return _selectedGender; }
            set { _selectedGender = value; OnPropertyChanged(); }
        }
        public List<Breed> Breeds { get; set; }
        private Breed _selectedBreed;
        public Breed SelectedBreed
        {
            get { return _selectedBreed; }
            set { _selectedBreed = value; OnPropertyChanged(); }
        }
        public List<Cage> Cages { get; set; }
        private Cage _selectedCage;
        public Cage SelectedCage
        {
            get { return _selectedCage; }
            set { _selectedCage = value; OnPropertyChanged(); }
        }

        private readonly DogContext _dogContext;
        private readonly StrayDogzEntities _db;

        private Dog _dog = new Dog() { Timestamp = DateTime.Now };
        public Dog Dog
        {
            get { return _dog; }
            set { _dog = value; OnPropertyChanged(); }
        }

        public ICommand PopCommand { get; }
        public ICommand OpenFileDialogCommand { get; }
        public ICommand KillDogCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public DogEditViewModel(INavService back, DogContext dogContext, StrayDogzEntities db)
        {
            PopCommand = new PopCommand(back);
            OpenFileDialogCommand = new RelayCommand(OpenImage);
            SaveChangesCommand = new RelayAsyncCommand(SaveChangesAsync);
            KillDogCommand = new RelayAsyncCommand(KillDogAsync);

            _dogContext = dogContext;
            _db = db;

            Genders = _db.Genders.ToList();
            Breeds = _db.Breeds.ToList();
            Cages = _db.Cages.ToList();

            if (_dogContext.SelectedDog != null)
            {
                Dog = new Dog
                {
                    Id = _dogContext.SelectedDog.Id,
                    Number = _dogContext.SelectedDog.Number,
                    Height = _dogContext.SelectedDog.Height,
                    Weight = _dogContext.SelectedDog.Weight,
                    Age = _dogContext.SelectedDog.Age,
                    Gender = _dogContext.SelectedDog.Gender,
                    Timestamp = _dogContext.SelectedDog.Timestamp,
                    BinImage = _dogContext.SelectedDog.BinImage,
                    Breed = _dogContext.SelectedDog.Breed,
                };
                SelectedBreed = Dog.Breed;
                SelectedGender = Dog.Gender;
            }
        }

        private async Task SaveChangesAsync()
        {
            var dog = Dog.Id == 0 ? null : await _db.Dogs.FirstOrDefaultAsync(it => it.Id == Dog.Id);

            if (dog != null)
            {
                dog.Weight = Dog.Weight;
                dog.Height = Dog.Height;
                dog.Age = Dog.Age;
                dog.Timestamp = Dog.Timestamp;
                dog.BinImage = Dog.BinImage;
                dog.Breed = SelectedBreed;
                dog.Gender = SelectedGender;
                dog.Cage = SelectedCage;

                await _db.SaveChangesAsync();
                Dog = dog;

                PopCommand?.Execute(null);
                MessageBox.Show("Сохранено");
            }
            else
            {
                try
                {
                    var addingDog = new Dog
                    {
                        Number = "temp123",
                        Weight = Dog.Weight,
                        Height = Dog.Height,
                        Age = Dog.Age,
                        Gender = SelectedGender,
                        Timestamp = Dog.Timestamp,
                        BinImage = Dog.BinImage,
                        Breed = SelectedBreed,
                        Phone = 123,
                        Cage = SelectedCage,
                    };

                    _db.Dogs.Add(addingDog);
                    await _db.SaveChangesAsync();

                    addingDog.Number = $"D{addingDog.Id}_{addingDog.Timestamp:dd-MM-yy}";
                    await _db.SaveChangesAsync();

                    _dogContext.AddDogNotification(addingDog);

                    PopCommand?.Execute(null);
                    MessageBox.Show("Добавлено");
                }
                catch (Exception)
                {
                    MessageBox.Show("Что-то пошло не так :[");
                }
            }
        }

        private async Task KillDogAsync()
        {
            var deadDog = new DeadDog
            {
                DogId = Dog.Id,
                Timestamp = DateTime.Now,
            };

            _db.DeadDogs.Add(deadDog);
            await _db.SaveChangesAsync();

            _dogContext.KillDogNotification(Dog.Id);

            PopCommand?.Execute(null);
            MessageBox.Show("Собака умэрла");
        }

        private void OpenImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Медиа|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                Dog.BinImage = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        public override void Dispose()
        {
            _dogContext.SelectedDog = null;
            GC.SuppressFinalize(this);
        }
    }
}
