using FantasyFootball.Models;
using System.Collections.Generic;
using MyToolbox.ProFootballReference;

namespace FantasyFootball.ViewModels
{
    public class PlayerViewModel : BasePlayerViewModel
    {

        private Player selected_player;
        public Player SelectedPlayer
        {
            get { return selected_player; }
            set
            {
                selected_player = value;
                PlayerPool.CheckPlayerInfo(selected_player);
                RaisePropertyChanged();
            }
        }

        public PlayerViewModel()
        {

        }
    }
}
