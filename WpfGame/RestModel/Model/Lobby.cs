using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGame.RestModel.Views
{
    class Lobby
    {
        public string Id { get; }

        public PlayerViewModel TicPlayer { get; }
        public PlayerViewModel TacPlayer { get; }

        public int PlayerCount { get; set; }

        public Lobby(LobbyView lobbyView)
        {
            Id = lobbyView.Id;
            TacPlayer = lobbyView.TacPlayer;
            TicPlayer = lobbyView.TicPlayer;

            if (lobbyView.TacPlayer == null && lobbyView.TicPlayer == null)
            {                
                PlayerCount = 0;
            }
            else if (lobbyView.TacPlayer == null || lobbyView.TicPlayer == null)
            {                
                PlayerCount = 1;
            }
            else
            {                
                PlayerCount = 2;
            }
        }
    }
}
