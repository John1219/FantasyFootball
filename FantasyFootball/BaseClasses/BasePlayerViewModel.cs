using FantasyFootball.Models;

namespace FantasyFootball
{
    public class BasePlayerViewModel : BaseViewModel
    {
        protected BasePlayerViewModel()
        {
            player_pool = new FootballPlayerPool();
        }

        protected FootballPlayerPool PlayerPool
        { get => player_pool; set => player_pool = value; }

        private static FootballPlayerPool player_pool;
    }
}
