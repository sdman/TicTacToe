using System;
using TicTacToe;

namespace WpfGame.Model
{
    public class TurnedEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellState CellState { get; set; }
    }
}