using Sobaki.Data.Remote;
using System;

namespace Sobaki.Domain.Contexts
{
    public class DogContext
    {
        public Dog SelectedDog { get; set; }

        public event Action<int> DogDied;
        public void KillDogNotification(int dogId)
        {
            DogDied?.Invoke(dogId);
        }
    }
}
