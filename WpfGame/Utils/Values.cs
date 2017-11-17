using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfGame.RestModel;
using WpfGame.RestModel.Views;

namespace WpfGame.Utils
{
    static class Values
    {
        public static Token CurrentToken { get; set; }

        public static string Name { get; set; }

        public static Lobby CurrentLobby { get; set; }

        public static Dictionary<string, Lobby> Lobbies { get; set; }

        public const string URL = "http://localhost:58534";
    }
}
