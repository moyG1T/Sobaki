
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
    public class DogsViewModel : ViewModel
    {
        private readonly INavService _dog;
        private readonly DogContext _dogContext;
        private readonly StrayDogzEntities _db;

        public ICommand PopCommand { get; }
        public ICommand ReverseDateCommand { get; }
        public ICommand PushDogCommand { get; }

        private List<Dog> _initialDogsCollection = new List<Dog>();
        private List<Dog> _dogs = new List<Dog>();
        public List<Dog> Dogs
        {
            get => _dogs;
            set
            {
                _dogs = value; OnPropertyChanged();
            }
        }
        private bool _isDescending = false;

        private bool _isDeadFilter;
        public bool IsDeadFilter
        {
            get => _isDeadFilter;
            set
            {
                _isDeadFilter = value;
                OnPropertyChanged();

                ApplyFilters();
            }
        }
        private bool _isGivenFilter;
        public bool IsGivenFilter
        {
            get => _isGivenFilter;
            set
            {
                _isGivenFilter = value;
                OnPropertyChanged();

                ApplyFilters();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();

                ApplyFilters();
            }
        }

        public DogsViewModel(INavService guest, INavService dog, DogContext dogContext, StrayDogzEntities db)
        {
            PopCommand = new PopAndPushCommand(guest);
            ReverseDateCommand = new RelayCommand(ReverseDate);
            PushDogCommand = new RelayCommand(PushDog);

            _dog = dog;
            _dogContext = dogContext;
            _db = db;

            Task.Run(LoadDogs);
        }

        private async Task LoadDogs()
        {
            _initialDogsCollection = await _db.Dogs.ToListAsync();

            Dogs = _isDescending
                ? _initialDogsCollection
                    .Where(it => it.IsDead == IsDeadFilter && it.IdGiven == IsGivenFilter)
                    .OrderByDescending(it => it.Timestamp)
                    .ToList()
                : _initialDogsCollection
                    .Where(it => it.IsDead == IsDeadFilter && it.IdGiven == IsGivenFilter)
                    .OrderBy(it => it.Timestamp)
                    .ToList();

            OnPropertyChanged(nameof(Dogs));
        }
        private void ApplyFilters()
        {
            Dogs = _isDescending
                ? string.IsNullOrEmpty(SearchText)
                    ? _initialDogsCollection
                        .Where(it => it.IsDead == IsDeadFilter && it.IdGiven == IsGivenFilter)
                        .OrderByDescending(it => it.Timestamp)
                        .ToList()
                    : _initialDogsCollection
                        .Where(it => it.IsDead == IsDeadFilter
                            && it.IdGiven == IsGivenFilter
                            && it.Number.ToString().ToLower().Contains(SearchText.ToLower()))
                        .OrderByDescending(it => it.Timestamp)
                        .ToList()
                : string.IsNullOrEmpty(SearchText)
                    ? _initialDogsCollection
                        .Where(it => it.IsDead == IsDeadFilter && it.IdGiven == IsGivenFilter)
                        .OrderBy(it => it.Timestamp)
                        .ToList()
                    : _initialDogsCollection
                        .Where(it => it.IsDead == IsDeadFilter
                            && it.IdGiven == IsGivenFilter
                            && it.Number.ToString().ToLower().Contains(SearchText.ToLower()))
                        .OrderBy(it => it.Timestamp)
                        .ToList();
        }
        private void ReverseDate()
        {
            _isDescending = !_isDescending;

            Dogs = _isDescending
                ? Dogs
                    .OrderByDescending(it => it.Timestamp)
                    .ToList()
                : Dogs
                    .OrderBy(it => it.Timestamp)
                    .ToList();
        }
        private void PushDog(object param)
        {
            if (param is Dog dog)
            {
                _dogContext.SelectedDog = dog;
                _dog.Push();
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
