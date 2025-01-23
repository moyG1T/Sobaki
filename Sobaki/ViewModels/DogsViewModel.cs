
using Sobaki.Data.Remote;
using Sobaki.Domain.Commands;
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
        private readonly StrayDogzEntities _db;

        public ICommand PopCommand { get; }
        public ICommand ReverseDateCommand { get; }

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


        public DogsViewModel(INavService back, StrayDogzEntities db)
        {
            PopCommand = new PopCommand(back);
            ReverseDateCommand = new RelayCommand(ReverseDate);

            _db = db;

            Task.Run(LoadDogs);
        }

        private async Task LoadDogs()
        {
            if (_isDescending)
            {
                Dogs = await _db.Dogs
                    .Where(it => !it.IsDead && !it.IdGiven)
                    .OrderByDescending(it => it.Timestamp)
                    .ToListAsync();
            }
            else
            {
                Dogs = await _db.Dogs
                    .Where(it => !it.IsDead && !it.IdGiven)
                    .OrderBy(it => it.Timestamp)
                    .ToListAsync();
            }
            OnPropertyChanged(nameof(Dogs));
        }

        private void ApplyFilters()
        {
            if (_isDescending)
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    Dogs = Dogs
                        .Where(it => it.IsDead == IsDeadFilter && it.IdGiven == IsGivenFilter)
                        .OrderByDescending(it => it.Timestamp)
                        .ToList();
                }
                else
                {
                    Dogs = Dogs
                        .Where(it => it.IsDead == IsDeadFilter
                            && it.IdGiven == IsGivenFilter
                            && SearchText.ToLower().Contains(it.Number.ToString().ToLower()))
                        .OrderByDescending(it => it.Timestamp)
                        .ToList();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    Dogs = Dogs
                        .Where(it => it.IsDead == IsDeadFilter && it.IdGiven == IsGivenFilter)
                        .OrderBy(it => it.Timestamp)
                        .ToList();
                }
                else
                {
                    Dogs = Dogs
                        .Where(it => it.IsDead == IsDeadFilter
                            && it.IdGiven == IsGivenFilter
                            && SearchText.ToLower().Contains(it.Number.ToString().ToLower()))
                        .OrderBy(it => it.Timestamp)
                        .ToList();
                }
            }
        }

        private void ReverseDate()
        {
            _isDescending = !_isDescending;

            if (_isDescending)
            {
                Dogs = Dogs
                    .OrderByDescending(it => it.Timestamp)
                    .ToList();
            }
            else
            {
                Dogs = Dogs
                    .OrderBy(it => it.Timestamp)
                    .ToList();
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
