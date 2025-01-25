
using Microsoft.Win32;
using Sobaki.Data.Remote;
using Sobaki.Domain.Commands;
using Sobaki.Domain.Contexts;
using Sobaki.Domain.IServices;
using Sobaki.Domain.Models;
using Sobaki.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Document.NET;
using Xceed.Words.NET;

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
        public ICommand FormReportCommand { get; }

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

        public List<ArchivalReason> ArchivalReasons { get; } = new List<ArchivalReason>
        {
            new ArchivalReason{ Id = 1, Title = "Смэрть" },
            new ArchivalReason{ Id = 2, Title = "Отдан" },
            new ArchivalReason{ Id = 3, Title = "Все" },
        };

        private ArchivalReason _archivalReason;
        public ArchivalReason ArchivalReason
        {
            get { return _archivalReason; }
            set { _archivalReason = value; OnPropertyChanged(); }
        }

        private DateTime _startTimestamp = DateTime.Today.AddDays(-1);
        public DateTime StartTimestamp
        {
            get { return _startTimestamp; }
            set { _startTimestamp = value; OnPropertyChanged(); }
        }

        private DateTime _endTimestamp = DateTime.Today.AddDays(1);
        public DateTime EndTimestamp
        {
            get { return _endTimestamp; }
            set { _endTimestamp = value; OnPropertyChanged(); }
        }

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

        public DogsViewModel(INavService back, INavService dog, DogContext dogContext, StrayDogzEntities db)
        {
            PopCommand = new PopCommand(back);
            ReverseDateCommand = new RelayCommand(ReverseDate);
            PushDogCommand = new RelayCommand(PushDog);
            FormReportCommand = new RelayAsyncCommand(FormReport);

            _dog = dog;
            _dogContext = dogContext;
            _db = db;

            _dogContext.DogDied += KillDog;

            Task.Run(LoadDogs);
        }

        private async Task LoadDogs()
        {
            _initialDogsCollection = await _db.Dogs.ToListAsync();

            Dogs = _isDescending
                ? _initialDogsCollection
                    .Where(it => it.GivenDogs.Count != 0 == IsGivenFilter && it.DeadDogs.Count != 0 == IsDeadFilter)
                    .OrderByDescending(it => it.Timestamp)
                    .ToList()
                : _initialDogsCollection
                    .Where(it => it.GivenDogs.Count != 0 == IsGivenFilter && it.DeadDogs.Count != 0 == IsDeadFilter)
                    .OrderBy(it => it.Timestamp)
                    .ToList();

            OnPropertyChanged(nameof(Dogs));
        }
        private void ApplyFilters()
        {
            Dogs = _isDescending
                ? string.IsNullOrEmpty(SearchText)
                    ? _initialDogsCollection
                        .Where(it => (it.GivenDogs.Count != 0 == IsGivenFilter) && (it.DeadDogs.Count != 0 == IsDeadFilter))
                        .OrderByDescending(it => it.Timestamp)
                        .ToList()
                    : _initialDogsCollection
                        .Where(it => (it.GivenDogs.Count != 0 == IsGivenFilter) 
                            && (it.DeadDogs.Count != 0 == IsDeadFilter)
                            && it.Number.ToString().ToLower().Contains(SearchText.ToLower()))
                        .OrderByDescending(it => it.Timestamp)
                        .ToList()
                : string.IsNullOrEmpty(SearchText)
                    ? _initialDogsCollection
                    .Where(it => (it.GivenDogs.Count != 0 == IsGivenFilter) && (it.DeadDogs.Count != 0 == IsDeadFilter))
                        .OrderBy(it => it.Timestamp)
                        .ToList()
                    : _initialDogsCollection
                        .Where(it => (it.GivenDogs.Count != 0 == IsDeadFilter)
                            && (it.DeadDogs.Count != 0 == IsGivenFilter)
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
            if (param is Dog dog && dog.DeadDogs.Count == 0 && dog.GivenDogs.Count == 0)
            {
                _dogContext.SelectedDog = dog;
                _dog.Push();
            }
        }
        private void KillDog(int dogId)
        {
            var dog = Dogs.FirstOrDefault(it => it.Id == dogId);
            if (dog != null)
            {
                Dogs.Remove(dog);
                ApplyFilters();
            }
        }

        private async Task FormReport()
        {
            if (StartTimestamp >= EndTimestamp)
            {
                MessageBox.Show("Неправильная дата");
                return;
            }

            if (ArchivalReason == null)
            {
                ArchivalReason = ArchivalReasons.First(it => it.Id == 3);
            }

            switch (ArchivalReason.Id)
            {
                case 1:
                    var deadDogs = await _db.Dogs
                        .Where(it => it.Timestamp >= StartTimestamp && it.Timestamp <= EndTimestamp)
                        .Where(it => it.DeadDogs.Count != 0 && it.GivenDogs.Count == 0)
                        .ToListAsync();

                    MakeReport(deadDogs);
                    break;
                case 2:
                    var givenDogs = await _db.Dogs
                        .Where(it => it.Timestamp >= StartTimestamp && it.Timestamp <= EndTimestamp)
                        .Where(it => it.GivenDogs.Count != 0 && it.DeadDogs.Count == 0)
                        .ToListAsync();

                    MakeReport(givenDogs);
                    break;
                case 3:
                    var dogs = await _db.Dogs
                        .Where(it => 
                        it.GivenDogs.Any(gd => gd.Timestamp >= StartTimestamp && gd.Timestamp <= EndTimestamp) 
                        ||
                        it.DeadDogs.Any(gd => gd.Timestamp >= StartTimestamp && gd.Timestamp <= EndTimestamp))
                        .ToListAsync();

                    MakeReport(dogs);
                    break;
                default:
                    break;
            }
        }

        private void MakeReport(ICollection<Dog> dogs)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                Title = "Сохранить отчет",
                FileName = "Отчет.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;

                    var docx = DocX.Create(filePath);
                    var title = docx.InsertParagraph("Архив собак");
                    title.Alignment = Alignment.center;
                    title.FontSize(14);

                    var table = docx.AddTable(1, 3);
                    table.Rows[0].Cells[0].Paragraphs[0].Append("Номер");
                    table.Rows[0].Cells[1].Paragraphs[0].Append("Причина");
                    table.Rows[0].Cells[2].Paragraphs[0].Append("Дата");

                    foreach (var dog in dogs)
                    {
                        var isDead = dog.DeadDogs.Count != 0;
                        var isGiven = dog.GivenDogs.Count != 0;

                        if (isDead && isGiven)
                        {
                            var deadTimestamp = dog.DeadDogs.First().Timestamp.ToString("dd MMM yyyy");
                            var deadRow = table.InsertRow();
                            deadRow.Cells[0].Paragraphs[0].Append(dog.Number);
                            deadRow.Cells[1].Paragraphs[0].Append("Умерла");
                            deadRow.Cells[2].Paragraphs[0].Append(deadTimestamp);

                            var givenTimestamp = dog.DeadDogs.First().Timestamp.ToString("dd MMM yyyy");
                            var givenRow = table.InsertRow();
                            givenRow.Cells[0].Paragraphs[0].Append(dog.Number);
                            givenRow.Cells[1].Paragraphs[0].Append("Отдана");
                            givenRow.Cells[2].Paragraphs[0].Append(deadTimestamp);
                        }
                        else if (isDead)
                        {
                            var timestamp = dog.DeadDogs.First().Timestamp.ToString("dd MMM yyyy");

                            var row = table.InsertRow();
                            row.Cells[0].Paragraphs[0].Append(dog.Number);
                            row.Cells[1].Paragraphs[0].Append("Умерла");
                            row.Cells[2].Paragraphs[0].Append(timestamp);

                        }
                        else if (isGiven)
                        {
                            var timestamp = dog.GivenDogs.First().Timestamp.ToString("dd MMM yyyy");

                            var row = table.InsertRow();
                            row.Cells[0].Paragraphs[0].Append(dog.Number);
                            row.Cells[1].Paragraphs[0].Append("Отдана");
                            row.Cells[2].Paragraphs[0].Append(timestamp);
                        }
                    }

                    docx.InsertTable(table);

                    docx.Save();

                    MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public override void Dispose()
        {
            _dogContext.DogDied -= KillDog;

            GC.SuppressFinalize(this);
        }
    }
}
