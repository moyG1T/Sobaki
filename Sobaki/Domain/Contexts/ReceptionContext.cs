using Sobaki.Data.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sobaki.Domain.Contexts
{
    public class ReceptionContext
    {
        public event Action<Reception> ReceptionAdded;
        public void AddReceptionNotification(Reception reception)
        {
            ReceptionAdded?.Invoke(reception);
        }
    }
}
