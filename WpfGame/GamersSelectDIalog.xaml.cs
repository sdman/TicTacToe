using System;
using System.Windows;
using WpfGame.Model;
using WpfGame.ViewModel;

namespace WpfGame
{
    public partial class GamersSelectDialog : Window
    {
        public GamersSelectDialog()
        {
            InitializeComponent();
        }

        private void Window_OnInitialized(object sender, EventArgs e)
        {
            IBotSelector botSelector = new BotSelector();
            GamersSelectViewModel vm = new GamersSelectViewModel(botSelector);
            vm.PlayersSelected += OnPlayersSelected;

            DataContext = vm;
        }

        private void OnPlayersSelected(object sender, EventArgs eventArgs)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }
    }
}