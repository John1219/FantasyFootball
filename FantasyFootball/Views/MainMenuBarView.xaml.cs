using FantasyFootball.ViewModels;
using System.Windows.Controls;

namespace FantasyFootball.Views
{
    /// <summary>
    /// Interaction logic for MainMenuBarView.xaml
    /// </summary>
    public partial class MainMenuBarView : UserControl
    {
        public MainMenuBarView()
        {
            InitializeComponent();
            DataContext = new MainMenuBarViewModel();
        }
    }
}
