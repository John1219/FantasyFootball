using FantasyFootball.Models;
using System;
using System.Collections.Generic;
using MyToolbox.ProFootballReference;
using System.Linq;

namespace FantasyFootball.ViewModels
{
    public class AllPlayersViewModel : BasePlayerViewModel
    {
        private List<string> filter_options;
        public List<string> FilterOptions
        {
            get { return filter_options; }
        }

        private string selected_filter;
        public string SelectedFilter
        {
            get { return selected_filter; }
            set
            {
                selected_filter = value;
                RaisePropertyChanged();
                RaisePropertyChanged("AllPlayers");
            }
        }


        public List<Player> AllPlayers
        {
            get { return GetFilteredPlayerList(); }
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
            filter_options = new List<string>();
            filter_options.Add("None");
            filter_options.Add("Active Only");
            selected_filter = filter_options[0];
        }

        private List<Player> GetFilteredPlayerList()
        {
            if(selected_filter == "Active Only")
            {
                return PlayerPool.AllPlayers.Where(p => p.IsActive).ToList();
            }
            return PlayerPool.AllPlayers;
        }
    }
}
