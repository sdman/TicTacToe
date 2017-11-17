
using System.Windows;
using TicTacToe;
using WpfGame.ViewModel;

namespace WpfGame
{
    /// <summary>
    /// Логика взаимодействия для NewWaitWindow.xaml
    /// </summary>
    public partial class NewWaitWindow : Window
    {
        private MainViewModel mainViewModel;

        private MainWindow mainWindow;

        public NewWaitWindow(CellState cellState)
        {
            mainWindow = new MainWindow(this, cellState);            
            InitializeComponent();
        }
    }
}
