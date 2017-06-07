using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    public class Field
    {
        private const int VictorySequenceSize = 5;
        private readonly Dictionary<Point, Cell> _cells = new Dictionary<Point, Cell>();

        public int VictorySequencySize => VictorySequenceSize;
        public Cell LastTurn { get; private set; }

        public Cell this[Point point] => this[point.X, point.Y];

        public Cell this[int x, int y]
        {
            get
            {
                Point key = new Point(x, y);
                if (_cells.ContainsKey(key))
                {
                    return _cells[key];
                }
                return new Cell(x, y);
            }
        }

        public void Turn(Cell cell)
        {
            Turn(cell.State, cell.X, cell.Y);
        }

        public void Turn(CellState state, Point point)
        {
            Turn(state, point.X, point.Y);
        }

        public void Turn(CellState state, int x, int y)
        {
            CheckEmptyState(state);
            CheckDoubleTurn(state);
            CheckOccupiedCell(x, y);

            Point key = new Point(x, y);
            LastTurn = new Cell(x, y, state);
            _cells.Add(key, LastTurn);
        }

        private void CheckEmptyState(CellState state)
        {
            if (state == CellState.Empty)
            {
                throw new ApplicationException("Нельзя пойти пустым местом");
            }
        }

        private void CheckDoubleTurn(CellState state)
        {
            if (LastTurn != null && LastTurn.State == state)
            {
                throw new ApplicationException("Нельзя пойти дважды.");
            }
        }

        private void CheckOccupiedCell(int x, int y)
        {
            if (this[x, y].State != CellState.Empty)
            {
                throw new ApplicationException("Нельзя пойти в занятую ячейку.");
            }
        }

        public bool IsEnd()
        {
            return LastTurn != null && CalcMaxScore() >= VictorySequenceSize;
        }

        public IEnumerable<Point> GetWinningPoints()
        {
            if (!IsEnd())
            {
                return null;
            }

            return new[] { new Point(0, 0) };
        }

        private int CalcMaxScore()
        {
            int[] scores =
            {
                CountScoreInDirection(-1, 0) + CountScoreInDirection(1, 0),
                CountScoreInDirection(0, -1) + CountScoreInDirection(0, 1),
                CountScoreInDirection(-1, -1) + CountScoreInDirection(1, 1),
                CountScoreInDirection(-1, 1) + CountScoreInDirection(1, -1)
            };

            return scores.Max() + 1;
        }

        private int CountScoreInDirection(int dx, int dy)
        {
            int result = 0;
            for (int i = 1; i < VictorySequenceSize; i++)
            {
                if (this[LastTurn.X + i * dx, LastTurn.Y + i * dy].State == LastTurn.State)
                {
                    result++;
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private CellState GetOpponent(CellState state)
        {
            if (state == CellState.Empty)
            {
                return state;
            }
            return state == CellState.Tick ? CellState.Tack : CellState.Tick;
        }

        public List<Cell> GetMarkedCells()
        {
            return _cells.Values.ToList();
        }
    }
}