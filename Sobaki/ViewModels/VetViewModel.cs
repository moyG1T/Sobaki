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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sobaki.ViewModels
{
    public class VetViewModel : ViewModel
    {
        private readonly ReceptionContext _receptionContext;
        private readonly StrayDogzEntities _db;

        public ObservableCollection<Reception> Receptions { get; set; } = new ObservableCollection<Reception>();

        public ICommand PopCommand { get; }
        public ICommand PushReceptionCommand { get; }

        public VetViewModel(INavService guest, INavService reception, ReceptionContext receptionContext, StrayDogzEntities db)
        {
            PopCommand = new PopAndPushCommand(guest);
            PushReceptionCommand = new PushCommand(reception);

            _receptionContext = receptionContext;
            _db = db;

            _receptionContext.ReceptionAdded += AddReception;

            _ = LoadReceptions();
        }

        private async Task LoadReceptions()
        {
            var receptions = await _db.Receptions
                .OrderByDescending(it => it.Timestamp)
                .Take(20)
                .ToListAsync();

            Receptions = new ObservableCollection<Reception>(receptions);

            OnPropertyChanged(nameof(Receptions));
        }

        private void AddReception(Reception reception)
        {
            Receptions.Add(reception);
        }

        public override void Dispose()
        {
            _receptionContext.ReceptionAdded -= AddReception;

            GC.SuppressFinalize(this);
        }
    }
}
