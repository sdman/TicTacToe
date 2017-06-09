using System;
using System.Collections.Generic;
using System.Linq;
using Vector = System.Tuple<int, int>;

namespace TicTacToe
{
    public class Field
    {
        private const int VictorySequenceSize = 5;
        private readonly Dictionary<Point, Cell> _cells = new Dictionary<Point, Cell>();

        private readonly Vector[] _directions =
            { new Vector(1, 0), new Vector(0, 1), new Vector(1, 1), new Vector(1, -1) };

        private readonly Vector[] _reverseDirections =
            { new Vector(-1, 0), new Vector(0, -1), new Vector(-1, -1), new Vector(-1, 1) };

        private Vector _winningVector;

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

            List<Point> winningPoints = new List<Point> { new Point(LastTurn.X, LastTurn.Y) };

            for (int i = 1; i < 5; i++)
            {
                Cell currrentCell = this[LastTurn.X + i * _winningVector.Item1, LastTurn.Y + i * _winningVector.Item2];

                if (currrentCell.State == LastTurn.State)
                {
                    winningPoints.Add(new Point(currrentCell.X, currrentCell.Y));

                    if (winningPoints.Count == 5)
                    {
                        return winningPoints;
                    }
                }

                currrentCell = this[LastTurn.X + i * -1 * _winningVector.Item1,
                    LastTurn.Y + i * -1 * _winningVector.Item2];

                if (currrentCell.State == LastTurn.State)
                {
                    winningPoints.Add(new Point(currrentCell.X, currrentCell.Y));

                    if (winningPoints.Count == 5)
                    {
                        return winningPoints;
                    }
                }
            }

            return winningPoints;
        }

        private int CalcMaxScore()
        {
            int[] scores =
            {
                CountScoreInDirection(_reverseDirections[0]) + CountScoreInDirection(_directions[0]) + 1,
                CountScoreInDirection(_reverseDirections[1]) + CountScoreInDirection(_directions[1]) + 1,
                CountScoreInDirection(_reverseDirections[2]) + CountScoreInDirection(_directions[2]) + 1,
                CountScoreInDirection(_reverseDirections[3]) + CountScoreInDirection(_directions[3]) + 1
            };

            for (int i = 0; i < 4; i++)
            {
                if (scores[i] >= VictorySequencySize)
                {
                    _winningVector = _directions[i];
                }
            }

            return scores.Max();
        }

        private int CountScoreInDirection(Vector direction)
        {
            int result = 0;
            for (int i = 1; i < VictorySequenceSize; i++)
            {
                if (this[LastTurn.X + i * direction.Item1, LastTurn.Y + i * direction.Item2].State == LastTurn.State)
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