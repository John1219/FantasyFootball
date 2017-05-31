namespace WebReader.ProFootballReference
{
    public class GameLog
    {
        public int Id { get; set; }
        public string GameDate { get; set; }
        public string GameNumber { get; set; }
        public string Team { get; set; }
        public string GameLocation { get; set; }
        public string Opponent { get; set; }
        public string GameResult { get; set; }
        public string Targets { get; set; }
        public string Receptions { get; set; }
        public string ReceivingYards { get; set; }
        public string ReceivingTouchdowns { get; set; }
        public string TwoPointsMade { get; set; }
        public string AllTouchdowns { get; set; }
        public string Points { get; set; }
        public string Sacks { get; set; }
        public string SoloTackles { get; set; }
        public string Assists { get; set; }
    }
}
