using System;
using System.Windows;

namespace WpfGame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            MainViewModel m = new MainViewModel();
            m.RestartNeeded += OnRestartNeeded;
            DataContext = m;
        }

        private void OnRestartNeeded(object sender, EventArgs eventArgs)
        {
            GamersSelectDialog dialog = new GamersSelectDialog();
            dialog.Show();

            Close();
        }
    }
}