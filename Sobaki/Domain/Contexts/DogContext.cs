using Sobaki.Data.Remote;
using System;

namespace Sobaki.Domain.Contexts
{
    public class DogContext
    {
        public Dog SelectedDog { get; set; }

        public event Action<Dog> DogAdded;
        public event Action<Dog> DogChanged;
        public event Action<int> DogDied;
        public void KillDogNotification(int dogId)
        {
            DogDied?.Invoke(dogId);
        }
        public void ChangeDogNotification(Dog dog)
        {
            DogChanged?.Invoke(dog);
        }
        public void AddDogNotification(Dog dog)
        {
            DogAdded?.Invoke(dog);
        }
    }
}
