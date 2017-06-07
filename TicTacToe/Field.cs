using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe;

namespace TicTacToe
{
    public class Field
    {
        private const int VICTORY_SEQUENCE_SIZE = 5;
        private Dictionary<Point, Cell> _cells = new Dictionary<Point, Cell>();
        private Cell _lastTurn;

        public int VictorySequencySize => VICTORY_SEQUENCE_SIZE;
        public Cell LastTurn => _lastTurn;

        public Cell this[Point point]
        {
            get
            {
                return this[point.X, point.Y];
            }
        }

        public Cell this[int x, int y]
        {
            get
            {
                Point key = new Point(x, y);
                if(_cells.ContainsKey(key))
                {
                    return _cells[key];
                } else 
                {
                    return new Cell(x, y);
                }
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
            _lastTurn = new Cell(x, y, state);
            _cells.Add(key, _lastTurn);
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
            if (_lastTurn != null && _lastTurn.State == state)
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
            return _lastTurn != null ? CalcMaxScore() >= VICTORY_SEQUENCE_SIZE : false;
        }

        private int CalcMaxScore()
        {
            int[] scores = new int[] {
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
            for (int i = 1; i < VICTORY_SEQUENCE_SIZE; i++)
            {
                if (this[_lastTurn.X + i * dx, _lastTurn.Y + i * dy].State == _lastTurn.State)
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
            if(state == CellState.Empty)
            {
                return state;
            } else
            {
                return state == CellState.Tick ? CellState.Tack : CellState.Tick;
            }
        }

        public List<Cell> GetMarkedCells()
        {
            return _cells.Values.ToList();
        }
    }
}
