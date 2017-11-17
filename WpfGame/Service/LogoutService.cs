using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Refit;
using WpfGame.RefitRequest;
using static WpfGame.Utils.Values;

namespace WpfGame
{
    static class LogoutService
    {
        public static async void Logout(object sender, CancelEventArgs e)
        {
            var ticTakApi = RestService.For<ITicTacApi>(URL);
            await ticTakApi.Logout(CurrentToken.AuthToken);
        }
    }
}
