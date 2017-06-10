using FantasyFootball.Models;
using System;
using System.Collections.Generic;
using MyToolbox.ProFootballReference;

namespace FantasyFootball.ViewModels
{
    public class AllPlayersViewModel : BaseViewModel
    {

        private List<Player> all_players;
        public List<Player> AllPlayers
        {
            get { return all_players; }
            set
            {
                all_players = value;
                RaisePropertyChanged();
            }
        }

        private Player selected_player;
        public Player SelectedPlayer
        {
            get { return selected_player; }
            set
            {
                selected_player = value;
                RaisePropertyChanged();
            }
        }

        public AllPlayersViewModel()
        {
            player_pool = new FootballPlayerPool();
            player_pool.PlayerListChanged += RefreshPlayerList;
            AllPlayers = player_pool.AllPlayers;
        }

        private FootballPlayerPool player_pool;

        private void RefreshPlayerList()
        {
            AllPlayers = player_pool.AllPlayers;
        }
    }
}
