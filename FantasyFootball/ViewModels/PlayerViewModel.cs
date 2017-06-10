using FantasyFootball.Models;
using System.Collections.Generic;
using MyToolbox.ProFootballReference;

namespace FantasyFootball.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {

        private Player selected_player;
        public Player SelectedPlayer
        {
            get { return selected_player; }
            set
            {
                selected_player = value;
                if ((string.IsNullOrEmpty(selected_player.Height)) || (string.IsNullOrEmpty(selected_player.Weight)) || (string.IsNullOrEmpty(selected_player.BirthDate)) || (string.IsNullOrEmpty(selected_player.BirthPlace)) || (string.IsNullOrEmpty(selected_player.DeathDate)) || (string.IsNullOrEmpty(selected_player.College)))
                {
                    pfr.LoadPlayerData(selected_player);
                }
                RaisePropertyChanged();
            }
        }

        public PlayerViewModel()
        {
            pfr = new ProFootballReference();
            pfr.PlayerInfoChanged += RefreshPlayer;

        }

        private ProFootballReference pfr;

        private void RefreshPlayer()
        {
            RaisePropertyChanged("SelectedPlayer");
        }
    }
}
