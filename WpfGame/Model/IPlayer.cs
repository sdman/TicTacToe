using System;
using System.Threading.Tasks;
using TicTacToe;

namespace WpfGame.Model
{
    public interface IPlayer
    {
        CellState PlayerCellState { get; }

        Task SetCanTurn();
        Task ForceTurn(int x, int y);

        event EventHandler<TurnedEventArgs> Turned;
    }
}