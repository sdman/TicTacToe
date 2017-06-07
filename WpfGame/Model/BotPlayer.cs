using System;
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
            _canTurn = true;
            Cell cell = _bot.Step();
            return ForceTurn(cell.X, cell.Y);
        }

        public Task ForceTurn(int x, int y)
        {
            return Task.Run(async () =>
            {
                if (!_canTurn || _bot.Field[x, y].State != CellState.Empty)
                {
                    return;
                }

                await Task.Delay(700);

                _canTurn = false;
                _bot.Field.Turn(_bot.State, x, y);
                Turned?.Invoke(this, new TurnedEventArgs { CellState = PlayerCellState, X = x, Y = y });
            });
        }

        public event EventHandler<TurnedEventArgs> Turned;
    }
}