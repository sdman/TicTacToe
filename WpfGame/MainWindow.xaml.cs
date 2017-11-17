using System;
using System.Windows;
using TicTacToe;
using WpfGame.ViewModel;

namespace WpfGame
{
    public partial class MainWindow : Window
    {
        private MainViewModel m;

        public MainWindow(NewWaitWindow newWaitWindow, CellState cellState)
        {            
            m = new MainViewModel(this, newWaitWindow, cellState);

            InitializeComponent();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {            
            m.RestartNeeded += OnRestartNeeded;
            DataContext = m;
        }

        private void OnRestartNeeded(object sender, EventArgs eventArgs)
        {            
            Close();
        }
    }
}