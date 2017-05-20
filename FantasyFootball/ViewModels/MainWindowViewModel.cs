using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyFootball.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private object currentView;
        public object CurrentView
        {
            get
            {
                return currentView;
            }
            set
            {
                currentView = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, HandelNotificationMessage);

            CheckSignInStatus();

        }

        public void CheckSignInStatus()
        {
            if (IsSignedIn)
            {
                //CurrentView = new UserView();
            }
            else
            {
                //CurrentView = new GuestView();
            }
        }

        private void HandelNotificationMessage(NotificationMessage notificationMessage)
        {
            switch (notificationMessage.Notification)
            {
                case "SignedIn":
                case "SignedOut":
                default:
                    CheckSignInStatus();
                    break;
            }
        }
    }
}
