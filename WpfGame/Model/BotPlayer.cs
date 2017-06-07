using System;
using System.Threading;
using System.Threading.Tasks;
using IBot;
using TicTacToe;

namespace WpfGame.Model
{
    public class BotPlayer : IPlayer
    {
        private readonly AbstractBot _bot;
        private bool _canTurn;

        public BotPlayer(AbstractBot bot)
        {
            _bot = bot;
        }

        public CellState PlayerCellState => _bot.State;

        public Task SetCanTurn()
        {
            return SetCanTurn(CancellationToken.None);
        }

        public Task SetCanTurn(CancellationToken token)
        {
            _canTurn = true;
            Cell cell = _bot.Step();
            return ForceTurn(cell.X, cell.Y, token);
        }

        public Task ForceTurn(int x, int y)
        {
            return ForceTurn(x, y, CancellationToken.None);
        }

        public Task ForceTurn(int x, int y, CancellationToken cancellationToken)
        {
            if (!_canTurn || _bot.Field[x, y].State != CellState.Empty)
            {
                return Task.FromResult(false);
            }

            return Task.Delay(Game.BotStepDelay, cancellationToken).ContinueWith(task =>
            {
                _canTurn = false;
                _bot.Field.Turn(_bot.State, x, y);
                Turned?.Invoke(this, new TurnedEventArgs { CellState = PlayerCellState, X = x, Y = y });
            }, cancellationToken);
        }

        public event EventHandler<TurnedEventArgs> Turned;
    }
}