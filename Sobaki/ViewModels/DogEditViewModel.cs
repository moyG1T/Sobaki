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


        private Dog _dog;
        private readonly DogContext _dogContext;
        private readonly StrayDogzEntities _db;

        public Dog Dog
        {
            get { return _dog; }
            set { _dog = value; OnPropertyChanged(); }
        }

        public ICommand PopCommand { get; }
        public ICommand OpenFileDialogCommand { get; }


        public DogEditViewModel(INavService back, DogContext dogContext, StrayDogzEntities db)
        {
            PopCommand = new PopCommand(back);
            OpenFileDialogCommand = new RelayCommand(OpenImage);

            _dogContext = dogContext;
            _db = db;

            Genders = _db.Genders.ToList();

            if (_dogContext.SelectedDog != null)
            {
                Dog = _dogContext.SelectedDog;
                SelectedGender = Dog.Gender;
            }
        }

        private void OpenImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "*.jpg;*.png|Медиа"
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
