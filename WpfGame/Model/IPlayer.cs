using System;
using System.Threading;
using System.Threading.Tasks;
using TicTacToe;

namespace WpfGame.Model
{
    public interface IPlayer
    {
        CellState PlayerCellState { get; }

        Task SetCanTurn();
        Task SetCanTurn(CancellationToken cancellationToken);
        Task ForceTurn(int x, int y);
        Task ForceTurn(int x, int y, CancellationToken cancellationToken);

        event EventHandler<TurnedEventArgs> Turned;
    }
}