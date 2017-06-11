using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MyToolbox.ProFootballReference;

namespace FantasyFootball.Models
{
    public class FootballPlayerPool
    {
        public event Action PlayerListChanged = delegate { };

        private List<Player> all_players;
        public List<Player> AllPlayers
        {
            get { return all_players; }
            set
            {
                all_players = value;
            }
        }

        public FootballPlayerPool()
        {
            pfr = new ProFootballReference();
            pfr.PlayerListChanged += UpdatePlayerList;
            pfr.LoadPlayers();
        }

        public void GetPlayerInfo(Player player)
        {
            pfr.LoadPlayerData(player);
        }

        public bool CheckPlayerInfo(Player player)
        {
            if(player.LastUpdated < DateTime.Now.AddDays(-1))
            {
                GetPlayerInfo(player);
                return true;
            }
            return false;
        }

        private ProFootballReference pfr;

        private void UpdatePlayerList()
        {
            AllPlayers = pfr.AllPlayers;
            PlayerListChanged();
        }
    }
}
