using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using TicTacToe;

namespace WpfGame.Utils
{
    static class WebSocket
    {

        public static HubConnection connection = new HubConnectionBuilder()
                    .WithUrl(Values.URL + "/lobbyhub") // Урл хаба
                    .WithTransport(TransportType
                        .LongPolling) // Если не лонгпулинг, то не пашут хидеры. Видимо из-за альфы
                    .WithMessageHandler(new AuthTokenMessageHandler())
                    .Build();        

        internal class AuthTokenMessageHandler : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                // Тут добавляется токен аутентификации к хидерам запроса
                request.Headers.Add("AuthToken", Values.CurrentToken.AuthToken);
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
