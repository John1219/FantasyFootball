using FantasyFootball.ViewModels;
using System.Windows.Controls;

namespace FantasyFootball.Views
{
    /// <summary>
    /// Interaction logic for AllPlayersView.xaml
    /// </summary>
    public partial class AllPlayersView : UserControl
    {
        public AllPlayersView()
        {
            InitializeComponent();
            DataContext = new AllPlayersViewModel();
        }
    }
}
