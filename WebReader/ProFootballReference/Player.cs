using System;
using System.Collections.Generic;

namespace MyToolbox.ProFootballReference
{
    public class Player
    {
        public DateTime LastUpdated { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Years { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string DeathDate { get; set; }
        public string College { get; set; }
        public string HighSchool { get; set; }
        public string Team { get; set; }
        public string DraftedTeam { get; set; }
        public string DraftedPosition { get; set; }
        public string DraftedYear { get; set; }
        public string Twitter { get; set; }
        public string HeadShotPath { get; set; }
        public string GameLogsLink { get; set; }
        public List<GameLog> GameLogs { get; set; }
    }
}
