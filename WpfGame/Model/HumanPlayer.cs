using System;
using System.Threading.Tasks;
using TicTacToe;

namespace WpfGame.Model
{
    public class HumanPlayer : IPlayer
    {
        private bool _canTurn;

        public HumanPlayer(Field field, CellState playerState)
        {
            PlayerCellState = playerState;
            Field = field;
        }

        public Field Field { get; }
        public CellState PlayerCellState { get; }

        public Task SetCanTurn()
        {
            _canTurn = true;
            return Task.FromResult(false);
        }

        public Task ForceTurn(int x, int y)
        {
            return Task.Run(() =>
            {
                if (!_canTurn || Field[x, y].State != CellState.Empty)
                {
                    return;
                }

                Field.Turn(PlayerCellState, x, y);
                _canTurn = false;
                Turned?.Invoke(this, new TurnedEventArgs { CellState = PlayerCellState, X = x, Y = y });
            });
        }

        public event EventHandler<TurnedEventArgs> Turned;
    }
}