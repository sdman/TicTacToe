using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfGame.RestModel.Views
{
    class LobbyView
    {        
        public string Id { get; set; }

        public PlayerViewModel TicPlayer { get; set; }
        public PlayerViewModel TacPlayer { get; set; }
    }
}
