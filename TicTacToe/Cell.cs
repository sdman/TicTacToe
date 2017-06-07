using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Cell
    {
        public CellState State { get; set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Cell(int x, int y, CellState state = CellState.Empty)
        {
            X = x;
            Y = y;
            State = state;
        }

        public Cell(Point point, CellState state = CellState.Empty)
        {
            X = point.X;
            Y = point.Y;
            State = state;
        }

        public Cell(Cell cell)
        {
            X = cell.X;
            Y = cell.Y;
            State = cell.State;
        }
    }
}
