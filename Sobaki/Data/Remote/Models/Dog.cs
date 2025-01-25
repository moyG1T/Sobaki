using Sobaki.Domain.Utilities;

namespace Sobaki.Data.Remote
{
    public partial class Dog : ObservableObject
    {
        private byte[] _binImage;
        public byte[] BinImage
        {
            get => _binImage; set
            {
                _binImage = value;
                OnPropertyChanged();
            }
        }
    }
}
