using GalaSoft.MvvmLight;

namespace FantasyFootball
{
    public class BaseViewModel : ViewModelBase
    {
        private static bool is_signed_in;

        protected bool IsSignedIn
        { get => is_signed_in; set => is_signed_in = value; }
    }
}
