using FantasyFootball.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using WebReader.ProFootballReference;

namespace FantasyFootball.Views
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : UserControl
    {
        public static readonly DependencyProperty CurrentPlayerProperty
            = DependencyProperty.Register("CurrentPlayer", typeof(Player), typeof(PlayerView), new PropertyMetadata(OnCurrentPlayerPropertyChanged));

        public PlayerView()
        {
            InitializeComponent();
            DataContext = new PlayerViewModel();
        }

        public Player CurrentPlayer
        {
            get { return (Player)GetValue(CurrentPlayerProperty); }
            set { SetValue(CurrentPlayerProperty, value); }
        }

        private static void OnCurrentPlayerPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PlayerView view = (PlayerView)source;
            PlayerViewModel view_model = (PlayerViewModel)view.DataContext;
            view_model.SelectedPlayer = (Player)e.NewValue;
        }
    }
}
