using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Refit;
using TicTacToe;
using WpfGame.RestModel;
using WpfGame.RestModel.Forms;
using WpfGame.RestModel.Views;

namespace WpfGame.RefitRequest
{
    interface ITicTacApi
    {
        [Put("/api/player/")]
        Task<Token> Login([Body]LoginForm name);

        [Delete("/api/player/")]
        Task<Object> Logout([Header("AuthToken")] string authToken);

        [Get("/api/lobby/")]
        Task<IEnumerable<LobbyView>> GetLobbies([Header("AuthToken")] string authToken);

        [Get("/api/player/{id}")]
        Task<PlayerViewModel> GetMe([AliasAs("id")] string authToken);

        [Put("/api/lobby/")]
        Task<LobbyView> CreateLobby([Header("AuthToken")] string authToken, [Body] LobbyCreateForm lobbyCreateForm);

        [Post("/api/lobby/join")]
        Task<CellState> JoinLobby([Header("AuthToken")] string authToken, [Body] LobbyJoinForm lobbyCreateForm);

        [Post("/api/lobby/exit")]
        Task<IEnumerable<Lobby>> ExitLobby([Header("AuthToken")] string authToken);
    }
}
