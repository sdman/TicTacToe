using System.Collections.Generic;
using System.Windows;
using Refit;
using TicTacToe;
using WpfGame.RefitRequest;
using WpfGame.RestModel.Forms;
using WpfGame.RestModel.Model;
using WpfGame.RestModel.Views;
using WpfGame.Utils;
using WpfGame.ViewModel;
using static WpfGame.Utils.Values;

namespace WpfGame
{
    /// <summary>
    /// Логика взаимодействия для LobbyListWindow.xaml
    /// </summary>
    public partial class LobbyListWindow : Window
    {
        ITicTacApi ticTakApi = RestService.For<ITicTacApi>(URL);
        private NewWaitWindow waitWindow;

        public  LobbyListWindow()
        {
            InitializeComponent();
            Lobbies = new Dictionary<string, Lobby>();
            Refresh();            
        }

        public void RefreshScreen(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public async void Refresh()
        {            
            IEnumerable<LobbyView> lobbieViews = await ticTakApi.GetLobbies(CurrentToken.AuthToken);            
            Lobbies = new Dictionary<string, Lobby>();
            List<string> lobbiesOutputList = new List<string>();
            foreach (var lobbyView in lobbieViews)
            {                                
                Lobby lobby = new Lobby(lobbyView);
                Lobbies.Add(lobby.Id + "      " + lobby.PlayerCount, lobby);
                lobbiesOutputList.Add(lobby.Id + "      " + lobby.PlayerCount);
            }
            LobbyList.ItemsSource = lobbiesOutputList;
        }

        public void CreateLobby(object sender, RoutedEventArgs e)
        {
            LobbyCreateWindow lobbyCreateWindow = new LobbyCreateWindow();
            lobbyCreateWindow.Show();
            Close();
        }

        public async void LobbyIn(object sender, RoutedEventArgs e)
        {

            CurrentLobby = Lobbies[(string)LobbyList.SelectedItem];
            if (CurrentLobby.PlayerCount.Equals(1))
            {
                CellState cellState = await ticTakApi.JoinLobby(Values.CurrentToken.AuthToken, new LobbyJoinForm {LobbyId = CurrentLobby.Id} );                
                
                waitWindow = new NewWaitWindow(cellState);
                waitWindow.Show();
                Close();
            }                        
        }
    }
}
