using System.Collections.Generic;
using System.Windows;
using Refit;
using TicTacToe;
using WpfGame.RefitRequest;
using WpfGame.RestModel.Forms;
using WpfGame.RestModel.Model;
using WpfGame.RestModel.Views;
using static WpfGame.Utils.Values;
using WpfGame.Utils;

namespace WpfGame.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для LobbyCreateWindow.xaml
    /// </summary>
    public partial class LobbyCreateWindow : Window
    {
        public LobbyCreateWindow()
        {
            InitializeComponent();            
        }

        public async void CreateLobby(object sender, RoutedEventArgs e)
        {
            LobbyCreateForm lobbyCreateForm = new LobbyCreateForm();
            CellState cellState;
            if (TicChoise.IsChecked == true)
            {
                lobbyCreateForm.PlayerType = PlayerType.Tic;
                cellState = CellState.Tick;
            }
            else
            {
                lobbyCreateForm.PlayerType = PlayerType.Tac;
                cellState = CellState.Tack;
            }            

            var ticTakApi = RestService.For<ITicTacApi>(URL);
            CurrentLobby = new Lobby(await ticTakApi.CreateLobby(CurrentToken.AuthToken, lobbyCreateForm));            

            NewWaitWindow waitWindow = new NewWaitWindow(cellState);  
            waitWindow.Show();
            Close();
        }
    }
}
