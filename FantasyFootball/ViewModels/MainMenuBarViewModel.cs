using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;

namespace FantasyFootball.ViewModels
{
    public class MainMenuBarViewModel : BaseViewModel
    {
        public ICommand SignInCommand => new RelayCommand(SignIn);

        public bool SignedInStatus
        {
            get
            {
                return IsSignedIn;
            }
            set
            {
                IsSignedIn = value;
                RaisePropertyChanged();
            }
        }

        public MainMenuBarViewModel()
        {
            IsSignedIn = false;
        }

        private void SignIn()
        {
            IsSignedIn = !IsSignedIn;
            if (IsSignedIn)
            {
                MessengerInstance.Send(new NotificationMessage("SignedIn"));
            }
            else
            {
                MessengerInstance.Send(new NotificationMessage("SignedOut"));
            }
            RaisePropertyChanged("SignedInStatus");
        }
    }
}
