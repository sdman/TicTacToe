using System;
using System.Windows;
using System.Diagnostics;
using Refit;
using WpfGame.RefitRequest;
using WpfGame.RestModel.Forms;
using static WpfGame.Utils.Values;

namespace WpfGame
{
    /// <summary>
    /// Логика взаимодействия для AuthenticateWindow.xaml
    /// </summary>
    public partial class AuthenticateWindow : Window
    {
        private ITicTacApi ticTakApi;

        public AuthenticateWindow()
        {
            InitializeComponent();
            DataContext = this;            
        }        

        private async void LogIn(object sender, RoutedEventArgs e)
        {
            ticTakApi = RestService.For<ITicTacApi>(URL);

            try
            {                
                CurrentToken = await ticTakApi.Login(new LoginForm(LoginBox.Text));
                Name = LoginBox.Text;

                LobbyListWindow lobbyListWindow = new LobbyListWindow();
                lobbyListWindow.Show();
                Close();
            }
            catch (Exception exception)
            {                
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.StackTrace);
            }
        }
    }
}
